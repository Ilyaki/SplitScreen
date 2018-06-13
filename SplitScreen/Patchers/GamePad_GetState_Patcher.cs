using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen.Patchers
{
	//This fixes the issue where Controller 1 (as in x360ce's Controller 1) will control any foucsed window, even if not assigned to it

	[HarmonyPatch(typeof(Microsoft.Xna.Framework.Input.GamePad))]
	[HarmonyPatch("GetState")]
	[HarmonyPatch(new Type[] { typeof(PlayerIndex), typeof(GamePadDeadZone)})]//The GetState(PlayerIndex playerIndex) calls this method, so only need to edit one method
	class GamePad_GetState_Patcher
	{
		public static GamePadState Postfix(GamePadState g, PlayerIndex playerIndex, GamePadState __result)
		{
			if (playerIndex.Equals(PlayerIndex.One) && !ModEntry._playerIndexController.IsPlayerIndexEqual(PlayerIndex.One))
				return ModEntry._playerIndexController.GetRawGamePadState();
			else return __result;
		}
	}
}
