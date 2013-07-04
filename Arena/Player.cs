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
		public bool Ready = false;
		public bool Loaded = false;
		public float LoadingPercent = 0;

		public Player(string name, int number, Teams team, Roles role) {
			_name = name.Replace(" ", "");
			Number = number;
			Team = team;
			Role = role;
		}
		public override void Update(GameTime gameTime) {
		}
	}
	public class Bot : Player {
		public Bot(string name, int number, Teams team, Roles role) : base(name, number, team, role) {
		}
		public override void Update(GameTime gameTime) {
			/*Unit attackUnit = null;
			double attackUnitDistance = 10000000;
			foreach (KeyValuePair<int, Unit> kvp in Server.Local.Units) {
				if (CurrentUnit.AttitudeTowards(kvp.Value.Owner) == Attitude.Enemy && Vector2.Distance(CurrentUnit.Position, kvp.Value.Position) < attackUnitDistance) {
					SendAttackOrder(kvp.Value);
					attackUnitDistance = Vector2.Distance(CurrentUnit.Position, kvp.Value.Position);
				}
			}
			if (attackUnit != null && attackUnitDistance <= 200) {
				SendAttackOrder(attackUnit);
			}*/
		}
		protected void SendAttackOrder(Unit u) {
			if (CurrentUnit.AttackTarget != u)
				Server.Local.ReceiveAttackOrder(Server.Local.Units.FirstOrDefault(x => x.Value == CurrentUnit).Key, Server.Local.Units.FirstOrDefault(x => x.Value == u).Key);
		}
	}
}

