using System;
using System.Linq;
using VGame;

namespace ArenaClient {
	public class ConnectMenu : GenericMenu {
		string address = Arena.Config.LastServerAddress;
		MenuEntry connectEntry;
		public ConnectMenu() : base("CONNECT TO SERVER") {

			Entries.Add(new AddressInputEntry("ADDRESS", Arena.Config.LastServerAddress));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				address = e.Text;
				connectEntry.Enabled = e.Text != "";
			};
			connectEntry = new MenuEntry("CONNECT");
			connectEntry.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				Arena.Config.LastServerAddress = address;
				ScreenManager.AddScreen(new ConnectionScreen(address), null);
			};
			Entries.Add(connectEntry);

			Entries.Add(new CancelEntry("BACK"));

		}
	}
}

