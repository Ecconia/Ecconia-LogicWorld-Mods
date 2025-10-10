# MTU Control by @Ecconia

### Description:

The game sends packets through the internet. And packets normally should only have a certain size. If the packet is too big, it might be dropped instead of being delivered.\
Some LW players like to use a certain public proxy to not have to port-forward the game port. But that proxy drops all packets with MTU of 1433 bytes. BRUH.\
The result is, that clients will try to join forever and nothing happens, cause the world packets don't ever arrive.

This mod lowers the MTU of packets LW sends to something which will not break in that proxy.

On dedicated servers this mod automatically lowers the MTU to `1380`. On single-player servers it won't do that. But it in theory also works on clients (did not test that - might be required for uploading subassemblies to the server).

You can at any time run `setmtu <your desire>` to set the MTU. Just keep in mind that players have to rejoin for the MTU to apply to the connection.

# Install:

Just drop the `MtuControl` folder into the `GameData` folder.

You will also need the mod `EcconiaLogicWorldAPI` for this mod to function.\
You can find it in my mod collection (root folder).
