# RG_ExtendedSave
A (limited) port of the ExtendedSave/ExtensibleSaveFormat plugins in [BepisPlugins](https://github.com/IllusionMods/BepisPlugins/)

## Features
- Hooks into Save & Load methods for characters & outfits to provide support for extended save data
- Provides API to add data to & read from character cards & outfits via `ExtendedSave` in `RG_ExtendedSave.dll`

## Limitations
- Currently `PluginData` only supports string values for its dictionary (fix in progress)
- No support for the `Extensions` methods in the BepisPlugins original (fix impossible?)
- No studio support yet

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
