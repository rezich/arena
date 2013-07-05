using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VGame {
	public class PlayerIndexEventArgs : EventArgs {
		public PlayerIndexEventArgs(PlayerIndex playerIndex) {
			this.playerIndex = playerIndex;
		}

		public PlayerIndexEventArgs() {
		}

		public PlayerIndex PlayerIndex {
			get { return playerIndex; }
		}

		PlayerIndex playerIndex;
	}

	public class AsciiEventArgs : EventArgs {
		public AsciiEventArgs(List<char> ascii, bool backspace) {
			Chars = ascii;
			Backspace = backspace;
		}
		public List<char> Chars;
		public bool Backspace;
	}
	public class TextChangeArgs : EventArgs {
		public TextChangeArgs(string text) {
			Text = text;
		}
		public string Text;
	}
}
