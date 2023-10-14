# SimulationStopwatch

This mod adds a Stopwatch component to the game.\
This component can be started and stopped.

It will collect 5 time values:
- The amount of ticks processed.
- The total amount of time taken.
- The time the server spent sleeping (waiting, doing nothing).
- The time the server spent on the simulation tick processing.
- The time the server spend processing packets and doing things that are not simulation.

Besides measuring the tick duration of certain applications (which is not the main focus of this component).\
One can measure the load on the simulation. And how much time the server spends on other things.

Output will look like this:

Printed on start:\
`Server debug: Random Test> Start`

Printed on end:\
`Server debug: Random Test> Simulation ticks: 167 Server ticks: 165`\
`Server debug: Random Test> Total time: 2789.97ms Simulation: 3.28ms (0.12%) Server: 3.24ms (0.12%) Idle: 2783.45ms (99.77%)`

The name `"Random Test"`, can be configured in the component.

### More Debugging:

It is possible to enable debugging printing per server tick.

This will print the collected data per server tick like this:\
`Server debug: Random Test> ST> Ticks: 1 Simulation: 0.04ms Server: 0.01ms Idle: 17.18ms T/SimT: 4.1322314049586776E-10ms`

And if server ticks are being skipped, it prints this:\
`Server debug: Random Test> SKIP 1`

### Small warning:

Just having this mod installed, will inject debugging `IServerTicker` into the servers main loop, to measure the time.\
This will add a very small overhead to the main loop. But this is by default only 60 times a second.

## Install / Dependencies:

Just drop the `SimulationStopwatch` & `SimulationStopwatchGui` folders into your `GameData` folder.

For this mod to function, you will need two additional mod: `EccsLogicWorldAPI` & `EccsGuiBuilder`\
You can find both in my mod collection (root folder).
