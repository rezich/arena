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
		public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();

		public Server() {
			IsLocalServer = true;
		}

		public void Update(GameTime gameTime) {
		}
		public void AddPlayer(string name, int number, Teams team, Roles role) {
			Player player = new Player(name, number, team, role);
			int index = Players.Count == 0 ? 0 : Players.Last().Key + 1;
			Players.Add(index, player);
			SendNewPlayer(index);
		}
		public void SendNewPlayer(int index) {
			if (IsLocalServer) {
				Player p = Players[index];
				Client.Local.RecieveNewPlayer(index, p.Name, p.Number, p.Team, p.Role);
			}
		}
		public void MakePlayerUnit() {
		}
	}
}

