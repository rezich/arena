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
			string readLine;
			Console.Write("Enter name: ");
			readLine = Console.ReadLine();
			Arena.Config.PlayerName = (readLine == "" ? "UNNAMED" : readLine);
			bool validNumber = false;
			while (!validNumber) {
				Console.Write("Enter number: ");
				readLine = Console.ReadLine();
				if (readLine == "") {
					Arena.Config.PlayerNumber = 0;
					validNumber = true;
				}
				else
					validNumber = int.TryParse(readLine, out Arena.Config.PlayerNumber);
			}
			Console.Write("Server IP: ");
			readLine = Console.ReadLine();
			Arena.Config.ServerAddress = (readLine == "" ? "localhost" : readLine);
			game = new GameSession();
			try {
				game.Run();
			}
			catch(Exception e) {
				Console.Write(e.Message);
				System.IO.File.WriteAllText("log.txt", e.StackTrace);
				System.Threading.Thread.Sleep(2000);
				Console.ReadKey();
			}
			finally {
				VGame.Renderer.Dispose();
			}
		}
	}
}
