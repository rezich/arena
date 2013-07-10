
// This file has been generated by the GUI designer. Do not modify.
public partial class MainWindow
{
	private global::Gtk.VBox vbox1;
	private global::Gtk.HPaned hpaned1;
	private global::Gtk.Frame frameChangelog;
	private global::Gtk.Alignment GtkAlignment;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.TextView tvChangelog;
	private global::Gtk.Label GtkLabel1;
	private global::Gtk.Frame frameConsole;
	private global::Gtk.Alignment GtkAlignment1;
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	private global::Gtk.TextView tvConsole;
	private global::Gtk.Label GtkLabel2;
	private global::Gtk.HBox hbox4;
	private global::Gtk.Frame frameProgress;
	private global::Gtk.Alignment GtkAlignment3;
	private global::Gtk.Table table3;
	private global::Gtk.Label label2;
	private global::Gtk.Label label3;
	private global::Gtk.ProgressBar pbCurrent;
	private global::Gtk.ProgressBar pbTotal;
	private global::Gtk.Label GtkLabel4;
	private global::Gtk.Frame frameLogin;
	private global::Gtk.Alignment GtkAlignment4;
	private global::Gtk.VBox vbox5;
	private global::Gtk.Table table4;
	private global::Gtk.Entry enLoginPassword;
	private global::Gtk.Entry enLoginUsername;
	private global::Gtk.Label label4;
	private global::Gtk.Label label5;
	private global::Gtk.CheckButton cbLoginRemember;
	private global::Gtk.Button btnLoginLogin;
	private global::Gtk.Button btnLoginRegister;
	private global::Gtk.Label GtkLabel6;
	private global::Gtk.Frame frame6;
	private global::Gtk.Alignment GtkAlignment2;
	private global::Gtk.VBox vbox6;
	private global::Gtk.Table table6;
	private global::Gtk.Entry entry3;
	private global::Gtk.Label label6;
	private global::Gtk.Label label7;
	private global::Gtk.Label label8;
	private global::Gtk.Label GtkLabel8;
	private global::Gtk.Button btnPlay;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.WidthRequest = 800;
		this.HeightRequest = 600;
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("arena Launcher");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		this.BorderWidth = ((uint)(3));
		this.Resizable = false;
		this.AllowGrow = false;
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hpaned1 = new global::Gtk.HPaned ();
		this.hpaned1.CanFocus = true;
		this.hpaned1.Name = "hpaned1";
		this.hpaned1.Position = 550;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.frameChangelog = new global::Gtk.Frame ();
		this.frameChangelog.Name = "frameChangelog";
		this.frameChangelog.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child frameChangelog.Gtk.Container+ContainerChild
		this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
		this.GtkAlignment.Name = "GtkAlignment";
		this.GtkAlignment.LeftPadding = ((uint)(12));
		this.GtkAlignment.BorderWidth = ((uint)(3));
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.tvChangelog = new global::Gtk.TextView ();
		this.tvChangelog.CanFocus = true;
		this.tvChangelog.Name = "tvChangelog";
		this.tvChangelog.Editable = false;
		this.GtkScrolledWindow.Add (this.tvChangelog);
		this.GtkAlignment.Add (this.GtkScrolledWindow);
		this.frameChangelog.Add (this.GtkAlignment);
		this.GtkLabel1 = new global::Gtk.Label ();
		this.GtkLabel1.Name = "GtkLabel1";
		this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("Changelog");
		this.GtkLabel1.UseMarkup = true;
		this.frameChangelog.LabelWidget = this.GtkLabel1;
		this.hpaned1.Add (this.frameChangelog);
		global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this.hpaned1 [this.frameChangelog]));
		w4.Resize = false;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.frameConsole = new global::Gtk.Frame ();
		this.frameConsole.Name = "frameConsole";
		this.frameConsole.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child frameConsole.Gtk.Container+ContainerChild
		this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
		this.GtkAlignment1.Name = "GtkAlignment1";
		this.GtkAlignment1.LeftPadding = ((uint)(12));
		this.GtkAlignment1.BorderWidth = ((uint)(3));
		// Container child GtkAlignment1.Gtk.Container+ContainerChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.tvConsole = new global::Gtk.TextView ();
		this.tvConsole.Sensitive = false;
		this.tvConsole.CanFocus = true;
		this.tvConsole.Name = "tvConsole";
		this.tvConsole.Editable = false;
		this.GtkScrolledWindow1.Add (this.tvConsole);
		this.GtkAlignment1.Add (this.GtkScrolledWindow1);
		this.frameConsole.Add (this.GtkAlignment1);
		this.GtkLabel2 = new global::Gtk.Label ();
		this.GtkLabel2.Name = "GtkLabel2";
		this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("Console");
		this.GtkLabel2.UseMarkup = true;
		this.frameConsole.LabelWidget = this.GtkLabel2;
		this.hpaned1.Add (this.frameConsole);
		this.vbox1.Add (this.hpaned1);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hpaned1]));
		w9.Position = 0;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox4 = new global::Gtk.HBox ();
		this.hbox4.Name = "hbox4";
		this.hbox4.Spacing = 6;
		// Container child hbox4.Gtk.Box+BoxChild
		this.frameProgress = new global::Gtk.Frame ();
		this.frameProgress.Name = "frameProgress";
		this.frameProgress.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child frameProgress.Gtk.Container+ContainerChild
		this.GtkAlignment3 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
		this.GtkAlignment3.Name = "GtkAlignment3";
		this.GtkAlignment3.LeftPadding = ((uint)(12));
		this.GtkAlignment3.BorderWidth = ((uint)(3));
		// Container child GtkAlignment3.Gtk.Container+ContainerChild
		this.table3 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
		this.table3.Name = "table3";
		this.table3.RowSpacing = ((uint)(6));
		this.table3.ColumnSpacing = ((uint)(6));
		// Container child table3.Gtk.Table+TableChild
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Total");
		this.table3.Add (this.label2);
		global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table3 [this.label2]));
		w10.XOptions = ((global::Gtk.AttachOptions)(4));
		w10.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table3.Gtk.Table+TableChild
		this.label3 = new global::Gtk.Label ();
		this.label3.Name = "label3";
		this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Current file");
		this.table3.Add (this.label3);
		global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table3 [this.label3]));
		w11.TopAttach = ((uint)(1));
		w11.BottomAttach = ((uint)(2));
		w11.XOptions = ((global::Gtk.AttachOptions)(4));
		w11.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table3.Gtk.Table+TableChild
		this.pbCurrent = new global::Gtk.ProgressBar ();
		this.pbCurrent.WidthRequest = 250;
		this.pbCurrent.Name = "pbCurrent";
		this.table3.Add (this.pbCurrent);
		global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table3 [this.pbCurrent]));
		w12.TopAttach = ((uint)(1));
		w12.BottomAttach = ((uint)(2));
		w12.LeftAttach = ((uint)(1));
		w12.RightAttach = ((uint)(2));
		w12.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table3.Gtk.Table+TableChild
		this.pbTotal = new global::Gtk.ProgressBar ();
		this.pbTotal.WidthRequest = 250;
		this.pbTotal.Name = "pbTotal";
		this.table3.Add (this.pbTotal);
		global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table3 [this.pbTotal]));
		w13.LeftAttach = ((uint)(1));
		w13.RightAttach = ((uint)(2));
		w13.YOptions = ((global::Gtk.AttachOptions)(4));
		this.GtkAlignment3.Add (this.table3);
		this.frameProgress.Add (this.GtkAlignment3);
		this.GtkLabel4 = new global::Gtk.Label ();
		this.GtkLabel4.Name = "GtkLabel4";
		this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString ("Update progress");
		this.GtkLabel4.UseMarkup = true;
		this.frameProgress.LabelWidget = this.GtkLabel4;
		this.hbox4.Add (this.frameProgress);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.frameProgress]));
		w16.Position = 0;
		w16.Expand = false;
		w16.Fill = false;
		// Container child hbox4.Gtk.Box+BoxChild
		this.frameLogin = new global::Gtk.Frame ();
		this.frameLogin.Name = "frameLogin";
		this.frameLogin.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child frameLogin.Gtk.Container+ContainerChild
		this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
		this.GtkAlignment4.Name = "GtkAlignment4";
		this.GtkAlignment4.LeftPadding = ((uint)(12));
		this.GtkAlignment4.BorderWidth = ((uint)(3));
		// Container child GtkAlignment4.Gtk.Container+ContainerChild
		this.vbox5 = new global::Gtk.VBox ();
		this.vbox5.Name = "vbox5";
		this.vbox5.Spacing = 6;
		// Container child vbox5.Gtk.Box+BoxChild
		this.table4 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
		this.table4.Name = "table4";
		this.table4.RowSpacing = ((uint)(6));
		this.table4.ColumnSpacing = ((uint)(6));
		// Container child table4.Gtk.Table+TableChild
		this.enLoginPassword = new global::Gtk.Entry ();
		this.enLoginPassword.CanFocus = true;
		this.enLoginPassword.Name = "enLoginPassword";
		this.enLoginPassword.IsEditable = true;
		this.enLoginPassword.InvisibleChar = '●';
		this.table4.Add (this.enLoginPassword);
		global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table4 [this.enLoginPassword]));
		w17.TopAttach = ((uint)(1));
		w17.BottomAttach = ((uint)(2));
		w17.LeftAttach = ((uint)(1));
		w17.RightAttach = ((uint)(2));
		w17.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table4.Gtk.Table+TableChild
		this.enLoginUsername = new global::Gtk.Entry ();
		this.enLoginUsername.CanFocus = true;
		this.enLoginUsername.Name = "enLoginUsername";
		this.enLoginUsername.IsEditable = true;
		this.enLoginUsername.InvisibleChar = '●';
		this.table4.Add (this.enLoginUsername);
		global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table4 [this.enLoginUsername]));
		w18.LeftAttach = ((uint)(1));
		w18.RightAttach = ((uint)(2));
		w18.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table4.Gtk.Table+TableChild
		this.label4 = new global::Gtk.Label ();
		this.label4.Name = "label4";
		this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Username");
		this.table4.Add (this.label4);
		global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table4 [this.label4]));
		w19.XOptions = ((global::Gtk.AttachOptions)(4));
		w19.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table4.Gtk.Table+TableChild
		this.label5 = new global::Gtk.Label ();
		this.label5.Name = "label5";
		this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Password");
		this.table4.Add (this.label5);
		global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table4 [this.label5]));
		w20.TopAttach = ((uint)(1));
		w20.BottomAttach = ((uint)(2));
		w20.XOptions = ((global::Gtk.AttachOptions)(4));
		w20.YOptions = ((global::Gtk.AttachOptions)(4));
		this.vbox5.Add (this.table4);
		global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.table4]));
		w21.Position = 0;
		w21.Expand = false;
		w21.Fill = false;
		// Container child vbox5.Gtk.Box+BoxChild
		this.cbLoginRemember = new global::Gtk.CheckButton ();
		this.cbLoginRemember.CanFocus = true;
		this.cbLoginRemember.Name = "cbLoginRemember";
		this.cbLoginRemember.Label = global::Mono.Unix.Catalog.GetString ("Save login credentials");
		this.cbLoginRemember.DrawIndicator = true;
		this.cbLoginRemember.UseUnderline = true;
		this.vbox5.Add (this.cbLoginRemember);
		global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.cbLoginRemember]));
		w22.Position = 1;
		// Container child vbox5.Gtk.Box+BoxChild
		this.btnLoginLogin = new global::Gtk.Button ();
		this.btnLoginLogin.CanFocus = true;
		this.btnLoginLogin.Name = "btnLoginLogin";
		this.btnLoginLogin.UseUnderline = true;
		this.btnLoginLogin.Label = global::Mono.Unix.Catalog.GetString ("Login");
		this.vbox5.Add (this.btnLoginLogin);
		global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.btnLoginLogin]));
		w23.Position = 2;
		w23.Expand = false;
		w23.Fill = false;
		// Container child vbox5.Gtk.Box+BoxChild
		this.btnLoginRegister = new global::Gtk.Button ();
		this.btnLoginRegister.CanFocus = true;
		this.btnLoginRegister.Name = "btnLoginRegister";
		this.btnLoginRegister.Label = global::Mono.Unix.Catalog.GetString ("Register");
		this.vbox5.Add (this.btnLoginRegister);
		global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.btnLoginRegister]));
		w24.Position = 3;
		w24.Expand = false;
		w24.Fill = false;
		this.GtkAlignment4.Add (this.vbox5);
		this.frameLogin.Add (this.GtkAlignment4);
		this.GtkLabel6 = new global::Gtk.Label ();
		this.GtkLabel6.Name = "GtkLabel6";
		this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString ("Login");
		this.GtkLabel6.UseMarkup = true;
		this.frameLogin.LabelWidget = this.GtkLabel6;
		this.hbox4.Add (this.frameLogin);
		global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.frameLogin]));
		w27.Position = 1;
		// Container child hbox4.Gtk.Box+BoxChild
		this.frame6 = new global::Gtk.Frame ();
		this.frame6.Name = "frame6";
		this.frame6.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child frame6.Gtk.Container+ContainerChild
		this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
		this.GtkAlignment2.Name = "GtkAlignment2";
		this.GtkAlignment2.LeftPadding = ((uint)(12));
		this.GtkAlignment2.BorderWidth = ((uint)(3));
		// Container child GtkAlignment2.Gtk.Container+ContainerChild
		this.vbox6 = new global::Gtk.VBox ();
		this.vbox6.Name = "vbox6";
		this.vbox6.Spacing = 6;
		// Container child vbox6.Gtk.Box+BoxChild
		this.table6 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
		this.table6.Name = "table6";
		this.table6.RowSpacing = ((uint)(6));
		this.table6.ColumnSpacing = ((uint)(6));
		// Container child table6.Gtk.Table+TableChild
		this.entry3 = new global::Gtk.Entry ();
		this.entry3.CanFocus = true;
		this.entry3.Name = "entry3";
		this.entry3.IsEditable = true;
		this.entry3.InvisibleChar = '●';
		this.table6.Add (this.entry3);
		global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.table6 [this.entry3]));
		w28.LeftAttach = ((uint)(1));
		w28.RightAttach = ((uint)(2));
		w28.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table6.Gtk.Table+TableChild
		this.label6 = new global::Gtk.Label ();
		this.label6.Name = "label6";
		this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Username");
		this.table6.Add (this.label6);
		global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.table6 [this.label6]));
		w29.XOptions = ((global::Gtk.AttachOptions)(4));
		w29.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table6.Gtk.Table+TableChild
		this.label7 = new global::Gtk.Label ();
		this.label7.Name = "label7";
		this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Password");
		this.table6.Add (this.label7);
		global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.table6 [this.label7]));
		w30.TopAttach = ((uint)(1));
		w30.BottomAttach = ((uint)(2));
		w30.XOptions = ((global::Gtk.AttachOptions)(4));
		w30.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table6.Gtk.Table+TableChild
		this.label8 = new global::Gtk.Label ();
		this.label8.Name = "label8";
		this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Confirm Password");
		this.table6.Add (this.label8);
		global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table6 [this.label8]));
		w31.TopAttach = ((uint)(2));
		w31.BottomAttach = ((uint)(3));
		w31.XOptions = ((global::Gtk.AttachOptions)(4));
		w31.YOptions = ((global::Gtk.AttachOptions)(4));
		this.vbox6.Add (this.table6);
		global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.table6]));
		w32.Position = 0;
		w32.Expand = false;
		w32.Fill = false;
		this.GtkAlignment2.Add (this.vbox6);
		this.frame6.Add (this.GtkAlignment2);
		this.GtkLabel8 = new global::Gtk.Label ();
		this.GtkLabel8.Name = "GtkLabel8";
		this.GtkLabel8.LabelProp = global::Mono.Unix.Catalog.GetString ("Register");
		this.GtkLabel8.UseMarkup = true;
		this.frame6.LabelWidget = this.GtkLabel8;
		this.hbox4.Add (this.frame6);
		global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.frame6]));
		w35.Position = 2;
		// Container child hbox4.Gtk.Box+BoxChild
		this.btnPlay = new global::Gtk.Button ();
		this.btnPlay.Sensitive = false;
		this.btnPlay.CanFocus = true;
		this.btnPlay.Name = "btnPlay";
		this.btnPlay.UseUnderline = true;
		this.btnPlay.Label = global::Mono.Unix.Catalog.GetString ("Play");
		this.hbox4.Add (this.btnPlay);
		global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.btnPlay]));
		w36.PackType = ((global::Gtk.PackType)(1));
		w36.Position = 3;
		this.vbox1.Add (this.hbox4);
		global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox4]));
		w37.Position = 1;
		w37.Expand = false;
		w37.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 1017;
		this.DefaultHeight = 644;
		this.frameLogin.Hide ();
		this.frame6.Hide ();
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}