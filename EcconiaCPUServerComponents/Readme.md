# EcconiaCPUServerComponents by @Ecconia

## Current issue:

You get some great and some junk components with this mod. They should be split into two mods eventually.\
Be careful, splitting mods might cause some inconvenience to your world. Please tell @Ecconia if you honestly plan on using this mod.\
I will do the splitting for you sooner then!

### Description:

This mod provides generic and special purpose components that are used on one of @Ecconias servers.

Some of the components (memory & display) are only used to decrease the component count. Their timing is exactly the same as the real memory/display. Highly useless for everyone except @Ecconia.

But it also provides amazing components like the `FlatKey` and the `RealTimePulser`. Read more about them below.

### GUI:

You can edit the RealTimePulser and the FlatKey components via `X`.

## Install / Dependencies:

Just drop the `EcconiaCPUServerComponents` folder into your `GameData` folder.

For this mod to function, you will need two additional mod: `EccsLogicWorldAPI` & `EccsGuiBuilder`\
You can find both in my mod collection (root folder).

## Usage:

This mod only adds a few component:

### Eccs Weird Display:

This display has 65 inputs and 32² pixels.\
The peg in the middle is for the data.\
All other inputs are grouped in pairs, one for each pixel.\
The shorter pegs are for inverting a pixel, while the longer ones are for selecting a pixel for update.

When a pixel is selected for inversion, it takes 2 ticks until the pixel is inverted.\
When selecting a pixel for update, it takes 3 ticks for the pixel to be updated.\
BUT the data line only needs 2 ticks. So send the XY selection 1 tick more early, or the data input 1 tick late.

Currently LogicWorld does not allow changing the color profile of components, without editing its input peg amounts.\
This means you have to change the two pixel colors manually in code (its RGB). This is client sided only.

### Flat Key:

The flat key pretty much acts like any normal `Panel Key`.\
Besides that it has no moving part, but instead lights up when pressed.

The new thing about it, is that one can customize the text printed on it. (Oh and you can resize it).\
Just press `V` to resize or `X` to edit like any other LW component.

### RAM256b8:

This is a memory component which holds 256 8-bit numbers.\
It has four 8 bit ports, two for data in and out.\
And one for read and another for write address, these two also each have an enable bit.

The enable bits for read and write have to be sent 2 ticks later than the actual read and write address.\
It takes 4 ticks to read something and 3 ticks to write something.

The side of the memory shows the content of the memory with numbers.\
Currently it also shows which letter that content represents, but the charset is a custom one for my server.\
However you can change it locally since it is client sided only.\
As soon as LogicWorld gives me better custom data control, I likely gonna add the ability to send the charset from the server.

### TPS Printer:

The TPS printer prints the current TPS to its outputs.\
Since currently there is no way to properly listen to this, this component will be processed every tick - you do not want too many of them in your world.

The tps printer is not like your common real-time clock system, which outputs a pulse every second or every half a second.\
That would be boring, with this component you have to do the pulse generation yourself! Good luck.

### Real Time Pulser

The Real Time Pulser will emit a pulse on a rising edge input. Once the pulse is done emitting, it will output a 1 tick pulse on the top output.\
The pulse length is 1 second (not ticks), by default. You can edit the Real Time Pulser with `X` like any other LW component.

Useful to drive Singers, on a world with variable tick speed.\
Due to the pulse-output, these components can be easily chained.
