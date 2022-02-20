# EcconiaCPUServerComponents by @Ecconia

### Description:

`Logic World` is still in its early days, and as of now (Version: 0.93) still suffers from heavy memory issues.

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
All other inputs are in a group of 4 pegs.\
Here the outer ones are for selecting a pixel to update.\
The inner ones are for selecting a pixel to invert.

When a pixel is selected for inversion, it takes 2 ticks until the pixel is inverted.\
When selecting a pixel for update, it takes 3 ticks for the pixel to be updated.\
BUT the data line only needs 2 ticks. So send the XY selection 1 tick more early, or the data input 1 tick late.
