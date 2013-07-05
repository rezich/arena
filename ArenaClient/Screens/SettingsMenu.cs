using System;
using VGame;

namespace ArenaClient {
	public class SettingsMenu : GenericMenu  {
		string newName = Arena.Config.PlayerName;
		int newNumber = Arena.Config.PlayerNumber;
		MenuEntry saveEntry;
		public SettingsMenu() : base("SETTINGS") {
			MenuEntry e1 = new HeadingEntry("Player");
			TextInputEntry e2 = new TextInputEntry("Name", Arena.Config.PlayerName);
			e2.TextChanged += delegate(object sender, TextChangeArgs e) {
				newName = e.Text;
				saveEntry.Enabled = (newName != Arena.Config.PlayerName || newNumber != Arena.Config.PlayerNumber);
			};
			NumberInputEntry e3 = new NumberInputEntry("Number", Arena.Config.PlayerNumber);
			e3.TextChanged += delegate(object sender, TextChangeArgs e) {
				newNumber = int.Parse(e.Text);
				saveEntry.Enabled = (newName != Arena.Config.PlayerName || newNumber != Arena.Config.PlayerNumber);
			};
			saveEntry = new MenuEntry("Save");
			saveEntry.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				Arena.Config.PlayerName = newName;
				Arena.Config.PlayerNumber = newNumber;
				ExitScreen();
			};
			saveEntry.Enabled = false;
			CancelEntry e4 = new CancelEntry("Back");
			Entries.Add(e1);
			Entries.Add(e2);
			Entries.Add(e3);
			Entries.Add(saveEntry);
			Entries.Add(e4);
		}
		protected override void OnCancel() {
			ExitScreen();
			base.OnCancel();
		}
	}
}

