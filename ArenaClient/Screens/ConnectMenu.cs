using System;
using VGame;

namespace ArenaClient {
	public class ConnectMenu : GenericMenu {
		public ConnectMenu() : base("Connect to a server") {
			AddressInputEntry e1 = new AddressInputEntry("Address", Arena.Config.LastServerAddress);
			e1.TextChanged += delegate(object sender, TextChangeArgs e) {
				Arena.Config.LastServerAddress = e.Text;
			};
			MenuEntry e2 = new MenuEntry("Connect");
			e2.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				ScreenManager.AddScreen(new ConnectionScreen(Arena.Config.LastServerAddress), null);
			};
			CancelEntry e3 = new CancelEntry("Back");

			Entries.Add(e1);
			Entries.Add(e2);
			Entries.Add(e3);
		}
	}
}

