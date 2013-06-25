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
			if (!System.IO.File.Exists(@"C:\Windows\Fonts\04B_19_.TTF"))
				if (System.IO.File.Exists(@"04B_19_.TTF"))
					System.IO.File.Copy("04B_19_.TTF", @"C:\Windows\Fonts\04B_19_.TTF");
			game = new GameSession();
			try {
				game.Run();
			}
			catch(Exception e) {
				Console.Write(e.Message);
				Console.ReadKey();
			}
			finally {
				VGame.Renderer.Dispose();
			}
		}
	}
}
