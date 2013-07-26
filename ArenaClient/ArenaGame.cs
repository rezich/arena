using System;
using System.Collections;
using System.Collections.Generic;
using Arena;
using VGame;

namespace ArenaClient {
	public class ArenaGame : Game {
		public ArenaGame() : base(false) {
		}
		protected override void Initialize() {
			BaseSize = 7;
			Arena.Commands.Load();
			ArenaClient.Commands.Load();
			Role.Initialize();
			Config.Initialize();
			Config.Read();
			ChangeResolution(Config.Resolution, Config.Fullscreen, Config.Borderless, Config.DoubleBuffered);
			Renderer.Antialiasing = Config.Antialiasing;
			HUD.Recalculate(Renderer);
			CursorVisible = false;

			Binding.Bind(new KeyCombination(Keys.Q, false, false, false), "ability_execute 0");
			Binding.Bind(new KeyCombination(Keys.W, false, false, false), "ability_execute 1");
            Binding.Bind(new KeyCombination(Keys.E, false, false, false), "ability_execute 2");
            Binding.Bind(new KeyCombination(Keys.R, false, false, false), "ability_execute 3");

			StateManager.AddState(new TitleScreen());
		}
		protected override void LoadFonts() {
			Renderer.LoadFont("console", "ProFontWindows.ttf", true);
			Renderer.LoadFont("chunky", "04B_19.ttf");
			Renderer.LoadFont("chunky_aa", "04B_19.ttf", true);
			Renderer.LoadFont("pixel", "04B_25__.ttf");
			Renderer.LoadFont("wide", "04B_20_.ttf");
		}
		public override bool IsClient() {
			return Client.Local != null;
		}
		public override bool IsServer() {
			return Server.Local != null;
		}
	}
}

