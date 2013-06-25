using System;
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
		private bool justActivated = false;
		public bool Ready = true;
		protected Ability(string name, AbilityActivationType activationType, int levels) {
			Name = name;
			ActivationType = activationType;
			Levels = levels;
			Level = 0;
		}
		public void Activate() {
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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
	public class Tackle : Ability {
		public Tackle() : base("Tackle", AbilityActivationType.NoTarget, 4) {
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
		protected override void OnActivate() {

		}
		protected override void OnUpdate(GameTime gameTime) {

		}
		protected override void OnDraw(GameTime gameTime, Context g) {

		}
	}
}