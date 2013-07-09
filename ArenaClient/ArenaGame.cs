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
			Role.Initialize();
			Config.Initialize();
			Config.Read();
			Renderer = new Renderer(this, Config.Resolution.Width, Config.Resolution.Height, Config.Fullscreen, Config.Borderless);
			HUD.Recalculate(Renderer);
			CursorVisible = false;

			StateManager.AddState(new TitleScreen());
		}
	}
}

