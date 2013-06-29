using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Arena {
	public enum PacketType {
		Connect,
		NewPlayer,
		MakePlayerUnit,
		MoveOrder,
		AttackOrder,
		Damage,
		LevelUp,
		UseAbility
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

		public Server(bool isLocalServer) {
			IsLocalServer = isLocalServer;
			if (!IsLocalServer) {
				config = new NetPeerConfiguration(Arena.Config.ApplicationID);
				config.Port = Arena.Config.Port;
				config.MaximumConnections = 16;
				config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
				server = new NetServer(config);
				server.Start();
			}
			AddBot(Teams.Away, Roles.Nuker);
			Local = this;
		}

		public void Tick() {
			if ((incoming = server.ReadMessage()) != null) {
				switch (incoming.MessageType) {
					case NetIncomingMessageType.ConnectionApproval:
						if (incoming.ReadByte() == (byte)PacketType.Connect) {
							incoming.SenderConnection.Approve();
							AddPlayer(incoming.ReadString(), (int)incoming.ReadByte(), (Teams)incoming.ReadByte(), (Roles)incoming.ReadByte());
						}
						break;
					case NetIncomingMessageType.Data:
						switch ((PacketType)incoming.ReadByte()) {
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
						}
						break;
				}
			}
		}
		
		public void AddPlayer(string name, int number, Teams team, Roles role) {
			AddPlayer(new Player(name, number, team, role));
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
			Unit u = MakePlayerUnit(player, new Vector2(150, 500 * playerIndex));
				if (!(player is Bot))
				foreach (KeyValuePair<int, Unit> kvp in Units)
					if (kvp.Key != GetUnitID(u))
						rc.SendNewPlayerUnit(GetUnitID(kvp.Value), GetPlayerID(kvp.Value.Owner));
			playerIndex++;
		}
		public void AddBot(Teams team, Roles role) {
			Bot bot = new Bot(team, role);
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
		public int GetPlayerID(UnitController player) {
			return Players.FirstOrDefault(x => x.Value == player).Key;
		}
		public int GetUnitID(Unit unit) {
			return Units.FirstOrDefault(x => x.Value == unit).Key;
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
		}
	}
}

