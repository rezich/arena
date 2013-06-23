using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cairo;

namespace Arena {
	public static class Extensions {
		public static Vector2 AddLengthDir(this Vector2 v, double length, double dir) {
			return v + new Vector2((float)(Math.Cos(dir) * length), (float)(Math.Sin(dir) * length));
		}
		public static PointD ToPointD(this Vector2 v) {
			return new PointD(v.X, v.Y);
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
	}
}

