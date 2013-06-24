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
		public readonly AbilityActivationType ActivationType;
		public readonly int Levels;
		public abstract int EnergyCost { get; }
		public abstract double Cooldown { get; }
		public int Level;
		protected Ability(string name, AbilityActivationType activationType, int levels) {
			Name = name;
			ActivationType = activationType;
			Levels = levels;
			Level = 0;
		}
	}
}

namespace Arena.Abilities {
	public class Placeholder : Ability {
		public Placeholder() : base("PLACEHOLDER", AbilityActivationType.Passive, 4) {
		}
		public override int EnergyCost {
			get {
				return 0;
			}
		}
		public override double Cooldown {
			get {
				return 0;
			}
		}
	}
	public class Sprint : Ability {
		public Sprint() : base("Sprint", AbilityActivationType.NoTarget, 4) {
		}
		public override int EnergyCost {
			get {
				return 5;
			}
		}
		public override double Cooldown {
			get {
				return 10;
			}
		}
	}

	// GRAPPLER
	public class Grab : Ability {
		public Grab() : base("Grab", AbilityActivationType.TargetDirection, 4) {
		}
		public override int EnergyCost {
			get {
				return 10;
			}
		}
		public override double Cooldown {
			get {
				return 20;
			}
		}
	}
	public class Hookshot : Ability {
		public Hookshot() : base("Hookshot", AbilityActivationType.TargetDirection, 4) {
		}
		public override int EnergyCost {
			get {
				return 5;
			}
		}
		public override double Cooldown {
			get {
				return 5;
			}
		}
	}
	public class Dive : Ability {
		public Dive() : base("Dive", AbilityActivationType.NoTarget, 4) {
		}
		public override int EnergyCost {
			get {
				return 20;
			}
		}
		public override double Cooldown {
			get {
				return 60;
			}
		}
	}
	public class Grapple: Ability {
		public Grapple() : base("Grapple", AbilityActivationType.TargetEnemy, 3) {
		}
		public override int EnergyCost {
			get {
				return 10;
			}
		}
		public override double Cooldown {
			get {
				return 60;
			}
		}
	}
}