# FixClientBugs by @Ecconia

### Description:

`Logic World` is still in its early days (Version 0.90.3), and currently there are a few bugs, that really ruin the fun.

This mod will fix some of the mods using `Harmony`.

#### List of bugs:

- Flying with `noclip`?

When flying `noclip` should be enabled by default. And there is even a setting to archive this: `MHG.Flying.Secret.AutoEnterNoclipOnStartFlying`.\
So what is the problem? Well, with this setting enabled, `Logic World` will freeze because of a StackOverflow. And be unplayable.\
This mod fixes the issue that two methods to call itself constantly (`setNoclip` & `setFlying`).

## Install / Dependencies:

Just drop the `FixClientBugs` folder into your `GameData` folder.

You will also need the two mods `LWHarmony` and `DllUtil` by @FalsePattern for this mod to function.\
You can find them in my mod collection (root folder).

## Usage(s):

#### Flying fix:

To make use of the flying fix, you have to enable the "noclip while flying" hidden setting:

- Open the file `settings_master.succ`.
- Change the entry `MHG.Flying.Secret.AutoEnterNoclipOnStartFlying` to `true`.

Now when you fly, `noclip` will be enabled, and when you stop flying, it is disabled.

#### Other fixes:

These just apply by default.
