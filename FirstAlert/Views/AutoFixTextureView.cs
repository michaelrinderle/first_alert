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

using Android.Content;
using Android.Util;
using Android.Views;
using System;

namespace FirstAlert.Views
{
    public class AutoFitTextureView : TextureView
    {
        private int mRatioWidth = 0;
        private int mRatioHeight = 0;

        public AutoFitTextureView (Context context, IAttributeSet attrs) : this (context, attrs, 0) { }

        public AutoFitTextureView (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle) { }

        public void SetAspectRatio(int width, int height)
        {
            if (width == 0 || height == 0)
                throw new ArgumentException ();
            mRatioWidth = width;
            mRatioHeight = height;
            RequestLayout ();
        }

        protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
            int width = MeasureSpec.GetSize (widthMeasureSpec);
            int height = MeasureSpec.GetSize (heightMeasureSpec);
            if (0 == mRatioWidth || 0 == mRatioHeight) {
                SetMeasuredDimension (width, height);
            } else {
                if (width < (float)height * mRatioWidth / (float)mRatioHeight) {
                    SetMeasuredDimension (width, width * mRatioHeight / mRatioWidth);
                } else {
                    SetMeasuredDimension (height * mRatioWidth / mRatioHeight, height);
                }
            }
        }
    }
}

