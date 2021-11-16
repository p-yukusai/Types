using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: AssemblyTitle("Types setup")]

class Setup : Form
{
	bool concluded = false;
	Exception fail = null;
	
	string installDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Types";
	
	bool addPanel = true;
	bool addMenu = true;
	bool addStart = false;
	bool addDesktop = false;
	
	Panel area = new Panel();
	TongueCombo tongueCombo = new TongueCombo();
	GroupBox linksGroup = new GroupBox();
	CheckBox panelCheck = new CheckBox();
	CheckBox menuCheck = new CheckBox();
	CheckBox startCheck = new CheckBox();
	CheckBox desktopCheck = new CheckBox();
	TextBox pathField = new TextBox();
	Button pathButton = new Button();
	FolderBrowserDialog pathBrowser = new FolderBrowserDialog();
	Button goButton = new Button();
	Button doneButton = new Button();
	ProgressBar progress = new ProgressBar();
	TextBox failBox = new TextBox();
	
	[STAThread] static void Main ()
	{
		Application.EnableVisualStyles();
		Application.Run(new Setup());
	}
	
	public Setup ()
	{
		ClientSize = new Size(280, 260);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		StartPosition = FormStartPosition.CenterScreen;
		MaximizeBox = MinimizeBox = false;
		Padding = new Padding(10);
		
		area.Dock = DockStyle.Fill;
		Controls.Add(area);
		
		int cw = area.ClientSize.Width;
		int ch = area.ClientSize.Height;
		
		pathField.Top = 0;
		pathField.Width = cw - 30;
		pathField.Text = installDir;
		area.Controls.Add(pathField);
		
		pathButton.Text = "…";
		pathButton.Location = new Point(pathField.Width + 10, pathField.Top);
		pathButton.Size = new Size(cw - pathField.Width - 10, pathField.Height);
		area.Controls.Add(pathButton);
		
		linksGroup.Top = pathField.Bottom + 10;
		linksGroup.Width = cw;
		area.Controls.Add(linksGroup);
		
		int gw = linksGroup.ClientSize.Width - 30;
		
		panelCheck.Location = new Point(20, 20);
		panelCheck.Width = gw;
		panelCheck.Checked = addPanel;
		linksGroup.Controls.Add(panelCheck);
		
		menuCheck.Location = new Point(20, panelCheck.Top + panelCheck.Height);
		menuCheck.Width = gw;
		menuCheck.Checked = addMenu;
		linksGroup.Controls.Add(menuCheck);
		
		startCheck.Location = new Point(20, menuCheck.Top + menuCheck.Height);
		startCheck.Width = gw;
		startCheck.Checked = addStart;
		linksGroup.Controls.Add(startCheck);
		
		desktopCheck.Location = new Point(20, startCheck.Top + startCheck.Height);
		desktopCheck.Width = gw;
		desktopCheck.Checked = addDesktop;
		linksGroup.Controls.Add(desktopCheck);
		
		linksGroup.ClientSize = new Size(linksGroup.ClientSize.Width, desktopCheck.Top + panelCheck.Top * 2);
		
		tongueCombo.Location = new Point(0, linksGroup.Bottom + 20);
		tongueCombo.Width = cw / 3 * 2;
		tongueCombo.TongueSelected += delegate (string nt) { Own.Tongue = nt; Localize(); };
		area.Controls.Add(tongueCombo);
		
		goButton.Size = new Size(cw - tongueCombo.Width - 10, tongueCombo.Height + 2);
		goButton.Location = new Point(tongueCombo.Right + 10, tongueCombo.Top - 1);
		area.Controls.Add(goButton);
		
		progress.Location = goButton.Location;
		progress.Size = goButton.Size;
		progress.Style = ProgressBarStyle.Marquee;
		area.Controls.Add(progress);
		
		doneButton.Visible = false;
		doneButton.Location = goButton.Location;
		doneButton.Size = goButton.Size;
		area.Controls.Add(doneButton);
		
		ClientSize = new Size(ClientSize.Width, goButton.Bottom + Padding.Bottom + Padding.Top);
		
		pathButton.Click += delegate { if (pathBrowser.ShowDialog() == DialogResult.OK) pathField.Text = pathBrowser.SelectedPath; };
		goButton.Click += delegate { Go(); };
		doneButton.Click += delegate { Application.Exit(); };
		
		failBox.Dock = DockStyle.Fill;
		failBox.BackColor = Color.Brown;
		failBox.ForeColor = Color.Yellow;
		failBox.ReadOnly = true;
		failBox.Multiline = true;
		failBox.ScrollBars = ScrollBars.Vertical;
		failBox.Visible = false;
		Controls.Add(failBox);
		
		Localize();
	}
	
	void Localize ()
	{
		Icon = Own.Icon("Setup");
		Text = Own.Line("Types setup") + " (" + Application.ProductVersion.ToString() + ")";
		panelCheck.Text = Own.Line("Add to Control Panel");
		menuCheck.Text = Own.Line("Add to context menus");
		startCheck.Text = Own.Line("Start menu shortcut");
		desktopCheck.Text = Own.Line("Desktop shortcut");
		goButton.Text = Own.Line("Install");
		doneButton.Text = Own.Line("Done");
		pathBrowser.Description = Own.Line("Where to put program files?");
	}
	
	void Go ()
	{
		installDir = pathField.Text;
		
		addPanel = panelCheck.Checked;
		addMenu = menuCheck.Checked;
		addStart = startCheck.Checked;
		addDesktop = desktopCheck.Checked;
		
		tongueCombo.Enabled = false;
		linksGroup.Enabled = false;
		pathField.Enabled = false;
		pathButton.Enabled = false;
		
		progress.Show();
		goButton.Hide();
		
		new Thread ( delegate () {
			try { Install(); }
			catch (Exception x) { fail = x; }
			finally { concluded = true; }
		} ).Start();
		
		while (!concluded) Application.DoEvents();
		
		if (fail == null) {
			doneButton.Show();
			progress.Hide();
		} else {
			SystemSounds.Beep.Play();
			failBox.Text = fail.ToString();
			failBox.Show();
			area.Hide();
		}
	}
	
	void Install ()
	{
		string guid = "{76402ED7-C2D7-4FD7-A4A5-BD52204550B8}";
		bool isVista = System.Environment.OSVersion.Version.Major > 5;
		string bits = (IntPtr.Size == 8) ? "64" : "32";
		
		Deployment.Installer inst = new Deployment.Installer();
		
		Own.Tongue = tongueCombo.Tongue;
		Options.Set("Tongue", Own.Tongue);
		inst.RememberKey(@"Software\\Types");
		
		inst.InstallDir = installDir;
		
		inst.AddFileToDeploy(inst.RemoveCommand = "Remove.exe");
		inst.AddFileToDeploy(inst.MainExe = "Types.exe");
		
		string mainExePath = inst.InstallDir + "\\" + inst.MainExe;
		
		if (addPanel)
		{
			if (isVista)
			{
				string cpl = "Tasks.xml";
				string cplPath = inst.InstallDir + "\\" + cpl;
				
				string pax = new StreamReader(Own.Get(cpl)).ReadToEnd();
				
				pax = pax.Replace("PanelDescription", Own.Line("PanelDescription", "Manage file types"));
				pax = pax.Replace("PanelKeywords", Own.Line("PanelKeywords", ""));
				pax = pax.Replace("{path}", mainExePath);
				
				inst.AddTextToDeploy(pax, cpl);
				
				string clsidpath = @"Software\Classes\CLSID";
				RegistryKey ck = Registry.LocalMachine.OpenSubKey(clsidpath, true).CreateSubKey(guid);
				ck.SetValue("", Own.Line(inst.ID));
				ck.SetValue("InfoTip", Own.Line("PanelDescription"));
				ck.SetValue("System.ApplicationName", "control.exe /name " + inst.ID);
				ck.SetValue("System.ControlPanel.Category", "1,5");
				ck.SetValue("System.Software.TasksFileUrl", cplPath);
				ck.CreateSubKey("DefaultIcon").SetValue("", mainExePath);
				ck.CreateSubKey("Shell").CreateSubKey("Open").CreateSubKey("Command").SetValue("", mainExePath);
				inst.RememberKey(clsidpath + "\\" + guid);
				
				string nspath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace";
				Registry.LocalMachine.OpenSubKey(nspath, true).CreateSubKey(guid).SetValue("", inst.ID);
				inst.RememberKey(nspath + "\\" + guid);
			}
			else
			{
				string cplin = bits + ".Types.cpl";
				string cplout = "Types.cpl";
				
				inst.AddFileToDeploy(cplin, cplout);
				
				string cplspath = @"Software\Microsoft\Windows\CurrentVersion\Control Panel\Cpls";
				RegistryKey pk = Registry.LocalMachine.OpenSubKey(cplspath, true);
				pk.SetValue(inst.ID, inst.InstallDir + "\\" + cplout);
				inst.RememberValue(cplspath, inst.ID);
			}
		}
		
		if (addStart) inst.AddStartShortcut(Own.Line("Types"), "Types.exe");
		if (addDesktop) inst.AddDesktopShortcut(Own.Line("Types"), "Types.exe");
		
		if (addMenu)
		{
			string cipath = @"Software\Classes\*\shell\Type";
			RegistryKey tk = Registry.LocalMachine.CreateSubKey(cipath);
			
			tk.SetValue("", Own.Line("Edit file type"));
			tk.CreateSubKey("command").SetValue("", "\"" + mainExePath + "\"" + " \"%1\"");
			
			inst.RememberKey(cipath);
		}
		
		foreach (string file in Own.Items)
		{
			if (file.EndsWith(".tongue")) inst.AddFileToDeploy(file);
		}
		
		inst.Go();
	}
}