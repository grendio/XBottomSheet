using System;
using CoreGraphics;
using UIKit;
using XBottomSheet.Touch.Views;

namespace XBottomSheet.Touch.Sample
{
    public partial class ViewController : UIViewController
    {
        BottomSheetViewController bottomSheetViewController;
        bool viewWasSet;

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
            var heightWhereToStart = 150;
            var bottom = UIScreen.MainScreen.Bounds.Height - UIApplication.SharedApplication.StatusBarFrame.Height;
            bottomSheetViewController = new BottomSheetViewController(100, 300, 600, heightWhereToStart, true, BottomSheetState.Bottom);

            // Add BottomSheetViewController as a child view 
            AddChildViewController(bottomSheetViewController);
            View.AddSubview(bottomSheetViewController.View);
            bottomSheetViewController.DidMoveToParentViewController(this);

            // BottomSheetViewController frame
            bottomSheetViewController.View.Frame = new CGRect(0, View.Frame.GetMaxY(), View.Frame.Width, View.Frame.Height);

            btMain.TouchUpInside += BtMain_TouchUpInside;

            UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(HandleAction);
            View.AddGestureRecognizer(tapGesture);

            btTest.TouchUpInside += BtTest_TouchUpInside;
        }

        private void BtTest_TouchUpInside(object sender, EventArgs e)
        {
            // Just to check if the button is actually clickable
        }

        void HandleAction()
        {
            bottomSheetViewController.Hide(true);
        }

        void BtMain_TouchUpInside(object sender, EventArgs e)
        {
            bottomSheetViewController.Show();
            if (!viewWasSet)
            {
                var customView = CustomView.Create();
                customView.Frame = View.Frame;
                bottomSheetViewController.SetCustomView(customView);
                viewWasSet = true;
            }

            // Check what CurrentState of the BottomSheetViewController
            var checkState = bottomSheetViewController.CurrentState;
        }
    }
}