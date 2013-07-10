using System;
using VGame;

namespace Arena {
	public class Commands {
		public static void Load() {
			Add("move_to", typeof(MoveOrder));
			Add("+scoreboard", typeof(OnScoreboard));
			Add("-scoreboard", typeof(OffScoreboard));
		}
		public static void Add(string name, Type type) {
			CommandDefinition.Add(name, type);
		}

		public class MoveOrder : CommandDefinition {
			public MoveOrder() : base(ParameterType.Float, ParameterType.Float) { }
			public override void Run(CommandManager cmdMan, Command cmd) {
				Console.WriteLine("Moving to {0},{1}", cmd.Parameters[0].FloatData, cmd.Parameters[1].FloatData);
			}
		}
		public class OnScoreboard : CommandDefinition {
			public OnScoreboard() : base() { }
			public override void Run(CommandManager cmdMan, Command cmd) {
				Client.Local.IsShowingScoreboard = true;
			}
		}
		public class OffScoreboard : CommandDefinition {
			public OffScoreboard() : base() { }
			public override void Run(CommandManager cmdMan, Command cmd) {
				Client.Local.IsShowingScoreboard = false;
			}
		}
	}
}