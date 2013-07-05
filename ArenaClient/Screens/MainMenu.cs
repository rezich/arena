using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class MainScreen : GenericMenu {
		public MainScreen() : base("", false) {
			ShowCancel = false;
			MenuEntry e1 = new MenuEntry("Find lobby");
			MenuEntry e2 = new MenuEntry("Settings");
			MenuEntry e3 = new MenuEntry("Quit");
		}

	}
}

