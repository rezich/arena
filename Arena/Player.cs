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

