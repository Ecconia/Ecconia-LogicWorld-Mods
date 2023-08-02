# ServerOnlyMods by @Ecconia

### Description:

This mod allows to write mods which only run on the server and are not required on the client.

Currently when using a mod, that is only useful on servers and does nothing on the client, it has to be installed on the client nevertheless.
Because if it is not, any server will reject the client, that don't have the same mods as itself.

With this mod, clients can connect to the server without having the server-only mods installed!
These mods however must declare that they are optional on the client, see #Usage below for details.

A side effect of this mod is, that the list of missing mods also contains the mod version.\
This might be handy for some, even if you do not have server-only mods.

### Usage:

This mod is optional itself. You do not have to install it.\
But you should install it on any dedicated server, to allow mods to not be required on the client, if a mod desires this.

If you want your mod to be optional for clients, you only have to create an empty file `OnlyServerMods` (content does not matter), next to the `manifest.succ` file in the same folder.

**WARNING:** Keep in mind, that mods, which modify the network packets or add components, should **not** be server-only sided (causes issues).

### How does it work?

LogicWorld stores a list of `IClientVerifier`s in its `NetworkManager`, this list processes incoming connections, and if any rejects the connection one cannot join.\
This mod replaces the verifier in charge of checking the clients mod list, with a custom one.\
The custom mod list verifier checks mods like the normal one, but skips the ones which have the special file.

# Install:

Just drop the `ServerOnlyMods` folder into your (servers) `GameData` folder.

For this mod to function you will need one additional mod: `EccsLogicWorldAPI`\
You can find it in my mod collection (root folder).

Unless you want the version to be included in the missing mods list, you only need to add this mod, if you have server only mods.
