using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VGame;

namespace ArenaClient {
	public class SettingsMenu : GenericMenu  {
		string newName = Arena.Config.PlayerName;
		int newNumber = Arena.Config.PlayerNumber;
		List<Point> displayModes = new List<Point>();
		SelectManyEntry resolutionEntry;
		SelectManyEntry fullscreenEntry;
		MenuEntry saveEntry;
		public SettingsMenu() : base("SETTINGS") {
			List<string> convertedDisplayModes = new List<string>();
			foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
				bool found = false;
				foreach (Point p in displayModes)
					if (p.X == mode.Width && p.Y == mode.Height)
						found = true;
				if (!found) {
					displayModes.Add(new Point(mode.Width, mode.Height));
					convertedDisplayModes.Add(mode.Width + "x" + mode.Height);
				}
			}

			Entries.Add(new HeadingEntry("PLAYER"));

			Entries.Add(new TextInputEntry("NAME", Arena.Config.PlayerName));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				newName = e.Text;
				CheckForChanges(sender, e);
			};

			Entries.Add(new NumberInputEntry("NUMBER", Arena.Config.PlayerNumber));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				newNumber = int.Parse(e.Text);
				CheckForChanges(sender, e);
			};

			Entries.Add(new SpacerEntry());

			Entries.Add(new HeadingEntry("GRAPHICS"));

			resolutionEntry = new SelectManyEntry("RESOLUTION", convertedDisplayModes);
			if (displayModes.Contains(Arena.Config.Resolution))
				resolutionEntry.SelectedIndex = displayModes.IndexOf(Arena.Config.Resolution);
			resolutionEntry.SwipeLeft += CheckForChanges;
			resolutionEntry.SwipeRight += CheckForChanges;
			resolutionEntry.Selected += CheckForChanges;
			resolutionEntry.Enabled = false;
			Entries.Add(resolutionEntry);

			fullscreenEntry = new SelectManyEntry("FULLSCREEN", new List<string>() { "NO", "YES" });
			if (Arena.Config.Fullscreen)
				fullscreenEntry.SelectedIndex = 1;
			/*if (Arena.Config.Borderless)
				fullscreenEntry.SelectedIndex = 2;*/
			fullscreenEntry.SwipeLeft += CheckForChanges;
			fullscreenEntry.SwipeRight += CheckForChanges;
			fullscreenEntry.Selected += CheckForChanges;
			fullscreenEntry.Enabled = false;
			Entries.Add(fullscreenEntry);

			Entries.Add(new TextInputEntry("VSYNC", "OFF"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("ANTIALIASING", "ON"));
			Entries.Last().Enabled = false;
			Entries.Add(new TextInputEntry("DOUBLE-BUFFERING", "ON"));
			Entries.Last().Enabled = false;

			Entries.Add(new SpacerEntry());

			saveEntry = new MenuEntry("SAVE");
			saveEntry.Selected += delegate(object sender, PlayerIndexEventArgs e) {
				Arena.Config.PlayerName = newName;
				Arena.Config.PlayerNumber = newNumber;
				if (Arena.Config.Resolution != displayModes[resolutionEntry.SelectedIndex] || Arena.Config.Fullscreen != (fullscreenEntry.SelectedIndex == 1)) {
					if (!Resolution.Set(displayModes[resolutionEntry.SelectedIndex].X, displayModes[resolutionEntry.SelectedIndex].Y, fullscreenEntry.SelectedIndex == 1, false, false))
						throw new Exception("oops");
					Arena.Config.Resolution = displayModes[resolutionEntry.SelectedIndex];
					Arena.Config.Fullscreen = fullscreenEntry.SelectedIndex == 1;
					Renderer.Resize(displayModes[resolutionEntry.SelectedIndex].X, displayModes[resolutionEntry.SelectedIndex].Y);
				}
				ExitScreen();
			};
			saveEntry.Enabled = false;
			Entries.Add(saveEntry);

			Entries.Add(new CancelEntry("BACK"));

		}
		protected void CheckForChanges(object sender, EventArgs e) {
			saveEntry.Enabled = (newName != Arena.Config.PlayerName || newNumber != Arena.Config.PlayerNumber || displayModes[resolutionEntry.SelectedIndex] != Arena.Config.Resolution|| Arena.Config.Fullscreen != (fullscreenEntry.SelectedIndex == 1));
		}
		protected override void OnCancel() {
			ExitScreen();
			base.OnCancel();
		}
	}
}

