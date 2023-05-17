# Ecconias Logic World Mod Collection

In this repository you will find all mods created by @Ecconia for the game `Logic World`.

**All mods in this repository are for LogicWorld 0.91.1**\
To find mods for older LogicWorld versions, check out the GitHub tags of this repository.

If you still use 0.91.0 (you should not). Get `FixClientBugs`, if you do not want bad crashes while building.\
Besides `EcconiasChaosClientMod` (player list command broken), all mods are still compatible.

## How to install / use them?

This project contains one `project folder` for each mod.\
In each of these folder you will find descriptions (`Readme.md`) and further instructions like this one.\
Inside each `project folder` you will find another folder, which is the actual `mod folder` (contains the `manifest.succ` file).\
You can copy/paste or `link` that `mod folder` to your `GameData` folder, where `Logic World` searches for mods.

You can ignore all other files outside of the `mod folder`s.

## No Harmony on the latest preview server!

Since LogicWorld has switched to .Net 7 on the server, `Harmony` is no longer functional.\
It loads perfectly fine and won't throw any error, but no patch will be applied.\
Time will solve this issue, as one day it will support .Net 7, but that time is not now.

However this means, that most of my server mods using Harmony won't do what they should, and are useless.\
The only exception is `CustomChatManager`, which just does not have a on-join message but everything else works (/tps command won't work too).

You can however download the dedicated server and use the `net 6` as your SP server folder. That way Harmony works again and you can use these mods.

## List of mods:

#### Client:

- `DisableCollision` **Must have!**: Allows you to build even when LogicWorld does not want you to. (On floating-point/collider issues, or when refactoring buildings).
- `CustomWirePlacer` **Must have!**: Replaces the vanilla wire placer with a power-user tool (that you won't want to miss later on).
- `SimulationControl`: Adds a console command `tps`, which is much less complicated than `server "simulation.rate <number>"`. 
- `RandomDebugCollection`: Prints stack traces (that happen when going to error screen) to logs and prints compiler errors, which normally require `loglevel trace`.
- `EcconiasChaosClientMod`: Mod made for Ecconia to dump tiny feature inside. Noteworthy: Custom skybox, command to list connected players.
- `FlexibleComponentModUsage`: **Useful** Mod that will allow you to join servers with less components than you - without crashing.

Libraries:

- `EccsWindowHelper`: Allows mods to more easily create LogicWorld-Styled windows (menus).

#### Server:

- `RemoveUnusedComponentsOnSave`: Mod, which removes components from the component-id-map of a save, if a component is not placed in the world. **Use with care.**
- `CustomChatManager`: Prevents players from abusing `sayraw` and adds chat commands, such as `/list` and `/tps` which allows anyone in multiplayer to control the simulation. 

Libraries:

- `ServerOnlyMods`: Allows you to develop mods for the server, that the client does not need to install.
- `ServerModdingTools`: Very small collection of convenience features.

#### Client | Server: (Installed on either or both sides)

- `WireTracer` **Must have!**: Allows you to easily see what powers wires and what they power in your spaghetti. If installed on the server, connected clusters are highlighted too.

Libraries:

- `AssemblyLoader`: Loads DLL files (like for example Harmony) into runtime.
- `HarmonyForLogicWorld`: Loads Harmony into the runtime.

#### Client & Server: (Installed on both sides)

- `EcconiaCPUServerComponents`: Components that I use in my text-editor project. My favorite is the custom key. Ask for it as a standalone mod :)

### How to contribute / develop on these?

Download this repository or optionally a fork.\
Create a `link` named `LogicWorld` from within this projects root folder to your `Logic World` installation directory. (Probably in your Steam installations folder `[...]/steamapps/common/Logic World/`).\
Open the repository with your favorite C# IDE and hope that it works. (This repository is managed with JetBrains Rider).

### Contact:

If you have questions, you can find me @Ecconia on the official `Logic World` discord server.\
Just sent me a message :)

You can also join my [Development Discord Server](https://discord.com/invite/dYYxNvp) to find me.\
Alternatively use GitHubs Discussions system.
