using Android.Views;
using FirstAlert.Fragments;
using System;

namespace FirstAlert.Listeners
{
    public class SurfaceTextureListener: Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly CameraFragment owner;

        public SurfaceTextureListener(CameraFragment owner)
        {
            this.owner = owner ?? throw new ArgumentException();
        }

        public void OnSurfaceTextureAvailable(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface) { }
    }
}