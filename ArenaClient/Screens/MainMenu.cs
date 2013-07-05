using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class MainMenu : GenericMenu {
		public MainMenu() : base("arena", false) {
			ShowCancel = false;
			MenuEntry e1 = new MenuEntry("Find lobby");
			e1.Enabled = false;
			MenuEntry e2 = new MenuEntry("Settings");
			e2.Selected += Settings;
			MenuEntry e3 = new MenuEntry("Quit");
			e3.IsCancel = true;
			Entries.Add(e1);
			Entries.Add(e2);
			Entries.Add(e3);
		}
		public void Settings(object sender, PlayerIndexEventArgs e) {
			ScreenManager.AddScreen(new SettingsScreen(), ControllingPlayer);
		}
		protected override void OnCancel() {
			GameSession.Current.Exit();
		}
	}
}

