# RadioConnection by @Ecconia

### Description:

This mod provides the component `RadioComponent`, which allows you to wirelessly connect clusters (pegs) with each other.

### Features:

In the edit window of the component you can choose one of 1024 channels to transmit your signals over it.\
Each channel can have up to 64 bits. Anything from 1 upwards can be configured in the component.\
If the first bits of a channels are not used by a component, they can be skipped.

Further if static channels are not appropriate, the channel can be chosen by upto 10 address pegs.\
If the address pegs define a channel address and the normal channel address is set, both will be added.

64 blocks too long? Enable compact mode, which squishes up to 3 pegs into one block.\
Peg order the wrong way? Flip them via the edit window.

### Optimization / Performance:

Linking pegs or clusters in LogicWorld with each other can be done with two different ways:

- `Secret link`s or "invisible wires" are one way to connect things. Wires are 0% load for the simulation. They do not matter, they just connect clusters.\
  However adding and removing secret links and wires in general is a huge calculation effort, it gets worse the more pegs and wires you have connected with each other - very slow.
- `Phasic link`s LogicWorlds solution for quickly adding and removing connections between pegs. Used by relays and fast-buffers. Can be quickly edited without needing much time.\
  However these are not free for the simulation and take a little bit of performance.

The `RadioComponent` lets you choose which of the two types of links it uses. Which one should be chosen?

In general `Secret links` should be preferred, as they are 0% load for the simulation.\
While editing the component you will have exactly the same amount of performance as when placing/deleting manual wires.

But the `RadioComponent` has the ability to change its channel via input pegs.\
If players are controlling these pegs via buttons and levers `Secret links` are sufficient. As players are slow in CPU speed.

When however the simulation (your circuit) controls the channel, without the players interaction you should choose `Phasic link`s.\
`Phasic link`s allow you to change the channel of your `RadioComponent` over thousand times per second, without much performance penalty.\
If `Secret link`s are used instead, for each channel switch and each bit a secret wire has to be removed and added again, which is very very expensive (in terms of time). This will decrease the maximum amount of possible TPS on your server/world.

### Known issues:

- The edit window flickers once when the address pegs are changed from 0 to something else.
- When changing peg counts, sounds of wire destruction/placement will be played.
- By design - as pegs are centered - they are not always properly aligned with the grid.
- Due to the mod internally using virtual pegs (which need a fake-component-address), whenever you load the world / start the server, your component address counter will be incremented by 1. (Not really a big issue, this happens every time when you place something). 

## Install / Dependencies:

Just drop the `RadioConnection` & `RadioConnectionGui` folders into your `GameData` folder.

For `RadioConnection` & `RadioConnectionGui` to function, you will need four additional mods: `EccsGuiBuilder` & `EccsLogicWorldAPI` & `AssemblyLoader` & `LogicWorldForHarmony`\
You can find them in my mod collection (root folder).
