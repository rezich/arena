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
			Role.Initialize();
			Config.Initialize();
			Config.Read();
			ChangeResolution(Config.Resolution, Config.Fullscreen, Config.Borderless);
			HUD.Recalculate(Renderer);
			CursorVisible = false;

			StateManager.AddState(new TitleScreen());
		}
	}
}

