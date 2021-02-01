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
using Android.Media;
using FirstAlert.Fragments;
using Java.IO;
using Java.Lang;
using Java.Nio;

namespace FirstAlert.Imaging
{
    public class ImageInferer : Java.Lang.Object, IRunnable
    {
        private Image mImage;
        private File mFile;

        private readonly CameraFragment mOwner;

        public ImageInferer(Image image, File file, CameraFragment owner)
        {
            mImage = image ?? throw new ArgumentNullException();
            mFile = file ?? throw new ArgumentNullException();
            mOwner = owner ?? throw new ArgumentException();
        }

        public void Run()
        {
            ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
            byte[] bytes = new byte[buffer.Remaining()];
            buffer.Get(bytes);

            mOwner.tensorflow.ScanImage(bytes);

            //using (var output = new FileOutputStream(mFile))
            //{
            //    try
            //    {
            //        output.Write(bytes);
            //    }
            //    catch (IOException e)
            //    {
            //        e.PrintStackTrace();
            //    }
            //    finally
            //    {
            //        mImage.Close();
            //    }
            //}
        }
    }
}