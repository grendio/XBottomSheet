using System;
using Android.Content;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;

namespace XBottomSheet.Droid.Views
{
    public class XBottomSheetView : CoordinatorLayout
    {
        private BottomSheetBehavior behavior;

        #region Properties
        public event EventHandler StateChanged;

        public View ContentView { get; set; }

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
        public XBottomSheetView(Context context) : base(context)
        {
        }

        public XBottomSheetView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public XBottomSheetView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        protected XBottomSheetView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        #endregion
    }
}