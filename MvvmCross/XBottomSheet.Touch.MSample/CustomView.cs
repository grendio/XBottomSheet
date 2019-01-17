using Foundation;
using System;
using UIKit;

namespace XBottomSheet.Touch.MSample
{
    public partial class CustomView : UIView
    {
        public CustomView(IntPtr handle) : base(handle)
        {
        }

        public static CustomView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("CustomView", null, null);
            var v = arr.GetItem<CustomView>(0);
            return v;
        }

        public UILabel CustomValue
        {
            get
            {
                return lbCustomValue;
            }
            set
            {
                lbCustomValue = value;
            }
        }
    }
}