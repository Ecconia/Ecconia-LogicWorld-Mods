# FlexibleComponentModUsage by @Ecconia

### Description:

You can install all the component mods you need.\
With this mod, components will be hidden from the selection window, when you join a server that does not have them installed.\
Which makes choosing components much easier. And you won't be confused when placing one that cannot be placed.

Previously this mod also prevent crashes, but these are no longer an issue since LW 0.91.3.

### What is the problem?

You can join servers that do not have components which you have installed.\
But you cannot place the components that are not installed.\
And worse, you will crash, when one player undoes a container component with components on it.

### Heads up! Priority: -50

This mod MUST load after other component mods, else their thumbnail won't render and you will crash.

This is because it accesses a class in charge of ComponentPrefabVariants - which triggers it to scan for Prefab classes. This is not done again after this mod is initialized.\
Also, because other component mods, might register Component Action API handlers, that must happen first.

Make sure your component mod has a priority higher than `-50`. But if your component mod does not have priority `0`, you are probably doing something wrong anyway.

## Install / Dependencies:

Just drop the `FlexibleComponentModUsage` folder into your `GameData` folder.

For this mod to function you will need one additional mod: `EccsLogicWorldAPI`\
You can find it in my mod collection (root folder).
