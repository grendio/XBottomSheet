using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using XBottomSheet.Core.MSample.ViewModels;

namespace XBottomSheet.Touch.MSample
{
    [MvxRootPresentation]
    public partial class MainViewController : MvxViewController<MainViewModel>
    {
        public MainViewController() : base("MainViewController", null)
        {
        }
    }
}