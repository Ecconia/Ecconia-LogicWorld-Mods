# RandomDebugCollection by @Ecconia

This mod provides a set of tools and features, to ramp up your modding experience with LogicWorld.

Features:
- Provides client startup argument `-version <Version>` to override the version LW thinks it is.
- Adds some random debugging commands (details below)...

## Install / Dependencies:

Just drop the `RandomDebugCollection` folder into your `GameData` folder.

You will also need the three mods `HarmonyForLogicWorld` and `AssemblyLoader` and `EccsLogicWorldAPI` for this mod to function.\
You can find them in my mod collection (root folder).

## Usage:

### Version hack:

On the client you can simply start it with argument `-version <Version>` to make the client think it is a different version.\
On the server, you will need to uncomment a line in the server mod class for this effect.

Just keep in mind, that until mods are loaded the client/server will assume the correct version.\
Settings will update, cache will be deleted and the little version text in the bottom right will show the actual version.\
But when joining servers the version you supplied applies.

This feature is mostly important while developing new LW versions, not sure what modders need it for.

### Commands:

This mod has a few commands:

- `ClearHistory`: Clears your action history, you will no longer be able to undo then. Added out of curiosity, if it would free memory.
- `ClearSubassemblies`: Clears stored subassemblies, there exists an official command for that, but its broken. Also added to see if it frees memory.
- `CollectGarbage`: Runs the garbage collection. Added also for memory investigation reasons.
- `Theme`: Reloads the current color theme. Useful when you are designing a layout and don't want to restart.
