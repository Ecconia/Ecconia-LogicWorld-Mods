# HarmonyForLogicWorld by @Ecconia

### Description:

This mod allows to hijack `Logic World` client and server using `Harmony`, it is a library mod, which other mods can use.

If the mod `lwharmony` is installed, this mod will do nothing (it is nevertheless required as dependency).\
^The behavior might change in future, when updating to a newer Harmony version.

You should replace `lwharmony` with this mod, if possible. Since this mod loads on client and server.

### What does it do?

It only loads the `Harmony` DLL, using the `AssemblyLoader` mod. This way Harmony is only loaded once on the server.

# Install:

Just drop the `HarmonyForServers` folder into the `GameData` folder.

You will also need the mod `AssemblyLoader` for this mod to function.\
If you are running this mod on a server, you should also install `ServerOnlyMods`, to allow players to join without this mod.\
You can find them in my mod collection (root folder).
