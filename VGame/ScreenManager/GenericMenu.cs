using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;

namespace VGame {
	public class GenericMenu : GameScreen {
		public string Title;
		public string Description;
		public bool Cancelable = true;
		public bool ShowCancel = true;
		bool initialized = false;
		private List<MenuEntry> entries = new List<MenuEntry>();
		private int selectedIndex = 0;
		protected IList<MenuEntry> Entries {
			get {
				return entries;
			}
		}

		public GenericMenu(string title) {
			Title = title;
		}

		public GenericMenu(string title, bool cancelable) {
			Title = title;
			Cancelable = cancelable;
		}

		public override void HandleInput(GameTime gameTime, InputState input) {
			if (input.IsNewKeyPress(Keys.Down)) {
				do {
					selectedIndex++;
					if (selectedIndex >= entries.Count)
						selectedIndex -= entries.Count;
				} while (!entries[selectedIndex].Enabled);
			}
			if (input.IsNewKeyPress(Keys.Up)) {
				do {
					selectedIndex--;
					if (selectedIndex < 0)
						selectedIndex += entries.Count;
				} while (!entries[selectedIndex].Enabled);
			}

			if (input.IsNewKeyPress(Keys.Space) || input.IsNewKeyPress(Keys.Enter)) {
				OnSelectEntry(selectedIndex);
			}
			if (input.IsNewKeyPress(Keys.Left)) {
				OnSwipeLeftEntry(selectedIndex);
			}
			if (input.IsNewKeyPress(Keys.Right)) {
				OnSwipeRightEntry(selectedIndex);
			}
			if (input.IsNewKeyPress(Keys.Escape)) {
				OnCancel();
			}
			List<char> ascii = input.GetAscii();
			if (ascii.Count > 0 || input.IsNewKeyPress(Keys.Back)) {
				OnTextEntry(selectedIndex, ascii, input.IsNewKeyPress(Keys.Back));
			}
		}
		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			bool foundGoodOne = false;
			foreach (MenuEntry e in entries) {
				if (!initialized) {
					if (!foundGoodOne && e.Enabled) {
						foundGoodOne = true;
						selectedIndex = entries.IndexOf(e);
						initialized = true;
					}
				}
				e.IsSelected = false;
			}
			entries[selectedIndex].IsSelected = true;
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = Renderer.Context;

			Vector2 midTop = new Vector2(Renderer.Width / 2, 0);
			g.MoveTo((midTop + new Vector2(-150, 0)).ToPointD());
			g.LineTo((midTop + new Vector2(150, 0)).ToPointD());
			g.LineTo((midTop + new Vector2(150, Renderer.Height)).ToPointD());
			g.LineTo((midTop + new Vector2(-150, Renderer.Height)).ToPointD());
			g.ClosePath();
			g.Color = new Cairo.Color(0.5, 0.5, 0.5);
			g.Fill();

			Vector2 origin = new Vector2(Renderer.Width / 2, 8);
			Vector2 offset = new Vector2();
			if (Title != null) {
				Util.DrawText(g, origin + offset, Title, 20, TextAlign.Center, TextAlign.Top, new Cairo.Color(0.75, 0.75, 0.75), new Cairo.Color(0, 0, 0), new Cairo.Color(0.25, 0.25, 0.25), 0, null);
				offset.Y += 24;
			}
			foreach (MenuEntry e in entries) {
				e.Draw(g, gameTime, origin + offset, 1f);
				offset.Y += e.Height;
			}
		}

		protected virtual void OnSelectEntry(int entryIndex) {
			if (entries[entryIndex].IsCancel)
				OnCancel();
			else
				entries[entryIndex].OnSelectEntry();
		}
		protected virtual void OnSwipeLeftEntry(int entryIndex) {
			entries[entryIndex].OnSwipeLeftEntry();
		}
		protected virtual void OnSwipeRightEntry(int entryIndex) {
			entries[entryIndex].OnSwipeRightEntry();
		}
		protected virtual void OnTextEntry(int entryIndex, List<char> ascii, bool backspace) {
			entries[entryIndex].OnTextInputEntry(ascii, backspace);
		}
		protected virtual void OnCancel() {
			ExitScreen();
		}
		protected void OnCancel(object sender, PlayerIndexEventArgs e) {
			OnCancel();
		}
	}

	public class MenuEntry {
		public string Text;
		public string Text2;
		public string Content;
		public bool IsSelected = false;
		public bool Enabled {
			get {
				return selectable && Visible;
			}
			set {
				selectable = value;
			}
		}
		public bool Visible = true;
		public bool IsCancel = false;
		protected bool selectable = true;

		public int Height = 22;
		public int FontSize = 20;

		public event EventHandler<PlayerIndexEventArgs> Selected;
		public event EventHandler<PlayerIndexEventArgs> SwipeLeft;
		public event EventHandler<PlayerIndexEventArgs> SwipeRight;
		public event EventHandler<AsciiEventArgs> TextInput;
		public event EventHandler<TextChangeArgs> TextChanged;

		public MenuEntry(string text) {
			Text = text;
		}

		protected internal virtual void OnSelectEntry() {
			if (Selected != null)
				Selected(this, new PlayerIndexEventArgs());
		}

		public virtual void OnSwipeLeftEntry() {
			if (SwipeLeft != null)
				SwipeLeft(this, new PlayerIndexEventArgs());
		}

		public virtual void OnSwipeRightEntry() {
			if (SwipeRight != null)
				SwipeRight(this, new PlayerIndexEventArgs());
		}
		public virtual void OnTextInputEntry(List<char> ascii, bool backspace) {
			if (TextInput != null)
				TextInput(this, new AsciiEventArgs(ascii, backspace));
		}
		public virtual void OnTextChanged() {
			TextChanged(this, new TextChangeArgs(Text2));
		}

		public virtual void Draw(Cairo.Context g, GameTime gameTime, Vector2 origin, float alpha) {
			if (Text2 != null) {
				Util.DrawText(g, origin + new Vector2(-4, 0), Text, FontSize, TextAlign.Right, TextAlign.Top, IsSelected ? new Cairo.Color(0, 1, 1) : new Cairo.Color(1, 1, 1), new Cairo.Color(0, 0, 0), null, 0, null);
				Util.DrawText(g, origin + new Vector2(4, 0), Text2, FontSize, TextAlign.Left, TextAlign.Top, IsSelected ? new Cairo.Color(0, 1, 1) : new Cairo.Color(1, 1, 1), new Cairo.Color(0, 0, 0), null, 0, null);
			}
			else {
				Util.DrawText(g, origin, Text, FontSize, TextAlign.Center, TextAlign.Top, IsSelected ? new Cairo.Color(0, 1, 1) : new Cairo.Color(1, 1, 1), new Cairo.Color(0, 0, 0), null, 0, null);
			}
		}
	}
	public class TextInputEntry : MenuEntry {
		public int CharacterLimit = 16;
		protected string Text2Last = "";
		public TextInputEntry(string text, string editableText) : base(text) {
			Text2 = editableText;
			Text2Last = Text2;
		}
		public virtual bool IsCharValid(char c) {
			return true;
		}
		public override void OnTextInputEntry(List<char> ascii, bool backspace) {
			if (backspace && Text2.Length > 0) {
				Text2 = Text2.Substring(0, Text2.Length - 1);
			}
			if (ascii.Count > 0) {
				foreach (char c in ascii) {
					if (IsCharValid(c))
						Text2 += c;
				}
			}
			if (Text2 != Text2Last) {
				OnTextChanged();
			}
			Text2Last = Text2;
		}
	}
	public class NumberInputEntry : TextInputEntry {
		public NumberInputEntry(string text, int number) : base(text, number.ToString()) {
		}
		public override bool IsCharValid(char c) {
			return char.IsDigit(c);
		}
	}
	public class AddressInputEntry : TextInputEntry {
		protected static Regex regex = new Regex(@"[a-zA-Z0-9\.\-]", RegexOptions.IgnoreCase);
		public AddressInputEntry(string text, string address) : base(text, address) {
		}
		public override bool IsCharValid(char c) {
			return regex.IsMatch(c.ToString());
		}
	}
	public class HeadingEntry : MenuEntry {
		public HeadingEntry(string text) : base(text) {
			selectable = false;
			FontSize = 14;
			Height = 16;
		}
	}
	public class SpacerEntry : MenuEntry {
		public SpacerEntry() : base("") {
			Enabled = false;
		}
	}
	public class CancelEntry : MenuEntry {
		public CancelEntry() : base("Cancel") {
			IsCancel = true;
		}
		public CancelEntry(string text) : base(text) {
			IsCancel = true;
		}
	}
}

