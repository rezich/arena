using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class MainMenu : Menu {
		public MainMenu() : base("arena") {

			Entries.Add(new MenuEntry(this, "BROWSE SERVERS"));
			Entries.Last().Enabled = false;

			Entries.Add(new MenuEntry(this, "CONNECT TO SERVER"));
			Entries.Last().Selected += delegate(object sender, EventArgs e) {
				StateManager.AddState(new ConnectMenu());
			};

			Entries.Add(new MenuEntry(this, "PRACTICE WITH BOTS"));
			Entries.Last().Selected += delegate(object sender, EventArgs e) {
				StateManager.AddState(new ConnectionScreen(""));
			};

			Entries.Add(new SpacerEntry(this));

			Entries.Add(new MenuEntry(this, "SETTINGS"));
			Entries.Last().Selected += delegate(object sender, EventArgs e) {
				StateManager.AddState(new SettingsMenu(false));
			};

			Entries.Add(new MenuEntry(this, "MANUAL"));
			Entries.Last().Enabled = false;

			Entries.Add(new SpacerEntry(this));

			Entries.Add(new CancelEntry(this, "QUIT"));

		}
		protected override void OnCancel() {
			Command.Parse("quit").Run(StateManager.Game.Cmd);
		}
	}
}

