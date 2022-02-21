# Ecconias Logic World Mod Collection

In this repository you will find all mods created by @Ecconia for the game `Logic World`.

## How to install / use them?

This project contains one `project folder` for each mod.\
In each of these folder you will find descriptions and further instructions like this one.\
Inside each `project folder` you will find another folder, which is the actual `mod folder`.\
You can copy/paste or `link` that `mod folder` to your `GameData` folder, where `Logic World` searches for mods.

You can ignore all other files outside of the `mod folder`s.

## WARNING: Logic World black screen! "Path is empty"

There is currently an issue with the `Logic World` compiler and `Harmony`.\
Quite a few `Logic World` mods are using the framework `Harmony`.\
But as soon as one of the mods which use `Harmony` is loaded, the `Logic World` compiler breaks.\
This means, you can only compile new mods, as long as no other mod which uses `Harmony` is loaded.

-> If you add a new mod, there may be no mod which uses `Harmony` in your `GameData` folder!\
-> The new mod may use `Harmony`.
-> To compile/add more than one new mod which uses `Harmony` you have to add/compile them one by one.

You may have `Harmony` using mods in `GameData`, if you know, that they will be loaded after all compilation is done.

### How to contribute / develop on these?

Download this repository or optionally a fork.\
Create a `link` named `LogicWorld` from within this projects root folder to your `Logic World` installation directory. (Probably in your Steam installations folder `[...]/steamapps/common/Logic World/`).\
Open the repository with your favorite C# IDE and hope that it works.

### Contact:

If you have questions, you can find me @Ecconia on the official `Logic World` discord server.\
Just sent me a message :)

You can also join my [Development Discord Server](https://discord.com/invite/dYYxNvp) to find me.\
Alternatively use GitHubs Discussions system.
