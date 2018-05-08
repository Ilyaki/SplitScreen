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

			Program.releaseBuild = false;

			//Game1.chatBox = null;

			

			StardewModdingAPI.Events.InputEvents.ButtonPressed += OnButtonPressed;

			/*StardewModdingAPI.Events.ControlEvents.ControllerButtonPressed += (o, e) =>
			{
				Console.WriteLine($"Player {e.PlayerIndex} pressed button {e.ButtonPressed}");
			};*/

			StardewModdingAPI.Events.GameEvents.HalfSecondTick += (o, e) =>
			{
				//Console.WriteLine($"Active:{Game1.game1.IsActive}, freeze:{Game1.freezeControls}");//freeze seems to be false always?


				//this seems to return all false when window inactive even if pressed...
				var state0 = (helper.Reflection.GetField<InputState>(typeof(Game1), "input")).GetValue().GetGamePadState();
				Console.WriteLine($"DEFAULT: Value of A,B,X,Y : {state0.IsButtonDown(Buttons.A)},{state0.IsButtonDown(Buttons.B)},{state0.IsButtonDown(Buttons.X)},{state0.IsButtonDown(Buttons.Y)}");

				var state1 = GamePad.GetState(PlayerIndex.One);
				Console.WriteLine($"RAW: Value of A,B,X,Y : {state1.IsButtonDown(Buttons.A)},{state1.IsButtonDown(Buttons.B)},{state1.IsButtonDown(Buttons.X)},{state1.IsButtonDown(Buttons.Y)}");

				Console.WriteLine($"DEFAULT: CONNECTED STATE: {state0.IsConnected}");
				Console.WriteLine($"RAW: CONNECTED STATE: {state1.IsConnected}");

				Console.WriteLine($"Release build: {Program.releaseBuild}");
				//Console.WriteLine($"Chatbox is null: {Game1.chatBox == null}, suppressed: {Game1.chatBox != null && !Game1.chatBox.isActive()}");


				Console.WriteLine($"Convention mode: {Game1.conventionMode}");
				
			};

			StardewModdingAPI.Events.GameEvents.UpdateTick += (o, e) =>
			{
				//var platform = (typeof(Game1)).GetPrivatePropertyValue<object>("Platform");
				//var platform = helper.Reflection.GetField<object>(Game1.game1, "Platform");
				//platform.SetPrivateFieldValue("_isActive", true);
				var state0 = GamePad.GetState(PlayerIndex.One);//(helper.Reflection.GetField<InputState>(typeof(Game1), "input")).GetValue().GetGamePadState();
															   /*if (Game1.options.gamepadControls && state0.IsConnected && Game1.activeClickableMenu != null)
															   {
																   Console.WriteLine("t1");
																   if (!Game1.activeClickableMenu.areGamePadControlsImplemented() && state0.IsButtonDown(Buttons.A) && (!Game1.oldPadState.IsButtonDown(Buttons.A) || ((float)Game1.gamePadAButtonPolling > 650f && !(Game1.activeClickableMenu is DialogueBox))))
																   {
																	   if (!Game1.game1.IsActive)
																		   Game1.activeClickableMenu.receiveLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y, true);
																	   Console.WriteLine("t22222222222222222222222222222222222222222222222222222222222222222222");
																   }
																   else if (!Game1.activeClickableMenu.areGamePadControlsImplemented() && !state0.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
																   {
																	   if (!Game1.game1.IsActive)
																		   Game1.activeClickableMenu.releaseLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
																	   Console.WriteLine("t______________-------------------///////////");
																   }
															   }*/

				


				//Find window position
				//var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
				//form.Location = new System.Drawing.Point(0, 0);

				//Find mouse position relative to top left of window
				//Point windowTopLeftPosition = Game1.game1.Window.ClientBounds.Location;
				//Point mousePos = MouseTracker.GetCursorPosition();
				//Point relativeMousePos = new Point (mousePos.X - windowTopLeftPosition.X, mousePos.Y- windowTopLeftPosition.Y);

				//Console.WriteLine($"WINDOWS Mouse position: {relativeMousePos.X},{relativeMousePos.Y}");
				//Console.WriteLine($"DEFAULT Mouse position: {Game1.getMousePosition().X},{Game1.getMousePosition().Y}");






				try
				{
					var manager = (helper.Reflection.GetField<InputState>(typeof(Game1), "input")).GetValue();
					manager.SetPrivatePropertyValue("RealController", GamePad.GetState(PlayerIndex.One));



					//Find mouse position relative to top left of window
					Point windowTopLeftPosition = Game1.game1.Window.ClientBounds.Location;
					Point mousePos = MouseTracker.GetCursorPosition();
					Point relativeMousePos = new Point(mousePos.X - windowTopLeftPosition.X, mousePos.Y - windowTopLeftPosition.Y);

					//Copy mouseState but change X and Y to that from Windows
					var mouseState0 = Mouse.GetState();
					var mouseState = new MouseState(relativeMousePos.X, relativeMousePos.Y,
						mouseState0.ScrollWheelValue, mouseState0.LeftButton, mouseState0.MiddleButton, mouseState0.RightButton, mouseState0.XButton1, mouseState0.XButton2);

					manager.SetPrivatePropertyValue("RealMouse", mouseState);
					//var type = typeof(InputState);
					//type.GetProperty("RealController").SetValue(manager, GamePad.GetState(PlayerIndex.One), null);
				}catch(ArgumentOutOfRangeException exception)
				{
					Console.WriteLine("failed to set input: "+exception.Message);
				}

				if (
				
				(
				!Game1.game1.IsSaving 
				&& Game1.gameMode != 11 
				&& (Game1.gameMode == 2 || Game1.gameMode == 3)
				&& Game1.currentLocation != null
				//&& Game1.currentMinigame == null
				&& Game1.activeClickableMenu == null
				&& !Game1.globalFade
				&& !Game1.freezeControls
				&& !Game1.game1.IsActive) // so its the opposite
				
				|| (

				Game1.farmEvent == null
				&&Game1.dialogueUp == true
				&&Game1.globalFade
				&&Game1.gameMode != 11
				&&!Game1.game1.IsSaving 
			 //&&Game1._newDayTask == null
				)
				
				
				)
				{
					//Console.WriteLine("Doing manual update");
					Game1.game1.InvokeMethod("UpdateControlInput", Game1.currentGameTime);
				}

				if(
						Game1.graphics.GraphicsDevice != null
					//&& Game1.game1._newDayTask == null
					&& !Game1.game1.IsSaving
					&&Game1.gameMode != 11
					&& !Game1.game1.IsActive
				)
					{
					//TODO: does this ever run/is ever useful???
					//Game1.game1.InvokeMethod("checkForEscapeKeys");
				}

				if(
					//Game1._newDayTask == null
					!Game1.game1.IsSaving
					&& (Game1.gameMode == 2 || Game1.gameMode == 3) && (Game1.gameMode != 11 )
//					&& doMainGameUpdates
					&&Game1.currentLocation != null && Game1.currentMinigame == null
					&& Game1.activeClickableMenu != null)
				{
					Game1.updateActiveMenu(Game1.currentGameTime);
					}



			};

			/*StardewModdingAPI.Events.GameEvents.FirstUpdateTick += (o, e) =>
			{
				Monitor.Log("Switching input manager...", StardewModdingAPI.LogLevel.Trace);
				SplitInputState splitInputState = new SplitInputState();
				helper.Reflection.GetField<InputState>(typeof(Game1), "input", true).SetValue(splitInputState);
				Monitor.Log(">>"+Game1.game1.GetType());
			};*/
		}

		private void OnButtonPressed( object sender, StardewModdingAPI.Events.EventArgsInput args)
		{
			if (args.Button.Equals(SButton.RightControl))
			{
				/*Task.Factory.StartNew(() =>
				{
					System.Threading.Thread.Sleep(4000);
					Console.WriteLine("setting active now");

					var platform = Game1.game1.Plat

					var prop = Game1.game1.GetType().GetField("IsActive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					prop.SetValue(Game1.game1, "new value");
				});*/
			}
		}
	}
}
