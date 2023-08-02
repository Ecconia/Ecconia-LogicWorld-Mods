# FlexibleComponentModUsage by @Ecconia

### Description:

Lets you install all component mods that you need, and still let you join vanilla or servers with less components than you have, without crashing.

Will hide the components you cannot use from the component selection window.

### What is the problem?

You can join servers that do not have components which you have installed.\
But you cannot place the components that are not installed.\
And worse, you will crash, when one player undoes a container component with components on it.

### How does it work?

This happens, because LWs component system will add all components you have installed into its "awareness" system.\
But at some point (on `undo`), it compares the servers with the clients component mapping lists and detects a different, leading to a crash.

The fix that this mod uses is simple. When joining a server, it simply removes components from LWs component awareness system. It will re-add components on demand again.

### Heads up! Priority: -50

This mod MUST load after other component mods, else their thumbnail won't render and you will crash.

This is because it accesses a class in charge of ComponentPrefabVariants - which triggers it to scan for Prefab classes. This is not done again after this mod is initialized.

Make sure your component mod has a priority higher than `-50`. But if your component mod does not have priority `0`, you are probably doing something wrong anyway.

## Install / Dependencies:

Just drop the `FlexibleComponentModUsage` folder into your `GameData` folder.

For this mod to function you will need one additional mod: `EccsLogicWorldAPI`\
You can find it in my mod collection (root folder).
