using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using XBottomSheet.Droid.Views;

namespace XBottomSheet.Droid.Sample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }
    }
}