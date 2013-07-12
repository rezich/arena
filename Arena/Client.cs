using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using VGame;

namespace Arena {
	public class Client {
		public static Client Local = null;
		public Game Game;
		public Player LocalPlayer = null;
		public bool IsConnected = false;
		public bool IsLocalServer = false;
		public Match Match = null;

		public SortedDictionary<int, Player> Players = new SortedDictionary<int, Player>();
		public SortedDictionary<int, Unit> Units = new SortedDictionary<int, Unit>();
		public List<Actor> Actors = new List<Actor>();
		public List<Effect> Effects = new List<Effect>();
		public List<Message> Messages = new List<Message>();

		public string ChatBuffer = "";
		public bool IsChatting = false;
		public bool IsAllChatting = false;
		public bool IsShowingScoreboard = false;
		public double IsChattingScale = 0;
		public bool AllPlayersReady = false;

		public int? CurrentAbility = null;

		public double? StartTime = null;

		protected NetClient client;

		public Client(Game game, bool isLocalServer) {
			Game = game;
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
				Console.WriteLine("Connected successfully.");
				IsConnected = true;
			}
			if (IsConnected && client.ConnectionStatus == NetConnectionStatus.Disconnected) {
				Console.WriteLine("Disconnected from server.");
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
						case PacketType.ChangeRole:
							ReceiveChangeRole((int)incoming.ReadByte(), (Roles)incoming.ReadByte());
							break;
						case PacketType.Ready:
							ReceiveReady((int)incoming.ReadByte(), (bool)(incoming.ReadByte() > 0));
							break;
						case PacketType.StartLoading:
							ReceiveStartLoading();
							break;
						case PacketType.LoadingPercent:
							ReceiveLoadingPercent((int)incoming.ReadByte(), incoming.ReadFloat());
							break;
						case PacketType.StartMatch:
							ReceiveStartMatch(incoming.ReadTime(true) - incoming.ReceiveTime);
							break;
					}
				}
				client.Recycle(incoming);
			}
		}

		public void Connect() {
			if (IsLocalServer) {
				IsConnected = true;
				Server.Local.AddPlayer(Arena.Config.PlayerName, Arena.Config.PlayerNumber, Teams.Neutral, Roles.Runner);
				Game.Cmd.Console.WriteLine("Connected to local single-player server.");
			}
			else
				throw new Exception("Can't local-connect to remote server.");
		}
		public void Connect(string address) {
			if (!IsLocalServer) {
				Game.Cmd.Console.WriteLine("Connecting to server... ");
				NetOutgoingMessage msg = client.CreateMessage();
				client.Start();
				msg.Write((byte)PacketType.Connect);
				msg.Write(Arena.Config.PlayerName);
				msg.Write((byte)Arena.Config.PlayerNumber);
				/* TODO: Use these for reconnecting
				msg.Write((byte)Teams.Home);
				msg.Write((byte)Roles.Runner);
				*/
				client.Connect(address, Arena.Config.Port, msg);
			}
			else
				throw new Exception("Can't remote-connect to local server.");
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
			Game.Cmd.Console.WriteLine("Recieving new player: " + name + " | " + number + " | " + (Teams)team + " | " + (Roles)role);
			Messages.Add(new Message(string.Format("{0} has connected.", name)));
			if (Players.ContainsKey(index))
				Players.Remove(index);
			Players.Add(index, new Player(name, number, (Teams)team, (Roles)role));
			if (Players.Count == 1) {
				Game.Cmd.Console.WriteLine("I didn't have any other players, so this new player is the local player.");
				LocalPlayer = Players[index];
			}
			Game.Cmd.Console.WriteLine("I now have " + Players.Count + " players.");
		}
		public void ReceiveNewPlayerUnit(int unitIndex, int playerIndex, float x, float y, double direction) {
			Game.Cmd.Console.WriteLine("Recieving new player unit for " + Players[playerIndex].Name + " at (" + x + ", " + y + ")");
			Player player = Players[playerIndex];
			Unit u = new Unit(player, Role.List[player.Role].Health, Role.List[player.Role].Energy);
			u.Owner = player;
			u.Team = player.Team;
			u.JumpTo(new Vector2(x, y));
			Role.SetUpUnit(ref u, player.Role);
			player.PlayerUnit = u;

			VGame.Shape shape = Role.MakeShape(player.Role);
			u.Actor = new Actor(u, shape);
			u.Actor.Unit = u;

			Actors.Add(u.Actor);
			Units.Add(unitIndex, u);
			player.ControlledUnits.Add(u);
			if (player.ControlledUnits.Count == 0) {
				player.PlayerUnit = u;
				player.CurrentUnit = u;
			}
		}
		public void SendAttackOrder(Unit unit) {
			LocalPlayer.CurrentUnit.AttackTarget = unit;
			if (IsLocalServer) {
				Server.Local.ReceiveAttackOrder(Units.FirstOrDefault(x => x.Value == LocalPlayer.CurrentUnit).Key, Units.FirstOrDefault(x => x.Value == unit).Key);
			}
			else {
				Game.Cmd.Console.WriteLine("Sending attack order: " + LocalPlayer.Name + " -> " + unit.Owner.Name);
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.AttackOrder);
				msg.Write(GetUnitID(LocalPlayer.CurrentUnit));
				msg.Write(GetUnitID(unit));
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void SendMoveOrder(Vector2 position) {
			Game.Cmd.Console.WriteLine("Sending move order for unit " + GetUnitID(LocalPlayer.CurrentUnit) + " to (" + position.X + ", " + position.Y + ")");
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
			Game.Cmd.Console.WriteLine("Receiving attack order: " + Units[attackerIndex].Owner.Name + " -> " + Units[victimIndex].Owner.Name);
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
				Game.Cmd.Console.WriteLine("Sending level up for player " + LocalPlayer.Name);
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
			Game.Cmd.Console.WriteLine("Receiving level up for " + Units[unitIndex].Owner.Name);
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
				Game.Cmd.Console.WriteLine("Sending ability use for " + LocalPlayer.Name);
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
			Game.Cmd.Console.WriteLine("Receiving ability use for " + Units[unitIndex].Owner.Name + ", ability " + ability);
			Units[unitIndex].UseAbility(ability, val1, val2);
		}
		public void ReceiveDisconnect(int playerIndex) {
			Game.Cmd.Console.WriteLine("Receiving player disconnect for " + Players[playerIndex]);
			Messages.Add(new Message(string.Format("{0} has disconnected.", Players[playerIndex].Name)));
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
			Game.Cmd.Console.WriteLine(string.Format("{0}: {1}", Players[playerIndex].Name, message));
			Messages.Add(new Message(Players[playerIndex].Name, message, Teams.Neutral));
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
			Game.Cmd.Console.WriteLine(string.Format("{0} ({1}): {2}", Players[playerIndex].Name, Players[playerIndex].Team, message));
			Messages.Add(new Message(Players[playerIndex].Name, message, Players[playerIndex].Team));
		}
		public void ChangeTeam(Teams team) {
			if (LocalPlayer.Ready)
				return;
			Game.Cmd.Console.WriteLine("Moving to " + team);
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
			if (LocalPlayer.Ready)
				return;
			if (LocalPlayer.Team != Teams.Home && LocalPlayer.Team != Teams.Away)
				return;
			Game.Cmd.Console.WriteLine("Changing role to " + role);
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
			Game.Cmd.Console.WriteLine("Setting READY to " + !LocalPlayer.Ready);
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
			ShowReadyMessage();
		}
		public void ReceiveReady(int playerIndex, bool ready) {
			Players[playerIndex].Ready = ready;
			ShowReadyMessage();
		}

		void ShowReadyMessage() {
			bool allPlayersReadyLast = AllPlayersReady;
			AllPlayersReady = Players.Where(x => x.Value.Ready == false).Count() == 0;
			if (AllPlayersReady && !allPlayersReadyLast)
				Messages.Add(new Message(string.Format("All players are ready! Loading begins in {0} seconds.", Arena.Config.ReadyCountdown)));
			if (!AllPlayersReady && allPlayersReadyLast)
				Messages.Add(new Message(string.Format("Somebody unreadied! Loading suspended.")));
		}

		public void ReceiveStartLoading() {
			Match = new Match();
		}
		public void SendLoadingPercent() {
			if (IsLocalServer) {
				Server.Local.ReceiveLoadingPercent(Server.Local.Players[GetPlayerID(LocalPlayer)], LocalPlayer.LoadingPercent);
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.LoadingPercent);
				msg.Write(LocalPlayer.LoadingPercent);
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveLoadingPercent(int playerIndex, float percent) {
			Players[playerIndex].LoadingPercent = percent;
		}
		public void DoneLoading() {
			if (LocalPlayer.Loaded)
				return;
			LocalPlayer.Loaded = true;
			if (IsLocalServer) {
				Server.Local.ReceiveLoaded(Server.Local.Players[GetPlayerID(LocalPlayer)]);
			}
			else {
				NetOutgoingMessage msg = client.CreateMessage();
				msg.Write((byte)PacketType.Loaded);
				client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}
		public void ReceiveStartMatch(double offset) {
			Game.Cmd.Console.WriteLine(string.Format("Received match start notification with an offset of {0}", offset));
			Messages.Add(new Message(string.Format("All players loaded, the game will begin in {0} seconds.", Arena.Config.PostLoadingCountdown)));
			StartTime = NetTime.Now + offset + Config.PostLoadingCountdown;
		}
		public bool HandleChatInput(InputManager inputManager) {
			if (IsChatting) {
				if (inputManager.KeyState(Keys.Tab) == ButtonState.Pressed) {
					string[] split = ChatBuffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (split.Length > 0) {
						List<Player> found = new List<Player>();
						foreach (KeyValuePair<int, Player> kvp in Players) {
							if (kvp.Value.Name.Length >= split[split.Length - 1].Length && kvp.Value.Name.Substring(0, split[split.Length - 1].Length).ToLower() == split[split.Length - 1].ToLower()) {
								found.Add(kvp.Value);
							}
						}
						if (found.Count == 1) {
							ChatBuffer = ChatBuffer.Substring(0, ChatBuffer.Length - split[split.Length - 1].Length);
							string toAdd = found[0].Name;
							if (ChatBuffer.Length == 0 && (IsAllChatting || found[0].Team == LocalPlayer.Team))
								toAdd += ": ";
							else
								toAdd += " ";
							ChatBuffer += toAdd;
						}
					}
				}
				if (inputManager.KeyState(Keys.Backspace) == ButtonState.Pressed && ChatBuffer.Length > 0)
					ChatBuffer = ChatBuffer.Substring(0, ChatBuffer.Length - 1);
				foreach (char c in inputManager.GetTextInput()) {
					ChatBuffer = ChatBuffer + c;
				}
				if (inputManager.KeyState(Keys.Escape) == ButtonState.Pressed) {
					ChatBuffer = "";
					IsChatting = false;
				}
				if (inputManager.KeyState(Keys.Enter) == ButtonState.Pressed) {
					if (IsAllChatting)
						SendAllChat(ChatBuffer);
					else
						SendTeamChat(ChatBuffer);
					ChatBuffer = "";
					IsChatting = false;
				}
				return true;
			}
			else {
				if (inputManager.KeyState(Keys.Enter) == ButtonState.Pressed) {
					if (inputManager.IsShiftKeyDown) {
						// All chat
						IsAllChatting = true;
					}
					else {
						// Team chat
						IsAllChatting = false;
					}
					IsChatting = true;
				}
			}
			return false;
		}
		public void DrawChat(Renderer renderer, Vector2 position, int entries) {
			string font = "04b25";
			int textSize = 12;

			renderer.Context.SelectFontFace(font, Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
			renderer.Context.SetFontSize(textSize);
			float chatHeight = (float)renderer.Context.FontExtents.Height;

			for (int i = 0; i < Math.Min(Client.Local.Messages.Count, entries); i++) {
				Message msg = Client.Local.Messages[Client.Local.Messages.Count - 1 - i];
				string str = msg.ToString();
				Cairo.Color? col = null;
				if (msg.Team == Teams.Home)
					col = Config.HomeColor2;
				if (msg.Team == Teams.Away)
					col = Config.AwayColor2;
				renderer.DrawText(position + new Vector2(0, -chatHeight * (float)((double)i + IsChattingScale)), str, 12, TextAlign.Left, TextAlign.Bottom, ColorPresets.White, ColorPresets.Black, col, 0, "04b25", 0);
			}
			if (Client.Local.IsChatting) {
				Cairo.Color? col = null;
				if (!Client.Local.IsAllChatting && Client.Local.LocalPlayer.Team == Teams.Home)
					col = Config.HomeColor2;
				if (!Client.Local.IsAllChatting && Client.Local.LocalPlayer.Team == Teams.Away)
					col = Config.AwayColor2;
				renderer.DrawText(position + new Vector2(0, -chatHeight * (float)((double)-1 + IsChattingScale)), "> " + Client.Local.ChatBuffer, 12, TextAlign.Left, TextAlign.Bottom, ColorPresets.White, ColorPresets.Black, col, 0, "04b25", 0);
			}
		}

		public void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
			Tick();
			if (Match != null && !Match.Started && StartTime.HasValue && NetTime.Now >= StartTime) {
				Match.Started = true;
				StartTime = null;
			}
			foreach (KeyValuePair<int, Unit> kvp in Units)
				kvp.Value.Update(gameTime);
			foreach (Actor a in Actors)
				a.Update(gameTime, viewPosition, viewOrigin);
			foreach (Effect e in Effects)
				e.Update(gameTime, viewPosition, viewOrigin);
			Effect.Cleanup(ref Effects);
			if (Client.Local.IsChatting)
				IsChattingScale = Math.Min(IsChattingScale + 0.4, 1);
			else
				IsChattingScale = Math.Max(IsChattingScale - 0.1, 0);
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

