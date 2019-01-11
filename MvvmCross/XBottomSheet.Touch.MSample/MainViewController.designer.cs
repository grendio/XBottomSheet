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
    [Register ("MainViewController")]
    partial class MainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btTest { get; set; }

        [Action ("TestButtonAction:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TestButtonAction (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btTest != null) {
                btTest.Dispose ();
                btTest = null;
            }
        }
    }
}