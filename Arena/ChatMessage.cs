using System;

namespace Arena {
	public struct ChatMessage {
		public string Message;
		public string Sender;
		public DateTime Timestamp;
		public Teams Team;
		public ChatMessage(string sender, string message, Teams team) {
			Sender = sender;
			Message = message;
			Team = team;
			Timestamp = DateTime.UtcNow;
		}
	}
}

