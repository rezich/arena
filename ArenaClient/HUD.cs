using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VGame;
using Cairo;
using Arena;

namespace ArenaClient {
	public enum TextAlign {
		Left,
		Center,
		Right,
		Top,
		Middle,
		Bottom
	}
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
		
		public static double IsChattingScale = 0;
		public static double ScoreboardScale = 0;

		public static void Update(GameTime gameTime, Player player) {
			if (Client.Local.IsChatting)
				IsChattingScale = Math.Min(IsChattingScale + 0.4, 1);
			else
				IsChattingScale = Math.Max(IsChattingScale - 0.1, 0);
			if (Client.Local.IsShowingScoreboard)
				ScoreboardScale = Math.Min(ScoreboardScale + 0.1, 1);
			else
				ScoreboardScale = Math.Max(ScoreboardScale - 0.025, 0);
		}
		public static void Draw(GameTime gameTime, Cairo.Context g, Player player) {

			if (player == null)
				return;

			// SCOREBOARD
			int sbTop = 3;
			if (ScoreboardScale > 0) {
				DrawText(g, new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), "HOME", 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, Config.HomeColor2, 0, null);
				sbTop++;
				foreach (KeyValuePair<int, Player> kvp in Client.Local.Players) {
					if (kvp.Value.Team == Teams.Home) {
						DrawText(g, new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), kvp.Value.Name.ToUpper(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
						sbTop++;
					}
				}
				DrawText(g, new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), "AWAY", 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, Config.AwayColor2, 0, null);
				sbTop++;
				foreach (KeyValuePair<int, Player> kvp in Client.Local.Players) {
					if (kvp.Value.Team == Teams.Away) {
						DrawText(g, new Vector2(BoxWidth + Margin - 400 + (float)(400 * ScoreboardScale), Margin + sbTop * 22), kvp.Value.Name.ToUpper(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
						sbTop++;
					}
				}
			}


			// LEFT
			DrawBox(g, LeftBox, Arena.Config.HUDBackground, null);

			// health background
			g.MoveTo(new Vector2(Resolution.Left + Margin, Resolution.Top + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin + BarWidth), Resolution.Top + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin + BarWidth), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2(Resolution.Left + Margin, Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// health foreground
			if (player.CurrentUnit != null) {
				g.MoveTo(new Vector2(Resolution.Left + Margin, Renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.HealthPercent)).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin + BarWidth), Renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.HealthPercent)).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin + BarWidth), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.LineTo(new Vector2(Resolution.Left + Margin, Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.ClosePath();
				VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 128, 0), null);
			}

			// energy background
			g.MoveTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth), Resolution.Top + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth * 2), Resolution.Top + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth * 2), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// energy foreground
			if (player.CurrentUnit != null) {
				g.MoveTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth), Renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.EnergyPercent)).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth * 2), Renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.EnergyPercent)).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth * 2), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin * 2 + BarWidth), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.ClosePath();
				VGame.Util.StrokeAndFill(g, new Cairo.Color(128, 0, 0), null);
			}

			// experience background
			g.MoveTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 2), Resolution.Top + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 3), Resolution.Top + Margin * 2 + (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 3), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 2), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// experience foreground
			if (player.CurrentUnit != null) {
				g.MoveTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 2), Renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.ExperiencePercent)).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 3), Renderer.Height - Margin * 2 - (float)MinimapSize - (float)(BarHeight * player.CurrentUnit.ExperiencePercent)).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 3), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.LineTo(new Vector2((float)(Resolution.Left + Margin * 3 + BarWidth * 2), Renderer.Height - Margin * 2 - (float)MinimapSize).ToPointD());
				g.ClosePath();
				VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 128, 128), null);
			}

			// minimap
			DrawBox(g, MinimapBackground, new Cairo.Color(0, 0, 0), null);
			DrawText(g, MinimapBackground[0] + new Vector2((float)(MinimapSize / 2), (float)(MinimapSize / 2)), "MINIMAP", 14, TextAlign.Center, TextAlign.Middle, MainTextFill, MainTextStroke, null, 0, null);

			// RIGHT
			DrawBox(g, RightBox, Arena.Config.HUDBackground, null);

			if (player.CurrentUnit != null) {
				DrawAbility(gameTime, g, player, 0);
				DrawAbility(gameTime, g, player, 1);
				DrawAbility(gameTime, g, player, 2);
				DrawAbility(gameTime, g, player, 3);
			}

			if (player.CurrentUnit != null) {
				DrawText(g, new Vector2(Margin, Margin), "MOVE SPEED: " + player.CurrentUnit.MoveSpeed.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
				DrawText(g, new Vector2(Margin, Margin + 14), "TURN SPEED: " + player.CurrentUnit.TurnSpeed.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
				DrawText(g, new Vector2(Margin, Margin + 28), "ATTACK SPEED: " + player.CurrentUnit.AttackSpeed.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
				DrawText(g, new Vector2(Margin, Margin + 42), "ATTACK RANGE: " + player.CurrentUnit.AttackRange.ToString(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
				for (int i = 0; i < Math.Min(Client.Local.ChatMessages.Count, 10); i++) {
					ChatMessage msg = Client.Local.ChatMessages[Client.Local.ChatMessages.Count - 1 - i];
					string str = "<" + msg.Sender.ToUpper() + "> " + msg.Message.ToUpper();
					Cairo.Color? col = null;
					if (msg.Team == Teams.Home)
						col = Config.HomeColor2;
					if (msg.Team == Teams.Away)
						col = Config.AwayColor2;
					DrawText(g, new Vector2(BoxWidth + Margin, Renderer.Height - Margin - 20 * (float)((double)i + IsChattingScale)), str, 14, TextAlign.Left, TextAlign.Bottom, MainTextFill, MainTextStroke, col, 0, null);
				}
				if (Client.Local.IsChatting) {
					Cairo.Color? col = null;
					if (!Client.Local.IsAllChatting && Client.Local.LocalPlayer.Team == Teams.Home)
						col = Config.HomeColor2;
					if (!Client.Local.IsAllChatting && Client.Local.LocalPlayer.Team == Teams.Away)
						col = Config.AwayColor2;
					DrawText(g, new Vector2(BoxWidth + Margin, Renderer.Height - Margin - 20 * (float)((double)-1 + IsChattingScale)), "> " + Client.Local.ChatBuffer.ToUpper(), 14, TextAlign.Left, TextAlign.Bottom, MainTextFill, MainTextStroke, col, 0, null);
				}
				for (int i = 0; i < player.CurrentUnit.Buffs.Count; i++) {
					if (!player.CurrentUnit.Buffs[i].Hidden) {
						string str = (player.CurrentUnit.Buffs[i].Permanent ? "" : "  " + Math.Round(((double)(player.CurrentUnit.Buffs[i].ExpirationTime - gameTime.TotalGameTime).TotalMilliseconds) / (double)1000, 1).ToString().MakeDecimal());
						DrawText(g, new Vector2(Renderer.Width - BoxWidth - Margin, Renderer.Height -Margin - 20 * i), player.CurrentUnit.Buffs[i].Name + str, 14, TextAlign.Right, TextAlign.Bottom, MainTextFill, MainTextStroke, (player.CurrentUnit.Buffs[i].Type == BuffAlignment.Positive ? new Cairo.Color(0, 0.5, 0) : new Cairo.Color(0, 0, 0.5)), 0, null);
					}
				}
			}
			DrawText(g, new Vector2(Resolution.Left + BoxWidth + Margin, Resolution.Top + Margin), Renderer.FPS.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
			//DrawText(g, new Vector2(Resolution.Left + BoxWidth + Margin, Resolution.Top + Margin + 20), "P " + Client.Local.Players.Count.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
			//DrawText(g, new Vector2(Resolution.Left + BoxWidth + Margin, Resolution.Top + Margin + 40), "U " + Client.Local.Units.Count.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
			//DrawText(g, new Vector2(Resolution.Left + BoxWidth + Margin, Resolution.Top + Margin + 60), "A " + Client.Local.Actors.Count.ToString(), 20, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, null, 0, null);
		}
		private static void DrawAbility(GameTime gameTime, Cairo.Context g, Player player, int ability) {
			DrawBox(g, Ability[ability], new Cairo.Color(0, 0, 0), null);
			if (player.CurrentUnit.Abilities[ability].Ready || player.CurrentUnit.Abilities[ability].ActivationType == AbilityActivationType.Passive) {
				if (player.CurrentUnit.Abilities[ability].Level > 0)
					if (player.CurrentUnit.Energy >= player.CurrentUnit.Abilities[ability].EnergyCost && player.CurrentUnit.Abilities[ability].ActivationType != AbilityActivationType.Passive)
						DrawBox(g, Ability[ability], new Cairo.Color(0, 0.9, 0), null);
					else
						DrawBox(g, Ability[ability], new Cairo.Color(0.25, 0.5, 0.25), null);
				else
					DrawBox(g, Ability[ability], new Cairo.Color(0.1, 0.2, 0.1), null);
			}
			else {
				DrawBox(g, Ability[ability], new Cairo.Color(0, 0, 0.9), null);
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
					VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0.5, 0), null);
				else
					VGame.Util.StrokeAndFill(g, new Cairo.Color(0.25, 0.4, 0.25), null);
			}
			DrawText(g, Ability[ability][0] + new Vector2(Padding, Padding), player.CurrentUnit.Abilities[ability].Name.ToUpper(), 14, TextAlign.Left, TextAlign.Top, MainTextFill, MainTextStroke, AbilityNameBackground, 0, null);
			if (player.CurrentUnit.Abilities[ability].ActivationType != AbilityActivationType.Passive) {
				DrawText(g, Ability[ability][1] + new Vector2(-Padding, Padding), TemporaryKeyList[ability], 19, TextAlign.Right, TextAlign.Top, MainTextFill, MainTextStroke, AbilityKeyBackground, 0, null);
				DrawText(g, Ability[ability][2] + new Vector2(-Padding, -Padding), player.CurrentUnit.Abilities[ability].Cooldown.ToString(), 14, TextAlign.Right, TextAlign.Bottom, MainTextFill, MainTextStroke, AbilityCooldownBackground, 0, null);
				DrawText(g, Ability[ability][3] + new Vector2(Padding, -Padding), player.CurrentUnit.Abilities[ability].EnergyCost.ToString(), 14, TextAlign.Left, TextAlign.Bottom, MainTextFill, MainTextStroke, AbilityEnergyBackground, 0, null);
			}
			if (!player.CurrentUnit.Abilities[ability].Ready && player.CurrentUnit.Abilities[ability].ActivationType != AbilityActivationType.Passive) {
				string str = Math.Round(((double)(player.CurrentUnit.Abilities[ability].ReadyTime - gameTime.TotalGameTime).TotalMilliseconds) / (double)1000, 1).ToString().MakeDecimal();
				DrawText(g, Ability[ability][0] + new Vector2((float)(AbilitySize / 2), (float)(AbilitySize / 2)), str, 32, TextAlign.Center, TextAlign.Middle, MainTextFill, MainTextStroke, null, 0, null);
			}
			Vector2 levelOrigin = Ability[ability][1] + new Vector2(-Padding - LevelBoxSize, (float)((AbilitySize / 2) - (LevelBoxSize * player.CurrentUnit.Abilities[ability].Levels) / 2 - (LevelBoxPadding * (player.CurrentUnit.Abilities[ability].Levels) / 2) + LevelBoxPadding / 2));
			for (var i = 0; i < player.CurrentUnit.Abilities[ability].Levels; i++) {
				DrawBox(g, new List<Vector2>() {
					levelOrigin + new Vector2(0, LevelBoxSize * i + LevelBoxPadding * i),
					levelOrigin + new Vector2(LevelBoxSize, LevelBoxSize * i + LevelBoxPadding * i),
					levelOrigin + new Vector2(LevelBoxSize, LevelBoxSize * i + LevelBoxSize + LevelBoxPadding * i),
					levelOrigin + new Vector2(0, LevelBoxSize * i + LevelBoxSize + LevelBoxPadding * i)
				}, (player.CurrentUnit.Abilities[ability].Level >= i + 1 ? new Cairo.Color(1, 1, 1) : new Cairo.Color(0, 0, 0)), new Cairo.Color(1, 1, 1));
			}
		}
		private static void DrawText(Cairo.Context g, Vector2 position, string text, double scale, TextAlign hAlign, TextAlign vAlign, Cairo.Color? fillColor, Cairo.Color? strokeColor, Cairo.Color? backgroundColor, double angle, string font) {
			if (font == null)
				font = "04b_19";
			g.Save();
			g.SelectFontFace(font, FontSlant.Normal, FontWeight.Normal);
			g.SetFontSize(scale);
			TextExtents ext = g.TextExtents(text);
			TextExtents ext2 = g.TextExtents("|");
			Vector2 offset = new Vector2(0, 0);
			switch (hAlign) {
				case TextAlign.Left:
					break;
				case TextAlign.Center:
					offset.X = -(float)(ext.Width / 2);
					break;
				case TextAlign.Right:
					offset.X = -(float)(ext.Width);
					break;
			}
			switch (vAlign) {
				case TextAlign.Top:
					break;
					case TextAlign.Middle:
					offset.Y = -(float)(ext2.Height / 2);
					break;
					case TextAlign.Bottom:
					offset.Y = -(float)(ext2.Height);
					break;
			}
			Vector2 textPos = position - new Vector2((float)(ext.XBearing), (float)(ext2.YBearing)) + offset;
			Vector2 boxOffset = new Vector2((float)(ext.XBearing), (float)(-ext2.Height));
			if (backgroundColor.HasValue) {
				g.MoveTo((textPos + boxOffset + new Vector2(-TextBoxPadding, -TextBoxPadding)).ToPointD());
				g.LineTo((textPos + boxOffset + new Vector2((float)ext.Width, 0) + new Vector2(TextBoxPadding, -TextBoxPadding)).ToPointD());
				g.LineTo((textPos + boxOffset + new Vector2((float)ext.Width, (float)ext.Height) + new Vector2(TextBoxPadding, TextBoxPadding)).ToPointD());
				g.LineTo((textPos + boxOffset + new Vector2(0, (float)ext.Height) + new Vector2(-TextBoxPadding, TextBoxPadding)).ToPointD());
				g.ClosePath();
				g.Color = (Cairo.Color)backgroundColor;
				g.Fill();
			}
			if (fillColor.HasValue) {
				g.MoveTo(textPos.ToPointD());
				g.Color = (Cairo.Color)fillColor;
				if (angle != 0) g.Rotate(angle);
				g.ShowText(text);
			}
			if (strokeColor.HasValue) {
				g.Antialias = Antialias.None;
				g.MoveTo(textPos.ToPointD());
				g.Color = (Cairo.Color)strokeColor;
				g.LineWidth = 1;
				g.TextPath(text);
				if (angle != 0) g.Rotate(angle);
				g.Stroke();
			}
			g.Restore();
		}
		private static void DrawBox(Cairo.Context g, List<Vector2> points, Cairo.Color? fillColor, Cairo.Color? strokeColor) {
			g.MoveTo(points[0].ToPointD());
			g.LineTo(points[1].ToPointD());
			g.LineTo(points[2].ToPointD());
			g.LineTo(points[3].ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, fillColor, strokeColor);
		}
		public static void Recalculate() {
			MainTextFill = new Cairo.Color(1, 1, 1);
			MainTextStroke = new Cairo.Color(0, 0, 0);
			AbilityNameBackground = new Cairo.Color(0.5, 0.5, 0.5);
			AbilityEnergyBackground = new Cairo.Color(0.9, 0, 0);
			AbilityCooldownBackground = new Cairo.Color(0.9, 0, 0.9);
			AbilityKeyBackground = new Cairo.Color(0.4, 0.4, 0);

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
				new Vector2((float)(Resolution.Left + Margin), (float)(Renderer.Height - Margin - MinimapSize)),
				new Vector2((float)(Resolution.Left + Margin), (float)(Renderer.Height - Margin)),
				new Vector2((float)(Resolution.Left + Margin + MinimapSize), (float)(Renderer.Height - Margin)),
				new Vector2((float)(Resolution.Left + Margin + MinimapSize), (float)(Renderer.Height - Margin - MinimapSize))
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

