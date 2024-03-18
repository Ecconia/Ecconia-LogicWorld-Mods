# RandomDebugCollection by @Ecconia

This mod provides a set of tools and features, to ramp up your modding experience with LogicWorld.

Features:
- Mod compilation errors will be printed in Console/Logs
  - Compilation errors are filtered, to remove some pointless errors (in big mods you can get hundreds of lines of pointless errors).
- Provides executable launch argument `-loadworld <WorldFolderName>` to directly load into a world, skipping main menu.
- Adds some random debugging commands (details below)...

## Install / Dependencies:

Just drop the `RandomDebugCollection` folder into your `GameData` folder.

You will also need the three mods `HarmonyForLogicWorld` and `AssemblyLoader` and `EccsLogicWorldAPI` for this mod to function.\
You can find them in my mod collection (root folder).

## Usage:

To directly join a world, skipping the main menu, execute the game with following argument:  
`-loadworld "<WorldSaveFolderName>"` Ensure, that you are providing the folder name! If the name contains spaces, wrap it with quotes.

This feature is compatible with Steam:  
`%command% -loadworld "<WorldSaveFolderName>"`

I have the following run commands (Linux) in my IDE, to start the game:  
`killall Logic_World; steam -applaunch 1054340 -loadworld "TestWorld" && exit`  
`killall Logic_World; steam steam://rungameid/1054340 && exit`  
It stops all running instances of the game.
Instructs Steam to start the game, optionally supplies a world to open.
Finally it closes the IDE terminal, as Steam launches a decoupled process.
For whatever reason I am using two different ways to start the game :P

This mod has a few commands:

- `ClearHistory`: Clears your action history, you will no longer be able to undo then. Added out of curiosity, if it would free memory.
- `ClearSubassemblies`: Clears stored subassemblies, there exists an official command for that, but its broken. Also added to see if it frees memory.
- `CollectGarbage`: Runs the garbage collection. Added also for memory investigation reasons.
