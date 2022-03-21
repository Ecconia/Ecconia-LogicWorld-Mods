# ServerOnlyMods by @Ecconia

### Description:

This mod allows to write mods which only run on the server and not on the client.

Currently when writing a mod, that is optional on the client. The server will reject the connection, because the client does not have that mod.\
With this mod your mod can implement the interface `ServerOnlyMods.Server.IServerSideOnlyMod`. And clients no longer need your mod installed.\
Clients however may have the mod installed, it becomes optional.

Keep in mind, that mods, which modify the network packets, should not be server-only sided (causes issues).

A side effect of this mod is, that the list of missing mods also contains the mod version.\
This might be handy for some, even if you do not have server-only mods.

### How does it work?

LogicWorld stores a list of `IClientVerifier`s in its `NetworkManager`, this list processes incoming connections, and if any rejects the connection one cannot join.\
This mod replaces the verifier in charge of checking the clients mod list, with a custom one.\
The custom mod list verifier checks mods like the normal one, but skips the ones which implement the interface.

# Install:

Just drop the `ServerOnlyMods` folder into your (servers) `GameData` folder.

Unless you want the version to be included in the missing mods list, you only need to add this mod, if another mod depends on it.
