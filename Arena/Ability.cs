using System;
using System.Collections;
using System.Collections.Generic;
using Cairo;
using VGame;

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
		public readonly string Description;
		public readonly AbilityActivationType ActivationType;
		public readonly int Levels;
		public abstract int EnergyCost { get; }
		public abstract double Cooldown { get; }
		public int Level;
		public TimeSpan ReadyTime;
		public Unit Unit;
		private bool justActivated = false;
		public bool Ready = true;
		protected Ability(Unit unit, string name, string description, AbilityActivationType activationType, int levels) {
			Unit = unit;
			Name = name;
			Description = description;
			ActivationType = activationType;
			Levels = levels;
			Level = 0;
		}
		public void Activate(float? val1, float? val2) {
			if (Level < 1)
				return;
			justActivated = true;
			OnActivate();
		}
		protected abstract void OnActivate();
		public void Update(GameTime gameTime) {
			if (Level < 1)
				return;
			if (gameTime.TotalGameTime >= ReadyTime) {
				Ready = true;
			}
			if (justActivated) {
				justActivated = false;
				ReadyTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(Cooldown);
				Ready = false;
			}
			OnUpdate(gameTime);
		}
		protected abstract void OnUpdate(GameTime gameTime);
		public void Draw(GameTime gameTime, Cairo.Context g) {
			if (Level < 1)
				return;
			OnDraw(gameTime, g);
		}
		protected abstract void OnDraw(GameTime gameTime, Cairo.Context g);
	}
}

namespace Arena.Abilities {
	public class Placeholder : Ability {
		public Placeholder(Unit unit) : base(unit, "PLACEHOLDER", "Just a placeholder ability used until we balance the game out.", AbilityActivationType.Passive, 4) {
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
		protected override void OnActivate() {
		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}

	// RUNNER
	public class Sprint : Ability {
		public Sprint(Unit unit) : base(unit, "Sprint", "Temporarily gain a move speed bonus.", AbilityActivationType.NoTarget, 4) {
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
		protected override void OnActivate() {
			Unit.AddBuff(new Buff("Sprint", BuffAlignment.Positive, BuffType.MoveSpeed, 5, TimeSpan.FromSeconds(5), false));
			//Unit.Buffs.Add(new Buff("Sprint", BuffAlignment.Positive, BuffType.MoveSpeed, 5, gameTime.TotalGameTime + TimeSpan.FromSeconds(5), false));
		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class AgilityAura: Ability {
		Buff aura;
		public AgilityAura(Unit unit) : base(unit, "Agility Aura", "Passively gain move speed, turn speed, and attack speed.", AbilityActivationType.Passive, 4) {
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
		protected override void OnActivate() {
			if (Unit.Buffs.Contains(aura) && aura != null)
				Unit.Buffs.Remove(aura);
			aura = new Buff("Agility Aura", BuffAlignment.Positive, new List<Tuple<BuffType, double>>() {
				new Tuple<BuffType, double>(BuffType.MoveSpeed, 2 * Level),
				new Tuple<BuffType, double>(BuffType.TurnSpeed, 2 * Level),
				new Tuple<BuffType, double>(BuffType.AttackSpeed, 0.5 * Level)
			}, null, false);
			Unit.Buffs.Add(aura);
		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {
		}
	}
	public class Longshot : Ability {
		Buff aura;
		public Longshot(Unit unit) : base(unit, "Longshot", "Passively gain attack range. This is just an example spell.", Arena.AbilityActivationType.Passive, 4) {
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
		protected override void OnActivate() {
			if (Unit.Buffs.Contains(aura) && aura != null)
				Unit.Buffs.Remove(aura);
			aura = new Buff("Longshot", BuffAlignment.Positive, BuffType.AttackRange, 2 * Level, null, false);
			Unit.Buffs.Add(aura);
		}
		protected override void OnUpdate(GameTime gameTime) {
		}
		protected override void OnDraw(GameTime gameTime, Context g) {
		}
	}

	// GRAPPLER
	public class Grab : Ability {
		public Grab(Unit unit) : base(unit, "Grab", "Reach out with a hook to grab an enemy and pull him in.", AbilityActivationType.TargetDirection, 4) {
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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Hookshot : Ability {
		public Hookshot(Unit unit) : base(unit, "Hookshot", "Fire a hook that grabs onto pillars and pulls you to them, dealing damage to enemies along the way.", AbilityActivationType.TargetDirection, 4) {
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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Tackle : Ability {
		public Tackle(Unit unit) : base(unit, "Tackle", "Fire a hook in the direction you're facing, and, if it hits an enemy, pulls you to them.", AbilityActivationType.NoTarget, 4) {
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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Grapple: Ability {
		public Grapple(Unit unit) : base(unit, "Grapple", "Disable target enemy unit and yourself.", AbilityActivationType.TargetEnemy, 3) {
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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
}