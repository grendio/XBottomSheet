using Android.App;
using Android.OS;
using Android.Widget;
using XBottomSheet.Droid.Views;

namespace XBottomSheet.Droid.Sample
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var sheet = FindViewById<XBottomSheetView>(Resource.Id.BottomSheet);

            var collapse = FindViewById<Button>(Resource.Id.testButtonShowC);
            var expand = FindViewById<Button>(Resource.Id.testButtonShowE);
            var hide = FindViewById<Button>(Resource.Id.testButtonHide);
            var anchored = FindViewById<Button>(Resource.Id.testButtonShowA);

            collapse.Click += ShowCollapsed;
            expand.Click += ShowExpanded;
            hide.Click += Hide;
            anchored.Click += ShowAnchored;
        }

        private void ShowCollapsed(object sender, System.EventArgs e)
        {
            var sheet = FindViewById<XBottomSheetView>(Resource.Id.BottomSheet);
            sheet.State = "4";
        }
        private void ShowExpanded(object sender, System.EventArgs e)
        {
            var sheet = FindViewById<XBottomSheetView>(Resource.Id.BottomSheet);
            sheet.State = "3";
        }
        private void ShowAnchored(object sender, System.EventArgs e)
        {
            var sheet = FindViewById<XBottomSheetView>(Resource.Id.BottomSheet);
            sheet.State = "6";
        }
        private void Hide(object sender, System.EventArgs e)
        {
            var sheet = FindViewById<XBottomSheetView>(Resource.Id.BottomSheet);
            sheet.State = "5";
        }
    }
}