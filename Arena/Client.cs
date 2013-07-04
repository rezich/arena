using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Arena {
	public class Client {
		public static Client Local = null;
		public Player LocalPlayer = null;
		public bool IsConnected = false;
		public bool IsLocalServer = false;
		public Match Match = null;

		public Dictionary<int, Player> Players = new Dictionary<int, Player>();
		public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
		public List<Actor> Actors = new List<Actor>();
		public List<Effect> Effects = new List<Effect>();
		public List<ChatMessage> ChatMessages = new List<ChatMessage>();

		public string ChatBuffer = "";
		public bool IsChatting = false;
		public bool IsAllChatting = false;
		public bool IsShowingScoreboard = false;

		public int? CurrentAbility = null;

		protected NetClient client;

		public Client(bool isLocalServer) {
			IsLocalServer = isLocalServer;
			if (!IsLocalServer) {
				NetPeerConfiguration config = new NetPeerConfiguration(Arena.Config.ApplicationID);
				client = new NetClient(config);
			}
		}

		public void Tick() {
			if (IsLocalServer)
				return;
			if (client.ConnectionStatus == NetConnectionStatus.Connected && !IsConnected) {
				Console.WriteLine("[C] Connected successfully.");
				IsConnected = true;
			}
			if (IsConnected && client.ConnectionStatus == NetConnectionStatus.Disconnected) {
				Console.WriteLine("[C] Disconnected from server.");
				IsConnected = false;
			}
			NetIncomingMessage incoming;
			while ((incoming = client.ReadMessage()) != null) {
				NetConnectionStatus status = (NetConnectionStatus)incoming.ReadByte();
				switch (status) {
					case NetConnectionStatus.Disconnected:
						Console.WriteLine(incoming.ReadString());
						break;
				}
				if (incoming.MessageType == NetIncomingMessageType.ConnectionApproval) {
					Console.WriteLine(incoming.ReadString());
				}
				if (incoming.MessageType == NetIncomingMessageType.Data) {
					switch ((PacketType)incoming.ReadByte()) {
						case PacketType.NewPlayer:
							ReceiveNewPlayer((int)incoming.ReadByte(), incoming.ReadString(), (int)incoming.ReadByte(), incoming.ReadByte(), incoming.ReadByte());
							break;
						case PacketType.MakePlayerUnit:
							ReceiveNewPlayerUnit(incoming.ReadInt32(), (int)incoming.ReadByte(), incoming.ReadFloat(), incoming.ReadFloat(), (double)incoming.ReadFloat());
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
						case PacketType.Disconnect:
							ReceiveDisconnect((int)incoming.ReadByte());
							break;
						case PacketType.AllChat:
							ReceiveAllChat((int)incoming.ReadByte(), incoming.ReadString());
							break;
						case PacketType.TeamChat:
							ReceiveTeamChat((int)incoming.ReadByte(), incoming.ReadString());
							break;
						case PacketType.ChangeTeam:
							ReceiveChangeTeam((int)incoming.ReadByte(), (Teams)incoming.ReadByte());
							break;
						case PacketType.Ready:
							ReceiveReady((int)incoming.ReadByte(), (bool)(incoming.ReadByte() > 0));
							break;
						case PacketType.StartMatch:
							ReceiveStartMatch();
							break;
					}
				}
			}
		}

		/// <summary>
		/// Connect to local server.
		/// </summary>
		public void Connect() {
			if (IsLocalServer) {
				IsConnected = true;
				Server.Local.AddPlayer(Arena.Config.PlayerName, Arena.Config.PlayerNumber, Teams.Neutral, Roles.Runner);
				Console.WriteLine("[C] Connected to local single-player server.");
			}
			else {
				Console.WriteLine("[C] Connecting to server... ");
				NetOutgoingMessage msg = client.CreateMessage();
				client.Start();
				msg.Write((byte)PacketType.Connect);
				msg.Write(Arena.Config.PlayerName);
				msg.Write((byte)Arena.Config.PlayerNumber);
				/* TODO: Use these for reconnecting
				msg.Write((byte)Teams.Home);
				msg.Write((byte)Roles.Runner);
				*/
				NetConnection con = client.Connect(Arena.Config.ServerAddress, Arena.Config.Port, msg);

			}
		}

		public void Disconnect() {
			if (IsLocalServer) {
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.Disconnect);
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
				client.Disconnect("");
			}
		}

		public int GetPlayerID(UnitController player) {
			return Players.FirstOrDefault(x => x.Value == player).Key;
		}
		public int GetUnitID(Unit unit) {
			return Units.FirstOrDefault(x => x.Value == unit).Key;
		}

		public void AddPlayer(string name, int number, Teams team, Roles role) {
			if (IsLocalServer) {
				Server.Local.AddPlayer(name, number, team, role);
			}
		}
		public void ReceiveNewPlayer(int index, string name, int number, byte team, byte role) {
			Console.WriteLine("[C] Recieving new player: " + name + " | " + number + " | " + (Teams)team + " | " + (Roles)role);
			if (Players.ContainsKey(index))
				Players.Remove(index);
			Players.Add(index, new Player(name, number, (Teams)team, (Roles)role));
			if (Players.Count == 1) {
				Console.WriteLine("[C] I didn't have any other players, so this new player is the local player.");
				LocalPlayer = Players[index];
			}
			Console.WriteLine("[C] I now have " + Players.Count + " players.");
		}
		public void ReceiveNewPlayerUnit(int unitIndex, int playerIndex, float x, float y, double direction) {
			Console.WriteLine("[C] Recieving new player unit for " + Players[playerIndex].Name + " at (" + x + ", " + y + ")");
			Player player = Players[playerIndex];
			Unit u = new Unit(player, Role.List[player.Role].Health, Role.List[player.Role].Energy);
			u.Owner = player;
			u.Team = player.Team;
			u.JumpTo(new Vector2(x, y));
			Role.SetUpUnit(ref u, player.Role);
			player.PlayerUnit = u;

			VGame.IShape shape = Role.MakeShape(player.Role);
			u.Actor = new Actor(u, shape);
			u.Actor.Unit = u;

			Actors.Add(u.Actor);
			Units.Add(unitIndex, u);
			player.ControlledUnits.Add(u);
			if (player == LocalPlayer)
				LocalPlayer.CurrentUnit = u;
		}
		public void SendAttackOrder(Unit unit) {
			LocalPlayer.CurrentUnit.AttackTarget = unit;
			if (IsLocalServer) {
				Server.Local.ReceiveAttackOrder(Units.FirstOrDefault(x => x.Value == LocalPlayer.CurrentUnit).Key, Units.FirstOrDefault(x => x.Value == unit).Key);
			}
			else {
				Console.WriteLine("[C] Sending attack order: " + LocalPlayer.Name + " -> " + unit.Owner.Name);
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.AttackOrder);
				msg.Write(GetUnitID(LocalPlayer.CurrentUnit));
				msg.Write(GetUnitID(unit));
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void SendMoveOrder(Vector2 position) {
			Console.WriteLine("[C] Sending move order for unit " + GetUnitID(LocalPlayer.CurrentUnit) + " to (" + position.X + ", " + position.Y + ")");
			LocalPlayer.CurrentUnit.AttackTarget = null;
			LocalPlayer.CurrentUnit.IntendedPosition = position;
			if (IsLocalServer) {
				Server.Local.ReceiveMoveOrder(GetUnitID(LocalPlayer.CurrentUnit), position.X, position.Y);
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.MoveOrder);
				msg.Write(GetUnitID(LocalPlayer.CurrentUnit));
				msg.Write(LocalPlayer.CurrentUnit.IntendedPosition.X);
				msg.Write(LocalPlayer.CurrentUnit.IntendedPosition.Y);
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveAttackOrder(int attackerIndex, int victimIndex) {
			Console.WriteLine("[C] Receiving attack order: " + Units[attackerIndex].Owner.Name + " -> " + Units[victimIndex].Owner.Name);
			Units[attackerIndex].AttackTarget = Units[victimIndex];
		}
		public void ReceiveMoveOrder(int unitIndex, float x, float y) {
			Units[unitIndex].AttackTarget = null;
			Units[unitIndex].IntendedPosition = new Vector2(x, y);
		}
		public void MakeEffect(Effect effect) {
			Effects.Add(effect);
		}
		public void LevelUp(GameTime gameTime, int ability) {
			if (LocalPlayer.CurrentUnit.CanLevelUp(ability)) {
				Console.WriteLine("[C] Sending level up for player " + LocalPlayer.Name);
				if (IsLocalServer) {
					Server.Local.ReceiveLevelUp(GetUnitID(LocalPlayer.CurrentUnit), ability);
				}
				else {
					NetOutgoingMessage msg = client.CreateMessage();
					msg.Write((byte)PacketType.LevelUp);
					msg.Write(GetUnitID(LocalPlayer.CurrentUnit));
					msg.Write((byte)ability);
					client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
		}
		public void ReceiveLevelUp(int unitIndex, int ability) {
			Console.WriteLine("[C] Receiving level up for " + Units[unitIndex].Owner.Name);
			Units[unitIndex].LevelUp(ability);
		}
		public void BeginUsingAbility(GameTime gameTime, int ability) {
			if (LocalPlayer.CurrentUnit.CanUseAbility(ability)) {
				CurrentAbility = ability;
				if (LocalPlayer.CurrentUnit.Abilities[ability].ActivationType == AbilityActivationType.NoTarget) {
					FinishUsingAbility(gameTime);
				}
			}
		}
		public void FinishUsingAbility(GameTime gameTime) {
			if (!CurrentAbility.HasValue)
				return;
			if (LocalPlayer.CurrentUnit.CanUseAbility((int)CurrentAbility)) {
				Console.WriteLine("[C] Sending ability use for " + LocalPlayer.Name);
				if (IsLocalServer) {
					Server.Local.ReceiveUseAbility(GetUnitID(LocalPlayer.CurrentUnit), (int)CurrentAbility, null, null);
				}
				else {
					NetOutgoingMessage msg = client.CreateMessage();
					msg.Write((byte)PacketType.UseAbility);
					msg.Write(GetUnitID(LocalPlayer.CurrentUnit));
					msg.Write((byte)CurrentAbility);
					client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
			CurrentAbility = null;
		}
		public void CancelUsingAbility() {
			CurrentAbility = null;
		}
		public void ReceiveUseAbility(int unitIndex, int ability, float? val1, float? val2) {
			Console.WriteLine("[C] Receiving ability use for " + Units[unitIndex].Owner.Name + ", ability " + ability);
			Units[unitIndex].UseAbility(ability, val1, val2);
		}
		public void ReceiveDisconnect(int playerIndex) {
			Console.WriteLine("[C] Receiving player disconnect for " + Players[playerIndex]);
			List<Unit> units = new List<Unit>(Units.Values.ToList());
			List<int> keys = new List<int>(Units.Keys.ToList());
			for (int j = 0; j < Units.Count; j++) {
				if (units[j].AttackTarget != null && units[j].AttackTarget.Owner == Players[playerIndex]) {
					Units[keys[j]].AttackTarget = null;
					Units[keys[j]].IntendedPosition = Units[keys[j]].Position;
				}
				if (units[j].Owner == Players[playerIndex]) {
					for (int k = 0; k < Actors.Count; k++)
						if (Actors[k].Unit == units[j])
							Actors.Remove(Actors[k]);
					Units.Remove(keys[j]);
				}
			}
			Players.Remove(playerIndex);
		}
		public void SendAllChat(string message) {
			if (message.Length == 0)
				return;
			if (IsLocalServer) {
				Server.Local.ReceiveAllChat(Server.Local.Players[GetPlayerID(LocalPlayer)], message.Trim());
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.AllChat);
				msg.Write(message.Trim());
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveAllChat(int playerIndex, string message) {
			if (message.Length == 0)
				return;
			Console.WriteLine("[C] {0}: {1}", Players[playerIndex].Name, message);
			ChatMessages.Add(new ChatMessage(Players[playerIndex].Name, message, Teams.Neutral));
		}
		public void SendTeamChat(string message) {
			if (IsLocalServer) {
				Server.Local.ReceiveTeamChat(Server.Local.Players[GetPlayerID(LocalPlayer)], message.Trim());
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.TeamChat);
				msg.Write(message.Trim());
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveTeamChat(int playerIndex, string message) {
			Console.WriteLine("[C] {0} ({1}): {2}", Players[playerIndex].Name, Players[playerIndex].Team, message);
			ChatMessages.Add(new ChatMessage(Players[playerIndex].Name, message, Players[playerIndex].Team));
		}
		public void ChangeTeam(Teams team) {
			Console.WriteLine("[C] Moving to " + team);
			if (IsLocalServer) {
				Server.Local.ReceiveChangeTeam(Server.Local.Players[GetPlayerID(LocalPlayer)], team);
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.ChangeTeam);
				msg.Write((byte)team);
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ChangeRole(Roles role) {
			if (LocalPlayer.Team != Teams.Home && LocalPlayer.Team != Teams.Away)
				return;
			Console.WriteLine("[C] Changing role to " + role);
			if (IsLocalServer) {
				Server.Local.ReceiveChangeRole(Server.Local.Players[GetPlayerID(LocalPlayer)], role);
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.ChangeRole);
				msg.Write((byte)role);
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveChangeTeam(int playerIndex, Teams team) {
			Players[playerIndex].Team = team;
		}
		public void ReceiveChangeRole(int playerIndex, Roles role) {
			Players[playerIndex].Role = role;
		}
		public void ToggleReady() {
			if (LocalPlayer.Team == Teams.Neutral)
				return;
			Console.WriteLine("[C] Setting READY to " + !LocalPlayer.Ready);
			LocalPlayer.Ready = !LocalPlayer.Ready;
			if (IsLocalServer) {
				Server.Local.ReceiveReady(Server.Local.Players[GetPlayerID(LocalPlayer)], LocalPlayer.Ready);
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.Ready);
				msg.Write((byte)(LocalPlayer.Ready ? 1 : 0));
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveReady(int playerIndex, bool ready) {
			Players[playerIndex].Ready = ready;
		}
		public void ReceiveStartMatch() {
			Match = new Match();
		}

		public void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
			Tick();
			foreach (KeyValuePair<int, Unit> kvp in Units)
				kvp.Value.Update(gameTime);
			foreach (Actor a in Actors)
				a.Update(gameTime, viewPosition, viewOrigin);
			foreach (Effect e in Effects)
				e.Update(gameTime, viewPosition, viewOrigin);
			Effect.Cleanup(ref Effects);
		}
		public void Draw(GameTime gameTime, Cairo.Context g) {
			foreach (Actor a in Actors)
				a.DrawUIBelow(gameTime, g, LocalPlayer);
			foreach (Effect e in Effects.Where(x => x.Height == EffectPosition.BelowActor))
				e.Draw(gameTime, g, LocalPlayer);
			foreach (Actor a in Actors)
				a.Draw(gameTime, g, LocalPlayer);
			foreach (Actor a in Actors)
				a.DrawUIAbove(gameTime, g, LocalPlayer);
			foreach (Effect e in Effects.Where(x => x.Height == EffectPosition.AboveActor))
				e.Draw(gameTime, g, LocalPlayer);
		}
	}
}

