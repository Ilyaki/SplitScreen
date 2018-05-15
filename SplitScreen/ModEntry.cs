using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;


/*BREAKDOWN OF MOD:
 *	- Game1.game1.InactiveSleepTime = 0 : no fps throttling when window inactive
 *	- Program.releaseBuild = false : prevents updateActiveMenu returning immidiately(also enables cheats but haven't check what that means yet...)
 *	- Every update tick, (SInputState)Game1.input.RealController is set to GamePad.GetState(). This prevents suppression when window inactive
 *	- Every update tick, only if window is inactive, feed mouse X,Y from user32.dll into RealMouse of SInputState. (when window is inactive, Mouse.GetState() X,Y returns last recorded mouse X,Y before inactive)
 *	- UpdateControlInput is called when it should according to SDV's Game1 if statement logic, except ONLY when InActive
 *         >same with updateActiveMenu
 */

/*Loop works in this order:
 * - SMAPI Updates itself, including polling the input
 * - SMAPI calls Game1.Update
 * - GameEvents.UpdateTick is called
 * 
 * If there is a window active, Game1.Update is called BEFORE we mod the input, so there is no effect.
 * If inactive, we mod the input, then call UpdateControlInput
	 */

namespace SplitScreen
{
	public class ModEntry : Mod
	{
		private InputState sInputState;

		private const int secondsBetweenWarnings = 5;

		private bool hasShippingMenuCompleted = false;

		private PlayerIndexController playerIndexController;
				
		public override void Entry(IModHelper helper)
		{
			//Get player index if it is set in launch options, e.g. SMAPI.exe --log-path "third.txt" --player-index 3
			this.playerIndexController = new PlayerIndexController(Monitor, Environment.GetCommandLineArgs());

			//Removes FPS throttle when window inactive
			Game1.game1.InactiveSleepTime = new TimeSpan(0);

			/* When Program.releaseBuild = false, LeftStick and RightStick open a performance dialog and the chat window, respectively
			   So it is a good idea to unbind LeftStick and RightStick in x360ce */
			/* Prevents not continuing with update threads when InActive 
			  Also calls updateActiveMenu when window is inactive so we don't have to */
			Program.releaseBuild = false;

			//SMAPI sets Game1.input to an instance of SInputState
			sInputState = (Helper.Reflection.GetField<InputState>(typeof(Game1), "input", true)).GetValue();

			//Manually update the RealController and RealMouse fields of SMAPI's SInputeState (which derives from SDV's InputState but is internal and sealed, so needs reflection)
			//Also manually call Game1.UpdateControlInput
			StardewModdingAPI.Events.GameEvents.UpdateTick += OnUpdateTick;

			//Only used for manually closing Shipping Menu for non-Host players
			StardewModdingAPI.Events.SpecialisedEvents.UnvalidatedUpdateTick += OnUnvalidatedUpdateTick;
			StardewModdingAPI.Events.SaveEvents.AfterSave += (o, e) => { hasShippingMenuCompleted = false; };

			//Used to send (Toaster notification) warnings periodically if windows are set up incorrectly
			int secondsPassed = 0;
			StardewModdingAPI.Events.GameEvents.OneSecondTick += (o, e) =>
			{
				secondsPassed = ++secondsPassed % secondsBetweenWarnings;
				if(secondsPassed == 0)
				{
					/* playerIndex 1 should be active window */
					if (Context.IsMultiplayer && playerIndexController.IsPlayerIndexEqual(PlayerIndex.One) && !Game1.game1.IsActive)
						Game1.showGlobalMessage("Error: Controller index One's window must be active");
					/*else if (Context.IsMultiplayer && Context.IsMainPlayer && !playerIndexController.isPlayerIndexEqual(PlayerIndex.One))
						Game1.showGlobalMessage($"Error: Host must have controller index One (Currently is {playerIndexController.getIndexAsString()})"); (actually seems fine) */
				}
			};
		}
		
		private void OnUnvalidatedUpdateTick (object sender, EventArgs e)
		{
			/*From testing, it seems that between Saving (ie between SaveEvents.BeforeSave and SaveEvents.AfterSave), 
			  GameEvents.UpdateTick is not called. SpecializedEvents.UnvalidatedUpdateTick is always called, however.
			  For some reason, ShippingMenu is unresponsive during save period when window is inactive
			  Workaround: Wait for host to finish the menu, then manually call the OK button (which closes the menu)*/
			  
			if (!hasShippingMenuCompleted && !Game1.game1.IsActive && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShippingMenu shippingMenu)
			{
				if (Game1.player.team.GetNumberReady("wakeup") > 0)//This will equal 1 once host has finished with ShippingMenu
				{
					Monitor.Log("Manually closing ShippingMenu", LogLevel.Trace);
					hasShippingMenuCompleted = true;
					shippingMenu.InvokeMethod("okClicked");//Simulates menu close
				}
				//shippingMenu.update(Game1.currentGameTime); TODO: uncomment?
			}
		}

		private void OnUpdateTick(object sender, EventArgs e)
		{
			#region Insert raw GamePadState/MouseState to SInputState
			try
			{
				//If no playerIndex is set, a blank input state will be passed in
				GamePadState rawGamePadState = playerIndexController.GetRawGamePadState();

				//This only has effect when window is inactive, because otherwise , updateControlInput is called BEFORE this
				sInputState.SetPrivatePropertyValue("RealController", rawGamePadState);
				sInputState.SetPrivatePropertyValue("SuppressedController", rawGamePadState);

				#region Passing in MouseState
				if (!Game1.game1.IsActive)//Otherwise breaks mouse input for active window
				{
					//Find mouse position relative to top left of window
					Point windowTopLeftPosition = Game1.game1.Window.ClientBounds.Location;
					Point mousePos = MouseTracker.GetCursorPosition();
					Point relativeMousePos = new Point(mousePos.X - windowTopLeftPosition.X, mousePos.Y - windowTopLeftPosition.Y);

					//Copy mouseState but change X and Y to that from Windows
					MouseState rawMouseState = Mouse.GetState();
					MouseState artificialMouseState = new MouseState(relativeMousePos.X, relativeMousePos.Y,
						0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);

					Point relativeMousePosWithZoom = new Point((int)(relativeMousePos.X * (1.0 / Game1.options.zoomLevel)), (int)(relativeMousePos.Y * (1.0 / Game1.options.zoomLevel)));


					sInputState.SetPrivatePropertyValue("RealMouse", artificialMouseState);
					sInputState.SetPrivatePropertyValue("MousePosition", relativeMousePosWithZoom);//These last 2 lines fixed jittery mouse when moving either sticks AND fixed jittery mouse in menus
					sInputState.SetPrivatePropertyValue("SuppressedMouse", artificialMouseState);
				}
				#endregion
			}
			catch (ArgumentOutOfRangeException exception)
			{
				Monitor.Log("Failed to set input: " + exception.Message, LogLevel.Error);
			}
			#endregion
			
			#region Manually call UpdateControlInput
			if (
				(!Game1.game1.IsSaving 
				&& Game1.gameMode != 11 && (Game1.gameMode == 2 || Game1.gameMode == 3)
				&& Game1.currentLocation != null
				&& Game1.currentMinigame == null
				&& Game1.activeClickableMenu == null
				&& !Game1.globalFade
				&& !Game1.freezeControls
				&& !Game1.game1.IsActive)

			|| (Game1.farmEvent == null
				&& Game1.dialogueUp == true
				&& Game1.globalFade
				&& Game1.gameMode != 11
				&& !Game1.game1.IsSaving
				&& !Game1.game1.IsActive
				&& Game1.currentMinigame == null
				//&&Game1._newDayTask == null (cant access due to protection level, but probably not necessary...?)
				)
			)
			{
				Game1.game1.InvokeMethod("UpdateControlInput", Game1.currentGameTime);
			}
			#endregion

			#region Manually update minigames
			//Kill mouse otherwise it keeps moving when in minigame (which would interfere with the other players)
			var cursorSpeed = Helper.Reflection.GetField<float>(typeof(Game1), "_cursorSpeed");
			var cursorSpeedDirty = Helper.Reflection.GetField<bool>(typeof(Game1), "_cursorSpeedDirty");
			if (Game1.currentMinigame != null)
			{
				cursorSpeed.SetValue(0f);
				cursorSpeedDirty.SetValue(false);
			}
			else if (cursorSpeed.GetValue() == 0)//This happens once after leaving the minigame
			{
				cursorSpeedDirty.SetValue(true);
			}

			if (!Game1.fadeToBlack
				&& Game1.currentMinigame != null
				&& !Game1.HostPaused
				&& (Game1.gameMode == 3 || Game1.gameMode == 2) && (Game1.gameMode != 11)
				&& !Game1.game1.IsSaving
				//&& Game1._newDayTask == null
				&& !Game1.game1.IsActive )
			{
				MinigameUpdater.UpdateMinigameInput(Helper.Reflection, this.playerIndexController);
			}
			#endregion
		}		
	}
}
