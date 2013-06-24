using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Arena {
	public enum Teams {
		Home,
		Away
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
			if (Vector2.Distance(Position, IntendedPosition) > 4) {
				Vector2 velocity = Vector2.Normalize(IntendedPosition - Position);
				if (Math.Abs(MathHelper.WrapAngle((float)(Direction - IntendedDirection))) < MathHelper.Pi / 8) MoveInDirection(velocity, MoveSpeed);
				TurnTowardsIntended();
			}
		}
		public void MoveInDirection(Vector2 direction, double speed) {
			Position += direction * (float)speed;
		}
		public void TurnTowardsIntended() {
			IntendedDirection = Math.Atan2(IntendedPosition.Y - Position.Y, IntendedPosition.X - Position.X);
		}
	}
	/*class Bot : Player {
	}*/
}

