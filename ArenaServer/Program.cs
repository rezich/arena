using System;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Arena;


namespace ArenaServer {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		
		static bool Stop = false;
		static Stopwatch Stopwatch = new Stopwatch();
		static long TicksSinceLastUpdate = 0;
		static long TotalTicks = 0;
		static readonly double Interval = (double)Stopwatch.Frequency / 60;
		static TimeSpan LastUpdate = new TimeSpan();
		static Server Server;
		
		static void Main() {

			Console.Write("Initializing server... ");

			Role.Initialize();
			Arena.Config.Initialize();
			Server = new Server(false);
			Stopwatch.Start();
			TicksSinceLastUpdate = Stopwatch.GetTimestamp();

			Console.WriteLine("done.");
			//try {
				MainLoop();
			//}
			//catch(Exception e) {
			//	System.IO.File.WriteAllText("log.txt", e.StackTrace);
			//}
		}

		static void Update(GameTime gameTime) {
			Server.Tick();
			Server.Update(gameTime);
		}
		
		static void MainLoop() {
			while (!Stop) {
				TotalTicks = Stopwatch.GetTimestamp();
				if (TotalTicks >= TicksSinceLastUpdate + Interval) {
					Update(new GameTime(Stopwatch.Elapsed, Stopwatch.Elapsed - LastUpdate));
					TicksSinceLastUpdate = Stopwatch.GetTimestamp();
					LastUpdate = Stopwatch.Elapsed;
				}
			}
		}
	}
}
