using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using VGame;

namespace Arena {
	public enum PacketType {
		Connect,
		Disconnect,
		NewPlayer,
		MakePlayerUnit,
		MoveOrder,
		AttackOrder,
		Damage,
		LevelUp,
		UseAbility,
		AllChat,
		TeamChat,
		ChangeTeam,
		ChangeRole,
		Ready,
		StartLoading,
		LoadingPercent,
		Loaded,
		TimeSync,
		StartMatch
	}
	public class Server {
		protected NetServer server;
		protected NetPeerConfiguration config;
		protected NetIncomingMessage incoming;

		public static Server Local = null;
		public readonly bool IsLocalServer;

		public SortedDictionary<int, Player> Players = new SortedDictionary<int, Player>();
		protected int playerIndex = 0;
		public SortedDictionary<int, Unit> Units = new SortedDictionary<int, Unit>();
		protected int unitIndex = 0;
		protected SortedDictionary<int, RemoteClient> RemoteClients = new SortedDictionary<int, RemoteClient>();
		public List<ChatMessage> ChatMessages = new List<ChatMessage>();
		public Match Match = null;
		public TimeSpan? StartTime = null;

		public Server(bool isLocalServer) {
			IsLocalServer = isLocalServer;
			if (!IsLocalServer) {
				config = new NetPeerConfiguration(Arena.Config.ApplicationID);
				config.Port = Arena.Config.Port;
				config.ConnectionTimeout = 10;
				config.MaximumConnections = 16;
				config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
				server = new NetServer(config);
				server.Start();
			}
			Local = this;
		}

		protected void DisconnectClient(RemoteClient r) {
			List<Unit> units = new List<Unit>(Units.Values.ToList());
			List<int> keys = new List<int>(Units.Keys.ToList());
			for (int j = 0; j < Units.Count; j++) {
				if (units[j].AttackTarget != null && units[j].AttackTarget.Owner == Players[r.PlayerID]) {
					Units[keys[j]].AttackTarget = null;
					Units[keys[j]].IntendedPosition = Units[keys[j]].Position;
				}
				if (units[j].Owner == Players[r.PlayerID]) {
					Units.Remove(keys[j]);
				}
			}
			Players.Remove(r.PlayerID);
			int id = r.PlayerID;
			RemoteClients.Remove(r.PlayerID);
			foreach (RemoteClient rc in AllClients())
				rc.SendDisconnect(id);
			Console.WriteLine("[S] {0} clients, {1} players, {2} units remaining.", RemoteClients.Count, Players.Count, Units.Count);
		}

		public void Tick() {
			List<RemoteClient> clients = new List<RemoteClient>(RemoteClients.Values.ToList());
			/*for (int i = 0; i < clients.Count; i++) {
				if (clients[i].Connection == null || clients[i].Connection.Status == NetConnectionStatus.Disconnected) {
					Console.WriteLine("[S] " + Players[clients[i].PlayerID].Name + " timed out!");
					DisconnectClient(clients[i]);
				}
			}*/
			if ((incoming = server.ReadMessage()) != null) {
				switch (incoming.MessageType) {
					case NetIncomingMessageType.ConnectionApproval:
						if (incoming.ReadByte() == (byte)PacketType.Connect) {
							if (server.ConnectionsCount >= server.Configuration.MaximumConnections) {
								incoming.SenderConnection.Deny("Server full.");
							}
							else if (Match != null) {
								// TODO: Check to see if it's a reconnecting user.
								//       (Remember to send all abilities and stuff to that user.)
								incoming.SenderConnection.Deny("Match is already in progress.");
							}
							else {
								incoming.SenderConnection.Approve();
								//AddPlayer(incoming.ReadString(), (int)incoming.ReadByte(), (Teams)incoming.ReadByte(), (Roles)incoming.ReadByte());
								AddPlayer(incoming.ReadString(), (int)incoming.ReadByte(), Teams.Neutral, Roles.Runner);
								//RemoteClients[GetPlayerID(GetPlayerByConnection(incoming.SenderConnection))].SendTimeSync();
							}
						}
						break;
					case NetIncomingMessageType.StatusChanged:
						if (incoming.SenderConnection.Status == NetConnectionStatus.Disconnected)
							for (int i = 0; i < clients.Count; i++)
								if (clients[i].Connection == incoming.SenderConnection) {
									Console.WriteLine("[S] " + Players[clients[i].PlayerID].Name + " timed out!");
									DisconnectClient(clients[i]);
								}
						break;
					case NetIncomingMessageType.Data:
						switch ((PacketType)incoming.ReadByte()) {
							case PacketType.Disconnect:
								for (int i = 0; i < clients.Count; i++)
									if (clients[i].Connection == incoming.SenderConnection) {
									Console.WriteLine("[S] " + Players[clients[i].PlayerID].Name + " intentionally disconnected!");
									DisconnectClient(clients[i]);
								}
								break;
							case PacketType.MoveOrder:
								if (Match == null)
									break;
								ReceiveMoveOrder(incoming.ReadInt32(), incoming.ReadFloat(), incoming.ReadFloat());
								break;
							case PacketType.AttackOrder:
								if (Match == null)
									break;
								ReceiveAttackOrder(incoming.ReadInt32(), incoming.ReadInt32());
								break;
							case PacketType.LevelUp:
								if (Match == null)
									break;
								ReceiveLevelUp(incoming.ReadInt32(), (int)incoming.ReadByte());
								break;
							case PacketType.UseAbility:
								if (Match == null)
									break;
								int unitIndex = incoming.ReadInt32();
								int ability = (int)incoming.ReadByte();
								float? val1 = null;
								float? val2 = null;
								/*if (incoming.PositionInBytes < incoming.LengthBytes)
									val1 = (float?)incoming.ReadFloat();
								if (incoming.PositionInBytes < incoming.LengthBytes)
									val2 = (float?)incoming.ReadFloat();*/
								ReceiveUseAbility(unitIndex, ability, val1, val2);
								break;
							case PacketType.AllChat:
								ReceiveAllChat(GetPlayerByConnection(incoming.SenderConnection), incoming.ReadString());
								break;
							case PacketType.TeamChat:
								ReceiveTeamChat(GetPlayerByConnection(incoming.SenderConnection), incoming.ReadString());
								break;
							case PacketType.ChangeTeam:
								ReceiveChangeTeam(GetPlayerByConnection(incoming.SenderConnection), (Teams)incoming.ReadByte());
								break;
							case PacketType.ChangeRole:
								ReceiveChangeRole(GetPlayerByConnection(incoming.SenderConnection), (Roles)incoming.ReadByte());
								break;
							case PacketType.Ready:
								ReceiveReady(GetPlayerByConnection(incoming.SenderConnection), (bool)(incoming.ReadByte() > 0));
								break;
							case PacketType.LoadingPercent:
								ReceiveLoadingPercent(GetPlayerByConnection(incoming.SenderConnection), incoming.ReadFloat());
								break;
							case PacketType.Loaded:
								ReceiveLoaded(GetPlayerByConnection(incoming.SenderConnection));
								break;
						}
						break;
				}
			}
		}
		
		public void AddPlayer(string name, int number, Teams team, Roles role) {
			string baseName = name;
			int tryNumber = 0;
			while (true) {
				bool found = false;
				foreach (KeyValuePair<int, Player> kvp in Players)
					if (kvp.Value.Name == baseName + (tryNumber == 0 ? "" : tryNumber.ToString()))
						found = true;
				if (found)
					tryNumber++;
				else
					break;
			}
			AddPlayer(new Player(baseName + (tryNumber == 0 ? "" : tryNumber.ToString()), number, team, role));
		}
		public void AddPlayer(Player player) {
			Console.WriteLine("[S] Adding new player: " + player.Name + " | " + player.Number + " | " + player.Team + " | " + player.Role);
			RemoteClient rc = null;
			if (!(player is Bot)) {
				rc = new RemoteClient(playerIndex, IsLocalServer ? null : incoming.SenderConnection);
				RemoteClients.Add(playerIndex, rc);
			}
			Players.Add(playerIndex, player);
			foreach (RemoteClient r in AllClients())
				r.SendNewPlayer(playerIndex);
			if (!(player is Bot))
				foreach (KeyValuePair<int, Player> kvp in Players)
					if (kvp.Key != playerIndex)
						rc.SendNewPlayer(kvp.Key);
			/*Unit u = MakePlayerUnit(player, new Vector2(100, 100).AddLengthDir(100, Arena.Config.Random.NextDouble() * MathHelper.TwoPi));
				if (!(player is Bot))
				foreach (KeyValuePair<int, Unit> kvp in Units)
					if (kvp.Key != GetUnitID(u))
						rc.SendNewPlayerUnit(GetUnitID(kvp.Value), GetPlayerID(kvp.Value.Owner));*/
			playerIndex++;
		}
		public void AddBot(Teams team, Roles role) {
			int botNumber = 0;
			string name = "";
			while (name == "") {
				for (int i = 0; i < Config.BotNames.Count; i++) {
					string tryName = Config.BotNames[i] + " Bot" + (botNumber == 0 ? "" : botNumber.ToString());
					bool found = false;
					foreach (KeyValuePair<int, Player> kvp in Players) {
						if (tryName == kvp.Value.Name)
							found = true;
					}
					if (!found) {
						name = tryName;
						break;
					}
					botNumber++;
				}
			}
			Bot bot = new Bot(name, Config.Random.Next(0, 50), team, role);
			AddPlayer(bot);
		}
		public Unit MakePlayerUnit(Player player, Vector2 position) {
			Console.WriteLine("[S] Making new player unit for " + player.Name + " at (" + position.X + ", " + position.Y + ")");
			Unit u = new Unit(player, Role.List[player.Role].Health, Role.List[player.Role].Energy);
			u.Owner = player;
			u.Team = player.Team;
			u.JumpTo(position);
			Role.SetUpUnit(ref u, player.Role);
			player.ControlledUnits.Add(u);
			player.PlayerUnit = u;

			Units.Add(++unitIndex, u);
			foreach (RemoteClient r in AllClients())
				r.SendNewPlayerUnit(unitIndex, GetPlayerID(player));
			return u;
		}
		public void ReceiveAttackOrder(int attackerIndex, int victimIndex) {
			Console.WriteLine("[S] Receiving attack order: " + Units[attackerIndex].Owner.Name + " -> " + Units[victimIndex].Owner.Name);
			Units[attackerIndex].AttackTarget = Units[victimIndex];
			foreach (RemoteClient r in AllClientsButOne(GetPlayerID(Units[attackerIndex].Owner)))
				r.SendAttackOrder(Units[attackerIndex], Units[victimIndex]);
		}
		public void ReceiveMoveOrder(int unitIndex, float x, float y) {
			Console.WriteLine("[S] Receiving move order for unit " + unitIndex + " to (" + x + ", " + y + ")");
			Unit u = Units[unitIndex];
			u.AttackTarget = null;
			u.IntendedPosition = new Vector2(x, y);
			foreach (RemoteClient r in AllClientsButOne(GetPlayerID(Units[unitIndex].Owner)))
			//foreach (RemoteClient r in RemoteClients.Values)
				r.SendMoveOrder(Units[unitIndex], u.IntendedPosition);
		}
		public void ReceiveLevelUp(int unitIndex, int ability) {
			Console.WriteLine("[S] Receiving level up for player " + Units[unitIndex].Owner.Name);
			//foreach (RemoteClient r in AllClientsButOne(GetPlayerID((Player)Units[unitIndex].Owner)))
			if (Units[unitIndex].CanLevelUp(ability)) {
				Units[unitIndex].LevelUp(ability);
				foreach (RemoteClient r in AllClients())
					r.SendLevelUp(Units[unitIndex], ability);
			}
		}
		public void ReceiveUseAbility(int unitIndex, int ability, float? val1, float? val2) {
			Console.WriteLine("[S] Receiving ability use for player " + Units[unitIndex].Owner.Name + ", ability " + ability);
			Units[unitIndex].UseAbility(ability, val1, val2);
			foreach (RemoteClient r in AllClients())
				r.SendUseAbility(Units[unitIndex], ability, val1, val2);
		}
		public void ReceiveAllChat(Player player, string message) {
			if (message.Length == 0)
				return;
			Console.WriteLine("[S] {0}: {1}", player.Name, message.Trim());
			foreach (RemoteClient r in AllClients())
				r.SendAllChat(player, message.Trim());
		}
		public void ReceiveTeamChat(Player player, string message) {
			if (message.Length == 0)
				return;
			Console.WriteLine("[S] {0} ({1}): {2}", player.Name, player.Team, message.Trim());
			foreach (RemoteClient r in AllClients())
				if (Players[r.PlayerID].Team == player.Team)
					r.SendTeamChat(player, message.Trim());
		}
		public void ReceiveChangeTeam(Player player, Teams team) {
			if (player.Team == team)
				return;
			player.Team = team;
			Console.WriteLine("[S] {0} moving to {1}", player.Name, team);
			foreach (RemoteClient r in AllClients()) {
				r.SendChangeTeam(player, team);
			}
		}
		public void ReceiveChangeRole(Player player, Roles role) {
			if (player.Role == role || (player.Team != Teams.Home && player.Team != Teams.Away))
				return;
			player.Role = role;
			Console.WriteLine("[S] {0} is now a {1}", player.Name, role);
			foreach (RemoteClient r in AllClients()) {
				r.SendChangeRole(player, role);
			}
		}
		public void ReceiveReady(Player player, bool ready) {
			if (player.Team == Teams.Neutral)
				return;
			// TODO: IF IN LOBBY
			player.Ready = ready;
			Console.WriteLine("[S] {0}'s READY is now {1}", player.Name, ready);
			foreach (RemoteClient r in AllClientsButOne(GetPlayerID(player))) {
				r.SendReady(player, ready);
			}
		}
		public void ReceiveLoadingPercent(Player player, float percent) {
			player.LoadingPercent = percent;
			foreach (RemoteClient r in AllClientsButOne(GetPlayerID(player))) {
				r.SendLoadingPercent(player, percent);
			}
		}
		public void ReceiveLoaded(Player player) {
			player.Loaded = true;
		}

		public void StartLoading() {
			Console.WriteLine("[S] Starting match loading");
			Match = new Match();
			foreach (RemoteClient r in AllClients()) {
				r.SendStartLoading();
			}
			foreach (KeyValuePair<int, Player> kvp in Players) {
				Unit u = MakePlayerUnit(kvp.Value, new Vector2(100, 100).AddLengthDir(100, Arena.Config.Random.NextDouble() * MathHelper.TwoPi));
			}
		}
		public void StartMatch() {
			Console.WriteLine("[S] Starting match!");
			Match.Started = true;
			foreach (RemoteClient r in AllClients()) {
				r.SendStartMatch();
			}
		}

		public int GetPlayerID(UnitController player) {
			return Players.FirstOrDefault(x => x.Value == player).Key;
		}
		public int GetUnitID(Unit unit) {
			return Units.FirstOrDefault(x => x.Value == unit).Key;
		}
		public Player GetPlayerByConnection(NetConnection connection) {
			foreach (KeyValuePair<int, RemoteClient> kvp in RemoteClients) {
				if (kvp.Value.Connection == connection)
					return Players[kvp.Value.PlayerID];
			}
			return null;
		}

		protected List<RemoteClient> AllClientsButOne(int one) {
			Dictionary<int, RemoteClient> dict = new Dictionary<int, RemoteClient>(RemoteClients);
			if (dict.ContainsKey(one)) dict.Remove(one);
			return dict.Values.ToList<RemoteClient>();
		}
		protected List<RemoteClient> AllClients() {
			return RemoteClients.Values.ToList<RemoteClient>();
		}

		public void Update(GameTime gameTime) {
			if (Match == null) {
				if (Players.Count > 0 && Players.Where(x => x.Value.Ready == false).Count() == 0) {
					if (StartTime.HasValue) {
						if (gameTime.TotalGameTime >= StartTime) {
							StartLoading();
							StartTime = null;
						}
					}
					else {
						Console.WriteLine("[S] All clients ready, beginning countdown");
						StartTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(Config.ReadyCountdown);
					}
				}
				else {
					StartTime = null;
				}
			}
			else {
				if (!Match.Started) {
					if (Players.Where(x => x.Value.Loaded == false).Count() == 0) {
						StartMatch();
					}
				}
			}
			foreach (KeyValuePair<int, Player> kvp in Players)
				kvp.Value.Update(gameTime);
			foreach (KeyValuePair<int, Unit> kvp in Units)
				kvp.Value.Update(gameTime);
		}

		protected class RemoteClient {
			public int PlayerID;
			public NetConnection Connection;
			public RemoteClient(int playerID, NetConnection connection) {
				PlayerID = playerID;
				Connection = connection;
			}
			public void SendAttackOrder(Unit attacker, Unit victim) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveAttackOrder(Server.Local.GetUnitID(attacker), Server.Local.GetUnitID(victim));
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.AttackOrder);
					msg.Write(Server.Local.GetUnitID(attacker));
					msg.Write(Server.Local.GetUnitID(victim));
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendMoveOrder(Unit unit, Vector2 position) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveMoveOrder(Server.Local.GetUnitID(unit), position.X, position.Y);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.MoveOrder);
					msg.Write(Server.Local.GetUnitID(unit));
					msg.Write(unit.IntendedPosition.X);
					msg.Write(unit.IntendedPosition.Y);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendNewPlayerUnit(int unitIndex, int playerIndex) {
				if (Server.Local.IsLocalServer) {
					Unit u = Server.Local.Units[unitIndex];
					Client.Local.ReceiveNewPlayerUnit(unitIndex, playerIndex, u.Position.X, u.Position.Y, u.Direction);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.MakePlayerUnit);
					msg.Write(unitIndex);
					msg.Write((byte)playerIndex);
					msg.Write(Server.Local.Units[unitIndex].Position.X);
					msg.Write(Server.Local.Units[unitIndex].Position.Y);
					msg.Write((float)Server.Local.Units[unitIndex].Direction);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendNewPlayer(int index) {
				if (Server.Local.IsLocalServer) {
					Player p = Server.Local.Players[index];
					Client.Local.ReceiveNewPlayer(index, p.Name, p.Number, (byte)p.Team, (byte)p.Role);
				}
				else {
					Player p = Server.Local.Players[index];
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.NewPlayer);
					msg.Write((byte)index);
					msg.Write(p.Name);
					msg.Write((byte)p.Number);
					msg.Write((byte)p.Team);
					msg.Write((byte)p.Role);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendLevelUp(Unit unit, int ability) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveLevelUp(Server.Local.GetUnitID(unit), ability);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.LevelUp);
					msg.Write(Server.Local.GetUnitID(unit));
					msg.Write((byte)ability);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendUseAbility(Unit unit, int ability, float? val1, float? val2) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveUseAbility(Server.Local.GetUnitID(unit), ability, val1, val2);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.UseAbility);
					msg.Write(Server.Local.GetUnitID(unit));
					msg.Write((byte)ability);
					if (val1.HasValue)
						msg.Write((float)val1);
					if (val2.HasValue)
						msg.Write((float)val2);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendDisconnect(int playerIndex) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveDisconnect(playerIndex);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.Disconnect);
					msg.Write((byte)playerIndex);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendAllChat(Player player, string message) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveAllChat(Server.Local.GetPlayerID(player), message);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.AllChat);
					msg.Write((byte)Server.Local.GetPlayerID(player));
					msg.Write(message);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendTeamChat(Player player, string message) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveTeamChat(Server.Local.GetPlayerID(player), message);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.TeamChat);
					msg.Write((byte)Server.Local.GetPlayerID(player));
					msg.Write(message);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendChangeTeam(Player player, Teams team) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveChangeTeam(Server.Local.GetPlayerID(player), team);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.ChangeTeam);
					msg.Write((byte)Server.Local.GetPlayerID(player));
					msg.Write((byte)team);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendChangeRole(Player player, Roles role) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveChangeRole(Server.Local.GetPlayerID(player), role);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.ChangeRole);
					msg.Write((byte)Server.Local.GetPlayerID(player));
					msg.Write((byte)role);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendReady(Player player, bool ready) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveReady(Server.Local.GetPlayerID(player), ready);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.Ready);
					msg.Write((byte)Server.Local.GetPlayerID(player));
					msg.Write((byte)(ready ? 1 : 0));
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendStartLoading() {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveStartLoading();
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.StartLoading);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendLoadingPercent(Player player, float percent) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveLoadingPercent(Server.Local.GetPlayerID(player), percent);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.LoadingPercent);
					msg.Write((byte)Server.Local.GetPlayerID(player));
					msg.Write(percent);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendStartMatch() {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveStartMatch(0);
				}
				else {
					NetOutgoingMessage msg = Server.Local.server.CreateMessage();
					msg.Write((byte)NetConnectionStatus.Connected);
					msg.Write((byte)PacketType.StartMatch);
					msg.WriteTime(NetTime.Now, true);
					Server.Local.server.SendMessage(msg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
		}
	}
}

