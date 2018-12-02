using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using XBottomSheet.Droid.Behaviors;

namespace XBottomSheet.Droid.Views
{
    public class XBottomSheetView : CoordinatorLayout
    {
        private AnchoredBottomSheetBehavior behavior;
        private NestedScrollView contentScroller;

        #region Properties
        public int PeekHeight { get; set; } = 100;
        public int AnchorHeight { get; set; } = 600;

        private View contentView;
        public View ContentView
        {
            get
            {
                return contentView;
            }
            set
            {
                if(contentScroller != null)
                {
                    contentView = value;
                    RemoveAllViewsInLayout();
                    contentScroller.RemoveAllViews();
                    contentScroller.AddView(value, LayoutParams.MatchParent, LayoutParams.MatchParent);

                    var prm = new CoordinatorLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    prm.Behavior = behavior;

                    AddViewInLayout(contentScroller, 0, prm);
                    RequestLayout();
                }
            }
        }
        public Color BackgroundColor
        {
            set
            {
                contentScroller.SetBackgroundColor(value);
            }
        }

        private string state;
        public string State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;


                if (behavior != null)
                {
                    behavior.State = int.Parse(State);
                }
            }
        }
        #endregion

        #region Constructors
        public XBottomSheetView(Context context) : base(context) {}
        public XBottomSheetView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            behavior = new AnchoredBottomSheetBehavior(context,attrs)
            {
                Hideable = false,
                PeekHeight = PeekHeight,
                AnchorOffset = AnchorHeight
            };
            contentScroller = new NestedScrollView(context);
            contentScroller.SetBackgroundColor(Color.LightGray);
            ContentView = new FrameLayout(context);
        }
        public XBottomSheetView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) {}
        protected XBottomSheetView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {}
        #endregion
    }
}