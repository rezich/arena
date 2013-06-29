using System;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Arena;
using VGame;
using Cairo;


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
		
		static void Main() {

			Console.Write("Initializing server...");

			Role.Initialize();
			Server server = new Server(false);
			Stopwatch.Start();

			Console.WriteLine("done.");

			MainLoop();
		}

		static void Update(GameTime gameTime) {
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
