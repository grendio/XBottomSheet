using MvvmCross;
using MvvmCross.Binding.BindingContext;
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

            var set = this.CreateBindingSet<CustomViewController, CustomViewModel>();
            set.Bind(lbTest).To(vm => vm.CustomValue);
            set.Apply();
        }
    }
}