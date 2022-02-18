# FlyingWithAutoNoclip by @Ecconia

### Description:

`Logic World` is still in its early days, and as of now (Version: 0.93) still suffers under accidental StackOverflow errors.

When flying `noclip` should be enabled by default. And there is even a setting to archive this: `MHG.Flying.Secret.AutoEnterNoclipOnStartFlying`.\
So what is the problem? Well, with this setting enabled, `Logic World` will freeze because of a StackOverflow. And be unplayable.

But with this mod, the bug is fixed and there will no longer be a StackOverflow, with the setting enabled.

## Install / Dependencies:

Just drop the `FlyingWithAutoNoclip` folder into your `GameData` folder.

You will also need the two mods `LWHarmony` and `DllUtil` by @FalsePattern for this mod to function.\
You can find them in my mod collection (root folder).

## Usage:

- Open the file `settings_master.succ`.
- Change the entry `MHG.Flying.Secret.AutoEnterNoclipOnStartFlying` to `true`.

Now when you fly, `noclip` will be enabled, and when you stop flying, it is disabled.
