using System;
using System.Linq;
using VGame;

namespace ArenaClient {
	public class PauseMenu : Menu {
		public PauseMenu() : base("PAUSED") {
			Entries.Add(new MenuEntry(this, "RESUME"));
			Entries.Last().Selected += delegate(object sender, EventArgs e) {
				StateManager.RemoveState();
			};
			Entries.Last().IsCancel = true;

			Entries.Add(new MenuEntry(this, "SETTINGS"));
			Entries.Last().Selected += delegate(object sender, EventArgs e) {
				StateManager.AddState(new SettingsMenu(true));
			};

			Entries.Add(new MenuEntry(this, "QUIT"));
			Entries.Last().Selected += delegate(object sender, EventArgs e) {
				StateManager.ReplaceAllStates(new TitleScreen());
			};
		}
	}
}

