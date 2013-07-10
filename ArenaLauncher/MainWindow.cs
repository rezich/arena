using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Gtk;
using ArenaLauncher;

public partial class MainWindow: Gtk.Window {
	List<string> logBuffer = new List<string>();
	FileDownloader downloader = new FileDownloader(true);
	public const string ChangelogAddress = @"https://raw.github.com/adamrezich/arena/master/README.md";
	public bool downloadedChangelog = false;

	public MainWindow(): base (Gtk.WindowType.Toplevel) {
		if (!Directory.Exists("patch"))
			Directory.CreateDirectory("patch");
		downloader.LocalDirectory = System.IO.Directory.GetCurrentDirectory() + "/patch";
		downloader.Files.Clear();
		downloader.Files.Add(new FileDownloader.FileInfo(ChangelogAddress));
		downloader.FileDownloadStarted += delegate(object sender, EventArgs e) {
			pbCurrent.Text = downloader.CurrentFile.Name;
			Log(string.Format("Starting download of {0}", downloader.CurrentFile.Name));
		};
		downloader.FileDownloadAttempting += delegate(object sender, EventArgs e) {
			Log("Attempting download");
		};
		downloader.FileDownloadFailed += delegate(object sender, Exception ex) {
			Log(string.Format("!! Download failed !!"));
		};
		downloader.ProgressChanged += delegate(object sender, EventArgs e) {
			pbCurrent.Fraction = downloader.CurrentFilePercentage() / 100;
			pbTotal.Fraction = downloader.TotalPercentage() / 100;
			pbTotal.Text = string.Format("{0} / {1} KB", downloader.TotalProgress / 1024, downloader.TotalSize / 1024);
		};
		downloader.Completed += delegate(object sender, EventArgs e) {
			Log("All downloads complete.");
			btnPlay.Sensitive = true;
		};
		downloader.FileDownloadSucceeded += delegate(object sender, EventArgs e) {
			Log("Download complete.");
		};
		Build();
		downloader.Start();
	}

	protected void Log(string text) {
		tvConsole.Buffer.Text += string.Format("{0}\n", text);
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a) {
		downloader.Dispose();
		Application.Quit();
		a.RetVal = true;
	}

	void DownloadChangelog() {

	}
}
