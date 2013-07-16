using System;
using System.Collections.Generic;
using VGame;

namespace Arena {
	public class Commands {
		public static void Load() {
			Add("move_to", new CommandDefinition(new List<ParameterType>() { ParameterType.Float, ParameterType.Float }, delegate(CommandManager cmdMan, Command cmd) {
				Console.WriteLine("Moving to {0},{1}", cmd.Parameters[0].FloatData, cmd.Parameters[1].FloatData);
			}));
			Add("+scoreboard", new CommandDefinition(delegate(CommandManager cmdMan, Command cmd) {
				Client.Local.IsShowingScoreboard = true;
			}));
			Add("-scoreboard", new CommandDefinition(delegate(CommandManager cmdMan, Command cmd) {
				Client.Local.IsShowingScoreboard = false;
			}));
		}
		public static void Add(string name, CommandDefinition def) {
			CommandDefinition.Add(name, def);
		}
	}
}