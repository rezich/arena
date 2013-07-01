using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace VGame {
	public enum MouseButtons {
		Left,
		Right,
		Middle,
		X1,
		X2
	}
	public enum InputMethods {
		Gamepad,
		KeyboardMouse
	}
	public enum Inputs {
		MenuUp,
		MenuDown,
		MenuLeft,
		MenuRight,
		MenuCancel,
		MenuAccept,

		GamePause,
		GameMoveNorth,
		GameMoveSouth,
		GameMoveEast,
		GameMoveWest,

		PressStart
	}
	public class InputState {
		public const int MaxInputs = 4;

		public KeyboardState CurrentKeyboardState;
		public readonly GamePadState[] CurrentGamePadStates;
		public MouseState CurrentMouseState;

		public KeyboardState LastKeyboardState;
		public readonly GamePadState[] LastGamePadStates;
		public MouseState LastMouseState;

		public static InputMethods InputMethod;

		public readonly bool[] GamePadWasConnected;

		public InputState() {
			CurrentKeyboardState = new KeyboardState();
			CurrentGamePadStates = new GamePadState[MaxInputs];

			LastKeyboardState = new KeyboardState();
			LastGamePadStates = new GamePadState[MaxInputs];

			CurrentMouseState = new MouseState();
			LastMouseState = new MouseState();

			GamePadWasConnected = new bool[MaxInputs];
		}

		public void Update() {
			LastMouseState = CurrentMouseState;
			CurrentMouseState = Mouse.GetState();
			LastKeyboardState = CurrentKeyboardState;
			CurrentKeyboardState = Keyboard.GetState();
			for (int i = 0; i < MaxInputs; i++) {
				LastGamePadStates[i] = CurrentGamePadStates[i];

				CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

				if (CurrentGamePadStates[i].IsConnected) {
					GamePadWasConnected[i] = true;
				}
			}
			if (IsKeyboardInteracted() || IsMouseInteracted()) InputMethod = InputMethods.KeyboardMouse;
			if (IsGamepadInteracted()) InputMethod = InputMethods.Gamepad;
		}

		public bool IsNewKeyPress(Keys key) {
			return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
		}

		public bool IsKeyDown(Keys key) {
			return CurrentKeyboardState.IsKeyDown(key);
		}

		public bool IsShiftKeyDown {
			get {
				return IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
			}
		}
		public bool IsControlKeyDown {
			get {
				return IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
			}
		}
		public bool IsAltKeyDown {
			get {
				return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
			}
		}

		public bool IsKeyUp(Keys key) {
			return CurrentKeyboardState.IsKeyUp(key);
		}

		public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
		                             out PlayerIndex playerIndex) {
			if (controllingPlayer.HasValue) {
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return (CurrentGamePadStates[i].IsButtonDown(button) &&
				        LastGamePadStates[i].IsButtonUp(button));
			}
			else {
				return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
				        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
				        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
				        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool IsNewMousePress(MouseButtons button) {
			switch (button) {
				case MouseButtons.Left: return CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton != ButtonState.Pressed;
					case MouseButtons.Middle: return CurrentMouseState.MiddleButton == ButtonState.Pressed && LastMouseState.MiddleButton != ButtonState.Pressed;
					case MouseButtons.Right: return CurrentMouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton != ButtonState.Pressed;
					case MouseButtons.X1: return CurrentMouseState.XButton1 == ButtonState.Pressed && LastMouseState.XButton1 != ButtonState.Pressed;
					case MouseButtons.X2: return CurrentMouseState.XButton2 == ButtonState.Pressed && LastMouseState.XButton2 != ButtonState.Pressed;
					default: return false;
			}
		}

		public bool IsMouseMoved() {
			return CurrentMouseState.X != LastMouseState.X || CurrentMouseState.Y != CurrentMouseState.Y;
		}

		public bool IsMouseInteracted() {
			return CurrentMouseState != LastMouseState;
		}

		public bool IsKeyboardInteracted() {
			for (int i = 0; i < MaxInputs; i++) {
				if (CurrentKeyboardState != LastKeyboardState) return true;
			}
			return false;
		}

		public bool IsGamepadInteracted() {
			/*for (int i = 0; i < MaxInputs; i++) {
				if (CurrentGamePadStates[i] != LastGamePadStates[i]) return true;
			}
			return false;*/
			for (int i = 0; i < MaxInputs; i++) {
				if (
					CurrentGamePadStates[i].Buttons.A != LastGamePadStates[i].Buttons.A ||
					CurrentGamePadStates[i].Buttons.B != LastGamePadStates[i].Buttons.B ||
					CurrentGamePadStates[i].Buttons.X != LastGamePadStates[i].Buttons.X ||
					CurrentGamePadStates[i].Buttons.Y != LastGamePadStates[i].Buttons.Y ||
					CurrentGamePadStates[i].Buttons.Start != LastGamePadStates[i].Buttons.Start ||
					CurrentGamePadStates[i].Buttons.Back != LastGamePadStates[i].Buttons.Back ||
					CurrentGamePadStates[i].Buttons.BigButton != LastGamePadStates[i].Buttons.BigButton ||
					CurrentGamePadStates[i].Buttons.LeftShoulder != LastGamePadStates[i].Buttons.LeftShoulder ||
					CurrentGamePadStates[i].Buttons.RightShoulder != LastGamePadStates[i].Buttons.RightShoulder ||
					CurrentGamePadStates[i].Buttons.LeftStick != LastGamePadStates[i].Buttons.LeftStick ||
					CurrentGamePadStates[i].Buttons.RightStick != LastGamePadStates[i].Buttons.RightStick ||
					CurrentGamePadStates[i].DPad.Up != CurrentGamePadStates[i].DPad.Up ||
					CurrentGamePadStates[i].DPad.Left != CurrentGamePadStates[i].DPad.Left ||
					CurrentGamePadStates[i].DPad.Right != CurrentGamePadStates[i].DPad.Right ||
					CurrentGamePadStates[i].DPad.Down != CurrentGamePadStates[i].DPad.Down ||
					CurrentGamePadStates[i].ThumbSticks.Left.Length() > 0.1 ||
					CurrentGamePadStates[i].ThumbSticks.Right.Length() > 0.1 ||
					CurrentGamePadStates[i].Triggers.Left > 0.1 ||
					CurrentGamePadStates[i].Triggers.Right > 0.1
					) return true;
			}
			return false;
		}

		public List<char> GetAscii() {
			List<char> chars = new List<char>();
			// This is super fucking shitty but neither XNA nor Monogame provides
			// a platform-agnostic way of getting ASCII text input.
			// I'll probably eventually do a keyboard layout thing or something
			// fuck I don't know.
			if (IsNewKeyPress(Keys.OemTilde))
				chars.Add(IsShiftKeyDown ? '~' : '`');
			if (IsNewKeyPress(Keys.D1))
				chars.Add(IsShiftKeyDown ? '!' : '1');
			if (IsNewKeyPress(Keys.D2))
				chars.Add(IsShiftKeyDown ? '@' : '2');
			if (IsNewKeyPress(Keys.D3))
				chars.Add(IsShiftKeyDown ? '#' : '3');
			if (IsNewKeyPress(Keys.D4))
				chars.Add(IsShiftKeyDown ? '$' : '4');
			if (IsNewKeyPress(Keys.D5))
				chars.Add(IsShiftKeyDown ? '%' : '5');
			if (IsNewKeyPress(Keys.D6))
				chars.Add(IsShiftKeyDown ? '^' : '6');
			if (IsNewKeyPress(Keys.D7))
				chars.Add(IsShiftKeyDown ? '&' : '7');
			if (IsNewKeyPress(Keys.D8))
				chars.Add(IsShiftKeyDown ? '*' : '8');
			if (IsNewKeyPress(Keys.D9))
				chars.Add(IsShiftKeyDown ? '(' : '9');
			if (IsNewKeyPress(Keys.D0))
				chars.Add(IsShiftKeyDown ? ')' : '0');
			if (IsNewKeyPress(Keys.OemMinus))
				chars.Add(IsShiftKeyDown ? '_' : '-');
			if (IsNewKeyPress(Keys.OemPlus))
				chars.Add(IsShiftKeyDown ? '+' : '=');
			if (IsNewKeyPress(Keys.OemOpenBrackets))
				chars.Add(IsShiftKeyDown ? '{' : '[');
			if (IsNewKeyPress(Keys.OemCloseBrackets))
				chars.Add(IsShiftKeyDown ? '}' : ']');
			if (IsNewKeyPress(Keys.OemBackslash))
				chars.Add(IsShiftKeyDown ? '|' : '\\');

			if (IsNewKeyPress(Keys.A))
				chars.Add(IsShiftKeyDown ? 'A' : 'a');
			if (IsNewKeyPress(Keys.B))
				chars.Add(IsShiftKeyDown ? 'B' : 'b');
			if (IsNewKeyPress(Keys.C))
				chars.Add(IsShiftKeyDown ? 'C' : 'c');
			if (IsNewKeyPress(Keys.D))
				chars.Add(IsShiftKeyDown ? 'D' : 'd');
			if (IsNewKeyPress(Keys.E))
				chars.Add(IsShiftKeyDown ? 'E' : 'e');
			if (IsNewKeyPress(Keys.F))
				chars.Add(IsShiftKeyDown ? 'F' : 'f');
			if (IsNewKeyPress(Keys.G))
				chars.Add(IsShiftKeyDown ? 'G' : 'g');
			if (IsNewKeyPress(Keys.H))
				chars.Add(IsShiftKeyDown ? 'H' : 'h');
			if (IsNewKeyPress(Keys.I))
				chars.Add(IsShiftKeyDown ? 'I' : 'i');
			if (IsNewKeyPress(Keys.J))
				chars.Add(IsShiftKeyDown ? 'J' : 'j');
			if (IsNewKeyPress(Keys.K))
				chars.Add(IsShiftKeyDown ? 'K' : 'k');
			if (IsNewKeyPress(Keys.L))
				chars.Add(IsShiftKeyDown ? 'L' : 'l');
			if (IsNewKeyPress(Keys.M))
				chars.Add(IsShiftKeyDown ? 'M' : 'm');
			if (IsNewKeyPress(Keys.N))
				chars.Add(IsShiftKeyDown ? 'N' : 'n');
			if (IsNewKeyPress(Keys.O))
				chars.Add(IsShiftKeyDown ? 'O' : 'o');
			if (IsNewKeyPress(Keys.P))
				chars.Add(IsShiftKeyDown ? 'P' : 'p');
			if (IsNewKeyPress(Keys.Q))
				chars.Add(IsShiftKeyDown ? 'Q' : 'q');
			if (IsNewKeyPress(Keys.R))
				chars.Add(IsShiftKeyDown ? 'R' : 'r');
			if (IsNewKeyPress(Keys.S))
				chars.Add(IsShiftKeyDown ? 'S' : 's');
			if (IsNewKeyPress(Keys.T))
				chars.Add(IsShiftKeyDown ? 'T' : 't');
			if (IsNewKeyPress(Keys.U))
				chars.Add(IsShiftKeyDown ? 'U' : 'u');
			if (IsNewKeyPress(Keys.V))
				chars.Add(IsShiftKeyDown ? 'V' : 'v');
			if (IsNewKeyPress(Keys.W))
				chars.Add(IsShiftKeyDown ? 'W' : 'w');
			if (IsNewKeyPress(Keys.X))
				chars.Add(IsShiftKeyDown ? 'X' : 'x');
			if (IsNewKeyPress(Keys.Y))
				chars.Add(IsShiftKeyDown ? 'Y' : 'y');
			if (IsNewKeyPress(Keys.Z))
				chars.Add(IsShiftKeyDown ? 'Z' : 'z');

			if (IsNewKeyPress(Keys.OemSemicolon))
				chars.Add(IsShiftKeyDown ? ':' : ';');
			if (IsNewKeyPress(Keys.OemQuotes))
				chars.Add(IsShiftKeyDown ? '"' : '\'');
			if (IsNewKeyPress(Keys.OemComma))
				chars.Add(IsShiftKeyDown ? '<' : ',');
			if (IsNewKeyPress(Keys.OemPeriod))
			    chars.Add(IsShiftKeyDown ? '>' : '.');
			if (IsNewKeyPress(Keys.OemQuestion))
				chars.Add(IsShiftKeyDown ? '?' : '/');
			if (IsNewKeyPress(Keys.Space))
				chars.Add(' ');

			return chars;
		}

		/*public bool IsInput(Inputs input, PlayerIndex? controllingPlayer) {
			PlayerIndex playerIndex;
			return IsInput(input, controllingPlayer, out playerIndex);
		}*/

		/*public bool IsInput(Inputs input, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex) {
			switch (input) {
				case Inputs.MenuAccept:
					return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
						IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
							IsNewKeyPress(Keys.X, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);

					case Inputs.MenuCancel:
					return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
						IsNewKeyPress(Keys.Z, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);

					case Inputs.MenuUp:
					return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
						IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);

					case Inputs.MenuDown:
					return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
						IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);

					case Inputs.MenuLeft:
					return IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex) ||
						IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex);

					case Inputs.MenuRight:
					return IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex) ||
						IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex);

					case Inputs.GamePause:
					return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
						IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);

					case Inputs.PressStart:
					return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
						IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
							IsNewKeyPress(Keys.X, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex) ||
							IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex);

					default:
					goto case Inputs.MenuAccept;
			}
		}*/

		public Vector2 LeftThumbstick(PlayerIndex? controllingPlayer) {
			if (controllingPlayer == null) return Vector2.Zero;
			return CurrentGamePadStates[(int)controllingPlayer].ThumbSticks.Left * new Vector2(1, -1);
		}

		public Vector2 RightThumbstick(PlayerIndex? controllingPlayer) {
			if (controllingPlayer == null) return Vector2.Zero;
			return CurrentGamePadStates[(int)controllingPlayer].ThumbSticks.Right * new Vector2(1, -1);
		}
	}
}
