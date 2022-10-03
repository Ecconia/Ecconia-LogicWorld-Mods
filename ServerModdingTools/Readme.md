# ServerModdingTools by @Ecconia

### Description:

A collection of things handy server tools for other mods to use.

Currently:

- A utility for easily replacing an IPacketHandler (handle packets yourself). [Can clash with other mods].
- A player joined hook. Gets called as soon as a player joined the server, handy for sending server-greetings.

It is made for the server and only runs there.

# Install:

Just drop the `ServerModdingSuite` folder into the `GameData` folder.

You will also need the mods `HarmonyForServers` and `AssemblyLoader` for this mod to function.\
If you are running this mod on a server, you should also install `ServerOnlyMods`, to allow players to join without this mod.\
You can find them in my mod collection (root folder).
