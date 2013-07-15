using System;
using Arena;
using VGame;

namespace ArenaClient {
	public class Commands {
		public static void Load() {
			Add("pausemenu", typeof(PauseMenu));
			Add("ability_execute", typeof(AbilityExecute));
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
		public class AbilityExecute : CommandDefinition {
			public AbilityExecute() : base(ParameterType.Int) { }
			public override void Run(CommandManager cmdMan, Command cmd) {
				if (!(cmdMan.Game.StateManager.LastActiveState is MatchScreen))
					return;
				cmdMan.Console.WriteLine(string.Format("Pew pew, you fired ability #{0}", cmd.Parameters[0].IntData));
				Client.Local.BeginUsingAbility(cmdMan.Game.GetGameTime(), cmd.Parameters[0].IntData);
			}
		}
	}
}