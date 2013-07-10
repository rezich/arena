#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using VGame;
using Arena;

#endregion
namespace ArenaClient {
	static class Program {

		private static ArenaGame game;
        static void Main() {
			game = new ArenaGame();
			game.Run();
			if (Arena.Client.Local != null)
				Arena.Client.Local.Disconnect();
		}
	}
}
