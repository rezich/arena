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

		static GameTime GameTime = new GameTime();
		static bool stop = false;
		static Stopwatch stopwatch = new Stopwatch();
		static TimeSpan ElapsedTime = new TimeSpan();
		static TimeSpan TargetElapsedTime;
		
		static void Main() {

			Console.Write("Initializing server...");

			Role.Initialize();
			Server server = new Server(false);
			TargetElapsedTime = TimeSpan.FromSeconds((double)1 / (double)60);
			stopwatch.Start();

			Console.WriteLine("done.");

			MainLoop();
		}

		static void Update(GameTime gameTime) {
			Console.WriteLine("Hey!");
		}

		static void MainLoop() {
			while (!stop) {
				GameTime.TotalGameTime += stopwatch.Elapsed;
				ElapsedTime += stopwatch.Elapsed;
				stopwatch.Restart();
				if (ElapsedTime >= TargetElapsedTime) {
					GameTime.ElapsedGameTime = ElapsedTime;
					Update(GameTime);
					ElapsedTime = new TimeSpan();
				}
			}
		}
	}
}
