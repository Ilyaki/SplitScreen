﻿using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;

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


			if (Utils.TrueIsWindowActive())
				return true;
			
			FakeMouse.X = x;
			FakeMouse.Y = y;
			return false;
		}
	}
}
