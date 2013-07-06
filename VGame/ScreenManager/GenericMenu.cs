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
		public int Width = 400;
		public static int Margin = 8;
		private List<MenuEntry> entries = new List<MenuEntry>();
		private int? selectedIndex = 0;
		private bool mousing = false;
		private Shapes.Cursor cursor = new VGame.Shapes.Cursor();
		private Microsoft.Xna.Framework.Point cursorPosition = new Microsoft.Xna.Framework.Point();
		private Microsoft.Xna.Framework.Point cursorPositionLast = new Microsoft.Xna.Framework.Point();
		protected IList<MenuEntry> Entries {
			get {
				return entries;
			}
		}
		/*public int SelectedIndex {
			get {
				return selectedIndex;
			}
			set {
				int oldSelectedIndex = selectedIndex;
				selectedIndex = value;
				if (value > oldSelectedIndex) {
					do {
						if (selectedIndex >= entries.Count)
							selectedIndex -= entries.Count;
					} while (!entries[selectedIndex].Enabled);
				}
				else {
					do {
						selectedIndex--;
						if (selectedIndex < 0)
							selectedIndex += entries.Count;
					} while (!entries[selectedIndex].Enabled);
				}
			}
		}*/

		public GenericMenu(string title) {
			Title = title;
		}

		public GenericMenu(string title, bool cancelable) {
			Title = title;
			Cancelable = cancelable;
		}

		public override void HandleInput(GameTime gameTime, InputState input) {
			cursorPosition.X = input.CurrentMouseState.X;
			cursorPosition.Y = input.CurrentMouseState.Y;
			if (input.IsNewKeyPress(Keys.Scroll)) {
				ScreenManager.Game.IsMouseVisible = !ScreenManager.Game.IsMouseVisible;
			}
			if (cursorPosition != cursorPositionLast)
				mousing = true;
			if ((input.IsNewKeyPress(Keys.Down) || (input.IsNewKeyPress(Keys.Tab) && !input.IsShiftKeyDown)) && cursorPosition == cursorPositionLast) {
				mousing = false;
				if (selectedIndex == null)
					selectedIndex = 0;
				do {
					selectedIndex++;
					if (selectedIndex >= entries.Count)
						selectedIndex -= entries.Count;
				} while (!entries[(int)selectedIndex].Enabled);
			}
			if ((input.IsNewKeyPress(Keys.Up) || (input.IsNewKeyPress(Keys.Tab) && input.IsShiftKeyDown)) && cursorPosition == cursorPositionLast) {
				mousing = false;
				if (selectedIndex == null)
					selectedIndex = 0;
				do {
					selectedIndex--;
					if (selectedIndex < 0)
						selectedIndex += entries.Count;
				} while (!entries[(int)selectedIndex].Enabled);
			}

			if (input.IsNewMousePress(MouseButtons.Left)) {
				UpdateSelected();
				if (selectedIndex.HasValue && entries[(int)selectedIndex].Enabled)
					OnSelectEntry((int)selectedIndex);
			}

			if (input.IsNewKeyPress(Keys.Space) || input.IsNewKeyPress(Keys.Enter)) {
				mousing = false;
				if (selectedIndex.HasValue && entries[(int)selectedIndex].Enabled)
					OnSelectEntry((int)selectedIndex);
			}
			if (input.IsNewKeyPress(Keys.Left)) {
				mousing = false;
				if (selectedIndex.HasValue && entries[(int)selectedIndex].Enabled)
					OnSwipeLeftEntry((int)selectedIndex);
			}
			if (input.IsNewKeyPress(Keys.Right)) {
				mousing = false;
				if (selectedIndex.HasValue && entries[(int)selectedIndex].Enabled)
					OnSwipeRightEntry((int)selectedIndex);
			}
			if (input.IsNewKeyPress(Keys.Escape)) {
				OnCancel();
			}
			List<char> ascii = input.GetAscii();
			if (ascii.Count > 0 || input.IsNewKeyPress(Keys.Back)) {
				mousing = false;
				if (selectedIndex.HasValue && entries[(int)selectedIndex].Enabled)
					OnTextEntry((int)selectedIndex, ascii, input.IsNewKeyPress(Keys.Back));
			}
		}
		protected void UpdateSelected() {
			bool foundGoodOne = false;
			if (mousing)
				selectedIndex = null;
			foreach (MenuEntry e in entries) {
				if (selectedIndex == null && !mousing) {
					if (!foundGoodOne && e.Enabled) {
						foundGoodOne = true;
						selectedIndex = entries.IndexOf(e);
					}
				}
				e.IsSelected = false;
				if (mousing && e.Rectangle.Contains(cursorPosition)) {
					selectedIndex = entries.IndexOf(e);
				}
				if (selectedIndex == entries.IndexOf(e))
					e.IsSelected = true;
			}
		}
		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateSelected();
			foreach (MenuEntry e in entries)
				e.Update(gameTime);
			cursorPositionLast.X = cursorPosition.X;
			cursorPositionLast.Y = cursorPosition.Y;
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = Renderer.Context;

			Vector2 midTop = new Vector2(Renderer.Width / 2, 0);
			g.MoveTo((midTop + new Vector2(-(Width / 2), 0)).ToPointD());
			g.LineTo((midTop + new Vector2(Width / 2, 0)).ToPointD());
			g.LineTo((midTop + new Vector2(Width / 2, Renderer.Height)).ToPointD());
			g.LineTo((midTop + new Vector2(-(Width / 2), Renderer.Height)).ToPointD());
			g.ClosePath();
			g.Color = new Cairo.Color(0.5, 0.5, 0.5);
			g.Fill();

			Vector2 origin = new Vector2(Renderer.Width / 2, 8);
			Vector2 offset = new Vector2();
			if (Title != null) {
				string titleFont = "04b20";
				int titleTextSize = 16;
				int titleHeight = 24;
				Cairo.Color titleBackgroundColor = new Cairo.Color(0.25, 0.25, 0.25);
				g.MoveTo((origin + offset + new Vector2(-(Width / 2), -8)).ToPointD());
				g.LineTo((origin + offset + new Vector2(Width / 2, -8)).ToPointD());
				g.LineTo((origin + offset + new Vector2(Width / 2, titleHeight)).ToPointD());
				g.LineTo((origin + offset + new Vector2(-(Width / 2), titleHeight)).ToPointD());
				g.ClosePath();
				Util.StrokeAndFill(g, titleBackgroundColor, null);
				Util.DrawText(g, origin + offset, Title, titleTextSize, TextAlign.Center, TextAlign.Top, new Cairo.Color(0.75, 0.75, 0.75), new Cairo.Color(0, 0, 0), null, 0, titleFont);
				offset.Y += titleHeight;
			}
			offset.Y += Margin;
			foreach (MenuEntry e in entries) {
				e.Draw(g, gameTime, origin + offset, Width, 1f);
				offset.Y += e.Height;
			}
			if (this.TopActive) {
				cursor.Draw(g, new Vector2(cursorPosition.X, cursorPosition.Y), 0, new Cairo.Color(1, 1, 1), new Cairo.Color(0.1, 0.1, 0.1), 22);
				Util.DrawText(g, Vector2.Zero, Resolution.Width.ToString() + "x" + Resolution.Height.ToString(), 20, TextAlign.Left, TextAlign.Top, new Cairo.Color(1, 1, 1), new Cairo.Color(0, 0, 0), null, 0, null);
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
		public Cairo.Color DisabledFillColor = new Cairo.Color(0.25, 0.25, 0.25);
		public Cairo.Color NormalFillColor = new Cairo.Color(1, 1, 1);
		public Cairo.Color SelectedFillColor = new Cairo.Color(0, 1, 1);
		public Cairo.Color LabelFillColor = new Cairo.Color(0.75, 0.75, 0.75);
		public virtual Cairo.Color FillColor {
			get {
				return Enabled ? IsSelected ? SelectedFillColor : NormalFillColor : DisabledFillColor;
			}
		}
		public virtual Cairo.Color FillColor2 {
			get {
				return FillColor;
			}
		}
		public bool IsSelected = false;
		public bool Enabled {
			get {
				return selectable && Visible;
			}
			set {
				selectable = value;
			}
		}
		public Microsoft.Xna.Framework.Rectangle Rectangle = new Microsoft.Xna.Framework.Rectangle();
		public bool Visible = true;
		public bool IsCancel = false;
		protected bool selectable = true;

		public int Height = 22;
		public string Font = "04b_19";
		public string Font2 = "04b_19";
		public int TextSize = 20;
		public int TextSize2 = 20;
		public Vector2 TextOffset = Vector2.Zero;
		public Vector2 TextOffset2 = Vector2.Zero;
		public Cairo.Color TextStroke = new Cairo.Color(0, 0, 0);
		public Cairo.Color TextStroke2 = new Cairo.Color(0, 0, 0);
		public Cairo.Color? BackgroundColor = null;

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

		public virtual void Update(GameTime gameTime) {
		}
		public virtual void Draw(Cairo.Context g, GameTime gameTime, Vector2 origin, int width, float alpha) {
			Rectangle.X = (int)(origin.X - width / 2);
			Rectangle.Y = (int)origin.Y;
			Rectangle.Width = (int)width;
			Rectangle.Height = Height;
			if (BackgroundColor.HasValue) {
				g.MoveTo((origin + new Vector2(-(width / 2), 0)).ToPointD());
				g.LineTo((origin + new Vector2(width / 2, 0)).ToPointD());
				g.LineTo((origin + new Vector2(width / 2, Height)).ToPointD());
				g.LineTo((origin + new Vector2(-(width / 2), Height)).ToPointD());
				g.ClosePath();
				Util.StrokeAndFill(g, null, BackgroundColor);
			}
			if (Text2 != null) {
				Util.DrawText(g, origin + new Vector2(-4, Height / 2) + TextOffset, Text, TextSize, TextAlign.Right, TextAlign.Middle, FillColor, TextStroke, null, 0, Font);
				Util.DrawText(g, origin + new Vector2(4, Height / 2) + TextOffset2, Text2, TextSize2, TextAlign.Left, TextAlign.Middle, FillColor2, TextStroke2, null, 0, Font2);
			}
			else {
				Util.DrawText(g, origin + new Vector2(0, Height / 2) + TextOffset, Text, TextSize, TextAlign.Center, TextAlign.Middle, FillColor, TextStroke, null, 0, Font);
			}
		}
	}
	public class TextInputEntry : MenuEntry {
		public int CharacterLimit = 16;
		protected string Text2Last = "";
		public TextInputEntry(string text, string editableText) : base(text) {
			Text2 = editableText;
			Text2Last = Text2;
			Font2 = "04b25";
			TextSize2 = 24;
			TextOffset2 = new Vector2(0, 0);
		}
		public override Cairo.Color FillColor {
			get {
				return Enabled ? LabelFillColor : DisabledFillColor;
			}
		}
		public override Cairo.Color FillColor2 {
			get {
				return base.FillColor;
			}
		}
		public virtual bool IsCharValid(char c) {
			return true;
		}
		public virtual bool IsStringValid(string s) {
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
			if (IsStringValid(Text2)) {
				if (Text2 != Text2Last) {
					OnTextChanged();
				}
				Text2Last = Text2;
			}
			else {
				Text2 = Text2Last;
			}
		}
	}
	public class NumberInputEntry : TextInputEntry {
		public int? Maximum = 49;
		public NumberInputEntry(string text, int number) : base(text, number.ToString()) {
			TextChanged += delegate(object sender, TextChangeArgs e) {
				if (e.Text == "") {
					Text2 = "0";
					e.Text = "0";
				}
			};
		}
		public override void OnTextInputEntry(List<char> ascii, bool backspace) {
			if (Text2 == "0") {
				bool okay = true;
				if (ascii.Count > 0) {
					foreach (char c in ascii) {
						if (!IsCharValid(c))
							okay = false;
					}
				}
				if (okay)
					Text2 = "";
			}
			base.OnTextInputEntry(ascii, backspace);
		}
		public override bool IsCharValid(char c) {
			return char.IsDigit(c);
		}
		public override bool IsStringValid(string s) {
			if (Text2 == "")
				Text2 = "0";
			if (Maximum.HasValue) {
				if (int.Parse(Text2) > Maximum) {
					Text2 = Maximum.ToString();
				}
			}
			return true;
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
	public class SelectManyEntry : MenuEntry {
		public List<string> Options;
		public int SelectedIndex = 0;
		public SelectManyEntry(string text, List<string> options) : base(text) {
			Options = options;
			SwipeLeft += delegate(object sender, PlayerIndexEventArgs e) {
				SelectedIndex--;
				if (SelectedIndex < 0)
					SelectedIndex += Options.Count;
			};
			SwipeRight += delegate(object sender, PlayerIndexEventArgs e) {
				SelectedIndex++;
				if (SelectedIndex >= Options.Count)
					SelectedIndex -= Options.Count;
			};
			Selected += delegate(object sender, PlayerIndexEventArgs e) {
				SelectedIndex++;
				if (SelectedIndex >= Options.Count)
					SelectedIndex -= Options.Count;
			};
		}
		public override void Update(GameTime gameTime) {
			Text2 = Options[SelectedIndex];
		}
	}
	public class HeadingEntry : MenuEntry {
		public static Cairo.Color HeadingColor = new Cairo.Color(0.65, 0.65, 0.65);
		public HeadingEntry(string text) : base(text) {
			selectable = false;
			Font = "04b25";
			TextSize = 18;
			Height = 20;
		}
		public override Cairo.Color FillColor {
			get {
				return HeadingColor;
			}
		}
	}
	public class SpacerEntry : MenuEntry {
		public SpacerEntry() : base("") {
			Enabled = false;
			Height = GenericMenu.Margin;
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

