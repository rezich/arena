using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cairo;

namespace VGame {
	public static class Extensions {
		public static PointD ToPointD(this Vector2 v) {
			return new PointD(v.X, v.Y);
		}
		public static Vector2 AddLengthDir(this Vector2 v, double length, double dir) {
			return v + new Vector2((float)(Math.Cos(dir) * length), (float)(Math.Sin(dir) * length));
		}
		public static double LerpAngle(this double from, double to, double step) {
			// Ensure that 0 <= angle < 2pi for both "from" and "to" 
			while (from < 0)
				from += MathHelper.TwoPi;
			while (from >= MathHelper.TwoPi)
				from -= MathHelper.TwoPi;

			while (to < 0)
				to += MathHelper.TwoPi;
			while (to >= MathHelper.TwoPi)
				to -= MathHelper.TwoPi;

			if (System.Math.Abs(from - to) < MathHelper.Pi) {
				// The simple case - a straight lerp will do. 
				return (double)MathHelper.Lerp((float)from, (float)to, (float)step);
			}

			// If we get here we have the more complex case. 
			// First, increment the lesser value to be greater. 
			if (from < to)
				from += MathHelper.TwoPi;
			else
				to += MathHelper.TwoPi;

			float retVal = MathHelper.Lerp((float)from, (float)to, (float)step);

			// Now ensure the return value is between 0 and 2pi 
			if (retVal >= MathHelper.TwoPi)
				retVal -= MathHelper.TwoPi;
			return retVal;
		}
		public static string MakeDecimal(this string str) {
			if (str.Length < 2 || str.Substring(str.Length - 2, 1) != ".") {
				str += ".0";
			}
			return str;
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
	/*public class Tuple<T1, T2> {
		public T1 Item1 { get; set; }
		public T2 Item2 { get; set; }

		public Tuple(T1 item1, T2 item2) {
			Item1 = item1;
			Item2 = item2;
		}
	}
	public class Tuple<T1, T2, T3> {
		public T1 Item1 { get; set; }
		public T2 Item2 { get; set; }
		public T3 Item3 { get; set; }
		public Tuple(T1 item1, T2 item2, T3 item3) {
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
		}
	}*/
}

