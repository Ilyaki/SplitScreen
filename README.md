# SplitScreen
Stardew Valley mod that enables split screen multiplayer with multiple controllers.

Mod works by running multiple instances of StardewModdingAPI.exe, SplitScreen allows input to be received and used by inactive windows. Also creates "virtual" mouse for each instance

Use with WindowResize mod to resize windows smaller: https://www.nexusmods.com/stardewvalley/mods/2266

Breakdown of mod:
- Game1.game1.InactiveSleepTime = 0 : no fps throttling when window inactive
- Program.releaseBuild = false : prevents updateActiveMenu returning immidiately(also enables cheats but haven't check what that means yet...)
- Every update tick, (SInputState)Game1.input.RealController is set to GamePad.GetState(). Mouse is set to whatever is in FakeMouse (unless window is focused) This prevents suppression when window inactive
- UpdateControlInput is called when it should according to SDV's Game1 if statement logic, except ONLY when InActive
- XNA.Framework.Mouse.SetPosition is overwritten to only update FAKE mouse when unfocused
- Game1.getMousePosition() is overwritten return FAKE mouse when unfocused
