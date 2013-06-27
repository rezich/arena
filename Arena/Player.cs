using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Arena {
	public class Player : UnitController {
		public override string Name {
			get {
				return _name;
			}
		}
		protected string _name;
		public int Number;
		public Roles Role;
		public Unit PlayerUnit;

		public Player(string name, int number, Teams team, Roles role) {
			_name = name;
			Number = number;
			Team = team;
			Role = role;
		}
		/*public Unit MakePlayerUnit(Vector2 position) {
			Unit u = new Unit(this, Arena.Role.List[Role].Health, Arena.Role.List[Role].Energy, Arena.Role.MakeShape(Role));
			u.Team = Team;
			u.MoveSpeed = Arena.Role.List[Role].MoveSpeed;
			u.TurnSpeed = Arena.Role.List[Role].TurnSpeed;
			u.AttackRange = Arena.Role.List[Role].AttackRange;
			u.AttackDamage = Arena.Role.List[Role].AttackDamage;
			u.BaseAttackTime = Arena.Role.List[Role].BaseAttackTime;
			u.HealthRegen = Arena.Role.List[Role].HealthRegen;
			u.EnergyRegen = Arena.Role.List[Role].EnergyRegen;

			u.JumpTo(position);

			foreach (System.Type t in Arena.Role.List[Role].Abilities) {
				u.Abilities.Add((Ability)Activator.CreateInstance(t, u));
			}
			PlayerUnit = u;
			ControlledUnits.Add(PlayerUnit);
			return PlayerUnit;
		}*/
		public override void Update(GameTime gameTime) {
		}
	}
	public class Bot : Player {
		public Bot(Teams team, Roles role) : base("--BOT--", 0, team, role) {
			Number = Arena.Config.Random.Next(1, 49);
		}
		public override void Update(GameTime gameTime) {
			foreach (KeyValuePair<int, Unit> kvp in Server.Local.Units) {
				if (CurrentUnit.AttitudeTowards(kvp.Value.Owner) == Attitude.Enemy) {
					SendAttackOrder(kvp.Value);
				}
			}
		}
		protected void SendAttackOrder(Unit u) {
			if (CurrentUnit.AttackTarget != u)
				Server.Local.RecieveAttackOrder(Server.Local.Units.FirstOrDefault(x => x.Value == CurrentUnit).Key, Server.Local.Units.FirstOrDefault(x => x.Value == u).Key);
		}
	}
}

