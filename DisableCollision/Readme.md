# DisableCollision by @Ecconia

### Description:

Reasons for creation of this mod:

- Sometimes you cannot place buildings, because the game thinks they collide with each other (`0.90.X`) => Fixed by now!
- When you want to refactor or take a compact building apart, you will struggle to not delete wires by accident.
- The game unloads colliders in your reach, so you cannot place something without wires getting deleted (`0.91 Preview`) => Not currently.

But with this mod, you can do both actions, because `components` and `wires` will no longer have collision, hence they can be placed when normally collision would stop you from doing so.

## Install / Dependencies:

Just drop the `DisableCollision` folder into your `GameData` folder.

You will also need the two mods `HarmonyForLogicWorld` and `AssemblyLoader` for this mod to function.\
You can find them in my mod collection (root folder).

## Usage:

This mod can be enabled and disabled.

To toggle if it is active, run `ToggleCollision` in the `Logic World Console`. (You can also ignore the casing: `togglecollision`).

Once enabled, collision will no longer bother you.

# DO NOT MISUSE THIS MOD:

This mod was written only for these two situations, where you cannot build because of floating-point/collision mistakes by `Logic World`. Or when you want to take something apart. ONLY USE IT FOR THESE PURPOSES.

Do NOT use this mod to intentionally clip `components` into each other or `wires` through `components`.\
(If you however do this, and worse post images of that, always credit this mod, so that no misunderstandings exist. Else you are dead to me!)
