using System;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Java.Lang;

namespace XBottomSheet.Droid.JavaComponents
{
    public class SettleRunnable : Java.Lang.Object, IRunnable
    {
        private View view;
        private int targetState;
        private ViewDragHelper viewDragHelper;
        private Action<int> setStateInternal;

        public SettleRunnable()
        {
        }

        public SettleRunnable(View view, int targetState, ViewDragHelper viewDragHelper, Action<int> setStateInternal) : base(JNIEnv.StartCreateInstance("mono/java/lang/Runnable", "()V"),
                                                                                                                              JniHandleOwnership.TransferLocalRef)
        {
            JNIEnv.FinishCreateInstance(Handle, "()V");

            this.setStateInternal = setStateInternal;
            this.view = view;
            this.targetState = targetState;
            this.viewDragHelper = viewDragHelper;
        }

        public void Run()
        {
            if (viewDragHelper != null && viewDragHelper.ContinueSettling(true))
            {
                ViewCompat.PostOnAnimation(view, this);
            }
            else
            {
                setStateInternal(targetState);
            }
        }
    }
}