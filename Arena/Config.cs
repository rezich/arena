using System;
using System.Collections.Generic;
using Cairo;
using VGame;

namespace Arena {
	public enum KeyCommand {
		Ability1,
		Ability2,
		Ability3,
		Ability4,
		ChatWheel,
		Chat,
		CenterView,
		UnlockCursor,
		ToggleAntialiasing,
		ToggleDoubleBuffering,
		CameraUp,
		CameraDown,
		CameraLeft,
		CameraRight,
		Scoreboard
	}
	public static class Config {
		public static Color HomeColor1, HomeColor2, AwayColor1, AwayColor2, NeutralColor1, NeutralColor2, HealthColor1, HealthColor2, EnemyHealthColor1, EnemyHealthColor2, EnergyColor1, EnergyColor2, HUDBackground, HUDText;
		public static double ActorScale = 24;
		public static Random Random = new Random();
		public static string ApplicationID {
			get {
				return "arena-moba";
			}
		}
		public static int Port = 14242;
		public static string PlayerName = "UNNAMED";
		public static int PlayerNumber = 0;
		public static string LastServerAddress = "localhost";
		public static bool Fullscreen = false;
		//public static bool Borderless = false;
		public static bool Antialiasing = true;

		public static int ReadyCountdown = 5;
		public static int PostLoadingCountdown = 5;
		public static VGame.Point Resolution = new VGame.Point(1280, 720);

		public static Dictionary<KeyCommand, Keys> KeyBindings = new Dictionary<KeyCommand, Keys>();

		public static List<string> BotNames = new List<string>() {
			"Stupid",
			"Dumb",
			"Lame",
			"Poopy",
			"Bad-At-Game",
			"Feeding",
			"Noob",
			"Crappy"
		};

		public static void Initialize() {
			HomeColor1 = VGame.Util.MakeColor(152, 174, 188, 1);
			HomeColor2 = VGame.Util.MakeColor(92, 123, 142, 1);
			AwayColor1 = VGame.Util.MakeColor(242, 159, 98, 1);
			AwayColor2 = VGame.Util.MakeColor(238, 120, 30, 1);
			NeutralColor1 = VGame.Util.MakeColor(192, 192, 192, 1);
			NeutralColor2 = VGame.Util.MakeColor(128, 128, 128, 1);
			HealthColor1 = VGame.Util.MakeColor(16, 128, 16, 0.1);
			HealthColor2 = VGame.Util.MakeColor(8, 128, 8, 0.15);
			EnemyHealthColor1 = HealthColor1; //VGame.Util.MakeColor(128, 16, 16, 0.2);
			EnemyHealthColor2 = HealthColor2; //VGame.Util.MakeColor(128, 8, 8, 0.25);
			EnergyColor1 = VGame.Util.MakeColor(32, 32, 128, 0.2);
			EnergyColor2 = VGame.Util.MakeColor(16, 16, 128, 0.25);
			HUDBackground = VGame.Util.MakeColor(48, 48, 48, 1);
			HUDText = VGame.Util.MakeColor(255, 255, 255, 1);

			KeyBindings.Add(KeyCommand.Ability1, Keys.Q);
			KeyBindings.Add(KeyCommand.Ability2, Keys.W);
			KeyBindings.Add(KeyCommand.Ability3, Keys.E);
			KeyBindings.Add(KeyCommand.Ability4, Keys.R);
			KeyBindings.Add(KeyCommand.ChatWheel, Keys.T);
			KeyBindings.Add(KeyCommand.Chat, Keys.Enter);
			KeyBindings.Add(KeyCommand.CenterView, Keys.Space);
			KeyBindings.Add(KeyCommand.UnlockCursor, Keys.F1);
			KeyBindings.Add(KeyCommand.ToggleAntialiasing, Keys.F2);
			KeyBindings.Add(KeyCommand.ToggleDoubleBuffering, Keys.F3);
			KeyBindings.Add(KeyCommand.CameraUp, Keys.Up);
			KeyBindings.Add(KeyCommand.CameraDown, Keys.Down);
			KeyBindings.Add(KeyCommand.CameraLeft, Keys.Left);
			KeyBindings.Add(KeyCommand.CameraRight, Keys.Right);
			KeyBindings.Add(KeyCommand.Scoreboard, Keys.Tab);
		}
	}
}

