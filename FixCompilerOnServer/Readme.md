# FixCompilerOnServer by @Ecconia

### Description:

`Logic World` still suffers from bad frameworks loaded by modders.

Of of these bad frameworks is `Harmony`, a mandatory tool for modding C# properly.\
But as soon as a mod using Harmony is loaded, the compiler breaks and no other mod can be compiled.\
This makes running many mods tedious.

With this mod, you can however use all the mods at the same time without running into this issue.

**WARNING: Even with this mod the LW server WILL CRASH, the first time this mod gets compiled.**\
Only if you are compiling other mods at the same time. But after that one last crash this mod will be functional.

### Technical:

Harmony creates and injects an assembly called `HarmonySolidState`, to manage itself.\
But this assembly has no location on disk. Yet it is not marked as dynamic - as it should be.\
The LogicWorld compiler does not expect a rule-breaker and only filters runtime assemblies which are dynamic.\
This causes it to crash when compiling a mod, while a Harmony mod has already been loaded.

## Install / Dependencies:

Just drop the `FixCompilerOnServer` folder into your `GameData` folder.

You will also need the three mods `ServerOnlyMods` and `HarmonyForServers` and `AssemblyLoader` for this mod to function.\
You can find them in my mod collection (root folder).
