using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace XBottomSheet.Touch
{
    public partial class BottomSheetContentView : UIView
    {
        public BottomSheetContentView(IntPtr handle) : base(handle)
        {
        }

        public static BottomSheetContentView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("BottomSheetContentView", null, null);
            var v = arr.GetItem<BottomSheetContentView>(0);
            return v;
        }

        public void SetContentView(UIView customView)
        {
            customView.Frame = contentView.Frame;
            contentView.AddSubview(customView);
        }

        public void AdjustContentFrame(CGRect frame, nfloat startHeight, nfloat height)
        {
            var x = contentView.Subviews[0];
            x.Frame = new CGRect(x.Frame.X, x.Frame.Y, x.Frame.Width, frame.Height - height - startHeight);
        }
    }
}