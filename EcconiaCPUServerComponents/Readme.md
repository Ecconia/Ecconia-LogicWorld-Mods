# EcconiaCPUServerComponents by @Ecconia

### Description:

`Logic World` is still in its early days, and as of now (Version: 0.90.3) still suffers from heavy memory issues.

Reasons for creation of this mod:

- With high component count, comes high memory usage and slow world editing.

But with this mod, some of my big buildings are partially replaced by a single modded component. This saves memory. 

## Install / Dependencies:

Just drop the `EcconiaCPUServerComponents` folder into your `GameData` folder.

## Usage:

This mod only adds a single component:

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

The new thing about it, is that one can customize the text printed on it.\
Currently this has to be done using the `EditKeyLabel` command.
