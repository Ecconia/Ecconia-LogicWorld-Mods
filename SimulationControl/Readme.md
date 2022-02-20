# SimulationControl by @Ecconia

### Description:

`Logic World` is still in its early days, and as of now (Version: 0.90.3) does not have a direct way to change simulation speed. (Will get this feature in 0.91+ though).

Reasons for creation of this mod:

- To change the ticks per second, one has to type `server "simulation.rate <desired tick speed per second>"`. Instead of just `tps <desired tick speed per second>`.

But with this mod, you can just use an easy `tps` command.

## Install / Dependencies:

Just drop the `SimulationControl` folder into your `GameData` folder.

## Usage:

The mod provides the `tps` command with following usage:\
`tps < 0/Stop/H/Halt/P/Pause | R/Resume/Play/C | S/Step | <Floating Point Number> >`

For each normal command some argument can be used:

- `simulation.pause`: 0, stop, h, halt, p, pause
- `simulation.resume`: r, resume, play, c
- `step 1`: s, step
- `simulation.rate <floating-point-number>`: <floating-point-number>

TPS of 0, is mapped to `simulation.pause`.\
Setting a tps speed will not resume the simulation.
