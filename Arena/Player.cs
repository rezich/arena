using System;
using Microsoft.Xna.Framework;

namespace Arena {
	public class Player {
		public string Name;
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

		public Vector2 Position;
		public double Speed;

		public Player() {
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

