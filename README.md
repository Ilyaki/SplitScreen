# SplitScreen
Stardew Valley mod that enables split screen multiplayer with multiple controllers.

Breakdown of mod:
- Game1.game1.InactiveSleepTime = 0 : no fps throttling when window inactive
- Program.releaseBuild = false : prevents updateActiveMenu returning immidiately(also enables cheats but haven't check what that means yet...)
- Every update tick, (SInputState)Game1.input.RealController is set to GamePad.GetState(). This prevents suppression when window inactive
- Every update tick, only if window is inactive, feed mouse X,Y from user32.dll into RealMouse of SInputState. (when window is inactive, Mouse.GetState() X,Y returns last recorded mouse X,Y before inactive)
- UpdateControlInput is called when it should according to SDV's Game1 if statement logic, except ONLY when InActive
    >same with updateActiveMenu
