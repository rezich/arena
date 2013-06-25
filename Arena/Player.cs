using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Arena {
	public enum Teams {
		Home,
		Away
	}
	public enum Attitude {
		Neutral,
		Friend,
		Enemy
	}
	public class Player {
		public static List<Player> List = new List<Player>();

		public string Name;
		public Roles Role;
		public Actor Actor;
		public int Health;
		public int MaxHealth;
		public int Energy;
		public int MaxEnergy;
		public int Level = 0;
		public int Experience = 0;
		public int TurnSpeed = 2;
		public double HealthRegen = 0.001;
		private double healthRegenPart = 0;
		public double EnergyRegen = 0.0025;
		private double energyRegenPart = 0;
		public int Number;
		public Teams Team;

		public Vector2 Position;
		public Vector2 LastPosition;
		public Vector2 IntendedPosition;
		public double MoveSpeed;
		public double Direction = 0;
		public double IntendedDirection = 0;
		public bool IsBot = false;
		public List<Ability> Abilities = new List<Ability>();
		public Actor AttackTarget;

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
				return 0.5;
			}
		}
		public int ExperienceLeftOver {
			get {
				return Experience % 100;
			}
		}

		public Player(string name, int number, Teams team, Roles role) {
			Name = name;
			Number = number;
			Team = team;
			Role = role;

			MaxHealth = Arena.Role.List[Role].BaseHealth;
			MaxEnergy = Arena.Role.List[Role].BaseEnergy;
			MoveSpeed = Arena.Role.List[Role].MoveSpeed;
			Health = MaxHealth;
			Energy = MaxEnergy;

			foreach (System.Type t in Arena.Role.List[Role].Abilities) {
				Ability o = (Ability)Activator.CreateInstance(t);
				Abilities.Add(o);
			}

			Player.List.Add(this);
		}
		public void MakeActor() {
			Actor a = new Actor();
			a.Shape = Arena.Role.MakeShape(Role);
			a.Player = this;
			Actor.List.Add(a);
			Actor = a;
		}
		public void Update(GameTime gameTime) {
			Regen();
			foreach (Ability a in Abilities)
				a.Update(gameTime);
			MoveTowardsIntended();
			if (Direction != IntendedDirection)
				Direction = Direction.LerpAngle(IntendedDirection, (double)TurnSpeed / 10 / MathHelper.PiOver2);
			LastPosition = Position;
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
			Vector2 intendedPosition;
			if (AttackTarget != null) {
				intendedPosition = AttackTarget.Player.Position;
				TurnTowards(intendedPosition);
				if (Vector2.Distance(Position, intendedPosition) <= Arena.Role.List[Role].AttackRange * Arena.GameSession.ActorScale) {
					if (Math.Abs(MathHelper.WrapAngle((float)(Direction - IntendedDirection))) < MathHelper.Pi / 8) {
						// AUTOATTACK
						return;
					}
					return;
				}
			}
			else
				intendedPosition = IntendedPosition;
			if (Vector2.Distance(Position, intendedPosition) > Arena.GameSession.ActorScale * 0.25) {
				Vector2 velocity = Vector2.Normalize(intendedPosition - Position);
				if (Math.Abs(MathHelper.WrapAngle((float)(Direction - IntendedDirection))) < MathHelper.Pi / 8) {
					bool foundActor = false;
					foreach (Actor a in Actor.List) {
						if (a == Actor)
							continue;
						if (Vector2.Distance(Actor.Position + velocity, a.Position) < Arena.GameSession.ActorScale * 2) {
							foundActor = true;
							break;
						}
					}
					if (!foundActor) MoveInDirection(velocity, MoveSpeed);
				}
				TurnTowards(intendedPosition);
			}
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
		public Attitude AttitudeTowards(Player player) {
			if (player.Team != Team)
				return Attitude.Enemy;
			return Attitude.Friend;
		}
	}
	class Bot : Player {
		public Bot(Teams team, Roles role) : base("--BOT--", 0, team, role) {
			Number = Arena.GameSession.Random.Next(1, 49);
		}
	}
}

