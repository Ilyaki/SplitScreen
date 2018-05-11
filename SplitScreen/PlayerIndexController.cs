using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen
{
	class PlayerIndexController
	{
		private IMonitor monitor;

		private PlayerIndex? playerIndex;

		public PlayerIndexController(IMonitor monitor, string[] args)
		{
			this.monitor = monitor;

			playerIndex = GetPlayerIndexFromArgs(args);
			monitor.Log($"Using player index {getIndexAsString()}", LogLevel.Info);
		}

		public GamePadState GetRawGamePadState() => playerIndex.HasValue ? GamePad.GetState(playerIndex.GetValueOrDefault()) : new GamePadState(new Vector2(), new Vector2(), 0, 0);
		
		public bool isPlayerIndexEqual(PlayerIndex playerIndex) => this.playerIndex.HasValue && this.playerIndex.Equals(playerIndex);

		public string getIndexAsString() => playerIndex.HasValue ? playerIndex.ToString() : "NONE";

		private PlayerIndex? GetPlayerIndexFromArgs(string[] args)
		{
			if (args.Contains("--player-index"))
			{
				int playerIndex_Index = Array.LastIndexOf(args, "--player-index") + 1;
				if (playerIndex_Index >= 1 && args.Length >= playerIndex_Index)
				{
					string playerIndexString = args[playerIndex_Index];
					switch (playerIndexString)
					{
						case "0":
						case "zero":
						case "none":
						case "null":
							return null;
						case "1":
						case "one":
						case "first":
							return PlayerIndex.One;
						case "2":
						case "two":
						case "second":
							return PlayerIndex.Two;
						case "3":
						case "three":
						case "third":
							return PlayerIndex.Three;
						case "4":
						case "four":
						case "fourth":
							return PlayerIndex.Four;
						default:
							monitor.Log("'--player-index' launch option set incorrectly", LogLevel.Error);
							return null;
					}
				}
				else
				{
					monitor.Log("'--player-index' launch option set incorrectly", LogLevel.Error);
				}
			}

			return PlayerIndex.One;
		}
	}
}
