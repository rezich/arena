using System;
using System.Collections.Generic;

namespace Arena {
	public class GameState {
		public GameState() {
		}
	}
	public enum GameStateObject {
		Unit,
		Player
	}
}

// PacketType.GameState
// (Time)
// GameStateObject.Unit
// (int) count
// (Units)
// GameStateObject.Player
// (int) count
// (Players)
