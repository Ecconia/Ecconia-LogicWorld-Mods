# CustomChatManager by @Ecconia

### Description:

Pre-processes chat messages on the server, before broadcasting them to other clients. Can even stop messages from being forwarded.\
However it can not prevent the sent message on the client from appearing.

Currently there are following sub-systems:

- As the original LogicWorld chat handler, it validates, that the content of the message is not too large.
- It will prevent players from abusing 'SayRaw' to impersonate other players, if they try, their name will be used instead of what they sent.
- There is a chat command manager, which adds usable-for everyone commands.\
  Currently there are following commands by default:
    - `/help` : Prints a list of all commands.
    - `/list` : Prints a list of all connected players.
    - `/tps` : Allows anyone to change the simulation speed and state.

It is made for the server and only runs there.

# Install:

Just drop the `CustomChatManager` folder into the servers `GameData` folder.

You will also need the two mods `ServerModdingTools` and `ServerOnlyMods` for this mod to function.\
You can find them in my mod collection (root folder).
