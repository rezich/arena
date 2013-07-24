using System;
using System.Collections;
using System.Collections.Generic;
using VGame;
using Cairo;
using Arena;

namespace ArenaClient {
	public static class HUD {
		public static List<Vector2> LeftBox;
		public static List<Vector2> RightBox;
		public static List<List<Vector2>> Ability = new List<List<Vector2>>();
		public static List<Vector2> MinimapBackground;
		public static int BoxWidth;
		public static int Margin = 4;
		public static int Padding = 3;
		public static Vector2 CornerPadding;
		public static double BarWidth;
		public static double MinimapSize;
		public static double AbilitySize;
		public static double BarHeight;
		public static int LevelBoxSize = 10;
		public static int LevelBoxPadding = 4;
		public static int TextBoxPadding;
		public static List<string> TemporaryKeyList;

		public static Cairo.Color MainTextFill;
		public static Cairo.Color MainTextStroke;
		public static Cairo.Color AbilityNameBackground;
		public static Cairo.Color AbilityEnergyBackground;
		public static Cairo.Color AbilityCooldownBackground;
		public static Cairo.Color AbilityKeyBackground;

		public static double ScoreboardScale = 0;

		public static void Update(GameTime gameTime, Player player) {
			if (Client.Local.IsShowingScoreboard)
				ScoreboardScale = Math.Min(ScoreboardScale + 0.1, 1);
			else
				ScoreboardScale = Math.Max(ScoreboardScale - 0.025, 0);
		}
		public static void Draw(GameTime gameTime, Renderer renderer, Player player) {
			Context g = renderer.Context;
			if (player == null)
				return;

			// SCOREBOARD
			int sbTop = 3;
			if (ScoreboardScale > 0) {
				renderer.DrawText(new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), "HOME", 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, Config.HomeColor2, 0, "chunky");
				sbTop++;
				foreach (KeyValuePair<int, Player> kvp in Client.Local.Players) {
					if (kvp.Value.Team == Teams.Home) {
						renderer.DrawText(new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), kvp.Value.Name.ToUpper(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, "chunky");
						sbTop++;
					}
				}
				renderer.DrawText(new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), "AWAY", 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, Config.AwayColor2, 0, "chunky");
				sbTop++;
				foreach (KeyValuePair<int, Player> kvp in Client.Local.Players) {
					if (kvp.Value.Team == Teams.Away) {
						renderer.DrawText(new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), kvp.Value.Name.ToUpper(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, "chunky");
						sbTop++;
					}
				}
			}


			// LEFT
			DrawBox(renderer, LeftBox, Arena.Config.HUDBackground, null);

			// health background
			g.MoveTo(new Vector2(0 + Margin, 0 + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin + BarWidth), 0 + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin + BarWidth), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2(0 + Margin, renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(new Cairo.Color(0, 0, 0), null);

			// health foreground
			if (player.CurrentUnit != null) {
				g.MoveTo(new Vector2(0 + Margin, renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.GetHealthPercent())).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin + BarWidth), renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.GetHealthPercent())).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin + BarWidth), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.LineTo(new Vector2(0 + Margin, renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.ClosePath();
				renderer.StrokeAndFill(new Cairo.Color(0, 128, 0), null);
			}

			// energy background
			g.MoveTo(new Vector2((float)(0 + Margin * 2 + BarWidth), 0 + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin * 2 + BarWidth * 2), 0 + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin * 2 + BarWidth * 2), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin * 2 + BarWidth), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(new Cairo.Color(0, 0, 0), null);

			// energy foreground
			if (player.CurrentUnit != null) {
				g.MoveTo(new Vector2((float)(0 + Margin * 2 + BarWidth), renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.GetEnergyPercent())).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin * 2 + BarWidth * 2), renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.GetEnergyPercent())).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin * 2 + BarWidth * 2), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin * 2 + BarWidth), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.ClosePath();
				renderer.StrokeAndFill(new Cairo.Color(0, 0, 128), null);
			}

			// experience background
			g.MoveTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 2), 0 + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 3), 0 + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 3), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 2), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(new Cairo.Color(0, 0, 0), null);

			// experience foreground
			if (player.CurrentUnit != null) {
				g.MoveTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 2), renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.GetExperiencePercent())).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 3), renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.GetExperiencePercent())).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 3), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.LineTo(new Vector2((float)(0 + Margin * 3 + BarWidth * 2), renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.ClosePath();
				renderer.StrokeAndFill(new Cairo.Color(0, 128, 128), null);
			}

			// minimap
			DrawBox(renderer, MinimapBackground, new Cairo.Color(0, 0, 0), null);
			renderer.DrawText(MinimapBackground[0] + new Vector2((float)(MinimapSize / 2), (float)(MinimapSize / 2)), "MINIMAP", 14, TextAlign.Center, TextAlign.Middle, MainTextFill, MainTextStroke, null, 0, "chunky");

			// RIGHT
			DrawBox(renderer, RightBox, Arena.Config.HUDBackground, null);

			if (player.CurrentUnit != null) {
				DrawAbility(gameTime, renderer, player, 0);
				DrawAbility(gameTime, renderer, player, 2);
				DrawAbility(gameTime, renderer, player, 1);
				DrawAbility(gameTime, renderer, player, 3);
			}

			if (player.CurrentUnit != null) {
				renderer.DrawText(new Vector2(Margin, Margin), "MOVE SPEED: " + player.CurrentUnit.MoveSpeed.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, "chunky");
				renderer.DrawText(new Vector2(Margin, Margin + 14), "TURN SPEED: " + player.CurrentUnit.TurnSpeed.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, "chunky");
				renderer.DrawText(new Vector2(Margin, Margin + 28), "ATTACK SPEED: " + player.CurrentUnit.AttackSpeed.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, "chunky");
				renderer.DrawText(new Vector2(Margin, Margin + 42), "ATTACK RANGE: " + player.CurrentUnit.AttackRange.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, "chunky");

				Client.Local.DrawChat(renderer, new Vector2(BoxWidth + Margin, renderer.Height - Margin), 10);

				for (int i = 0; i < player.CurrentUnit.Buffs.Count; i++) {
					if (!player.CurrentUnit.Buffs[i].Hidden) {
						string str = (player.CurrentUnit.Buffs[i].Permanent ? "" : "  " + Math.Round(((double)(player.CurrentUnit.Buffs[i].ExpirationTime - gameTime.TotalGameTime).TotalMilliseconds) / (double)1000, 1).ToString().MakeDecimal());
						renderer.DrawText(new Vector2(renderer.Width - BoxWidth - Margin, renderer.Height -Margin - 20 * i), player.CurrentUnit.Buffs[i].Name + str, 14, TextAlign.Right, TextAlign.Bottom, MainTextFill, MainTextStroke, (player.CurrentUnit.Buffs[i].Type == BuffAlignment.Positive ? new Cairo.Color(0, 0.5, 0) : new Cairo.Color(0, 0, 0.5)), 0, "chunky");
					}
				}
			}
			//Renderer.DrawText(new Vector2(0 + BoxWidth + Margin, 0 + Margin), Renderer.FPS.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
			//DrawText(g, new Vector2(0 + BoxWidth + Margin, 0 + Margin + 20), "P " + Client.Local.Players.Count.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
			//DrawText(g, new Vector2(0 + BoxWidth + Margin, 0 + Margin + 40), "U " + Client.Local.Units.Count.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
			//DrawText(g, new Vector2(0 + BoxWidth + Margin, 0 + Margin + 60), "A " + Client.Local.Actors.Count.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
		}
		private static void DrawAbility(GameTime gameTime, Renderer renderer, Player player, int ability) {
			Context g = renderer.Context;
			DrawBox(renderer, Ability[ability], new Cairo.Color(0, 0, 0), null);
			if (player.CurrentUnit.Abilities[ability].Ready || player.CurrentUnit.Abilities[ability].ActivationType == AbilityActivationType.Passive) {
				if (player.CurrentUnit.Abilities[ability].Level > 0)
					if (player.CurrentUnit.Energy >= player.CurrentUnit.Abilities[ability].EnergyCost && player.CurrentUnit.Abilities[ability].ActivationType != AbilityActivationType.Passive)
						DrawBox(renderer, Ability[ability], new Cairo.Color(0, 0.9, 0), null);
					else
						DrawBox(renderer, Ability[ability], new Cairo.Color(0.25, 0.5, 0.25), null);
				else
					DrawBox(renderer, Ability[ability], new Cairo.Color(0.1, 0.2, 0.1), null);
			}
			else {
				DrawBox(renderer, Ability[ability], new Cairo.Color(0, 0, 0.9), null);
				Vector2 timerCenter = Ability[ability][0] + new Vector2((float)AbilitySize / 2);
				g.MoveTo(timerCenter.ToPointD());
				double angle = Math.Max(0, MathHelper.TwoPi - (player.CurrentUnit.Abilities[ability].ReadyTime - gameTime.TotalGameTime).TotalMilliseconds / player.CurrentUnit.Abilities[ability].Cooldown / 25 / MathHelper.TwoPi);
				double adjustedAngle = angle - MathHelper.PiOver2;
				double flip = angle % MathHelper.PiOver2;
				if (flip > MathHelper.PiOver4) {
					flip -= MathHelper.PiOver2;
					flip = Math.Abs(flip);
				}
				g.LineTo(timerCenter.AddLengthDir(AbilitySize / 2 / Math.Cos(flip), adjustedAngle).ToPointD());
				if (angle > 7 * MathHelper.PiOver4)
					g.LineTo(Ability[ability][0].ToPointD());
				if (angle > 5 * MathHelper.PiOver4)
					g.LineTo(Ability[ability][3].ToPointD());
				if (angle > 3 * MathHelper.PiOver4)
					g.LineTo(Ability[ability][2].ToPointD());
				if (angle > 1 * MathHelper.PiOver4)
					g.LineTo(Ability[ability][1].ToPointD());
				g.LineTo(timerCenter.AddLengthDir(AbilitySize / 2, 3 * MathHelper.PiOver2).ToPointD());
				//g.LineTo(timerCenter.AddLengthDir(AbilitySize / 2, 3 * MathHelper.PiOver2).ToPointD());
				g.ClosePath();
				if (player.CurrentUnit.Energy >= player.CurrentUnit.Abilities[ability].EnergyCost)
					renderer.StrokeAndFill(new Cairo.Color(0, 0.5, 0), null);
				else
					renderer.StrokeAndFill(new Cairo.Color(0.25, 0.4, 0.25), null);
			}
			renderer.DrawText(Ability[ability][0] + new Vector2(Padding, Padding), player.CurrentUnit.Abilities[ability].Name.ToUpper(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, AbilityNameBackground, 0, "chunky");
			if (player.CurrentUnit.Abilities[ability].ActivationType != AbilityActivationType.Passive) {
				renderer.DrawText(Ability[ability][1] + new Vector2(-Padding, Padding), TemporaryKeyList[ability], 19, TextAlign.Right, TextAlign.Top, MainTextFill, MainTextStroke, AbilityKeyBackground, 0, "chunky");
				renderer.DrawText(Ability[ability][2] + new Vector2(-Padding, -Padding), player.CurrentUnit.Abilities[ability].Cooldown.ToString(), 14, TextAlign.Right, TextAlign.Bottom, MainTextFill, MainTextStroke, AbilityCooldownBackground, 0, "chunky");
				renderer.DrawText(Ability[ability][3] + new Vector2(Padding, -Padding), player.CurrentUnit.Abilities[ability].EnergyCost.ToString(), 14, TextAlign.Left, TextAlign.Bottom, MainTextFill, MainTextStroke, AbilityEnergyBackground, 0, "chunky");
			}
			if (!player.CurrentUnit.Abilities[ability].Ready && player.CurrentUnit.Abilities[ability].ActivationType != AbilityActivationType.Passive) {
				string str = Math.Round(((double)(player.CurrentUnit.Abilities[ability].ReadyTime - gameTime.TotalGameTime).TotalMilliseconds) / (double)1000, 1).ToString().MakeDecimal();
				renderer.DrawText(Ability[ability][0] + new Vector2((float)(AbilitySize / 2), (float)(AbilitySize / 2)), str, 32, TextAlign.Center, TextAlign.Middle, MainTextFill, MainTextStroke, null, 0, "chunky");
			}
			Vector2 levelOrigin = Ability[ability][1] + new Vector2(-Padding - LevelBoxSize, (float)((AbilitySize / 2) - (LevelBoxSize * player.CurrentUnit.Abilities[ability].Levels) / 2 - (LevelBoxPadding * (player.CurrentUnit.Abilities[ability].Levels) / 2) + LevelBoxPadding / 2));
			for (var i = 0; i < player.CurrentUnit.Abilities[ability].Levels; i++) {
				DrawBox(renderer, new List<Vector2>() {
					levelOrigin + new Vector2(0, LevelBoxSize * i + LevelBoxPadding * i),
					levelOrigin + new Vector2(LevelBoxSize, LevelBoxSize * i + LevelBoxPadding * i),
					levelOrigin + new Vector2(LevelBoxSize, LevelBoxSize * i + LevelBoxSize + LevelBoxPadding * i),
					levelOrigin + new Vector2(0, LevelBoxSize * i + LevelBoxSize + LevelBoxPadding * i)
				}, (player.CurrentUnit.Abilities[ability].Level >= i + 1 ? new Cairo.Color(1, 1, 1) : new Cairo.Color(0, 0, 0)), new Cairo.Color(1, 1, 1));
			}
		}
		private static void DrawBox(Renderer renderer, List<Vector2> points, Cairo.Color? fillColor, Cairo.Color? strokeColor) {
			renderer.Context.MoveTo(points[0].ToPointD());
			renderer.Context.LineTo(points[1].ToPointD());
			renderer.Context.LineTo(points[2].ToPointD());
			renderer.Context.LineTo(points[3].ToPointD());
			renderer.Context.ClosePath();
			renderer.StrokeAndFill(fillColor, strokeColor);
		}
		public static void Recalculate(Renderer Renderer) {
			MainTextFill = new Cairo.Color(1, 1, 1);
			MainTextStroke = new Cairo.Color(0, 0, 0);
			AbilityNameBackground = new Cairo.Color(0.5, 0.5, 0.5);
			AbilityEnergyBackground = new Cairo.Color(0, 0, 0.9);
			AbilityCooldownBackground = new Cairo.Color(0.9, 0, 0.9);
			AbilityKeyBackground = new Cairo.Color(0, 0.4, 0.4);

			TextBoxPadding = Padding;
			BoxWidth = Renderer.Width / 10;
			BarWidth = (BoxWidth - 4 * Margin) / 3;
			MinimapSize = (BoxWidth - 2 * Margin);
			AbilitySize = MinimapSize;
			BarHeight = Renderer.Height - Margin * 4 - MinimapSize * 2;
			CornerPadding = new Vector2(Padding, Padding);
			Ability.Clear();
			TemporaryKeyList = new List<string>() { "Q", "W", "E", "R" };
			LeftBox = new List<Vector2>() {
				new Vector2(0, 0),
				new Vector2(BoxWidth, 0),
				new Vector2(BoxWidth, Renderer.Height),
				new Vector2(0, Renderer.Height)
			};
			RightBox = new List<Vector2>() {
				new Vector2(Renderer.Width, 0),
				new Vector2(Renderer.Width - BoxWidth, 0),
				new Vector2(Renderer.Width - BoxWidth, Renderer.Height),
				new Vector2(Renderer.Width, Renderer.Height)
			};
			MinimapBackground = new List<Vector2>() {
				new Vector2((float)(0 + Margin), (float)(Renderer.Height - Margin - MinimapSize)),
				new Vector2((float)(0 + Margin), (float)(Renderer.Height - Margin)),
				new Vector2((float)(0 + Margin + MinimapSize), (float)(Renderer.Height - Margin)),
				new Vector2((float)(0 + Margin + MinimapSize), (float)(Renderer.Height - Margin - MinimapSize))
			};
			Ability.Add(new List<Vector2>() {
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 - 3 * Margin / 2 - AbilitySize * 2)),
				new Vector2((float)(Renderer.Width - Margin), (float)(Renderer.Height / 2 - 3 * Margin / 2 - AbilitySize * 2)),
				new Vector2(Renderer.Width - Margin, (float)(Renderer.Height / 2 - 3 * Margin / 2 - AbilitySize)),
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 - 3 * Margin / 2 - AbilitySize))
			});
			Ability.Add(new List<Vector2>() {
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 - Margin / 2 - AbilitySize)),
				new Vector2((float)(Renderer.Width - Margin), (float)(Renderer.Height / 2 - Margin / 2 - AbilitySize)),
				new Vector2(Renderer.Width - Margin, Renderer.Height / 2 - Margin / 2),
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 - Margin / 2))
			});
			Ability.Add(new List<Vector2>() {
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 + Margin / 2)),
				new Vector2(Renderer.Width - Margin, Renderer.Height / 2 + Margin / 2),
				new Vector2((float)(Renderer.Width - Margin), (float)(Renderer.Height / 2 + Margin / 2 + AbilitySize)),
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 + Margin / 2 + AbilitySize))
			});
			Ability.Add(new List<Vector2>() {
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 + 3 * Margin / 2 + AbilitySize)),
				new Vector2(Renderer.Width - Margin, (float)(Renderer.Height / 2 + 3 * Margin / 2 + AbilitySize)),
				new Vector2((float)(Renderer.Width - Margin), (float)(Renderer.Height / 2 + 3 * Margin / 2 + AbilitySize * 2)),
				new Vector2((float)(Renderer.Width - Margin - AbilitySize), (float)(Renderer.Height / 2 + 3 * Margin / 2 + AbilitySize * 2))
			});
		}
	}
}

