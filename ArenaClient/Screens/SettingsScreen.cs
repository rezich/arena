using System;
using VGame;

namespace ArenaClient {
	public class SettingsScreen : GenericMenu  {
		public SettingsScreen() : base("Settings") {

			MenuEntry e1 = new HeadingEntry("Player");
			TextInputEntry e2 = new TextInputEntry("Name", Arena.Config.PlayerName);
			e2.TextChanged += delegate(object sender, TextChangeArgs e) {
				Arena.Config.PlayerName = e.Text;
			};
			NumberInputEntry e3 = new NumberInputEntry("Number", Arena.Config.PlayerNumber);
			e3.TextChanged += delegate(object sender, TextChangeArgs e) {
				Arena.Config.PlayerNumber = int.Parse(e.Text);
			};
			MenuEntry e4 = new MenuEntry("Back");
			e4.IsCancel = true;
			Entries.Add(e1);
			Entries.Add(e2);
			Entries.Add(e3);
			Entries.Add(e4);
		}
		protected override void OnCancel() {
			ExitScreen();
			base.OnCancel();
		}
	}
}

