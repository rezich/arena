using System;

namespace Arena {
	public struct Message {
		public string Contents;
		public string Sender;
		public DateTime Timestamp;
		public Teams Team;
		public MessageType Type;
		public Message(string sender, string contents, Teams team) {
			Sender = sender;
			Contents = contents;
			Team = team;
			Timestamp = DateTime.UtcNow;
			Type = MessageType.Chat;
		}
		public Message(string contents) {
			Sender = null;
			Contents = contents;
			Team = Teams.None;
			Timestamp = DateTime.UtcNow;
			Type = MessageType.System;
		}
		public Message(string contents, MessageType type) {
			Sender = null;
			Contents = contents;
			Team = Teams.None;
			Timestamp = DateTime.UtcNow;
			Type = type;
		}
		public override string ToString() {
			switch (Type) {
				case MessageType.Chat:
					return string.Format("<{0}> {1}", Sender, Contents);
				case MessageType.System:
					return string.Format("* {0} *", Contents);
			}
			return string.Format("[ChatMessage]");
		}
	}
	public enum MessageType {
		Chat,
		System
	}
}

