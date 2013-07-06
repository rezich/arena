using System;
using System.Linq;
using VGame;

namespace ArenaClient {
	public class SettingsMenu : GenericMenu  {
		string newName = Arena.Config.PlayerName;
		int newNumber = Arena.Config.PlayerNumber;
		MenuEntry saveEntry;
		public SettingsMenu() : base("SETTINGS") {

			Entries.Add(new HeadingEntry("PLAYER"));

			Entries.Add(new TextInputEntry("NAME", Arena.Config.PlayerName));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				newName = e.Text;
				saveEntry.Enabled = (newName != Arena.Config.PlayerName || newNumber != Arena.Config.PlayerNumber);
			};

			Entries.Add(new NumberInputEntry("NUMBER", Arena.Config.PlayerNumber));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				newNumber = int.Parse(e.Text);
				saveEntry.Enabled = (newName != Arena.Config.PlayerName || newNumber != Arena.Config.PlayerNumber);
			};

			Entries.Add(new SpacerEntry());

			Entries.Add(new HeadingEntry("GRAPHICS"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("RESOLUTION", "1280x720"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("FULLSCREEN", "OFF"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("VSYNC", "OFF"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("ANTIALIASING", "ON"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("DOUBLE-BUFFERING", "ON"));
			Entries.Last().Enabled = false;

			Entries.Add(new SpacerEntry());

			saveEntry = new MenuEntry("SAVE");
			saveEntry.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				Arena.Config.PlayerName = newName;
				Arena.Config.PlayerNumber = newNumber;
				ExitScreen();
			};
			saveEntry.Enabled = false;
			Entries.Add(saveEntry);

			Entries.Add(new CancelEntry("BACK"));

		}
		protected override void OnCancel() {
			ExitScreen();
			base.OnCancel();
		}
	}
}

