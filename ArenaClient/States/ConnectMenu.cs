using System;
using System.Linq;
using VGame;

namespace ArenaClient {
	public class ConnectMenu : Menu {
		string address = Arena.Config.LastServerAddress;
		MenuEntry connectEntry;
		public ConnectMenu() : base("CONNECT TO SERVER") {

			Entries.Add(new AddressInputEntry(this, "ADDRESS", Arena.Config.LastServerAddress));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				address = e.Text;
				connectEntry.Enabled = e.Text != "";
			};

			Entries.Add(new NumberInputEntry(this, "PORT", Arena.Config.Port));
			Entries.Last().Enabled = false;

			Entries.Add(new SpacerEntry(this));

			connectEntry = new MenuEntry(this, "CONNECT");
			connectEntry.Selected += delegate(object sender, EventArgs e) {
				Arena.Config.LastServerAddress = address;
				StateManager.AddState(new ConnectionScreen(address));
			};
			Entries.Add(connectEntry);

			Entries.Add(new CancelEntry(this, "BACK"));

		}
	}
}

