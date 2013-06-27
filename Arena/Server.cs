using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Arena {
	public class Server {
		public static Server Local;
		public readonly bool IsLocalServer;

		public Dictionary<int, Player> Players = new Dictionary<int, Player>();
		protected int playerIndex = 0;
		public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
		protected int unitIndex = 0;

		public Server() {
			IsLocalServer = true;
		}
		
		public void AddPlayer(string name, int number, Teams team, Roles role) {
			Player player = new Player(name, number, team, role);
			Players.Add(++playerIndex, player);
			SendNewPlayer(playerIndex);
			MakePlayerUnit(player, new Vector2(150, 150 * playerIndex));
		}
		public void SendNewPlayer(int index) {
			if (IsLocalServer) {
				Player p = Players[index];
				Client.Local.RecieveNewPlayer(index, p.Name, p.Number, p.Team, p.Role);
			}
		}
		public void MakePlayerUnit(Player player, Vector2 position) {
			Unit u = new Unit(player, Role.List[player.Role].Health, Role.List[player.Role].Energy);
			u.Owner = player;
			u.Team = player.Team;
			u.JumpTo(position);
			Role.SetUpUnit(ref u, player.Role);
			player.ControlledUnits.Add(u);

			Units.Add(++unitIndex, u);
			SendNewPlayerUnit(unitIndex, Players.FirstOrDefault(x => x.Value == player).Key);
		}
		public void SendNewPlayerUnit(int unitIndex, int playerIndex) {
			if (IsLocalServer) {
				Unit u = Units[unitIndex];
				Client.Local.RecieveNewPlayerUnit(unitIndex, playerIndex, u.Position.X, u.Position.Y, u.Direction);
			}
		}
		public void RecieveMoveOrder(int unitIndex, float x, float y) {
			Unit u = Units[unitIndex];
			u.AttackTarget = null;
			u.IntendedPosition = new Vector2(x, y);
		}

		public void Update(GameTime gameTime) {
			/*foreach (Player p in Players)
				p.Update(gameTime);*/
			foreach (KeyValuePair<int, Unit> kvp in Units)
				kvp.Value.Update(gameTime);
			/*foreach (Effect e in Effect.List)
				e.Update(gameTime, viewPosition, viewOrigin);*/
			//Effect.Cleanup();
		}
	}
}

