# AssemblyLoader by @Ecconia

### Description:

This mod allows to load DLL-Assembly files into the runtime. It works on server and client.

Is is purely to support other mods. *"When will I ever load something other than Harmony??" - Never!*

### Why does it exist?

Normally you could easily load an assembly like this: `Assembly.Load(modFile.readAllBytes())`.\
But the LogicWorld compiler breaks, if any loaded assembly has no file-location.\
Even worse, you can't just skip assemblies without location when compiling, if you depend on them.

# Install:

Just drop the `AssemblyLoader` folder into the `GameData` folder.

You will also need the mod `ServerOnlyMods` for this mod to function.\
You can find them in my mod collection (root folder).
