//    __ _/| _/. _  ._/__ /
// _\/_// /_///_// / /_|/
//            _/
// sof digital 2021
// written by michael rinderle <michael@sofdigital.net>

// mit license
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using FirstAlert.Handlers;
using Java.IO;
using Java.Nio;
using Java.Nio.Channels;
using Xamarin.TensorFlow.Lite;

namespace FirstAlert.Imaging
{
    public class ImageClassifier
    {
        private static readonly string TAG = "CameraFragment";
        private const string MODEL_FILE = "new_mobile_model.tflite";
        private const string LABEL_FILE = "labels.txt";
        private const int THREAD_NUM = 4;
        private const int FLOAT_SIZE = 4;
        private const int PIXEL_SIZE = 3;

        private List<string> _labels;
        private Interpreter _interpreter;
        private MappedByteBuffer _model;

        public event EventHandler<ClassificationEventArgs> ClassificationCompleted;

        public ImageClassifier()
        {
            try
            {
                LoadTensorflowModelLabels();
                LoadTensorflowModel();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.ToString());
            }
        }

        private void LoadTensorflowModelLabels()
        {
            var assets = Android.App.Application.Context.Assets;
            using var sr = new StreamReader(assets.Open(LABEL_FILE));
            var content = sr.ReadToEnd();
            _labels = content.Split("\n")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        private void LoadTensorflowModel()
        {
            var assets = Android.App.Application.Context.Assets;
            AssetFileDescriptor fileDescriptor = assets.OpenFd(MODEL_FILE);
            FileInputStream inputStream = new FileInputStream(fileDescriptor.FileDescriptor);
            FileChannel fileChannel = inputStream.Channel;
            long startOffset = fileDescriptor.StartOffset;
            long declaredLength = fileDescriptor.DeclaredLength;
            _model = fileChannel.Map(FileChannel.MapMode.ReadOnly, startOffset, declaredLength);
            Interpreter.Options options = new Interpreter.Options();
            options.SetNumThreads(THREAD_NUM);
            _interpreter = new Interpreter(_model, options);
        }

        public void ScanImage(byte[] bytes)
        {
            var tensor = _interpreter.GetInputTensor(0);
            var shape = tensor.Shape();

            var width = shape[1];
            var height = shape[2];

            var byteBuffer = ConvertImageByteArrayToByteBuffer(bytes, width, height);

            var outputLocation = new float[1][] { new float[_labels.Count] };
            var outputs = Java.Lang.Object.FromArray(outputLocation);

            _interpreter.Run(byteBuffer, outputs);

            var classificationResult = outputs.ToArray<float[]>();
            var result = new List<ImageClassification>();
            for (var i = 0; i < _labels.Count; i++)
            {
                var label = _labels[i];
                if (classificationResult != null)
                    result.Add(new ImageClassification(label, classificationResult[0][i]));
            }

            ClassificationCompleted?.Invoke(this, new ClassificationEventArgs(result));
        }

        private ByteBuffer ConvertImageByteArrayToByteBuffer(byte[] image, int width, int height)
        {
            var bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, true);

            var modelInputSize = FLOAT_SIZE * height * width * PIXEL_SIZE;
            var byteBuffer = ByteBuffer.AllocateDirect(modelInputSize);
            byteBuffer.Order(ByteOrder.NativeOrder());

            var pixels = new int[width * height];
            resizedBitmap.GetPixels(pixels, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

            var pixel = 0;

            //Loop through each pixels to create a Java.Nio.ByteBuffer
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var pixelVal = pixels[pixel++];

                    byteBuffer.PutFloat(pixelVal >> 16 & 0xFF);
                    byteBuffer.PutFloat(pixelVal >> 8 & 0xFF);
                    byteBuffer.PutFloat(pixelVal & 0xFF);
                }
            }

            bitmap.Recycle();

            return byteBuffer;
        }
    }
}