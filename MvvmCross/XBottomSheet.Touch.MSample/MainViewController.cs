﻿using System;
using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;
using XBottomSheet.Core.MSample.ViewModels;
using XBottomSheet.Touch.Views;

namespace XBottomSheet.Touch.MSample
{
    [MvxRootPresentation]
    public partial class MainViewController : MvxViewController<MainViewModel>
    {
        BottomSheetViewController bottomSheetViewController;
        CustomView customView;

        public MainViewController() : base("MainViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupBottomSheet();
        }

        private void SetupBottomSheet()
        {
            // Create BottomSheetViewController
            var bottom = UIScreen.MainScreen.Bounds.Height - UIApplication.SharedApplication.StatusBarFrame.Height;
            bottomSheetViewController = new BottomSheetViewController(100, 300, bottom, true, BottomSheetState.Bottom);

            // Add BottomSheetViewController as a child view 
            AddChildViewController(bottomSheetViewController);
            View.AddSubview(bottomSheetViewController.View);
            bottomSheetViewController.DidMoveToParentViewController(this);

            // BottomSheetViewController frame
            bottomSheetViewController.View.Frame = new CGRect(0, View.Frame.GetMaxY(), View.Frame.Width, View.Frame.Height);

            customView = CustomView.Create();
            customView.Frame = View.Frame;

            var set = this.CreateBindingSet<MainViewController, MainViewModel>();
            set.Bind(customView.CustomValue).For(t => t.Text).To(vm => vm.CustomValue);
            set.Apply();

            bottomSheetViewController.SetCustomView(customView);
        }

        partial void TestButtonAction(UIButton sender)
        {
            var rnd = new Random();
            int month = rnd.Next(1, 13);
            ViewModel.CustomValue = rnd.Next(1, 1000).ToString();
        }
    }
}