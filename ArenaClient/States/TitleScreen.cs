using System;
using Cairo;
using VGame;

namespace ArenaClient {
	public class TitleScreen : State {
		public override void Initialize() {
			StateManager.AddState(new MainMenu());
		}
		public override void Draw(GameTime gameTime) {
			Renderer.Clear(new Color(0.83, 0.83, 0.83));
		}
	}
}

