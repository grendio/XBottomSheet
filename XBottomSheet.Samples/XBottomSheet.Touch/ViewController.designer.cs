// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace XBottomSheet.Touch.Sample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton btMain { get; set; }

		[Outlet]
		UIKit.UIButton btTest { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btMain != null) {
				btMain.Dispose ();
				btMain = null;
			}

			if (btTest != null) {
				btTest.Dispose ();
				btTest = null;
			}
		}
	}
}
