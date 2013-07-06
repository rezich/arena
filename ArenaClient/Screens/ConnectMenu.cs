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

			Entries.Add(new NumberInputEntry("PORT", Arena.Config.Port));
			Entries.Last().Enabled = false;

			Entries.Add(new SpacerEntry());

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

