using System;
using VGame;

namespace Arena {
	public class Commands {
		public static void Load() {
			CommandDefinition.Add("move_to", typeof(MoveOrder));
		}

		public class MoveOrder : CommandDefinition {
			public MoveOrder() : base(ParameterType.Float, ParameterType.Float) { }
			public override void Run(CommandManager commandManager, Command cmd) {
				Console.WriteLine("Moving to {0},{1}", cmd.Parameters[0].FloatData, cmd.Parameters[1].FloatData);
			}
		}
	}
}