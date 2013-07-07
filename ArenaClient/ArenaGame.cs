using System;
using System.Collections;
using System.Collections.Generic;
using Arena;
using VGame;

namespace ArenaClient {
	public class ArenaGame : Game {
		protected override void Initialize() {
			CursorVisible = false;
			Role.Initialize();
			Arena.Config.Initialize();
			HUD.Recalculate(Renderer);

			StateManager.AddState(new TitleScreen());
		}
	}
}

