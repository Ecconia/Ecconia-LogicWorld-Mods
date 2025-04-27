# Subassembly Gui

### Description:

Adds a building operation for quickly naming and saving a subassembly.\
Adds a window for loading subassemblies into hotbar.

Reason for creation of this mod:

- Currently, `Logic World` added Subassemblies, but no GUI for it yet. The GUI will come, but until then we got this mod.
- Currently, there is only commands, GUI is easier to use.

The savings GUI is pretty much done. The loading GUI however could be significantly better, it currently only shows the title and no thumbnails or other details. And editing local subassemblies is not supported. Editing the Hotbar from the loading window won't be supported, that is for the vanilla implementation.

## Install / Dependencies:

Just drop the `SubassembliesGui` folder into your `GameData` folder.

For this mod to function, you will need two additional mod: `EccsLogicWorldAPI` & `EccsGuiBuilder`\
You can find them in my mod collection (root folder).

## Usage:

### Saving:

This mod adds a new Building Operation for saving a subassembly.

You can use that Building Operation via the Action Wheel while looking at a component or while having component(s) selected.\
Building Operations can be triggered via keybinding, this mod does not set a default keybinding. You can set one yourself in the keybinding setting. Just search for the header `Mod: Subassembly Gui`.

Personally I am using `Mod + S`.

### Loading:

This mod adds a new window for loading subassemblies.

You can only open this 'Load Subassembly' window by using a keybinding.\
This mod does not set a default keybinding for opening the loading window. You have to set one yourself in the keybinding setting. Just search for the header `Mod: Subassembly Gui`.

Personally I am using `Mod + Tab`.
