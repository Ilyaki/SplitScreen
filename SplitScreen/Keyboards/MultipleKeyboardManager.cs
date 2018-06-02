using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using RawInput_dll;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Harmony;
using System.Reflection;

namespace SplitScreen.Keyboards
{
	//https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard

	public class MultipleKeyboardManager
	{
		//KeyPressEvent.DeviceName is the key
		static private readonly Dictionary<string, KeyStateStore> keyboardsKeyStores = new Dictionary<string, KeyStateStore>();
		static private Dictionary<string, KeyboardState> oldKeyboardStates = new Dictionary<string, KeyboardState>();

		static string attachedKeyboardID = "";
		public static string AttachedKeyboardID
		{ 
			get
			{	if (attachedKeyboardID != "") return attachedKeyboardID;
				else return "ANY";
			}
		}
		public static bool HasAttachedKeyboard() => !String.IsNullOrWhiteSpace(attachedKeyboardID);

		private static KeyboardState? oldAttachedKeyboardState;

		public MultipleKeyboardManager(HarmonyInstance harmony, InputState SInputState)
		{
			RawInput rawinput = new RawInput(Game1.game1.Window.Handle, false);
			rawinput.KeyPressed += OnKeyPressed;
		}
		
		#region Events
		private void OnKeyPressed(object sender, RawInputEventArg e)
		{
			string deviceUniqueID = e.KeyPressEvent.DeviceHandle.ToString();

			Keys key = GetKey(e.KeyPressEvent.VKey);
			if (key != Keys.None)
			{
				if (!keyboardsKeyStores.ContainsKey(deviceUniqueID))
					keyboardsKeyStores.Add(deviceUniqueID, new KeyStateStore());

				if (keyboardsKeyStores.TryGetValue(deviceUniqueID, out KeyStateStore keyStateStore))
					keyStateStore.SetKeyState((int)key, e.KeyPressEvent.IsPressed());
			}
			else
			{
				Console.WriteLine($"Unknown key \"{KeyMapper.GetMicrosoftKeyName(e.KeyPressEvent.VKey)}\"");
			}
		}

		public void OnUpdate(object sender, EventArgs args)
		{
			//Update old keyboard states
			oldKeyboardStates.Clear();
			foreach (var k in keyboardsKeyStores) oldKeyboardStates.Add(k.Key, k.Value.GetKeyboardState());

			oldAttachedKeyboardState = Keyboard.GetState();
		}

		public void OnAfterReturnToTitle(object sender, EventArgs args) => DetachKeyboard();
		#endregion

		private void DetachKeyboard()
		{
			if (!String.IsNullOrWhiteSpace(attachedKeyboardID))
			{
				attachedKeyboardID = "";
				Monitor.Log("Detached keyboard", StardewModdingAPI.LogLevel.Trace);
			}
		}

		private Keys GetKey(int key_int)
		{
			//VKey seems to (mostly) correspond to Microsoft.XNA.Framework.Input.Keys

			if (Enum.IsDefined(typeof(Keys), key_int))
				return (Keys)key_int;
			else
			{
				//https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys
				switch (key_int)
				{
					case 16: return Keys.LeftShift;
					case 17: return Keys.LeftControl;
					case 18: return Keys.LeftAlt;
					default: return Keys.None;
				}
			}
		}

		public static KeyboardState? GetAttachedKeyboardState()
		{
			if (keyboardsKeyStores != null && keyboardsKeyStores.TryGetValue(attachedKeyboardID, out KeyStateStore attachedKeyStateStore))
				return attachedKeyStateStore.GetKeyboardState();
			return null;
		}

		public static bool WasKeyJustPressed(Keys key)
		{
			if (String.IsNullOrWhiteSpace(attachedKeyboardID))
				return Game1.game1.IsActive && Keyboard.GetState().IsKeyDown(key) && oldAttachedKeyboardState?.IsKeyUp(key) == true;
			else
				return oldKeyboardStates.TryGetValue(attachedKeyboardID, out KeyboardState oldKeyboardState) && (GetAttachedKeyboardState() ?? new KeyboardState()).IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
		}

		/// <summary>
		/// Returns true if found a keyboard
		/// </summary>
		/// <returns></returns>
		public bool CheckKeyboardsToAttach()
		{
			foreach (string keyboardID in keyboardsKeyStores.Keys)
			{
				keyboardsKeyStores.TryGetValue(keyboardID, out KeyStateStore keyStateStore); var keyboardState = keyStateStore.GetKeyboardState();
				oldKeyboardStates.TryGetValue(keyboardID, out KeyboardState oldKeyboardState);

				if (keyboardState.GetPressedKeys().Length > 0)
				{
					attachedKeyboardID = keyboardID;
					Console.WriteLine("Set new attached keyboard to " + keyboardID);
					return true;
				}
			}

			return false;
		}

		public void OnDetachButtonClicked() => DetachKeyboard();
	}

	static class KeyboardExtensions
	{
		public static bool IsPressed(this KeyPressEvent keyPressEvent) => keyPressEvent.KeyPressState == "MAKE";

		public static bool AreKeysDown(this KeyboardState keyboardState, params Keys[] keys) => keys.All(x => keyboardState.IsKeyDown(x));		
	}
}
