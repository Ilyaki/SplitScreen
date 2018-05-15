using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen
{
	class MinigameUpdater
	{
		public static void UpdateMinigameInput(IReflectionHelper reflectionHelper, PlayerIndexController playerIndexController)
		{
			#region Copy-pasted from SDV (Part of Game1.Update)
			KeyboardState currentKBState = Game1.GetKeyboardState();
			MouseState currentMouseState = Mouse.GetState();//TODO: change this?
			GamePadState currentPadState = playerIndexController.GetRawGamePadState();
			if (Game1.chatBox != null && Game1.chatBox.isActive())
			{
				currentKBState = default(KeyboardState);
			}
			Microsoft.Xna.Framework.Input.Keys[] pressedKeys = currentKBState.GetPressedKeys();
			foreach (Microsoft.Xna.Framework.Input.Keys i in pressedKeys)
			{
				if (!Game1.oldKBState.IsKeyDown(i) && Game1.currentMinigame != null)
				{
					Game1.currentMinigame.receiveKeyPress(i);
				}
			}
			ButtonCollection pressedButtons;
			ButtonCollection.ButtonEnumerator enumerator;
			if (Game1.options.gamepadControls)
			{
				if (Game1.currentMinigame == null)
				{
					Game1.oldMouseState = currentMouseState;
					Game1.oldKBState = currentKBState;
					Game1.oldPadState = currentPadState;
					//Game1.game1.UpdateChatBox();
					reflectionHelper.GetMethod(typeof(Game1), "UpdateChatBox");
					return;
				}
				pressedButtons = Utility.getPressedButtons(currentPadState, Game1.oldPadState);
				enumerator = pressedButtons.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Buttons b2 = enumerator.Current;
					if (Game1.currentMinigame != null)
					{
						Game1.currentMinigame.receiveKeyPress(Utility.mapGamePadButtonToKey(b2));
					}
				}
				if (Game1.currentMinigame == null)
				{
					Game1.oldMouseState = currentMouseState;
					Game1.oldKBState = currentKBState;
					Game1.oldPadState = currentPadState;
					reflectionHelper.GetMethod(typeof(Game1), "UpdateChatBox");
					return;
				}
				GamePadThumbSticks thumbSticks = currentPadState.ThumbSticks;
				if (thumbSticks.Right.Y < -0.2f)
				{
					thumbSticks = Game1.oldPadState.ThumbSticks;
					if (thumbSticks.Right.Y >= -0.2f)
					{
						Game1.currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Down);
					}
				}
				thumbSticks = currentPadState.ThumbSticks;
				if (thumbSticks.Right.Y > 0.2f)
				{
					thumbSticks = Game1.oldPadState.ThumbSticks;
					if (thumbSticks.Right.Y <= 0.2f)
					{
						Game1.currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Up);
					}
				}
				thumbSticks = currentPadState.ThumbSticks;
				if (thumbSticks.Right.X < -0.2f)
				{
					thumbSticks = Game1.oldPadState.ThumbSticks;
					if (thumbSticks.Right.X >= -0.2f)
					{
						Game1.currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Left);
					}
				}
				thumbSticks = currentPadState.ThumbSticks;
				if (thumbSticks.Right.X > 0.2f)
				{
					thumbSticks = Game1.oldPadState.ThumbSticks;
					if (thumbSticks.Right.X <= 0.2f)
					{
						Game1.currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Right);
					}
				}
				thumbSticks = Game1.oldPadState.ThumbSticks;
				if (thumbSticks.Right.Y < -0.2f)
				{
					thumbSticks = currentPadState.ThumbSticks;
					if (thumbSticks.Right.Y >= -0.2f)
					{
						Game1.currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Down);
					}
				}
				thumbSticks = Game1.oldPadState.ThumbSticks;
				if (thumbSticks.Right.Y > 0.2f)
				{
					thumbSticks = currentPadState.ThumbSticks;
					if (thumbSticks.Right.Y <= 0.2f)
					{
						Game1.currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Up);
					}
				}
				thumbSticks = Game1.oldPadState.ThumbSticks;
				if (thumbSticks.Right.X < -0.2f)
				{
					thumbSticks = currentPadState.ThumbSticks;
					if (thumbSticks.Right.X >= -0.2f)
					{
						Game1.currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Left);
					}
				}
				thumbSticks = Game1.oldPadState.ThumbSticks;
				if (thumbSticks.Right.X > 0.2f)
				{
					thumbSticks = currentPadState.ThumbSticks;
					if (thumbSticks.Right.X <= 0.2f)
					{
						Game1.currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Right);
					}
				}
				/*if (Game1.isGamePadThumbstickInMotion(0.2) && Game1.currentMinigame != null && !Game1.currentMinigame.overrideFreeMouseMovement())
				{
					int mouseX = Game1.getMouseX();
					thumbSticks = currentPadState.ThumbSticks;
					int x = mouseX + (int)(thumbSticks.Left.X * ThumbstickToMouseModifier);
					int mouseY = Game1.getMouseY();
					thumbSticks = currentPadState.ThumbSticks;
					Game1.setMousePosition(x, mouseY - (int)(thumbSticks.Left.Y * ThumbstickToMouseModifier));
				}
				else if (Game1.getMousePosition().X != Game1.getOldMouseX() || Game1.getMousePosition().Y != Game1.getOldMouseY())
				{
					Game1.lastCursorMotionWasMouse = true;
				}*/
			}
			pressedKeys = Game1.oldKBState.GetPressedKeys();
			foreach (Microsoft.Xna.Framework.Input.Keys j in pressedKeys)
			{
				if (!currentKBState.IsKeyDown(j) && Game1.currentMinigame != null)
				{
					Game1.currentMinigame.receiveKeyRelease(j);
				}
			}
			if (Game1.options.gamepadControls)
			{
				if (Game1.currentMinigame == null)
				{
					Game1.oldMouseState = currentMouseState;
					Game1.oldKBState = currentKBState;
					Game1.oldPadState = currentPadState;
					reflectionHelper.GetMethod(typeof(Game1), "UpdateChatBox");
					return;
				}
				if (currentPadState.IsConnected && currentPadState.IsButtonDown(Buttons.X) && !Game1.oldPadState.IsButtonDown(Buttons.X))
				{
					Game1.currentMinigame.receiveRightClick(Game1.getMouseX(), Game1.getMouseY(), true);
				}
				else if (currentPadState.IsConnected && currentPadState.IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))
				{
					Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
				}
				else if (currentPadState.IsConnected && !currentPadState.IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonDown(Buttons.X))
				{
					Game1.currentMinigame.releaseRightClick(Game1.getMouseX(), Game1.getMouseY());
				}
				else if (currentPadState.IsConnected && !currentPadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
				{
					Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
				}
				pressedButtons = Utility.getPressedButtons(Game1.oldPadState, currentPadState);
				enumerator = pressedButtons.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Buttons b = enumerator.Current;
					if (Game1.currentMinigame != null)
					{
						Game1.currentMinigame.receiveKeyRelease(Utility.mapGamePadButtonToKey(b));
					}
				}
				if (currentPadState.IsConnected && currentPadState.IsButtonDown(Buttons.A) && Game1.currentMinigame != null)
				{
					Game1.currentMinigame.leftClickHeld(0, 0);
				}
			}
			if (Game1.currentMinigame == null)
			{
				Game1.oldMouseState = currentMouseState;
				Game1.oldKBState = currentKBState;
				Game1.oldPadState = currentPadState;
				reflectionHelper.GetMethod(typeof(Game1), "UpdateChatBox");
				return;
			}
			if (Game1.currentMinigame != null && currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && Game1.oldMouseState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
			}
			if (Game1.currentMinigame != null && currentMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && Game1.oldMouseState.RightButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				Game1.currentMinigame.receiveRightClick(Game1.getMouseX(), Game1.getMouseY(), true);
			}
			if (Game1.currentMinigame != null && currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && Game1.oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
			}
			if (Game1.currentMinigame != null && currentMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && Game1.oldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
			}
			if (Game1.currentMinigame != null && currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && Game1.oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				Game1.currentMinigame.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
			}
			Game1.oldMouseState = currentMouseState;
			Game1.oldKBState = currentKBState;
			Game1.oldPadState = currentPadState;

			#endregion
		}
	}
}
