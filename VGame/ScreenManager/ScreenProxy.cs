using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VGame {
	class ScreenProxy : GameScreen {
		public GameScreen Now;
		public GameScreen After;
		bool updated = false;

		public ScreenProxy(GameScreen now, GameScreen after) {
			Now = now;
			After = after;
		}

		public override void Initialize() {
			ScreenManager.AddScreen(Now, ControllingPlayer);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			//if (!TopActive) return;
			if (ScreenManager.Last() != this) return;
			if (!updated) {
				updated = true;
				return;
			}
			ScreenManager.ReplaceScreen(After, ControllingPlayer);
		}
	}
}
