# XBottomSheet

## Build Status

| XBottomSheet.Touch  | XBottomSheet.Droid | .Touch.Sample  | .Droid.Sample |
| ------------- | ------------- | ------------- | ------------- |
|![Build status](https://build.appcenter.ms/v0.1/apps/2a487f1b-e2e0-470a-8613-691b971bf67e/branches/master/badge)|![Build status](https://build.appcenter.ms/v0.1/apps/7776b9f9-6d94-46ee-a52b-308a95474f4f/branches/master/badge)|![Build status](https://build.appcenter.ms/v0.1/apps/66402bbe-a256-4709-91e3-6a4edeedaa69/branches/master/badge)|![Build status](https://build.appcenter.ms/v0.1/apps/a0eaabee-958e-4df9-ad0a-df6810756f96/branches/master/badge)|

## Setup 

On client projects install the nuget XBottomSheet ([![NuGet](https://img.shields.io/nuget/v/XBottomSheet.svg?label=NuGet)](https://www.nuget.org/packages/XBottomSheet/)) and then follow the steps based on platform:

### iOS

#### Standard

1. Create a new ViewController of type BottomSheetViewController within the ViewController that you want to add it to:

```
public override void ViewDidLoad()
{
    base.ViewDidLoad();
    var bottomSheetViewController = new BottomSheetViewController(100, 300, 500, true, BottomSheetState.Bottom);
}
```
For more details on options for the constructors or their parameters, please check the implementation.

2. Add the newly created ViewController as a child to the current one:

```
    AddChildViewController(bottomSheetViewController);
    View.AddSubview(bottomSheetViewController.View);
    bottomSheetViewController.DidMoveToParentViewController(this);
```

3. Define the Frame for the View of the BottomSheet control: 

```
    bottomSheetViewController.View.Frame = new CGRect(0, View.Frame.GetMaxY(), View.Frame.Width, View.Frame.Height);
```

4. Add your custom view within as until now it would be only a blue view that can be dragged around:

```
    var custom = new CustomViewController();
    bottomSheetViewController.SetCustomView(custom.View);
```

#### MvvmCross

After following the steps from previous way, continue with following:

1. Add a ViewModel for the CustomViewController
2. Associate the newly created ViewModel with the CustomViewController

```
    public partial class CustomViewController : MvxViewController<CustomViewModel>
```
3. Replace code from step 4 from 'Simple way' with:

```
    var vmRequest = MvxViewModelRequest.GetDefaultRequest(typeof(CustomViewModel));
    var customViewController = new MvxViewController().CreateViewControllerFor<CustomViewModel>(vmRequest) as CustomViewController;
    bottomSheetViewController.SetCustomView(customViewController.View);
```
4. As the previous step will only create the ViewControler, but not the ViewModel associated with it, add the following within your CustomViewControler constructor:

```
    public CustomViewController() : base("CustomViewController", null)
    {
        var loaderService = Mvx.Resolve<IMvxViewModelLoader>();
        var mvxViewModelRequest = MvxViewModelRequest.GetDefaultRequest(typeof(CustomViewModel));
        ViewModel = loaderService.LoadViewModel(mvxViewModelRequest, null) as CustomViewModel;
    }
```

5. Now wherever you want to access or pass values to your CustomViewController ViewModel you can use something like this:

```
    customViewController.ViewModel.CustomValue = customValue;
```
Check the Touch.MSample for actual sample on how a value is passed in between.

### Android

#### Standard
1. You can use this view by adding it within your layout:

```
   <XBottomSheet.Droid.Views.XBottomSheetView
	android:id="@+id/BottomSheet"
	app:anchorOffset="320dp"
	app:peekHeight="192dp"
	app:defaultState="collapsed"
	android:layout_width="match_parent"
	android:layout_height="match_parent">
   </XBottomSheet.Droid.Views.XBottomSheetView>
```
2. In order to use a custom view you will need to inflate a desired Android layout file (ex. CustomView.axml) and assign it to the ContentView property of XBottomSheetView:

```
    var bottomSheetView = FindViewById<XBottomSheetView>(Resource.Id.BottomSheet);
    var customView = LayoutInflater.Inflate(Resource.Layout.CustomView, null);
    
    bottomSheetView.ContentView = customView;
    bottomSheetView.BackgroundColor = Color.Transparent;
```

#### MvvmCross

After following previous steps, in order to bind the ViewModel with your CustomView you'll need to replace LayoutInflater.Inflate with BindingInflate

```
   var customView = this.BindingInflate(Resource.Layout.CustomView, null);
```

## Demo

### iOS

![](demo_xbottomsheet.gif)

### Android

## Known issues/difficulties

- If you add this controll with **GMSMapView** or other similar feature that has its own gesture management it is possible to prevent XBottomSheet to trigger the **PanGesture** method, even if it registers it. For GMSMapView, the fix is to have "**mapView.Settings.ConsumesGesturesInView = false;**", where mapView is your GMSMapView object.

## Conclusion

### If you have a question or a suggestion, please add an issue and we'll discuss over it. We're open to respond, add new features, fine tune our solutions or, last, but most important, to fix bugs/problems that you encounter. 
#### As you've got this far and our code might helped you, support us to build more content like this through: 
<a href="https://www.buymeacoffee.com/grendio" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>
