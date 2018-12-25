# XBottomSheet

## Build Status

| XBottomSheet.Touch  | XBottomSheet.Droid | .Touch.Sample  | .Droid.Sample |
| ------------- | ------------- | ------------- | ------------- |
|![Build status](https://build.appcenter.ms/v0.1/apps/2a487f1b-e2e0-470a-8613-691b971bf67e/branches/master/badge)|![Build status](https://build.appcenter.ms/v0.1/apps/7776b9f9-6d94-46ee-a52b-308a95474f4f/branches/master/badge)|![Build status](https://build.appcenter.ms/v0.1/apps/66402bbe-a256-4709-91e3-6a4edeedaa69/branches/master/badge)|![Build status](https://build.appcenter.ms/v0.1/apps/a0eaabee-958e-4df9-ad0a-df6810756f96/branches/master/badge)|

## Setup 

On client projects install the nuget XBotomSheet ([![NuGet](https://img.shields.io/nuget/v/XBottomSheet.svg?label=NuGet)](https://www.nuget.org/packages/XBottomSheet/)) and then follow the steps based on platform:

### iOS

1. Create a new ViewController of type BottomSheetViewController within the ViewController that you want to add it to:

```
public override void ViewDidLoad()
{
    base.ViewDidLoad();
    var bottomSheetViewController = new BottomSheetViewController(100, 300, bottom, true, BottomSheetState.Bottom);
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

4. Add your custom view wihin as until now it would be only a blue view that can be dragged around:

```
    var custom = new CustomViewController();
    bottomSheetViewController.SetCustomView(custom.View);
```

### Android


## Demo

### iOS

### Android
