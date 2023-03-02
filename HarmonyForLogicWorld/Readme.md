# HarmonyForLogicWorld by @Ecconia

### Description:

This mod allows to hijack `Logic World` client and server using `Harmony`, it is a library mod, which other mods can use.

If the mod `lwharmony` is installed, it is undefined, what will happen.\
As then there might be two different version of Harmony installed.

If you are a modder, you should replace `lwharmony` with this mod.\
The advantage is, that this mod runs on client and server and will be updated to the latest Harmony version.\
^Not yet possible on the client for some unknown reason.

### What does it do?

It only loads the `Harmony` DLL, using the `AssemblyLoader` mod. This way Harmony is only loaded once on the server.

# Install:

Just drop the `HarmonyForServers` folder into the `GameData` folder.

You will also need the mod `AssemblyLoader` for this mod to function.\
If you are running this mod on a server, you should also install `ServerOnlyMods`, to allow players to join without this mod.\
You can find them in my mod collection (root folder).
