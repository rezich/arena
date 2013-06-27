using System;
using Cairo;
using VGame;

namespace Arena {
	public static class Config {
		public static Color HomeColor1, HomeColor2, AwayColor1, AwayColor2, HealthColor1, HealthColor2, EnemyHealthColor1, EnemyHealthColor2, EnergyColor1, EnergyColor2, HUDBackground, HUDText;
		public static double ActorScale = 24;
		public static Random Random = new Random();

		public static void Initialize() {
			HomeColor1 = VGame.Util.MakeColor(152, 174, 188, 1);
			HomeColor2 = VGame.Util.MakeColor(92, 123, 142, 1);
			AwayColor1 = VGame.Util.MakeColor(242, 159, 98, 1);
			AwayColor2 = VGame.Util.MakeColor(238, 120, 30, 1);
			HealthColor1 = VGame.Util.MakeColor(16, 128, 16, 0.1);
			HealthColor2 = VGame.Util.MakeColor(8, 128, 8, 0.15);
			EnemyHealthColor1 = HealthColor1; //VGame.Util.MakeColor(128, 16, 16, 0.2);
			EnemyHealthColor2 = HealthColor2; //VGame.Util.MakeColor(128, 8, 8, 0.25);
			EnergyColor1 = VGame.Util.MakeColor(32, 32, 128, 0.2);
			EnergyColor2 = VGame.Util.MakeColor(16, 16, 128, 0.25);
			HUDBackground = VGame.Util.MakeColor(48, 48, 48, 1);
			HUDText = VGame.Util.MakeColor(255, 255, 255, 1);
		}
	}
}

