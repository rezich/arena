#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion
namespace ArenaClient {
	static class Program {

		private static GameSession game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		//[STAThread]
        static void Main() {
			game = new GameSession();
			//try {
				game.Run();
			/*}
			catch(Exception e) {
				Console.Write(e.Message);
				System.Threading.Thread.Sleep(2000);
				Console.ReadKey();
			}
			finally {*/
				VGame.Renderer.Dispose();
			//}
		}
	}
}
