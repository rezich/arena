using System;
using System.Collections.Generic;
using Arena;
using VGame;

namespace ArenaClient {
	public class Commands {
		public static void Load() {
			Add("pausemenu", new CommandDefinition(typeof(MatchScreen), delegate(CommandManager cmdMan, Command cmd) {
				cmdMan.Game.StateManager.AddState(new ArenaClient.PauseMenu());
			}));
			Add("ability_execute", new CommandDefinition(new List<ParameterType>() { ParameterType.Int }, typeof(MatchScreen), delegate(CommandManager cmdMan, Command cmd) {
				cmdMan.Console.WriteLine(string.Format("Pew pew, you fired ability #{0}", cmd.Parameters[0].IntData));
				Client.Local.BeginUsingAbility(cmdMan.Game.GetGameTime(), cmd.Parameters[0].IntData);
			}));
		}
		public static void Add(string name, CommandDefinition def) {
			CommandDefinition.Add(name, def);
		}
	}
}