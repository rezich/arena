using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cairo;

namespace VGame {
	public enum TextAlign {
		Left,
		Center,
		Right,
		Top,
		Middle,
		Bottom
	}
	public class VectorGameSession : Game {
		protected ScreenManager screenManager;
		GraphicsDeviceManager graphics;

		public VectorGameSession() {
			IsFixedTimeStep = false;
			graphics = new GraphicsDeviceManager(this);
			screenManager = new ScreenManager(this);
			Components.Add(screenManager);
			//System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1;
		}
		protected override void Initialize() {
			Resolution.Initialize(graphics);
			graphics.PreferMultiSampling = false;
			graphics.SynchronizeWithVerticalRetrace = false;
			int w = 1280;
			int h = 720;
			Resolution.Set(w, h, false);
			VGame.Renderer.Initialize(graphics.GraphicsDevice, w, h);
			if (!SpriteBatchHelper.IsInitialized)
				SpriteBatchHelper.Initialize(graphics.GraphicsDevice);
			base.Initialize();
		}

		protected override void Update(GameTime gameTime) {
			Renderer.ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
			if (Renderer.ElapsedTime >= 1000) {
				Renderer.FPS = Renderer.TotalFrames;
				Renderer.TotalFrames = 0;
				Renderer.ElapsedTime = 0;
			}
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			Renderer.TotalFrames++;
			base.Draw(gameTime);
		}
		public void DrawVectors(GameTime gameTime) {
			screenManager.SpriteBatch.Begin();
			screenManager.SpriteBatch.Draw(VGame.Renderer.DisplayRenderTarget, new Microsoft.Xna.Framework.Rectangle(0, 0, Resolution.Width, Resolution.Height), Microsoft.Xna.Framework.Color.White);
			screenManager.SpriteBatch.End();
		}
	}
	public static class Renderer {
		private static GraphicsDevice graphics;
		public static ImageSurface Surface;
		public static List<Texture2D> RenderTargets = new List<Texture2D>();
		public static Context Context;
		public static int Width;
		public static int Height;
		public static bool Initialized = false;
		public static int TotalFrames = 0;
		public static double ElapsedTime = 0;
		public static int FPS = 0;
		private static bool _doubleBuffered;
		public static int CurrentBuffer = 0;
		public static bool Antialiasing = true;
		public static bool DoubleBuffered {
			get {
				return _doubleBuffered;
			}
			set {
				if (value) {
					if (RenderTargets.Count == 1)
						RenderTargets.Add(new Texture2D(graphics, Width, Height));
				}
				else {
					if (RenderTargets.Count == 2) {
						RenderTargets[1].Dispose();
						RenderTargets.RemoveAt(1);
						CurrentBuffer = 0;
					}
				}
				_doubleBuffered = value;
			}
		}
		public static Texture2D DisplayRenderTarget {
			get {
				if (DoubleBuffered)
					return RenderTargets[Renderer.CurrentBuffer == 0 ? 1 : 0];
				else
					return RenderTargets[0];
			}
		}
		public static Texture2D DrawRenderTarget {
			get {
				if (DoubleBuffered)
					return RenderTargets[Renderer.CurrentBuffer];
				else
					return RenderTargets[0];
			}
		}
		public static void Initialize(GraphicsDevice graphicsDevice, int width, int height) {
			graphics = graphicsDevice;
			Resize(width, height);
			Initialized = true;
			DoubleBuffered = true;
		}
		public static void Resize(int width, int height) {
			Width = width;
			Height = height;
			if (Context != null) ((IDisposable)Context).Dispose();
			if (Surface != null) ((IDisposable)Surface).Dispose();
			Surface = new Cairo.ImageSurface(Cairo.Format.Rgb24, Width, Height);
			Context = new Cairo.Context(Surface);
			EnableAntialiasing(Context);
			RenderTargets.Clear();
			RenderTargets.Add(new Texture2D(graphics, Width, Height));
			RenderTargets.Add(new Texture2D(graphics, Width, Height));
		}
		public static void EnableAntialiasing(Cairo.Context g) {
			g.Antialias = Antialiasing ? Cairo.Antialias.Subpixel : Cairo.Antialias.None;
		}
		public static void BeginDrawing() {
			if (!Initialized)
				throw new Exception("lolwat");
			Context.SetSourceRGB(0.83, 0.83, 0.83);
			Context.Paint();
		}
		public static void EndDrawing() {
			DrawRenderTarget.SetData(Surface.Data);
			if (DoubleBuffered)
				CurrentBuffer = CurrentBuffer == 0 ? 1 : 0;
			else
				CurrentBuffer = 0;
		}
		public static void Dispose() {
			if (!Initialized)
				return;
			((IDisposable)Context).Dispose();
		}
	}
	public static class Util {
		public static int TextBoxPadding = 4;
		public static Cairo.Color MakeColor(int r, int g, int b, double a) {
			return new Cairo.Color((double)b / 256, (double)g / 256, (double)r / 256, a);
		}
		public static void StrokeAndFill(Context g, Cairo.Color? fillColor, Cairo.Color? strokeColor) {
			if (fillColor.HasValue && fillColor != null) {
				g.Color = (Cairo.Color)fillColor;
				if (strokeColor.HasValue && fillColor != null)
					g.FillPreserve();
				else
					g.Fill();
			}
			if (strokeColor.HasValue && strokeColor != null) {
				g.Color = (Cairo.Color)strokeColor;
				g.Stroke();
			}
		}
		public static void DrawText(Cairo.Context g, Vector2 position, string text, double scale, TextAlign hAlign, TextAlign vAlign, Cairo.Color? fillColor, Cairo.Color? strokeColor, Cairo.Color? backgroundColor, double angle, string font) {
			if (font == null)
				font = "04b_19";
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
				g.LineWidth = 2;
			}
			VGame.Renderer.EnableAntialiasing(g);
		}
	}
	public interface IShape {
		void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale);
	}
	public abstract class Shape : IShape {
		public abstract void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale);
	}
}

namespace VGame.Shapes {
	public class Cursor : VGame.Shape {
		public override void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			g.MoveTo(position.ToPointD());
			g.LineTo(position.AddLengthDir(scale, MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(scale, MathHelper.PiOver4 * 9).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, fillColor, strokeColor);
		}
	}
}