using System;
using CoreGraphics;
using UIKit;

namespace XBottomSheet.Touch.Views
{
    public partial class BottomSheetViewController : UIViewController
    {
        float fullView = 100;
        nfloat partialView
        {
            get
            {
                return UIScreen.MainScreen.Bounds.Height - UIApplication.SharedApplication.StatusBarFrame.Height;
            }
        }

        public BottomSheetViewController() : base("BottomSheetViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var gesture = new UIPanGestureRecognizer((g) => PanGesture(g));
            View.AddGestureRecognizer(gesture);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            PrepareBackgroundView();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //UIView.animateAnimateWithDuration(0.3) { [weak self] in
            var frame = this?.View.Frame;
            var yComponent = UIScreen.MainScreen.Bounds.Height - 200;
            View.Frame = new CGRect(0, yComponent, frame.Value.Width, frame.Value.Height);
            //}
        }

        private void PanGesture(UIPanGestureRecognizer recognizer)
        {
            var translation = recognizer.TranslationInView(View);
            var y = View.Frame.GetMinY();
            View.Frame = new CGRect(0, y + translation.Y, View.Frame.Width, View.Frame.Height);
            recognizer.SetTranslation(CGPoint.Empty, View);

            var velocity = recognizer.VelocityInView(View);
            if ((y + translation.Y >= fullView) & (y + translation.Y <= partialView))
            {
                View.Frame = new CGRect(0, y + translation.Y, View.Frame.Width, View.Frame.Height);
                recognizer.SetTranslation(CGPoint.Empty, View);
            }

            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                var duration = velocity.Y < 0 ? ((y - fullView) / -velocity.Y) : ((partialView - y) / velocity.Y);
                duration = duration > 1.3 ? 1 : duration;

                UIView.Animate(duration, 0.0, UIViewAnimationOptions.AllowUserInteraction, () =>
                {
                    if (velocity.Y >= 0)
                    {
                        View.Frame = new CGRect(0, partialView, View.Frame.Width, View.Frame.Height);
                    }
                    else
                    {
                        View.Frame = new CGRect(0, fullView, View.Frame.Width, View.Frame.Height);
                    }
                }, null);
            }
        }


        private void PrepareBackgroundView()
        {
            var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
            var visualEffect = new UIVisualEffectView(blurEffect);
            var bluredView = new UIVisualEffectView(blurEffect);
            bluredView.ContentView.AddSubview(visualEffect);

            visualEffect.Frame = UIScreen.MainScreen.Bounds;
            bluredView.Frame = UIScreen.MainScreen.Bounds;

            View.InsertSubview(bluredView, 0);
        }
    }
}