using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cairo;

namespace VGame {
	public static class Extensions {
		public static PointD ToPointD(this Vector2 v) {
			return new PointD(v.X, v.Y);
		}
	}
	public static class SpriteBatchHelper {
		public static Texture2D Blank;
		public static bool IsInitialized = false;
		public static void Initialize(GraphicsDevice graphicsDevice) {
			Blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
			Blank.SetData(new[] { Microsoft.Xna.Framework.Color.White });
			IsInitialized = true;
		}
	}
}

