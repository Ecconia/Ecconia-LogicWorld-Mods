# Ecconias Logic World Mod Collection

In this repository you will find all mods created by @Ecconia for the game `Logic World`.

## How to install / use them?

This project contains one `project folder` for each mod.\
In each of these folder you will find descriptions and further instructions like this one.\
Inside each `project folder` you will find another folder, which is the actual `mod folder`.\
You can copy/paste or `link` that `mod folder` to your `GameData` folder, where `Logic World` searches for mods.

You can ignore all other files outside of the `mod folder`s.

## Logic World black screen: "Path is empty" => Install the fix mod!

There is currently an issue with the `Logic World` compiler and `Harmony`.\
Quite a few `Logic World` mods are using the framework `Harmony`.\
But as soon as one of the mods which use `Harmony` is loaded, the `Logic World` compiler breaks.\
This means, you can only compile new mods, as long as no other mod which uses `Harmony` is loaded.

**But even for this there is a mod! Install `FixCompilerOnClient` and/or `FixCompilerOnServer`** depend on where the issue occurs.\
You can also install them if the issue does not happen, then you are prepared for the future.

## List of mods:

#### Client:

- `DisableCollision` **Must have!**: Allows you to build even when LogicWorld does not want you to. (On floating point issues, or when refactoring buildings).
- `CustomWirePlacer` **Must have!**: Replaces the vanilla wire placer with a power-user tool (that you won't want to miss later on).
- `FlyingWithAutoNoclip` **Must have!**: Fixes a bug which prevents you from using LogicWorlds `noclip while flying` super secret setting.
- `SimulationControl`: Adds a console command `tps`, which is much less complicated than `server "simulation.rate <number>"`. 
- `RandomDebugCollection`: Prints stack traces (that happen when going to error screen) to logs and prints compiler errors, which normally require `loglevel trace`.
- `FixCompilerOnClient`: Fixes the compiler from breaking if Harmony is loaded.
- `CustomSkybox`: Experimental tool, that allows you to load custom skyboxes from an image. "Works for me" - Ecconia.

##### Libraries:

- `DllUtil`: Loads DLL files (like for example Harmony) into runtime.
- `LWHarmony`: Loads Harmony into the runtime.
- `EccsWindowHelper`: Allows mods to more easily create LogicWorld-Styled windows (menus).

#### Server:

- `CustomChatManager`: Prevents players from abusing `sayraw` and adds chat commands, such as `/list` and `/tps` which allows anyone in multiplayer to control the simulation. 
- `FixCompilerOnServer`: Fixes the compiler from breaking if Harmony is loaded.

##### Libraries:

- `ServerOnlyMods`: Allows you to develop mods for the server, that the client does not need to install.
- `AssemblyLoader`: Loads DLL files (like for example Harmony) into runtime.
- `HarmonyForServers`: Loads Harmony into the runtime.
- `ServerModdingTools`: Very small collection of convenience features.

#### Both:

- `EcconiaCPUServerComponents`: Components that I use in my text-editor project. My favorite is the custom key. As for it as a standalone mod :)

### How to contribute / develop on these?

Download this repository or optionally a fork.\
Create a `link` named `LogicWorld` from within this projects root folder to your `Logic World` installation directory. (Probably in your Steam installations folder `[...]/steamapps/common/Logic World/`).\
Open the repository with your favorite C# IDE and hope that it works. (This repository is created with JetBrains Rider).

### Contact:

If you have questions, you can find me @Ecconia on the official `Logic World` discord server.\
Just sent me a message :)

You can also join my [Development Discord Server](https://discord.com/invite/dYYxNvp) to find me.\
Alternatively use GitHubs Discussions system.
