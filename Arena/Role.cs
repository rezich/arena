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
				3,
				5,
				new List<System.Type>() {
					typeof(Arena.Abilities.Sprint),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder)
				}
			));
			List.Add(Roles.Grappler, new Role(
				"Grappler",
				30,
				15,
				2,
				5,
				new List<System.Type>() {
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder)
				}
			));
			List.Add(Roles.Nuker, new Role(
				"Nuker",
				10,
				40,
				2,
				5,
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
				1,
				5,
				new List<System.Type>() {
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder),
					typeof(Arena.Abilities.Placeholder)
				}
			));
		}

		public string Name;
		public int BaseHealth;
		public int BaseEnergy;
		public int MoveSpeed;
		public int AttackRange;
		public List<System.Type> Abilities;

		public Role(string name, int baseHealth, int baseEnergy, int moveSpeed, int attackRange, List<System.Type> abilities) {
			Name = name;
			BaseHealth = baseHealth;
			BaseEnergy = baseEnergy;
			MoveSpeed = moveSpeed;
			AttackRange = attackRange;
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
	}
}