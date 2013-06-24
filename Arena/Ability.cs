using System;

namespace Arena {
	public enum AbilityActivationType {
		Passive,
		NoTarget,
		Toggle,
		TargetPoint,
		TargetDirection,
		TargetEnemy,
		TargetFriendly,
		TargetCreep,
		TargetUnit,
		TargetBuilding,
		TargetPillar
	}
	public abstract class Ability {
		public readonly string Name;
		protected Ability(string name) {
			Name = name;
		}
	}
}

namespace Arena.Abilities {
	public class Placeholder : Ability {
		public Placeholder() : base("PLACEHOLDER") {
		}
	}
	public class Sprint : Ability {
		public Sprint() : base("Sprint") {
		}
	}
}