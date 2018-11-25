using System;
using CoreGraphics;
using UIKit;
using XBottomSheet.Touch.Views;

namespace XBottomSheet.Touch.Sample
{
    public partial class ViewController : UIViewController
    {
        BottomSheetViewController bottomSheetViewController;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupBottomSheet();
        }

        private void SetupBottomSheet()
        {
            // Create BottomSheetViewController
            var bottom = UIScreen.MainScreen.Bounds.Height - UIApplication.SharedApplication.StatusBarFrame.Height;
            bottomSheetViewController = new BottomSheetViewController(100, 300, bottom);

            // Add BottomSheetViewController as a child view 
            this.AddChildViewController(bottomSheetViewController);
            this.View.AddSubview(bottomSheetViewController.View);
            bottomSheetViewController.DidMoveToParentViewController(this);

            // BottomSheetViewController frame
            bottomSheetViewController.View.Frame = new CGRect(0, View.Frame.GetMaxY(), View.Frame.Width, View.Frame.Height);

            btMain.TouchUpInside += BtMain_TouchUpInside;

            UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(HandleAction);
            View.AddGestureRecognizer(tapGesture);
        }

        void HandleAction()
        {
            bottomSheetViewController.View.Hidden = true;
        }

        void BtMain_TouchUpInside(object sender, EventArgs e)
        {
            bottomSheetViewController.View.Hidden = false;
        }
    }
}