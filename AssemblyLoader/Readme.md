# AssemblyLoader by @Ecconia

### Description:

This mod allows to load DLL-Assembly files into the runtime. It works on server and client.

Is is purely to support other mods. *"When will I ever load something other than Harmony??" - Never!*

There are three core functionalities:

- Provide an easy way to let other mods load `ModFile`'s into the runtime.
- Automatically load all assemblies in the `assemblies` folder of mods that depend on this mod. Details below.
- Load all server assemblies, as it does not do that on its own. With this mods can depend on these assemblies

### Why does it exist?

Normally you could easily load an assembly like this: `Assembly.Load(modFile.readAllBytes())`.\
But the LogicWorld compiler breaks, if any loaded assembly has no file-location.\
Even worse, you can't just skip assemblies without location when compiling, if you depend on them.

### Automated assembly/dll loading:

If your mod depends on `AssemblyLoader`, assembly loader will try to find a `assemblies` folder in your mod.\
In it, there may be `client`, `server` and `shared` folders. Every `.dll` file inside these will be loaded.\
The `shared` folder is loaded on client and server. The `client` and `server` folder will only be loaded on the client and server side respectively.

# Install:

Just drop the `AssemblyLoader` folder into the `GameData` folder.

If you are running this mod on a server, you should also install `ServerOnlyMods`, to allow players to join without this mod.\
You can find it in my mod collection (root folder).
