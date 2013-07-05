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
			MenuEntry e0 = new MenuEntry("Browse servers");
			e0.Enabled = false;
			MenuEntry e1 = new MenuEntry("Connect to server");
			e1.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new ConnectMenu(), null);
			};
			MenuEntry e2 = new MenuEntry("Practice with bots");
			e2.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new ConnectionScreen(true), null);
			};
			MenuEntry e3 = new MenuEntry("Settings");
			e3.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new SettingsMenu(), null);
			};
			CancelEntry e4 = new CancelEntry("Quit");
			Entries.Add(e0);
			Entries.Add(e1);
			Entries.Add(e2);
			Entries.Add(e3);
			Entries.Add(e4);
		}
		protected override void OnCancel() {
			GameSession.Current.Exit();
		}
	}
}

