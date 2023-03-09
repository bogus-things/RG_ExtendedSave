# RG_ExtendedSave
A (limited) port of the ExtendedSave/ExtensibleSaveFormat plugins in [BepisPlugins](https://github.com/IllusionMods/BepisPlugins/)

## Features
- Hooks into Save & Load methods for characters & outfits to provide support for extended save data
- Provides API to add data to & read from character cards & outfits via `ExtendedSave` in `RG_ExtendedSave.dll`
- Provides API to add data to & read from additional character card classes via `Extensions`
- Exposes simple, subscribable save/load events for character & outfit cards via `Events`

## Limitations
- No studio support yet

## Usage

Check out the [TestController](https://github.com/bogus-things/RG_ExtendedSave/blob/develop/test/TestController.cs) for some examples on setting & getting plugin data.

### `PluginData`

`PluginData` is the primary class for storing extended save data for a plugin. It has two properties:
- `version` - An integer version you can set for comparing/handling older vs newer data
- `data` - A `string`, `object` dictionary you can use to manage your plugin data. Supported value types include strings and all primitives except for pointers

Using `PluginData` will usually look something like this:
```c#
PluginData myData = new PluginData();
myData.version = 1;
myData.data.add("myKey", "myValue");
myData.data.add("myOtherKey", 420);
```

### `ExtendedSave`

`ExtendedSave` provides several static methods for managing your plugin's extended data at the `ChaFile` level:
- `GetAllExtendedData(ChaFile)` - Returns the data for every plugin attached to the `ChaFile` as an instance of `ExtendedData` (which is just a `string`, `PluginData` dictionary)
- `GetExtendedDataById(ChaFile, string)` - Attempts to find & return the `PluginData` stored under the provided string key. Returns null if no data is found
- `SetExtendedDataById(ChaFile, string, Plugindata)` - Attaches `PluginData` to the specified `ChaFile`, under the provided string key

The above methods are also all available for `ChaFileCoordinate` -- i.e. `GetAllExtendedDataById(ChaFileCOordinate, string)` -- so you can attach data to clothing cards as well.

### `Extensions`

`Extensions` provides additional static methods for binding & managing plugin data on some of the classes which make up `ChaFile`:
- `GetAllExtendedData(obj)` - Returns the data for every plugin attached to the provided object as an instance of `ExtendedData` (which is just a `string`, `PluginData` dictionary)
- `TryGetExtendedDataById(obj, string, out PluginData)` - Attempts to retrieve the `PluginData` stored under the provided string key. Returns false if no data is found
- `SetExtendedDataById(obj, string, Plugindata)` - Attaches `PluginData` to the specified object, under the provided string key

The `Extensions` methods support the following types for `obj`:
- `ChaFileBody`
- `ChaFileFace`
- `ChaFileFace.EyesInfo`
- `ChaFileFace.MakeupInfo`
- `ChaFileHair`
- `ChaFileHair.PartsInfo`
- `ChaFileHair.PartsInfo.ColorInfo`
- `ChaFileClothes.PartsInfo`
- `ChaFileClothes.PartsInfo.ColorInfo`
- `ChaFileAccessory.PartsInfo`
- `ChaFileAccessory.PartsInfo.ColorInfo`

Additionally, extended data can be set for `ChaFileHair.PartsInfo.BundleInfo` using slightly different arguments:
- `GetAllExtendedData(ChaFileClothes.PartsInfo obj, int bundleId, string id)`
- `TryGetExtendedDataById(ChaFileClothes.PartsInfo obj, int bundleId, string id, out PluginData data)`
- `SetExtendedDataById(ChaFileClothes.PartsInfo obj, int bundleId, string id, PluginData data)`

### `Events`

`Events` exposes some basic save/load events which fire when character & outfit cards are saved or loaded:
- `CardBeingSaved` - On character card save
- `CardBeingLoaded` - On character card load
- `CoordinateBeingSaved` - On outfit card save
- `CoordinateBeingLoaded` - On outfit card load

You can subscribe to any of these events with a handler function that accepts a single `ChaFile` or `ChaFileCoordinate` argument depending on the event.

## Requirements
- BetterRepack >= R1.4 (untested on lower versions or other configurations)

## Installation
1. Download the plugin from [Releases](https://github.com/bogus-things/RG_ExtendedSave/releases) (Check the "Compatibility" section to ensure the plugin will work for you)
2. Extract the `BepInEx` folder from the `.zip` and place it in your game's root directory

## Reporting an issue
If you believe you've found a bug with RG_ExtendedSave, please use the following process to let me know!
1. Do your best to ensure the bug is with RG_ExtendedSave
    1. Check the log for errors referencing this specific plugin
    2. If the logs show errors for other plugins, disable those with KKManager and try again
    3. Disable RG_ExtendedSave with KKManager and see if the bug persists
2. Check [Issues](https://github.com/bogus-things/RG_ExtendedSave/issues) for any open issues (notably the pinned issues at the top)
    1. If you see an open issue for a bug matching the behavior you're seeing, please add a comment there instead of creating a new issue. And when adding a new comment, check the top post to confirm what information you should provide
    2. If you don't, feel free to create a new issue describing the bug you've found
3. When creating a new issue, please provide the following:
    1. A description of the behavior
    2. Your current BetterRepack version and your current RG_ExtendedSave version
    3. If you're able to reproduce the bug consistently, provide the steps you take to do so
    4. If there is an error in the logs, provide your game logs as an attached `.txt` file (please don't copy/paste it into the issue description)
  
## Contributing
If you'd like to contribute to feature development or bug fixing, pull requests are welcome! To set up your references:
1. Open the `RGExtendedSave.csproj` file in a text editor and grab the XML for the references
2. Create a `RGExtendedSave.csproj.user` file in the same directory, and drop in the references, and properly nest them in the XML. Something like this:
```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <ItemGroup>
    <Reference Include="Assembly-CSharp">
      <HintPath>C:\Path\To\My\BepInEx\Unhollowed\RoomGirl\unhollowed\Assembly-CSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    ...
  </ItemGroup>
</Project>
```
3. Edit the file paths for the references in the `.user` file to map to your game installation
