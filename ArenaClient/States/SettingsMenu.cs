using System;
using System.Collections.Generic;
using System.Linq;
using VGame;

namespace ArenaClient {
	public class SettingsMenu : Menu  {
		string newName = Arena.Config.PlayerName;
		int newNumber = Arena.Config.PlayerNumber;
		List<Rectangle> displayModes = new List<Rectangle>();
		SelectManyEntry resolutionEntry;
		SelectManyEntry fullscreenEntry;
		SelectManyEntry antialiasingEntry;
		MenuEntry saveEntry;
		public SettingsMenu() : base("SETTINGS") {
			List<string> convertedDisplayModes = new List<string>();
			foreach (Rectangle r in Renderer.Current.Resolutions) {
				displayModes.Add(r);
				convertedDisplayModes.Add(r.Width + "x" + r.Height);
			}

			Entries.Add(new HeadingEntry(this, "PLAYER"));

			Entries.Add(new TextInputEntry(this, "NAME", Arena.Config.PlayerName));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				newName = e.Text;
				CheckForChanges(sender, e);
			};

			Entries.Add(new NumberInputEntry(this, "NUMBER", Arena.Config.PlayerNumber));
			Entries.Last().TextChanged += delegate(object sender, TextChangeArgs e) {
				newNumber = int.Parse(e.Text);
				CheckForChanges(sender, e);
			};

			Entries.Add(new SpacerEntry(this));

			Entries.Add(new HeadingEntry(this, "GRAPHICS"));

			resolutionEntry = new SelectManyEntry(this, "RESOLUTION", convertedDisplayModes);
			if (displayModes.Contains(Arena.Config.Resolution))
				resolutionEntry.SelectedIndex = displayModes.IndexOf(Arena.Config.Resolution);
			resolutionEntry.SwipeLeft += CheckForChanges;
			resolutionEntry.SwipeRight += CheckForChanges;
			resolutionEntry.Selected += CheckForChanges;
			Entries.Add(resolutionEntry);

			fullscreenEntry = new SelectManyEntry(this, "FULLSCREEN", new List<string>() { "NO", "YES", "BORDERLESS" });
			if (Arena.Config.Fullscreen)
				fullscreenEntry.SelectedIndex = 1;
			if (Arena.Config.Borderless)
				fullscreenEntry.SelectedIndex = 2;
			fullscreenEntry.SwipeLeft += CheckForChanges;
			fullscreenEntry.SwipeRight += CheckForChanges;
			fullscreenEntry.Selected += CheckForChanges;
			Entries.Add(fullscreenEntry);

			Entries.Add(new TextInputEntry(this, "VSYNC", "OFF"));
			Entries.Last().Enabled = false;
			antialiasingEntry = new SelectManyEntry(this, "ANTIALIASING", new List<string>() { "OFF", "ON" });
			antialiasingEntry.SelectedIndex = Arena.Config.Antialiasing ? 1 : 0;
			antialiasingEntry.SwipeLeft += CheckForChanges;
			antialiasingEntry.SwipeRight += CheckForChanges;
			antialiasingEntry.Selected += CheckForChanges;
			Entries.Add(antialiasingEntry);

			Entries.Add(new SpacerEntry(this));

			saveEntry = new MenuEntry(this, "SAVE");
			saveEntry.Selected += delegate(object sender, EventArgs e) {
				Arena.Config.PlayerName = newName;
				Arena.Config.PlayerNumber = newNumber;
				Arena.Config.Antialiasing = (antialiasingEntry.SelectedIndex == 1) ;
				Renderer.Antialiasing = (antialiasingEntry.SelectedIndex == 1) ;

				if (Arena.Config.Resolution != displayModes[resolutionEntry.SelectedIndex] || Arena.Config.Fullscreen != (fullscreenEntry.SelectedIndex == 1) || Arena.Config.Borderless != (fullscreenEntry.SelectedIndex == 2)) {
					Arena.Config.Resolution = displayModes[resolutionEntry.SelectedIndex];
					Arena.Config.Fullscreen = (fullscreenEntry.SelectedIndex == 1);
					Arena.Config.Borderless = (fullscreenEntry.SelectedIndex == 2);
					StateManager.Game.ChangeResolution(displayModes[resolutionEntry.SelectedIndex], Arena.Config.Fullscreen, Arena.Config.Borderless);
				}
				Arena.Config.Write();
				Exit();
			};
			saveEntry.Enabled = false;
			Entries.Add(saveEntry);

			Entries.Add(new CancelEntry(this, "BACK"));

		}
		protected void CheckForChanges(object sender, EventArgs e) {
			saveEntry.Enabled = (newName != Arena.Config.PlayerName || newNumber != Arena.Config.PlayerNumber || (antialiasingEntry.SelectedIndex == 1) != Arena.Config.Antialiasing || displayModes[resolutionEntry.SelectedIndex] != Arena.Config.Resolution || Arena.Config.Fullscreen != (fullscreenEntry.SelectedIndex == 1) || Arena.Config.Borderless != (fullscreenEntry.SelectedIndex == 2));
		}
		protected override void OnCancel() {
			Exit();
			base.OnCancel();
		}
	}
}

