#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion
namespace Arena {
	static class Program {
		private static GameSession game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
        static void Main() {
			game = new GameSession();
			game.Run();
		}
	}
}
