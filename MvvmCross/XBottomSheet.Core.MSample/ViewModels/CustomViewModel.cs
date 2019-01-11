using MvvmCross.ViewModels;

namespace XBottomSheet.Core.MSample.ViewModels
{
    public class CustomViewModel : MvxViewModel
    {
        private string customValue;
        public string CustomValue
        {
            get
            {
                return customValue;
            }
            set
            {
                customValue = value;
                RaisePropertyChanged(() => CustomValue);
            }
        }
    }
}