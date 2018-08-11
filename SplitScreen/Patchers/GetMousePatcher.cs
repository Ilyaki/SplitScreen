using Harmony;
using StardewValley;

namespace SplitScreen.Patchers
{
	/*[HarmonyPatch(typeof(Game1))]
	[HarmonyPatch("getMouseX")]
	class GetMousePatcher_getMouseX
	{
		static int Postfix(int i, int __result)
		{
			return (!Utils.TrueIsWindowActive() && PlayerIndexController._PlayerIndex != null) ? (int)(FakeMouse.X / Game1.options.zoomLevel) : __result;
		}
	}

	[HarmonyPatch(typeof(Game1))]
	[HarmonyPatch("getMouseY")]
	class GetMousePatcher_getMouseY
	{
		static int Postfix(int i, int __result)
		{
			return (!Utils.TrueIsWindowActive() && PlayerIndexController._PlayerIndex != null) ? (int)(FakeMouse.Y / Game1.options.zoomLevel) : __result;
		}
	}*/
}
