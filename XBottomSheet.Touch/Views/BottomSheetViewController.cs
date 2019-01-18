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
        private readonly BottomSheetState defaultState;

        private BottomSheetState currentState;
        private UIView customView;

        /// <summary>
        /// Gets or sets the duration of controller on how fast it should reach the anchor point. Default value is -2.
        /// </summary>
        public nfloat Duration { get; set; } = -2;

        /// <summary>
        /// Create a new UIViewController that will behave as a BottomSheet control. As it will have the bottom stop point, there won't be autohide available. In order to have autohide, use the constructor without bottom parrameter.
        /// </summary>
        /// <param name="top">Top point for the control to expand to.</param>
        /// <param name="middle">Middle point where control will stop. This can be used as default state as well.</param>
        /// <param name="bottom">Point where controll will stay as expanded at the bottom of the screen.</param>
        /// <param name="animatedAppearance">Specify if control should appear animated.</param>
        /// <param name="defaultState">Specify which state should be default when control appears.</param>
        public BottomSheetViewController(nfloat top, nfloat middle, nfloat bottom, bool animatedAppearance = true, BottomSheetState defaultState = BottomSheetState.Middle) : base("BottomSheetViewController", null)
        {
            this.top = top;
            this.middle = middle;
            this.bottom = bottom;
            this.animatedAppearance = animatedAppearance;
            this.defaultState = defaultState;
        }

        /// <summary>
        /// Create a new UIViewController that will behave as a BottomSheet control. Autohide is on as the bottom point is not provided.
        /// </summary>
        /// <param name="top">Top point for the control to expand to.</param>
        /// <param name="middle">Middle point where control will stop. This can be used as default state as well.</param>
        /// <param name="animatedAppearance">Specify if control should appear animated.</param>
        /// <param name="defaultState">Specify which state should be default when control appears. As this constructor won't assign bottom value, DO NOT use the Bottom state as default.</param>
        public BottomSheetViewController(nfloat top, nfloat middle, bool animatedAppearance = true, BottomSheetState defaultState = BottomSheetState.Middle) : base("BottomSheetViewController", null)
        {
            this.top = top;
            this.middle = middle;
            this.animatedAppearance = animatedAppearance;
            this.defaultState = defaultState;
        }

        /// <summary>
        /// Create a new UIViewController that will behave as a BottomSheet control. Autohide is on as the bottom point is not provided.
        /// </summary>
        /// <param name="middle">Middle point where control will stop. This is the only possible state for the control. For more states, check rest of the constructors.</param>
        /// <param name="animatedAppearance">Specify if control should appear animated.</param>
        public BottomSheetViewController(nfloat middle, bool animatedAppearance = true) : base("BottomSheetViewController", null)
        {
            this.top = middle;
            this.middle = middle;
            this.animatedAppearance = animatedAppearance;
            this.defaultState = BottomSheetState.Middle;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var gesture = new UIPanGestureRecognizer((g) => PanGesture(g));
            View.AddGestureRecognizer(gesture);
        }

        //TODO Check if we can use this animated parameter instead of ours
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (animatedAppearance)
                UIView.Animate(Duration, 0.0, UIViewAnimationOptions.AllowUserInteraction, () =>
                {
                    CreateViewFrame(defaultState);
                }, null);
            else
                CreateViewFrame(defaultState);
        }

        /// <summary>
        /// Make the control visible again.
        /// </summary>
        public void Show()
        {
            View.Hidden = false;
        }

        /// <summary>
        /// Hide the control.
        /// </summary>
        /// <param name="resetState">Takes control back to default state (e.g. Middle).</param>
        public void Hide(bool resetState)
        {
            if (resetState && currentState != defaultState)
                CreateViewFrame(defaultState);
            View.Hidden = true;
        }

        //TODO Add this to constructor
        /// <summary>
        /// Add custom subview to the control.
        /// </summary>
        /// <param name="customView">Custom UIView that will be shown instead of the default one.</param>
        public void SetCustomView(UIView customView)
        {
            View.AddSubview(customView);
        }

        private void PanGesture(UIPanGestureRecognizer recognizer)
        {
            var translation = recognizer.TranslationInView(View);
            var location = recognizer.LocationInView(View.Superview);
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
                UIView.Animate(Duration, 0.0, UIViewAnimationOptions.AllowUserInteraction, () =>
                {
                    if (middle == top && bottom == 0)
                        CreateViewFrameForSingleState(location.Y);
                    else if (bottom - location.Y < (bottom - middle) / 2)
                        CreateViewFrame(BottomSheetState.Bottom);
                    else if (middle - location.Y < (middle - top) / 2)
                        CreateViewFrame(BottomSheetState.Middle);
                    else if (location.Y - top < (middle - top) / 2)
                        CreateViewFrame(BottomSheetState.Top);
                }, null);
            }
        }

        private void CreateViewFrame(BottomSheetState state)
        {
            if (state == BottomSheetState.Top)
            {
                View.Frame = new CGRect(0, top, View.Frame.Width, View.Frame.Height);
                currentState = BottomSheetState.Top;
            }
            else if (state == BottomSheetState.Middle)
            {
                View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
                currentState = BottomSheetState.Middle;
            }
            else if (state == BottomSheetState.Bottom)
            {
                View.Frame = new CGRect(0, bottom, View.Frame.Width, View.Frame.Height);
                currentState = BottomSheetState.Bottom;
            }
        }

        private void CreateViewFrameForSingleState(nfloat endTapLocation)
        {
            if (endTapLocation > middle)
                Hide(false);
            View.Frame = new CGRect(0, middle, View.Frame.Width, View.Frame.Height);
            currentState = BottomSheetState.Middle;
        }
    }
}