# RandomDebugCollection by @Ecconia

### Description:

Reasons for creation of this mod:

- Stacktraces are not printed to logs.
- Commands which might be needed for random debugging actions, can be dumped in here.
- Not being able to directly load a world by argument when starting the game.
- Compilation errors are not printed into logs with loglevel information.

This mod captures all exceptions which would cause the error-screen and prints them to the logs (using the Logger).\
This mod prints C# script compilation errors to console.\
This mod provides an application argument to directly load a world.\
This mod adds a bunch of commands, see `Usages` below.

## Install / Dependencies:

Just drop the `RandomDebugCollection` folder into your `GameData` folder.

You will also need the two mods `LWHarmony` and `DllUtil` by @FalsePattern for this mod to function.\
You can find them in my mod collection (root folder).

## Usage:

This mod has a few commands:

- `ClearHistory`: Clears your action history, you will no longer be able to undo then. Added out of curiosity, if it would free memory.
- `ClearSubassemblies`: Clears stored subassemblies, there exists an official command for that, but its broken. Also added to see if it frees memory.
- `CollectGarbage`: Runs the garbage collection. Added also for memory investigation reasons.
