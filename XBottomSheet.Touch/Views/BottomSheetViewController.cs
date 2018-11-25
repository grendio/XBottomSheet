using System;
using CoreGraphics;
using UIKit;

namespace XBottomSheet.Touch.Views
{
    public partial class BottomSheetViewController : UIViewController
    {
        private readonly nfloat top;
        private readonly nfloat middle;
        private readonly nfloat bottom;
        private readonly bool animatedAppearance;

        private BottomSheetState currentState;

        /// <summary>
        /// Create a new UIViewController that will behave as a BottomSheet control. As it will have the bottom stop point, there won't be autohide available. In order to have autohide, use the constructor without bottom parrameter.
        /// </summary>
        /// <param name="top">Top point for the control to expand to.</param>
        /// <param name="middle">Middle point where control will stop. This can be used as default state as well.</param>
        /// <param name="bottom">Point where controll will stay as expanded at the bottom of the screen.</param>
        /// <param name="animatedAppearance">Specify if control should appear animated.</param>
        public BottomSheetViewController(nfloat top, nfloat middle, nfloat bottom, bool animatedAppearance = true) : base("BottomSheetViewController", null)
        {
            this.top = top;
            this.middle = middle;
            this.bottom = bottom;
            this.animatedAppearance = animatedAppearance;
        }

        /// <summary>
        /// Create a new UIViewController that will behave as a BottomSheet control. Autohide is on as the bottom point is not provided.
        /// </summary>
        /// <param name="top">Top point for the control to expand to.</param>
        /// <param name="middle">Middle point where control will stop. This can be used as default state as well.</param>
        /// <param name="animatedAppearance">Specify if control should appear animated.</param>
        public BottomSheetViewController(nfloat top, nfloat middle, bool animatedAppearance = true) : base("BottomSheetViewController", null)
        {
            this.top = top;
            this.middle = middle;
            this.animatedAppearance = animatedAppearance;
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

        //TODO Check if we can use this animated parameter instead of ours
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (animatedAppearance)
                UIView.Animate(0.3, 0.0, UIViewAnimationOptions.AllowUserInteraction, () =>
                {
                    View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
                }, null);
            else
                View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
            currentState = BottomSheetState.Middle;
        }

        /// <summary>
        /// Make the control visible again.
        /// </summary>
        public void Show()
        {
            View.Hidden = false;
        }

        /// <summary>
        /// This will hide the control.
        /// </summary>
        /// <param name="resetState">Takes control back to default state (e.g. Middle).</param>
        public void Hide(bool resetState)
        {
            if (resetState && currentState != BottomSheetState.Middle)
            {
                View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
                currentState = BottomSheetState.Middle;
            }
            View.Hidden = true;
        }

        private void PanGesture(UIPanGestureRecognizer recognizer)
        {
            var translation = recognizer.TranslationInView(View);
            var y = View.Frame.GetMinY();
            View.Frame = new CGRect(0, y + translation.Y, View.Frame.Width, View.Frame.Height);
            recognizer.SetTranslation(CGPoint.Empty, View);

            var velocity = recognizer.VelocityInView(View);
            if ((y + translation.Y >= top) & (y + translation.Y <= middle))
            {
                View.Frame = new CGRect(0, y + translation.Y, View.Frame.Width, View.Frame.Height);
                recognizer.SetTranslation(CGPoint.Empty, View);
            }

            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                var duration = velocity.Y < 0 ? ((y - top) / -velocity.Y) : ((middle - y) / velocity.Y);
                duration = duration > 1.3 ? 1 : duration;

                UIView.Animate(duration, 0.0, UIViewAnimationOptions.AllowUserInteraction, () =>
                {
                    if (velocity.Y >= 0)
                    {
                        if (currentState == BottomSheetState.Top)
                        {
                            View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
                            currentState = BottomSheetState.Middle;
                        }
                        else if (bottom == 0)
                        {
                            Hide(true);
                        }
                        else
                        {
                            View.Frame = new CGRect(0, bottom, View.Frame.Width, View.Frame.Height);
                            currentState = BottomSheetState.Bottom;
                        }
                    }
                    else
                    {
                        if (currentState == BottomSheetState.Bottom)
                        {
                            View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
                            currentState = BottomSheetState.Middle;
                        }
                        else
                        {
                            View.Frame = new CGRect(0, top, View.Frame.Width, View.Frame.Height);
                            currentState = BottomSheetState.Top;
                        }
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