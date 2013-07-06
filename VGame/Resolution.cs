using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VGame {
	public static class Resolution {
		private static int _width = 1280;
		private static int _height = 720;
		private static int _x = 0;
		private static int _y = 0;
		private static bool _fullscreen = false;
		private static GraphicsDeviceManager _graphics;
		public static int Width {
			get {
				return _width;
			}
		}
		public static int Height {
			get {
				return _height;
			}
		}
		public static int X {
			get {
				return _x;
			}
		}
		public static int Y {
			get {
				return _y;
			}
		}
		public static int Top {
			get {
				return _x;
			}
		}
		public static int Right {
			get {
				return _x + _width;
			}
		}
		public static int Bottom {
			get {
				return _y + _height;
			}
		}
		public static int Left {
			get {
				return _y;
			}
		}
		public static bool Fullscreen {
			get {
				return _fullscreen;
			}
		}
		public static Rectangle Rectangle {
			get {
				return new Rectangle(_x, _y, _width, _height);
			}
		}
		public static void Initialize(GraphicsDeviceManager graphics) {
			_graphics = graphics;
		}
		public static bool Set(int width, int height, bool fullscreen) {
			int oldWidth = _width;
			int oldHeight = _height;
			_width = width;
			_height = height;
			_fullscreen = fullscreen;
			if (_fullscreen) {
				foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
					if (dm.Width == _width && dm.Height == _height) {
						_graphics.PreferredBackBufferWidth = _width;
						_graphics.PreferredBackBufferHeight = _height;
						_graphics.IsFullScreen = _fullscreen;
						_graphics.ApplyChanges();
						return true;
					}
				}
				_width = oldWidth;
				_height = oldHeight;
				return false;
			}
			else {
				if (_width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width && _height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height) {
					_graphics.PreferredBackBufferWidth = _width;
					_graphics.PreferredBackBufferHeight = _height;
					_graphics.IsFullScreen = _fullscreen;
					_graphics.ApplyChanges();
					return true;
				}
				else
					_width = oldWidth;
					_height = oldHeight;
					return false;
			}
		}
	}
}