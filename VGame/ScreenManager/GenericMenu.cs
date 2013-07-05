using System;
using System.Collections;
using System.Collections.Generic;
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

		public override void Draw(GameTime gameTime) {
			Cairo.Context g = Renderer.Context;
			for (int i = 0; i < entries.Count; i++) {
				entries[i].Draw(g, gameTime, new Vector2(0, 0), 1f);
			}
		}

		protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex) {
			entries[entryIndex].OnSelectEntry(playerIndex);
		}
		protected virtual void OnSwipeLeftEntry(int entryIndex, PlayerIndex playerIndex) {
			entries[entryIndex].OnSwipeLeftEntry(playerIndex);
		}
		protected virtual void OnSwipeRightEntry(int entryIndex, PlayerIndex playerIndex) {
			entries[entryIndex].OnSwipeRightEntry(playerIndex);
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

		public event EventHandler<PlayerIndexEventArgs> Selected;
		public event EventHandler<PlayerIndexEventArgs> SwipeLeft;
		public event EventHandler<PlayerIndexEventArgs> SwipeRight;

		public MenuEntry(string text) {
			Text = text;
		}

		protected internal virtual void OnSelectEntry(PlayerIndex playerIndex) {
			if (Selected != null)
				Selected(this, new PlayerIndexEventArgs(playerIndex));
		}

		protected internal virtual void OnSwipeLeftEntry(PlayerIndex playerIndex) {
			if (SwipeLeft != null)
				SwipeLeft(this, new PlayerIndexEventArgs(playerIndex));
		}

		protected internal virtual void OnSwipeRightEntry(PlayerIndex playerIndex) {
			if (SwipeRight != null)
				SwipeRight(this, new PlayerIndexEventArgs(playerIndex));
		}

		public virtual void Draw(Cairo.Context g, GameTime gameTime, Vector2 origin, float alpha) {
		}
	}
	public class HeadingEntry : MenuEntry {
		public HeadingEntry(string text) : base(text) {
			selectable = false;
		}
	}
	public class SpacerEntry : MenuEntry {
		public SpacerEntry() : base("") {
			Enabled = false;
		}
	}
}

