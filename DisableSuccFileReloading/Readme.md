# DisableSuccFileReloading by @Ecconia

### Description:

From LogicWorld `0.91.X` onwards SUCC settings files get reloaded when they change on disk.\
This mod prevents SUCC from doing that.

#### What is it good for? (Multiple LW instances)

Other SUCC files already got reloaded. There is however one big problem with doing this.\
Starting multiple LW instances would cause them to have the same set of settings at all times.\
Since if you change one, the others will update too.\
This mod prevents the game from noticing the change (immediately).\
It still might read the files, if it finds a reason to.

#### What is it good for? (LW Preview)

The other important thing, which this mod was actually written for, is that the current `LW 0.91 preview 485` breaks on some Linux systems because it tries to reload a file while it is writing it. However `C#` uses the write flag on both file access, which no Operating System likes.

## Install / Dependencies:

Just drop the `DisableSuccFileReloading` folder into your `GameData` folder.

You will also need the two mods `HarmonyForLogicWorld` and `AssemblyLoader` by @FalsePattern for this mod to function.\
You can find them in my mod collection (root folder).
