using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Arena {
	public enum PacketTypes {
		Connect,
		NewPlayer,
		MakePlayerUnit
	}
	public class Server {
		protected NetServer server;
		protected NetPeerConfiguration config;
		protected NetIncomingMessage incoming;


		public static Server Local;
		public readonly bool IsLocalServer;
		protected bool isDrawing = false;
		public bool IsDrawing {
			get {
				return IsLocalServer && isDrawing;
			}
			set {
				isDrawing = value;
			}
		}

		public Dictionary<int, Player> Players = new Dictionary<int, Player>();
		protected int playerIndex = 0;
		public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
		protected int unitIndex = 0;
		protected Dictionary<int, RemoteClient> RemoteClients = new Dictionary<int, RemoteClient>();
		protected Shapes.Runner GenericShape = new Arena.Shapes.Runner();

		public Server(bool isLocalServer) {
			IsLocalServer = isLocalServer;

			config = new NetPeerConfiguration(Arena.Config.ApplicationID);
			config.Port = Arena.Config.Port;
			config.MaximumConnections = 16;
			config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
			server = new NetServer(config);
			server.Start();
			Local = this;
		}

		public void Tick() {
			if ((incoming = server.ReadMessage()) != null) {
				switch (incoming.MessageType) {
					case NetIncomingMessageType.ConnectionApproval:
						if (incoming.ReadByte() == (byte)PacketTypes.Connect) {
							incoming.SenderConnection.Approve();
							AddPlayer(incoming.ReadString(), (int)incoming.ReadByte(), (Teams)incoming.ReadByte(), (Roles)incoming.ReadByte());
						}
						break;
					case NetIncomingMessageType.Data:
						switch ((PacketTypes)incoming.ReadByte()) {
							case PacketTypes.MakePlayerUnit:
								Console.WriteLine("PRETTY SURE THIS SHOULDN'T HAPPEN");
								break;
						}
						break;
				}
			}
		}
		
		public void AddPlayer(string name, int number, Teams team, Roles role) {
			Console.WriteLine("Adding new player: " + name);
			Player player = new Player(name, number, team, role);
			RemoteClient rc = new RemoteClient(playerIndex, incoming.SenderConnection);
			RemoteClients.Add(playerIndex, rc);
			Players.Add(playerIndex, player);
			foreach (RemoteClient r in AllClients())
				r.SendNewPlayer(playerIndex);
			foreach (KeyValuePair<int, Player> kvp in Players)
				if (kvp.Key != playerIndex)
					rc.SendNewPlayer(kvp.Key);
			Unit u = MakePlayerUnit(player, new Vector2(150, 150 * playerIndex));
			foreach (KeyValuePair<int, Unit> kvp in Units)
				if (kvp.Key != GetUnitID(u))
					rc.SendNewPlayerUnit(GetUnitID(kvp.Value), GetPlayerID(kvp.Value.Owner));
			playerIndex++;
		}
		public void AddBot(Teams team, Roles role) {
			Bot bot = new Bot(team, role);
			Players.Add(++playerIndex, bot);
			foreach(RemoteClient r in AllClients())
				r.SendNewPlayer(playerIndex);
			MakePlayerUnit(bot, new Vector2(150, 150 * playerIndex));
		}
		public Unit MakePlayerUnit(Player player, Vector2 position) {
			Console.Write("Making new player unit for " + player.Name + " at (" + position.X + ", " + position.Y + ")");
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
		public void RecieveAttackOrder(int attackerIndex, int victimIndex) {
			Console.WriteLine("Recieving attack order: " + Units[attackerIndex].Owner.Name + " -> " + Units[victimIndex].Owner.Name);
			Units[attackerIndex].AttackTarget = Units[victimIndex];
			foreach (RemoteClient r in AllClientsButOne(GetPlayerID(Units[attackerIndex].Owner)))
				r.SendAttackOrder(Units[attackerIndex], Units[victimIndex]);
		}
		public void RecieveMoveOrder(int unitIndex, float x, float y) {
			Unit u = Units[unitIndex];
			u.AttackTarget = null;
			u.IntendedPosition = new Vector2(x, y);
			foreach (RemoteClient r in AllClientsButOne(GetPlayerID(Units[unitIndex].Owner)))
				r.SendMoveOrder(Units[unitIndex], u.IntendedPosition);
		}
		public void RecieveLevelUp(int unitIndex, int ability) {
			//foreach (RemoteClient r in AllClientsButOne(GetPlayerID((Player)Units[unitIndex].Owner)))
			if (Units[unitIndex].CanLevelUp(ability)) {
				Units[unitIndex].LevelUp(ability);
				foreach (RemoteClient r in AllClients())
					r.SendLevelUp(Units[unitIndex], ability);
			}
		}
		public void RecieveUseAbility(int unitIndex, int ability, float? val1, float? val2) {
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
		public void Draw(GameTime gameTime, Cairo.Context g, Vector2 viewPosition, Vector2 viewOrigin) {
			foreach (KeyValuePair<int, Unit> kvp in Units) {
				g.Save();
				g.SetDash(new double[] { 4, 4 }, 0);
				GenericShape.Draw(g, kvp.Value.Position - viewPosition + viewOrigin, kvp.Value.Direction, null, new Cairo.Color(0, 0, 0.5, 0.5), Config.ActorScale);
				g.Restore();
			}
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
					Client.Local.RecieveAttackOrder(Server.Local.GetUnitID(attacker), Server.Local.GetUnitID(victim));
				}
			}
			public void SendMoveOrder(Unit unit, Vector2 position) {
				if (Server.Local.IsLocalServer) {
					Client.Local.RecieveMoveOrder(Server.Local.GetUnitID(unit), position.X, position.Y);
				}
			}
			public void SendNewPlayerUnit(int unitIndex, int playerIndex) {
				if (Server.Local.IsLocalServer) {
					Unit u = Server.Local.Units[unitIndex];
					Client.Local.RecieveNewPlayerUnit(unitIndex, playerIndex, u.Position.X, u.Position.Y, u.Direction);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketTypes.MakePlayerUnit);
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
					Client.Local.RecieveNewPlayer(index, p.Name, p.Number, p.Team, p.Role);
				}
				else {
					NetOutgoingMessage outMsg = Server.Local.server.CreateMessage();
					outMsg.Write((byte)PacketTypes.NewPlayer);
					outMsg.Write((byte)index);
					outMsg.Write(Server.Local.Players[index].Name);
					outMsg.Write(Server.Local.Players[index].Number);
					outMsg.Write((byte)Server.Local.Players[index].Team);
					outMsg.Write((byte)Server.Local.Players[index].Role);
					//Server.Local.server.SendMessage(outMsg, Server.Local.server., NetDeliveryMethod.ReliableOrdered, 0);
					Server.Local.server.SendMessage(outMsg, Connection, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			public void SendLevelUp(Unit unit, int ability) {
				if (Server.Local.IsLocalServer) {
					Client.Local.RecieveLevelUp(Server.Local.GetUnitID(unit), ability);
				}
			}
			public void SendUseAbility(Unit unit, int ability, float? val1, float? val2) {
				if (Server.Local.IsLocalServer) {
					Client.Local.RecieveUseAbility(Server.Local.GetUnitID(unit), ability, val1, val2);
				}
			}
		}
	}
}

