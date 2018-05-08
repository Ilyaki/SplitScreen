using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SplitScreen
{
	public class ModEntry : StardewModdingAPI.Mod
	{
		public override void Entry(StardewModdingAPI.IModHelper helper)
		{
			//Removes FPS throttle when window inactive
			Game1.game1.InactiveSleepTime = new TimeSpan(0);

			/* When Program.releaseBuild = false, LeftStick and RightStick open a performance dialog and the chat window, respectively
			* So it is a good idea to unbind LeftStick and RightStick in x360ce */
			//Prevents not continuing with update threads when InActive
			Program.releaseBuild = false;

			//Manually update the RealController and RealMouse fields of SMAPI's SInputeState (which derives from SDV's InputState but is internal and sealed, so needs reflection)
			StardewModdingAPI.Events.GameEvents.UpdateTick += (o, e) =>
			{

				#region Insert raw GamePadState/MouseState to SInputState
				try
				{
					//SMAPI sets Game1.input to an instance of SInputState
					InputState SInputState = (helper.Reflection.GetField<InputState>(typeof(Game1), "input")).GetValue();

					GamePadState rawGamePadState = GamePad.GetState(PlayerIndex.One);
					SInputState.SetPrivatePropertyValue("RealController", rawGamePadState);

					#region Passing in MouseState
					if (!Game1.game1.IsActive)//Otherwise breaks mouse input for active window
					{
						//Find mouse position relative to top left of window
						Point windowTopLeftPosition = Game1.game1.Window.ClientBounds.Location;//cache this, only update when window changes4
						Point mousePos = MouseTracker.GetCursorPosition();
						Point relativeMousePos = new Point(mousePos.X - windowTopLeftPosition.X, mousePos.Y - windowTopLeftPosition.Y);

						//Copy mouseState but change X and Y to that from Windows
						MouseState rawMouseState = Mouse.GetState();
						MouseState artificialMouseState = new MouseState(relativeMousePos.X, relativeMousePos.Y,
							0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
						//TODO: does scroll wheel value 0 break menus?

						SInputState.SetPrivatePropertyValue("RealMouse", artificialMouseState);
					}
					#endregion
				}
				catch (ArgumentOutOfRangeException exception)
				{
					Console.WriteLine("Failed to set input: " + exception.Message);
				}
				#endregion

				if (
					(!Game1.game1.IsSaving
					&& Game1.gameMode != 11
					&& (Game1.gameMode == 2 || Game1.gameMode == 3)
					&& Game1.currentLocation != null
					//&& Game1.currentMinigame == null
					&& Game1.activeClickableMenu == null
					&& !Game1.globalFade
					&& !Game1.freezeControls
					&& !Game1.game1.IsActive) // so its the opposite

				|| (Game1.farmEvent == null
					&& Game1.dialogueUp == true
					&& Game1.globalFade
					&& Game1.gameMode != 11
					&& !Game1.game1.IsSaving
					//&&Game1._newDayTask == null (cant access due to protection level, but probably not necessary...?)
					)
				)
				{
					Game1.game1.InvokeMethod("UpdateControlInput", Game1.currentGameTime);
				}

				if (//Game1._newDayTask == null && 
					!Game1.game1.IsSaving
					&& (Game1.gameMode == 2 || Game1.gameMode == 3) && (Game1.gameMode != 11)
					&& Game1.currentLocation != null && Game1.currentMinigame == null
					&& Game1.activeClickableMenu != null
					)
				{
					Game1.updateActiveMenu(Game1.currentGameTime);
				}
				
			};
		}
	}
}
