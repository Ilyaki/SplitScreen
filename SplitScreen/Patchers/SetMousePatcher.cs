using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Harmony;
using StardewValley;
using Microsoft.Xna.Framework.Input;

namespace SplitScreen.Patchers
{
	[HarmonyPatch(typeof(Mouse))]
	[HarmonyPatch("SetPosition")]
	[HarmonyPatch(new Type[] { typeof(int), typeof(int) })]
	class SetMousePatcher1
	{
		static bool Prefix(ref int x, ref int y)
		{
			var bounds = Game1.game1.GraphicsDevice.PresentationParameters.Bounds;
			x = Math.Max(0, Math.Min(x, bounds.Width - 1));
			y = Math.Max(0, Math.Min(y, bounds.Height - 1));

			if (Game1.game1.IsActive)
				return true;

			FakeMouse.X = x;
			FakeMouse.Y = y;
			return false;
		}
	}
}
