using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace SplitScreen.Keyboards
{
	[HarmonyPatch(typeof(Microsoft.Xna.Framework.Input.Keyboard))]
	[HarmonyPatch("GetState")]
	[HarmonyPatch(new Type[] { })]
	public static class KeyboardPatcher_GetState
	{
		//Seems unecessary?
		static bool Prefix()
		{
			return !MultipleKeyboardManager.HasAttachedKeyboard();
		}

		static KeyboardState Postfix(KeyboardState k, KeyboardState __result)
		{
			if (MultipleKeyboardManager.HasAttachedKeyboard())
				return MultipleKeyboardManager.GetAttachedKeyboardState() ?? new KeyboardState();

			return __result;
		}
	}
}
