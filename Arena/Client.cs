using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Arena {
	public class Client {
		public static Client Local;
		public Player LocalPlayer;
		public bool IsConnected = false;
		public bool IsLocalServer = false;

		public Dictionary<int, Player> Players = new Dictionary<int, Player>();
		public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
		public List<Actor> Actors = new List<Actor>();
		public List<Effect> Effects = new List<Effect>();

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
		public void RecieveNewPlayerUnit(int unitIndex, int playerIndex, float x, float y, double direction) {
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
				Server.Local.RecieveAttackOrder(Units.FirstOrDefault(x => x.Value == LocalPlayer.CurrentUnit).Key, Units.FirstOrDefault(x => x.Value == unit).Key);
			}
		}
		public void SendMoveOrder(Vector2 position) {
			LocalPlayer.CurrentUnit.AttackTarget = null;
			LocalPlayer.CurrentUnit.IntendedPosition = position;
			if (IsLocalServer) {
				Server.Local.RecieveMoveOrder(Units.FirstOrDefault(x => x.Value == LocalPlayer.CurrentUnit).Key, position.X, position.Y);
			}
		}
		public void RecieveAttackOrder(int attackerIndex, int victimIndex) {
			Units[attackerIndex].AttackTarget = Units[victimIndex];
		}
		public void RecieveMoveOrder(int unitIndex, float x, float y) {
			Units[unitIndex].IntendedPosition = new Vector2(x, y);
		}
		public void MakeEffect(Effect effect) {
			Effects.Add(effect);
		}

		public void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
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

