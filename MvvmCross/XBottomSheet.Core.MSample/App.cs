using MvvmCross.ViewModels;
using XBottomSheet.Core.MSample.ViewModels;

namespace XBottomSheet.Core.MSample
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<MainViewModel>();
        }
    }
}