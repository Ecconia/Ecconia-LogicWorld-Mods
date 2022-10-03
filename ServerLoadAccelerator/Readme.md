# ServerLoadAccelerator by @Ecconia

### Description:

This mod improves the world load speed of the `Logic World Server` using `Harmony`.

It is made for the server and only runs there.

### How does it work?

`Logic World` iterates over each wire when loading the world and places each into the world, one by one.\
This has the downside that some of the generates clusters will be removed and recreated identically over and over again.

This mod prevents the cluster creation while the wires are being added.\
And then creates the clusters after all wires have been added.\
But it skips all the wires, that already have a cluster assigned, to prevent the redundancy from happening.

A world that loads on the server for 25 minutes can with this mod load within 8 seconds.\
But such times are already in the extreme area, average worlds with ~150K wires load from 10 to 40 seconds,
 and even here can the loading time be at least halved.

Keep in mind, that this mod is only effective on wires. There might be other reasons for the server taking longer.

# Install:

Just drop the `ServerLoadAccelerator` folder into the `GameData` folder.

You will also need the mods `HarmonyForLogicWorld` and `AssemblyLoader` for this mod to function.\
If you are running this mod on a server, you should also install `ServerOnlyMods`, to allow players to join without this mod.\
You can find them in my mod collection (root folder).
