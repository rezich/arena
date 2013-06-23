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
		public double HealthRegen = 0.001;
		private double healthRegenPart = 0;
		public double EnergyRegen = 0.0025;
		private double energyRegenPart = 0;
		public int Number;
		public Teams Team;

		public Vector2 Position;
		public double Speed;
		public double Direction = 0;

		public Player(string name, int number, Teams team, Roles role) {
			Name = name;
			Number = number;
			Team = team;
			Role = role;

			MaxHealth = Arena.Role.List[Role].BaseHealth;
			MaxEnergy = Arena.Role.List[Role].BaseEnergy;
			Health = MaxHealth;
			Energy = MaxEnergy;
		}
		public void MakeActor() {
			Actor a = new Actor();
			a.Shape = Arena.Role.MakeShape(Role);
			a.Position = Position;
			a.Direction = Direction;
			a.Player = this;
			Actor.List.Add(a);
		}
		public void Update(GameTime gameTime) {
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
	}
}

