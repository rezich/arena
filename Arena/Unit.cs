using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arena {
	public enum Teams {
		Home,
		Away,
		Neutral
	}
	public enum Attitude {
		Neutral,
		Friend,
		Enemy
	}
	public class Unit {
		public static List<Unit> List = new List<Unit>();
		public UnitController Owner;
		public Actor Actor;
		public Teams Team;


		public double MoveSpeed;
		public double TurnSpeed;
		public int MaxHealth;
		public int MaxEnergy;
		public double HealthRegen;
		public double EnergyRegen;
		public double BaseAttackTime;
		public double AttackSpeed;
		public int AttackRange;

		public int Health;
		public int Energy;
		public int Level = 0;
		public int Experience = 0;

		private double healthRegenPart = 0;
		private double energyRegenPart = 0;

		public Vector2 Position;
		public Vector2 LastPosition;
		public Vector2 IntendedPosition;
		public double Direction = 0;
		public double IntendedDirection = 0;
		public List<Ability> Abilities = new List<Ability>();
		public Actor AttackTarget;
		public TimeSpan NextAutoAttackReady = new TimeSpan();
		public bool AutoAttacking = false;


		public double HealthPercent {
			get {
				return (double)Health / (double)MaxHealth;
			}
		}
		public double EnergyPercent {
			get {
				return (double)Energy / (double)MaxEnergy;
			}
		}
		public double ExperiencePercent {
			get {
				return (double)(Experience % 100) / 100;
			}
		}
		public int ExperienceLeftOver {
			get {
				return Experience % 100;
			}
		}

		public Unit(int maxHealth, int maxEnergy, VGame.IShape shape) {
			MaxHealth = maxHealth;
			MaxEnergy = maxEnergy;
			Health = MaxHealth;
			Energy = MaxEnergy;
			Actor = new Actor(this, shape);
			Unit.List.Add(this);
		}

		public void Update(GameTime gameTime) {
			Regen();
			foreach (Ability a in Abilities)
				a.Update(gameTime);
			MoveTowardsIntended();
			if (Direction != IntendedDirection)
				Direction = Direction.LerpAngle(IntendedDirection, (double)TurnSpeed / 10 / MathHelper.PiOver2);
			LastPosition = Position;
			if (AutoAttacking) {
				if (gameTime.TotalGameTime > NextAutoAttackReady) {
					NextAutoAttackReady = gameTime.TotalGameTime + TimeSpan.FromSeconds(BaseAttackTime);
					AttackTarget.Unit.Health = Math.Max(AttackTarget.Unit.Health - 1, 0);
				}
			}
		}
		public void Regen() {
			if (Health < MaxHealth)
				healthRegenPart += HealthRegen;
			else
				healthRegenPart = 0;
			if (healthRegenPart >= 1) {
				healthRegenPart = 0;
				Health = Math.Min(Health + 1, MaxHealth);
			}
			if (Energy < MaxEnergy)
				energyRegenPart += EnergyRegen;
			else
				energyRegenPart = 0;
			if (energyRegenPart >= 1) {
				energyRegenPart = 0;
				Energy = Math.Min(Energy + 1, MaxEnergy);
			}
		}
		public void JumpTo(Vector2 position) {
			Position = position;
			IntendedPosition = position;
		}
		public void MoveTowardsIntended() {
			AutoAttacking = false;
			Vector2 intendedPosition;
			if (AttackTarget != null) {
				intendedPosition = AttackTarget.Unit.Position;
				TurnTowards(intendedPosition);
				if (Vector2.Distance(Position, intendedPosition) <= AttackRange * Arena.GameSession.ActorScale) {
					if (Math.Abs(MathHelper.WrapAngle((float)(Direction - IntendedDirection))) < MathHelper.Pi / 8) {
						// AUTOATTACK
						AutoAttacking = true;
						return;
					}
					return;
				}
			}
			else
				intendedPosition = IntendedPosition;
			if (Vector2.Distance(Position, intendedPosition) > MoveSpeed) {
				Vector2 velocity = Vector2.Normalize(intendedPosition - Position);
				if (Math.Abs(MathHelper.WrapAngle((float)(Direction - IntendedDirection))) < MathHelper.Pi / 8) {
					Actor foundActor = null;
					if (foundActor == null)
						MoveInDirection(velocity, MoveSpeed);
					else {
					}
				}
				TurnTowards(intendedPosition);
			}
			else
				Position = IntendedPosition;
		}
		public void MoveInDirection(Vector2 direction, double speed) {
			Position += direction * (float)speed;
		}
		public void TurnTowards(Vector2 position) {
			IntendedDirection = Math.Atan2(position.Y - Position.Y, position.X - Position.X);
		}
		public AbilityActivationType? UseAbility(int ability) {
			if (Abilities[ability].Level > 0 && Energy >= Abilities[ability].EnergyCost && Abilities[ability].Ready) {
				Energy -= Abilities[ability].EnergyCost;
				Abilities[ability].Activate();
				return Abilities[ability].ActivationType;
			}
			return null;
		}
		public void LevelUp(int ability) {
			Abilities[ability].Level += 1;
		}
		public Attitude AttitudeTowards(UnitController unitController) {
			if (unitController.Team != Team)
				return Attitude.Enemy;
			return Attitude.Friend;
		}
		/*public void MakeActor(VGame.Shape shape) {
			Actor a = new Actor(this, shape);
			Actor.List.Add(a);
			Actor = a;
		}*/
	}
	public abstract class UnitController {
		public Unit CurrentUnit {
			get {
				if (ControlledUnits.Count > 0)
					return ControlledUnits[currentUnit];
				else
					return null;
			}
			set {
				if (!ControlledUnits.Contains(value))
					ControlledUnits.Add(value);
				currentUnit = ControlledUnits.IndexOf(value);
			}
		}
		protected int currentUnit;
		public List<Unit> ControlledUnits = new List<Unit>();
		public Teams Team;
	}
	public class CreepController : UnitController {
		public CreepController(Teams team) {
			Team = team;
		}
		public void SpawnCreep(Vector2 position) {

		}
	}
}

