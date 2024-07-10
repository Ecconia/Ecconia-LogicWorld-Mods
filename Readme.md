# Ecconias Logic World Mod Collection

In this repository you will find all mods created by @Ecconia for the game `Logic World`.

**All mods in this repository are for LogicWorld 0.91.3**\
To find mods for older LogicWorld versions, check out the GitHub tags of this repository.

## How to install / use them?

This project contains one `project folder` for each mod.\
In each of these folder you will find descriptions (`Readme.md`) and further instructions like this one.\
Inside each `project folder` you will find another folder, which is the actual `mod folder` (contains the `manifest.succ` file).\
You can copy/paste or `link` that `mod folder` to your `GameData` folder, where `Logic World` searches for mods.

You can ignore all other files outside of the `mod folder`s.

## List of mods:

#### Client:

- `DisableCollision` **Must have!**: Allows you to build even when LogicWorld does not want you to. (On floating-point/collider issues, or when refactoring buildings).
- `CustomWirePlacer` **Must have!**: Replaces the vanilla wire placer with a power-user tool (that you won't want to miss later on).
- `SimulationControl`: Adds a console command `tps`, which is much less complicated than `server "simulation.rate <number>"`.
- `RandomDebugCollection`: Provides small features for debugging, will display the stacktrace in logs and the error screen.
- `EcconiasChaosClientMod`: Mod made for Ecconia to dump tiny feature inside. Noteworthy: Custom skybox, command to list connected players.
- `FlexibleComponentModUsage`: **Useful** Mod that will hide components from your component window, when joining a server that does not have these components installed.
- `PrimitiveSelections`: Mod which allows you to multi select all child components of a selection.

Libraries:

- `EccsGuiBuilder`: Allows mods to more easily create LogicWorld-Styled windows/GUI (menus).

#### Server:

- `CustomChatManager`: Prevents players from abusing `sayraw` and adds chat commands, such as `/list` and `/tps` which allows anyone in multiplayer to control the simulation. 

Libraries:

- `ServerOnlyMods`: Allows you to develop mods for the server, that the client does not need to install.

#### Client | Server: (Installed on either or both sides)

- `WireTracer` **Must have!**: Allows you to easily see what powers wires and what they power in your spaghetti. If installed on the server, connected clusters are highlighted too.

Libraries:

- `AssemblyLoader`: Loads DLL files (like for example Harmony) into runtime.
- `HarmonyForLogicWorld`: Loads Harmony into the runtime.
- `EccsLogicWorldAPI`: A collection of useful tools for modders to make use of.

#### Client & Server: (Installed on both sides)

- `EcconiaCPUServerComponents`: Components that I use in my text-editor project. My favorite is the custom key. Ask for it as a standalone mod :)

### How to contribute / develop on these?

Download this repository or optionally a fork.\
Create a `symbolic link` named `LogicWorld` from within this projects root folder to your `Logic World` installation directory. (Probably in your Steam installations folder `[...]/steamapps/common/Logic World/`).\
Open the repository with your favorite C# IDE and hope that it works. (This repository is managed with JetBrains Rider).

### Contact:

If you have questions, you can find me @Ecconia on the official `Logic World` discord server.\
Just sent me a message :)

You can also join my [Development Discord Server](https://discord.com/invite/dYYxNvp) to find me.\
Alternatively use GitHubs Discussions system.
