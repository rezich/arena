using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cairo;

namespace VGame {
	public class VectorGameSession : Game {
		protected ScreenManager screenManager;
		PreRenderer preRenderer;
		PostRenderer postRenderer;
		GraphicsDeviceManager graphics;

		public VectorGameSession() {
			graphics = new GraphicsDeviceManager(this);
			preRenderer = new PreRenderer(this);
			preRenderer.DrawOrder = 1;
			screenManager = new ScreenManager(this);
			screenManager.DrawOrder = 2;
			postRenderer = new PostRenderer(this);
			postRenderer.DrawOrder = 3;

			Resolution.Initialize(graphics);
			int w = 1920;
			int h = 1080;
			Resolution.Set(w, h, true);
			VGame.Renderer.Initialize(graphics.GraphicsDevice, w, h);
			if (!SpriteBatchHelper.IsInitialized)
				SpriteBatchHelper.Initialize(graphics.GraphicsDevice);

			Components.Add(preRenderer);
			Components.Add(screenManager);
			Components.Add(postRenderer);
		}
		protected override void Initialize() {
			base.Initialize();
		}
		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime);
		}
		public void DrawVectors(GameTime gameTime) {
			graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.LightGray);

			screenManager.SpriteBatch.Begin();
			screenManager.SpriteBatch.Draw(VGame.Renderer.RenderTarget, Resolution.Rectangle, Microsoft.Xna.Framework.Color.White);
			screenManager.SpriteBatch.End();
		}
	}
	public class PreRenderer : DrawableGameComponent {
		public PreRenderer(VectorGameSession game)
			: base(game) {
		}
		public override void Draw(GameTime gameTime) {
			VGame.Renderer.BeginDrawing();
		}
	}
	public class PostRenderer : DrawableGameComponent {
		public PostRenderer(VectorGameSession game)
			: base(game) {
		}
		public override void Draw(GameTime gameTime) {
			((VectorGameSession)Game).DrawVectors(gameTime);
			VGame.Renderer.EndDrawing();
		}
	}
	public static class Renderer {
		private static GraphicsDevice graphics;
		public static ImageSurface Surface;
		public static RenderTarget2D RenderTarget;
		public static Context Context;
		public static int Width;
		public static int Height;
		public static void Initialize(GraphicsDevice graphicsDevice, int width, int height) {
			graphics = graphicsDevice;
			Resize(width, height);
		}
		public static void Resize(int width, int height) {
			Width = width;
			Height = height;
			//if (Surface != null) ((IDisposable)Surface).Dispose();
			//if (RenderTarget != null) ((IDisposable)RenderTarget).Dispose();
			Surface = new Cairo.ImageSurface(Cairo.Format.Rgb24, Width, Height);
			RenderTarget = new RenderTarget2D(graphics, Width, Height);
		}
		public static void BeginDrawing() {
			Context = new Cairo.Context(Surface);
			Context.Antialias = Cairo.Antialias.Subpixel;
			Context.Save();
			Context.SetSourceRGBA(0, 0, 0, 0);
			Context.Operator = Cairo.Operator.Source;
			Context.Paint();
			Context.Restore();
		}
		public static void EndDrawing() {
			((IDisposable)Context).Dispose();
			RenderTarget.SetData(Surface.Data);
		}
	}
	public static class Util {
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
			if (strokeColor.HasValue && fillColor != null) {
				g.Color = (Cairo.Color)strokeColor;
				g.Stroke();
			}
		}
	}
	public interface IShape {
		void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale);
	}
	public abstract class Shape : IShape {
		public abstract void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale);
	}
}

