# EcconiasChaosClientMod by @Ecconia

### Description:

This mod adds several random experimental or minor commands/features to the client.

Not all are easy to use. But some are super useful.

Mind the "Chaos" in the name. I can add random (passive) features to this at any time, which might not be meant to be used or difficult to use. But none will disrupt your normal gameplay without you activating it.

# Install:

Just drop the `EcconiasChaosClientMod` folder into the servers `GameData` folder.\
And add your custom skybox into the `skyboxes` folder.

For this mod to function you will need one additional mod: `EccsLogicWorldAPI`\
You can find it in my mod collection (root folder).

### List of features:

- Commands to list the currently connected players `listplayers`, or `listplayersall` if you like a more technical view.
- Command `rainbowchat` that sends the text argument in chat with rainbow colors (keep in mind, that there is a chat character limit, and color codes count and are big and per letter).
- Command `setcircuitcolor` will change the circuit color, to whatever you wish away from the default red. You can even modify the `ModClass` to have a new default color on game start.

There are a set of commands to modify your skybox and world light settings:
- Command `skybox` shows a custom skybox, you have to add ones first. Read further instructions in the readme found in the `skyboxes` folder.
- Command `lightambient`, changes the ambient color intensity.
- Command `lightsun`, changes the sun color intensity.
- Command `lightpreset`, activates skybox (if set), sets ambient and sun color intensity of previously set values. Change them in code if you want a different one.

There are a set of commands to modify the component you are looking at or the components you have multi-selected:
- Command `thisisblack`, will make components pitch black - useful if you need a black board.
- Command `thisisone`, will set the delay of `Delayer`s to be 1 tick - the shape of a Delayer is better than the one of Buffers anyway.
- Command `thisissideways`, will rotate AND gates by 90° around their peg axis. Allows the creation of AND gates on their side.
