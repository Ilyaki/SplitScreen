using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Harmony;
using StardewValley;

namespace SplitScreen.Patchers
{
	//Game1 : public static Point getMousePosition()
	[HarmonyPatch(typeof(Game1))]
	[HarmonyPatch("getMousePosition")]
	class GetMousePatcher
	{
		static bool Prefix()
		{
			//Console.WriteLine("GetMouse called!");
			return false;
		}

		static Point Postfix(Point p)
		{
			if (Game1.game1.IsActive)
				return new Point(Game1.getMouseX(), Game1.getMouseY());

			return new Point(FakeMouse.X, FakeMouse.Y);
		}
	}
}
