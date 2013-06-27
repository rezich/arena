using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Cairo;

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
		public TimeSpan ReadyTime;
		public Unit Unit;
		private bool justActivated = false;
		public bool Ready = true;
		protected Ability(Unit unit, string name, AbilityActivationType activationType, int levels) {
			Unit = unit;
			Name = name;
			ActivationType = activationType;
			Levels = levels;
			Level = 0;
		}
		public void Activate(GameTime gameTime) {
			if (Level < 1)
				return;
			justActivated = true;
			OnActivate(gameTime);
		}
		protected abstract void OnActivate(GameTime gameTime);
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
		public Placeholder(Unit unit) : base(unit, "PLACEHOLDER", AbilityActivationType.Passive, 4) {
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
		protected override void OnActivate(GameTime gameTime) {
		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}

	// RUNNER
	public class Sprint : Ability {
		public Sprint(Unit unit) : base(unit, "Sprint", AbilityActivationType.NoTarget, 4) {
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
		protected override void OnActivate(GameTime gameTime) {
			Unit.Buffs.Add(new Buff("Sprint", BuffAlignment.Positive, BuffType.MoveSpeed, 5, gameTime.TotalGameTime + TimeSpan.FromSeconds(5)));
		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}

	// GRAPPLER
	public class Grab : Ability {
		public Grab(Unit unit) : base(unit, "Grab", AbilityActivationType.TargetDirection, 4) {
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
		protected override void OnActivate(GameTime gameTime) {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Hookshot : Ability {
		public Hookshot(Unit unit) : base(unit, "Hookshot", AbilityActivationType.TargetDirection, 4) {
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
		protected override void OnActivate(GameTime gameTime) {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Tackle : Ability {
		public Tackle(Unit unit) : base(unit, "Tackle", AbilityActivationType.NoTarget, 4) {
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
		protected override void OnActivate(GameTime gameTime) {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Grapple: Ability {
		public Grapple(Unit unit) : base(unit, "Grapple", AbilityActivationType.TargetEnemy, 3) {
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
		protected override void OnActivate(GameTime gameTime) {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
}