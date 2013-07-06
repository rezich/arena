using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class MainMenu : GenericMenu {
		public MainMenu() : base("arena", false) {

			Entries.Add(new MenuEntry("BROWSE SERVERS"));
			Entries.Last().Enabled = false;

			Entries.Add(new MenuEntry("CONNECT TO SERVER"));
			Entries.Last().Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new ConnectMenu(), null);
			};

			Entries.Add(new MenuEntry("PRACTICE WITH BOTS"));
			Entries.Last().Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new ConnectionScreen(true), null);
			};

			Entries.Add(new SpacerEntry());

			Entries.Add(new MenuEntry("SETTINGS"));
			Entries.Last().Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new SettingsMenu(), null);
			};

			Entries.Add(new MenuEntry("MANUAL"));
			Entries.Last().Enabled = false;

			Entries.Add(new SpacerEntry());

			Entries.Add(new CancelEntry("QUIT"));

		}
		protected override void OnCancel() {
			GameSession.Current.Exit();
		}
	}
}

