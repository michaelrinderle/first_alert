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

using Android.Graphics;
using FirstAlert.Enums;

namespace FirstAlert.Imaging
{
    public static class ImageExtensions
    {
        private static readonly float ImageStd = 1.0f;

        private static float ImageMeanR(ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Retail:
                    return 0.0f;
                case ModelType.General:
                case ModelType.Landscape:
                default:
                    return 123.0f;
            }
        }

        private static float ImageMeanG(ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Retail:
                    return 0.0f;
                case ModelType.General:
                case ModelType.Landscape:
                default:
                    return 117.0f;
            }
        }

        private static float ImageMeanB(ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Retail:
                    return 0.0f;
                case ModelType.General:
                case ModelType.Landscape:
                default:
                    return 104.0f;
            }
        }

        public static float[] GetBitmapPixels(this Bitmap bitmap, int width, int height, ModelType modelType, bool hasNormalizationLayer)
        {
            var floatValues = new float[width * height * 3];
            var imageMeanB = hasNormalizationLayer ? 0f : ImageMeanB(modelType);
            var imageMeanG = hasNormalizationLayer ? 0f : ImageMeanG(modelType);
            var imageMeanR = hasNormalizationLayer ? 0f : ImageMeanR(modelType);

            using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, false))
            {
                using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                {
                    var intValues = new int[width * height];
                    resizedBitmap.GetPixels(intValues, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

                    for (int i = 0; i < intValues.Length; ++i)
                    {
                        var val = intValues[i];

                        floatValues[i * 3 + 0] = ((val & 0xFF) - imageMeanB) / ImageStd;
                        floatValues[i * 3 + 1] = (((val >> 8) & 0xFF) - imageMeanG) / ImageStd;
                        floatValues[i * 3 + 2] = (((val >> 16) & 0xFF) - imageMeanR) / ImageStd;
                    }

                    resizedBitmap.Recycle();
                }

                scaledBitmap.Recycle();
            }
            return floatValues;    
        }
    }
}