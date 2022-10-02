# SimulationControl by @Ecconia

### Description:

In `Logic World 0.90.X` there was only a set of complex commands to control simulation.\
In version `0.91+`, there is a GUI to control the TPS though. But if you prefer a better command, this mod might be your choice.

Reasons for creation of this mod:

- To change the ticks per second, one has to type `server "simulation.rate <desired tick speed per second>"`. Instead of just `tps <desired tick speed per second>`.

But with this mod, you can just use an easy `tps` command.

Keep in mind, that you need to be admin on the server if you want to use this command.\
Looking for a way to change the TPS without admin? Check out my mod `CustomChatManager`!

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
Setting a tps speed will resume the simulation! (This is done by LogicWorld).
