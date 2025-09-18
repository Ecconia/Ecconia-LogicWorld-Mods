# WireTracer by @Ecconia

### Description:

Logic World is a great game when it comes to cooking. Nothing else is a better tool for your spaghetti wire connections. But how to you know where one single wire is leading to? What does power it, what will it power?

This plugin allows you to see through your spaghetti.\
But even if your wires are placed with the finest OCD, it will help you to follow wires more easily.

## Install / Dependencies:

Just drop the `WireTracer` folder into your `GameData` folder.

For this mod to function you will need one additional mod: `EccsLogicWorldAPI`\
You can find it in my mod collection (root folder).

#### Warning: WireTracer is not "client only":

This mod will behave as intended in SinglePlayer.

But it comes with a server part. When joining a MultiPlayer server, it will not have its full capability, unless the server has installed `WireTracer` as well.\
If a server does not have it installed, ask the host to install it :)

## Performance warning:

`WireTracer` tries to be quite fast, but to obtain some information, it does some tasks that take a small duration.\
This is fine, unless you try to trace HUGE clusters (size to be determined).

For clients this can cause minor FPS drops when activating this tool.\
But one can keep the FPS stable, by distributing the work on multiple frames.

On the server however tracing a big cluster can lower the max TPS for a short moment. As it is trying to collect all data for clusters.\
If this becomes an issue here, one can significantly increase the performance, by removing the highlighting of Sockets/ThroughPegs/Relays/FastBuffers.

Let the mod author @Ecconia know if either side becomes an issue, and the performance optimizations will be implemented.

## Usage:

To activate this tool, press the `I` key, while looking at a `peg` or a `wire`.\
You can change this keybinding in the control settings.

Once activated, it will highlight/outline all wires and pegs of the cluster you looked at and connected clusters via the link layer (active relays & fast buffers).
