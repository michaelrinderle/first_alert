﻿using Android.Hardware.Camera2;
using FirstAlert.Fragments;

namespace FirstAlert.Listeners
{
    public class CaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly CameraFragment owner;

        public CaptureSessionCallback(CameraFragment owner)
        {
            this.owner = owner ?? throw new System.ArgumentNullException();
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            owner.ShowToast("Failed");
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (null == owner.mCameraDevice)
            {
                return;
            }

            // When the session is ready, we start displaying the preview.
            owner.mCaptureSession = session;
            try
            {
                // Auto focus should be continuous for camera preview.
                owner.mPreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                // Flash is automatically enabled when necessary.
                owner.SetAutoFlash(owner.mPreviewRequestBuilder);

                // Finally, we start displaying the camera preview.
                owner.mPreviewRequest = owner.mPreviewRequestBuilder.Build();
                owner.mCaptureSession.SetRepeatingRequest(owner.mPreviewRequest,
                    owner.mCaptureCallback, owner.mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}