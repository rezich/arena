using System;
using System.Collections;
using System.Collections.Generic;
using VGame;

namespace Arena {
	public enum Roles {
		Runner,
		Grappler,
		Nuker,
		Tank
	}
	public struct Role {
		public static Dictionary<Roles, Role> List = new Dictionary<Roles, Role>();
		public static void Initialize() {
			List.Add(Roles.Runner, new Role(
				"Runner",
				15,
				15,
				0.001,
				0.0025,
				5,
				2,
				8,
				1,
				1.5,
				new List<System.Type>() {
					typeof(Arena.Abilities.Sprint),
					typeof(Arena.Abilities.Longshot),
					typeof(Arena.Abilities.AgilityAura),
					typeof(Arena.Abilities.Placeholder)
				}
			));
			List.Add(Roles.Grappler, new Role(
				"Grappler",
				30,
				15,
				0.001,
				0.0025,
				2,
				2,
				5,
				1,
				1.7,
				new List<System.Type>() {
					typeof(Arena.Abilities.Grab),
					typeof(Arena.Abilities.Hookshot),
					typeof(Arena.Abilities.Tackle),
					typeof(Arena.Abilities.Grapple)
				}
			));
			List.Add(Roles.Nuker, new Role(
				"Nuker",
				10,
				40,
				0.001,
				0.0025,
				2,
				2,
				5,
				0,
				1.7,
				new List<System.Type>() {
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder)
				}
			));
			List.Add(Roles.Tank, new Role(
				"Tank",
				40,
				10,
				0.001,
				0.0025,
				1,
				2,
				5,
				1,
				1.7,
				new List<System.Type>() {
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder)
				}
			));
		}

		public string Name;
		public int Health;
		public int Energy;
		public double HealthRegen;
		public double EnergyRegen;
		public int MoveSpeed;
		public double TurnSpeed;
		public int AttackRange;
		public int AttackDamage;
		public double BaseAttackTime;
		public List<System.Type> Abilities;

		public Role(string name, int health, int energy, double healthRegen, double energyRegen, int moveSpeed, double turnSpeed, int attackRange, int attackDamage, double baseAttackTime, List<System.Type> abilities) {
			Name = name;
			Health = health;
			Energy = energy;
			HealthRegen = healthRegen;
			EnergyRegen = energyRegen;
			MoveSpeed = moveSpeed;
			TurnSpeed = turnSpeed;
			AttackRange = attackRange;
			AttackDamage = attackDamage;
			BaseAttackTime = baseAttackTime;
			Abilities = abilities;
		}
		public static Shape MakeShape(Roles role) {
			switch (role) {
				case Roles.Runner:
					return new Shapes.Runner();
				case Roles.Grappler:
					return new Shapes.Grappler();
				case Roles.Nuker:
					return new Shapes.Nuker();
				case Roles.Tank:
					return new Shapes.Tank();
			}
			return null;
		}
		public static void SetUpUnit(ref Unit unit, Roles role) {
			unit.MoveSpeed = Arena.Role.List[role].MoveSpeed;
			unit.TurnSpeed = Arena.Role.List[role].TurnSpeed;
			unit.AttackRange = Arena.Role.List[role].AttackRange;
			unit.AttackDamage = Arena.Role.List[role].AttackDamage;
			unit.BaseAttackTime = Arena.Role.List[role].BaseAttackTime;
			unit.HealthRegen = Arena.Role.List[role].HealthRegen;
			unit.EnergyRegen = Arena.Role.List[role].EnergyRegen;
			foreach (System.Type t in Arena.Role.List[role].Abilities) {
				unit.Abilities.Add((Ability)Activator.CreateInstance(t, unit));
			}
		}
	}
}