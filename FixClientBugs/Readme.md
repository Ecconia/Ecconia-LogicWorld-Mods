# FixClientBugs by @Ecconia

### Description:

`Logic World` is still in its early days (Version 0.90.3), and currently there are a few bugs, that really ruin the fun.

This mod will fix some of the bugs, with the help of `Harmony`.

#### List of bugs:

- <b>Flying with `noclip`?</b>

    When flying `noclip` should be enabled by default. And there is even a setting to archive this: `MHG.Flying.Secret.AutoEnterNoclipOnStartFlying`.\
So what is the problem? Well, with this setting enabled, `Logic World` will freeze because of a StackOverflow. And be unplayable.\
This mod fixes the issue that two methods to call itself constantly (`setNoclip` & `setFlying`).

- <b>Undo runs amok</b>:

    When applying operations on multiple components, LogicWorld will store all of them on the client. But it does a mistake, while indicating that a multi-history-entry operation is about to start.\
This causes undo to undo every action until the next multi-component history-entry. Or the history index becomes negative. In any case things go wrong and an error is thrown.\
This mod fixes the issue, that the multi-entry history starting tag is written on top of the stack and not at the stack-pointer location.

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
