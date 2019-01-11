using MvvmCross;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using XBottomSheet.Core.MSample.ViewModels;

namespace XBottomSheet.Touch.MSample
{
    public partial class CustomViewController : MvxViewController<CustomViewModel>
    {
        public CustomViewController() : base("CustomViewController", null)
        {
            var loaderService = Mvx.Resolve<IMvxViewModelLoader>();
            var mvxViewModelRequest = MvxViewModelRequest.GetDefaultRequest(typeof(CustomViewModel));
            ViewModel = loaderService.LoadViewModel(mvxViewModelRequest, null) as CustomViewModel;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}