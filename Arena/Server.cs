using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;

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
		TeamChat
	}
	public class Server {
		protected NetServer server;
		protected NetPeerConfiguration config;
		protected NetIncomingMessage incoming;


		public static Server Local = null;
		public readonly bool IsLocalServer;

		public Dictionary<int, Player> Players = new Dictionary<int, Player>();
		protected int playerIndex = 0;
		public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
		protected int unitIndex = 0;
		protected Dictionary<int, RemoteClient> RemoteClients = new Dictionary<int, RemoteClient>();
		public List<ChatMessage> ChatMessages = new List<ChatMessage>();

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
			AddBot(Teams.Away, Roles.Nuker);
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
							incoming.SenderConnection.Approve();
							AddPlayer(incoming.ReadString(), (int)incoming.ReadByte(), (Teams)incoming.ReadByte(), (Roles)incoming.ReadByte());
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
								ReceiveMoveOrder(incoming.ReadInt32(), incoming.ReadFloat(), incoming.ReadFloat());
								break;
							case PacketType.AttackOrder:
								ReceiveAttackOrder(incoming.ReadInt32(), incoming.ReadInt32());
								break;
							case PacketType.LevelUp:
								ReceiveLevelUp(incoming.ReadInt32(), (int)incoming.ReadByte());
								break;
							case PacketType.UseAbility:
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
			Unit u = MakePlayerUnit(player, new Vector2(100, 100).AddLengthDir(100, Arena.Config.Random.NextDouble() * MathHelper.TwoPi));
				if (!(player is Bot))
				foreach (KeyValuePair<int, Unit> kvp in Units)
					if (kvp.Key != GetUnitID(u))
						rc.SendNewPlayerUnit(GetUnitID(kvp.Value), GetPlayerID(kvp.Value.Owner));
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
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.AttackOrder);
					outMsg.Write(Server.Local.GetUnitID(attacker));
					outMsg.Write(Server.Local.GetUnitID(victim));
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendMoveOrder(Unit unit, Vector2 position) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveMoveOrder(Server.Local.GetUnitID(unit), position.X, position.Y);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.MoveOrder);
					outMsg.Write(Server.Local.GetUnitID(unit));
					outMsg.Write(unit.IntendedPosition.X);
					outMsg.Write(unit.IntendedPosition.Y);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendNewPlayerUnit(int unitIndex, int playerIndex) {
				if (Server.Local.IsLocalServer) {
					Unit u = Server.Local.Units[unitIndex];
					Client.Local.ReceiveNewPlayerUnit(unitIndex, playerIndex, u.Position.X, u.Position.Y, u.Direction);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.MakePlayerUnit);
					outMsg.Write(unitIndex);
					outMsg.Write((byte)playerIndex);
					outMsg.Write(Server.Local.Units[unitIndex].Position.X);
					outMsg.Write(Server.Local.Units[unitIndex].Position.Y);
					outMsg.Write((float)Server.Local.Units[unitIndex].Direction);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendNewPlayer(int index) {
				if (Server.Local.IsLocalServer) {
					Player p = Server.Local.Players[index];
					Client.Local.ReceiveNewPlayer(index, p.Name, p.Number, (byte)p.Team, (byte)p.Role);
				}
				else {
					Player p = Server.Local.Players[index];
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.NewPlayer);
					outMsg.Write((byte)index);
					outMsg.Write(p.Name);
					outMsg.Write((byte)p.Number);
					outMsg.Write((byte)p.Team);
					outMsg.Write((byte)p.Role);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendLevelUp(Unit unit, int ability) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveLevelUp(Server.Local.GetUnitID(unit), ability);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.LevelUp);
					outMsg.Write(Server.Local.GetUnitID(unit));
					outMsg.Write((byte)ability);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendUseAbility(Unit unit, int ability, float? val1, float? val2) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveUseAbility(Server.Local.GetUnitID(unit), ability, val1, val2);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.UseAbility);
					outMsg.Write(Server.Local.GetUnitID(unit));
					outMsg.Write((byte)ability);
					if (val1.HasValue)
						outMsg.Write((float)val1);
					if (val2.HasValue)
						outMsg.Write((float)val2);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendDisconnect(int playerIndex) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveDisconnect(playerIndex);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.Disconnect);
					outMsg.Write((byte)playerIndex);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendAllChat(Player player, string message) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveAllChat(Server.Local.GetPlayerID(player), message);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.AllChat);
					outMsg.Write((byte)Server.Local.GetPlayerID(player));
					outMsg.Write(message);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendTeamChat(Player player, string message) {
				if (Server.Local.IsLocalServer) {
					Client.Local.ReceiveTeamChat(Server.Local.GetPlayerID(player), message);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketType.TeamChat);
					outMsg.Write((byte)Server.Local.GetPlayerID(player));
					outMsg.Write(message);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
		}
	}
}

