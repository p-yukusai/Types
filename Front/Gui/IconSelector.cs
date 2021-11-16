using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

class IconSelector : Panel
{
	string defaultPath;
	
	public event Action Changed;
	public event Action Removed;
	
	Combo pathCombo = new Combo("IconFiles");
	FlatButton browseButton = new FlatButton(Own.Image("Browse"));
	FlatButton removeButton = new FlatButton(Own.Image("Delete"));
	OpenFileDialog iconOpener = new OpenFileDialog();
	ResIconList iconList = new ResIconList();
	
	public string DefaultPath
	{
		get { return defaultPath; }
		set {
			defaultPath = value;
			iconList.Path = value;
		}
	}
	
	public string Path
	{
		get { return pathCombo.Text; }
		set {
			pathCombo.Set(value);
			if (value != null) iconList.Path = value;
			else iconList.Path = defaultPath;
		}
	}
	
	public IconSelector ()
	{
		const int margin = 10;
		Dock = DockStyle.Fill;
		
		removeButton.Location = new Point(Width - removeButton.Width - margin, margin);
		removeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		Tip.Set(removeButton, "Remove icon");
		Controls.Add(removeButton);
		
		browseButton.Location = new Point(removeButton.Left - browseButton.Width - margin, margin);
		browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		Tip.Set(browseButton, "Select icon file");
		Controls.Add(browseButton);
		
		pathCombo.Location = new Point(margin, margin);
		pathCombo.Size = new Size(browseButton.Left - margin * 2, browseButton.Height);
		pathCombo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		Controls.Add(pathCombo);
		
		iconList.Location = new Point(margin, browseButton.Top + browseButton.Height + margin);
		iconList.Size = new Size(Width - margin * 2, Height - pathCombo.Height - margin * 3);
		iconList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
		Controls.Add(iconList);
		
		browseButton.Click += delegate { iconOpener.ShowDialog(); };
		removeButton.Click += delegate { Path = null; Removed(); };
		
		iconList.Changed += delegate { pathCombo.Text = iconList.Path; ConcludeChange(); };
		pathCombo.Changed += delegate { iconList.Path = pathCombo.Text; ConcludeChange(); };
		iconOpener.FileOk += delegate { Path = iconOpener.FileName; ConcludeChange(); };
		
		if (OS.Vista) defaultPath = "imageres.dll";
		else defaultPath = "shell32.dll";
	}
	
	void ConcludeChange ()
	{
		pathCombo.Remember(iconList.File);
		Changed();
	}
}