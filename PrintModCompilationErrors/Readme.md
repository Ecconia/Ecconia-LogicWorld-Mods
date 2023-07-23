# PrintModCompilationErrors by @Ecconia

You are writing a mod, see a black screen and your logs only say `Failed to compile scripts`, but you are too lazy to change the `loglevel` to `trace` and back every time?\
Then install this mod!

This mod will print compilation errors with loglevel information into the log files.

It also filters `mscorelib` warnings, which the compiler outputs.\
If you have a big mod like CustomWirePlacer, it literally outputs the same `mscorelib` error 470 times.

## Install / Dependencies:

Just drop the `PrintModCompilationErrors` folder into your `GameData` folder.

You will also need the two mods `HarmonyForLogicWorld` and `AssemblyLoader` for this mod to function.\
You can find them in my mod collection (root folder).
