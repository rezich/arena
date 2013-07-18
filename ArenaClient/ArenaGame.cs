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
			Arena.Commands.Load();
			ArenaClient.Commands.Load();
			Role.Initialize();
			Config.Initialize();
			Config.Read();
			ChangeResolution(Config.Resolution, Config.Fullscreen, Config.Borderless);
			HUD.Recalculate(Renderer);
			CursorVisible = false;

			Binding.Bind(new KeyCombination(Keys.Q, false, false, false), "ability_execute 0");
			Binding.Bind(new KeyCombination(Keys.W, false, false, false), "ability_execute 1");
            Binding.Bind(new KeyCombination(Keys.E, false, false, false), "ability_execute 2");
            Binding.Bind(new KeyCombination(Keys.R, false, false, false), "ability_execute 3");

			StateManager.AddState(new TitleScreen());
		}
		public override bool IsClient() {
			return Client.Local != null;
		}
		public override bool IsServer() {
			return Server.Local != null;
		}
	}
}

