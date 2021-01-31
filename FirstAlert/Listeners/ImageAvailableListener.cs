using Android.Media;
using FirstAlert.Fragments;
using Java.IO;
using Java.Lang;
using Java.Nio;
using System;

namespace FirstAlert.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private readonly File file;
        private readonly CameraFragment owner;

        public ImageAvailableListener(CameraFragment fragment, File file)
        {
            this.owner = fragment ?? throw new ArgumentNullException();
            this.file = file ?? throw new ArgumentNullException();
        }

        public void OnImageAvailable(ImageReader reader)
        {
            owner.mBackgroundHandler.Post(new ImageSaver(reader.AcquireNextImage(), file));
        }

        private class ImageSaver : Java.Lang.Object, IRunnable
        {
            private Image mImage;
            private File mFile;

            public ImageSaver(Image image, File file)
            {
                mImage = image ?? throw new ArgumentNullException();
                mFile = file ?? throw new ArgumentNullException();
            }

            public void Run()
            {
                ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);
                using (var output = new FileOutputStream(mFile))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                    finally
                    {
                        mImage.Close();
                    }
                }
            }
        }
    }
}