using System;
using System.Collections;
using System.Collections.Generic;

namespace Arena {
	public class Client {
		public static Client Local;
		public Player LocalPlayer;
		public bool IsConnected = false;
		public bool IsLocalServer = false;

		public Dictionary<int, Player> Players = new Dictionary<int, Player>();

		public Client() {
		}

		/// <summary>
		/// Connect to local server.
		/// </summary>
		public bool Connect() {
			IsConnected = true;
			IsLocalServer = true;
			return IsConnected;
		}

		public void AddPlayer(string name, int number, Teams team, Roles role) {
			if (IsLocalServer) {
				Server.Local.AddPlayer(name, number, team, role);
			}
		}
		public void RecieveNewPlayer(int index, string name, int number, Teams team, Roles role) {
			if (Players.ContainsKey(index))
				Players.Remove(index);
			Players.Add(index, new Player(name, number, team, role));
			if (Players.Count == 1)
				LocalPlayer = Players[index];
		}
	}
}

