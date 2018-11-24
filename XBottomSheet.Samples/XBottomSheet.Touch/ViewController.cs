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
            // Perform any additional setup after loading the view, typically from a nib.
            SetupBottomSheet();
        }

        private void SetupBottomSheet()
        {
            // 1- Init bottomSheetVC
            bottomSheetViewController = new BottomSheetViewController();

            // 2- Add bottomSheetVC as a child view 
            this.AddChildViewController(bottomSheetViewController);
            this.View.AddSubview(bottomSheetViewController.View);
            bottomSheetViewController.DidMoveToParentViewController(this);

            // 3- Adjust bottomSheet frame and initial position.
            var height = View.Frame.Height;
            var width = View.Frame.Width;
            bottomSheetViewController.View.Frame = new CGRect(0, this.View.Frame.GetMaxY(), width, height);

            //btMain.TouchUpInside += BtMain_TouchUpInside;

            //UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(HandleAction);
            //View.AddGestureRecognizer(tapGesture);
        }

        void HandleAction()
        {
            bottomSheetViewController.View.Hidden = true;
        }


        void BtMain_TouchUpInside(object sender, EventArgs e)
        {
            bottomSheetViewController.View.Hidden = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
