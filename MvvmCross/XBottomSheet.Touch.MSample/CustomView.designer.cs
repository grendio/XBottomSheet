// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace XBottomSheet.Touch.MSample
{
    [Register ("CustomView")]
    partial class CustomView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbCustomValue { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbCustomValue != null) {
                lbCustomValue.Dispose ();
                lbCustomValue = null;
            }
        }
    }
}