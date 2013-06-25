using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Arena {
	public class Player : UnitController {
		public static List<Player> List = new List<Player>();
		public string Name;
		public int Number;
		public Roles Role;
		public Unit PlayerUnit;

		public Player(string name, int number, Teams team, Roles role) {
			Name = name;
			Number = number;
			Team = team;
			Role = role;

			Player.List.Add(this);
		}
		public Unit MakePlayerUnit(Vector2 position) {
			Unit u = new Unit(Arena.Role.List[Role].BaseHealth, Arena.Role.List[Role].BaseEnergy, Arena.Role.MakeShape(Role));
			u.MoveSpeed = Arena.Role.List[Role].MoveSpeed;
			u.TurnSpeed = Arena.Role.List[Role].TurnSpeed;
			u.AttackRange = Arena.Role.List[Role].AttackRange;
			u.BaseAttackTime = Arena.Role.List[Role].BaseAttackTime;

			u.JumpTo(position);

			foreach (System.Type t in Arena.Role.List[Role].Abilities) {
				u.Abilities.Add((Ability)Activator.CreateInstance(t));
			}
			PlayerUnit = u;
			ControlledUnits.Add(PlayerUnit);
			return PlayerUnit;
		}
	}
	class Bot : Player {
		public Bot(Teams team, Roles role) : base("--BOT--", 0, team, role) {
			Number = Arena.GameSession.Random.Next(1, 49);
		}
	}
}

