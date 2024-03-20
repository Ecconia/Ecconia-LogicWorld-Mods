# ComponentAction Example Mod

**This mod is not to be used in normal gameplay!**

Component Actions are a new experimental API for Logic World 0.91.3+.

It serves the purpose of giving modders more possibilities and abilities to optimize their mods.\
One of its primary goals is to replace the current custom data abstraction layer.

It primarily provides a new world update and build request type which only contains a byte array addressed at a single component.\
As this API is a little bit different from how the normal LW workflow works, it has some unconventional helper classes that modders have to use.\
This is also why the API is experimental - it might be removed or changed in future.\
However you can rely on me making sure, that the ability it provides will not be removed from LW.

### API usage cases:

The API is split into two parts:
- World updates: These have different and dedicated handlers per component type on server and client.
- Build requests: These have a dedicated handler per component type on the server.

The Build request handler can act in multiple ways:
- It may or may not be able to / have to create undo build requests.
- It may distribute world updates via the vanilla way (being sent to client and then applied on server). Or it distributes world updates only to the client and hands the changes to the server component directly.
- It also may not distribute world mutations to the client at all.

### All possible example cases:

Following these possibilities, there are the following example cases in this example mod:

1. World updates:\
    Primarily useful to send updates to clients triggered by the simulation.\
    For example "Play sound" or "pixel changed" or "memory address changed".

2. Build requests with undo:\
    Useful, for cases where only the server simulation logic gets changed by it.

3. Build requests without undo:\
    Primarily useful to send events like "reset counter" via component edit GUI.

4. Build request and vanilla update distribution with undo:\
    **Don't do this!** If you do this, you have no respect for optimization *mean look*.\
    The problem is, that one would need to deserialize the action data to build the
     undo requests and then deserialize the action data (or a modified version of it) again,
     to apply it on the server - while it could be directly applied instead.

5. Build request and vanilla update distribution without undo:\
    Useful for actions like "button pressed", like when a vanilla button gets pressed and everyone can see it.

6. Build request with skip-server update distribution with undo:\
    Used for normal edit GUI actions, like change color or text of a component.\
    Advantage compared to the normal custom data framework is, that you change parts and not everything of the component - allows more fine grained undo operations in multi-player.

7. Build request with skip-server update distribution without undo:\
    Primarily for actions which are not applied on the server.

Ofc a mixture of these ways to handle packets is perfectly fine.\
All use the same entry classes, how it behaves is up to you.\
These examples simply show how it would look like to handle one use-case of the API.
