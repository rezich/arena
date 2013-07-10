using System;
using Arena;
using VGame;

namespace ArenaClient {
	public class Commands {
		public static void Load() {
			Add("pausemenu", typeof(PauseMenu));
		}
		public static void Add(string name, Type type) {
			CommandDefinition.Add(name, type);
		}

		public class PauseMenu : CommandDefinition {
			public PauseMenu() : base() { }
			public override void Run(CommandManager cmdMan, Command cmd) {
				cmdMan.Game.StateManager.AddState(new ArenaClient.PauseMenu());
			}
		}
	}
}