# RemoveUnusedComponentsOnSave by @Ecconia

### Description:

Currently as of version `0.90.3`, LogicWorld always includes every installed component into the current save-file.
 Regardless if such a component is placed in the world or not.\
This has the downside, that one has to remove mods to join such a world without changing the component-list.\
If this is not done, the world will have an exclamation mark and you need to force-load it.\
Sharing such a world also causes confusion for other players.

This mod will whenever LogicWorld creates a save check which components are actually placed and will skip every component in the save-file that is not placed.

It is made for the server and only runs there.

## WARNING:

This mod changes the default behavior of how `LogicWorld` saves files.\
This is currently supported, but it might not always be the case.\
Further there might be unforeseen side-effects which might break your save.\
**It "should" work, but I do not guarantee that it won't break your save!** 

Although I assume that in future, this behavior gonna be a part of the base-game. It better be!

### How does it work?

Whenever the save file bytes are generated, and the header with the component-id-map is written,
 this mod hooks itself into this process and replaces the component-id-map to be written, with a custom one.\
To create a new one, it loops once over all components placed in the world and collects the IDs present.\
Then it just creates a new component-id-map with the collected IDs.

This adds another loop over all components to your saving process, it will slow it down - slightly probably.

# Install:

Just drop the `RemoveUnusedComponentsOnSave` folder into the `GameData` folder.

You will also need the mods `ServerOnlyMods` and `HarmonyForServers` and `AssemblyLoader` for this mod to function.\
You can find them in my mod collection (root folder).
