# EccsGuiBuilder by @Ecconia

A library mod that allows other mods to more easily create LW-(Edit-)Windows and other GUI elements.

**This mod is strictly limited to the game-play phase of the game!** It will not be functional outside of a loaded world!

This mod will try to initialize itself, even if no other mod uses it.

## Install / Dependencies:

Just drop the `EccsGuiBuilder` folder into your `GameData` folder.

You will also need the mod `EccsLogicWorldAPI` for this mod to function.\
You can find it in my mod collection (root folder).

## General contents of this mod:

This mod has several purposes:

- Create ready-to-use components AND extract components from LWs-Gameplay-Phase. 
- Provide custom layouts to make smooth dynamic GUIs. (Instead of UIs where text and elements go out of bounds).
- Provide the `Wrapper` framework, which allows to simply throw GUI together without much code overhead.

The classes `VanillaStore` and `CustomStore` provide you with spicy components ready to use. You might however have to adjust them here and there for your specific purpose.

Make use of the `GapListLayout`, a list layout (vertical/horizontal), where one defined gap between components can grow to fill the parents size. The child expand functionality will not fill the parent fully, but only let children grow to the size of the biggest child. (This is different from the Unity list layouts).

The `Wrapper` framework basically wraps (almost) all of the components in the component store classes, you can easily access wrappers via the `WS` class.\
Wrappers can be chained to huge method chains, look at mods using this framework.\
The framework also comes with a build method, to automatically initialize LW (and custom) classes.\
And an injection framework to automatically initialize fields in your MonoBehaviors.

### Want to learn more? Look at the source code - lol

Mods that are using this mod are:
- CustomWirePlacer - has one non-editing window and three non-window overlays
- EcconiaCPUServerComponents - has two component edit windows
- CheeseRamMenu - contains one edit component window with dynamic size

It is worth looking at all of them to see how this mod can be utilized.

It comes with a big advantage if you know how Unity Layouts are working. If you have any questions, just ask @Ecconia on Discord.
