using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

partial class TypeForm : Form
{
	public readonly Type Type;
	
	TabControl tabs;
	
	ClassTab classTab;
	VerbsTab verbsTab;
	VerbsTab backVerbTab;
	IconTab iconTab;
	MiscTab miscTab;
	
	public event Action <string> Changed = null;
	
	public TypeForm (string type)
	{
		Type = new Type(type);
		
		Text = Own.Line("Type") + ": " + type;
		
		MinimizeBox = false;
		MaximizeBox = false;
		StartPosition = FormStartPosition.CenterScreen;
		SizeGripStyle = SizeGripStyle.Show;
		KeyPreview = true;
		
		if (!Type.Exists(Type.ID)) Type.Create(Type.ID);
		
		ClientSize = new Size(280, 270);
		
		tabs = new TabControl();
		tabs.Location = new Point(10, 10);
		tabs.Size = new Size(ClientSize.Width - 20, ClientSize.Height - 20);
		tabs.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
		Controls.Add(tabs);
		
		if (Type.IsValidExtension(Type.ID))
		{
			classTab = new ClassTab(Type);
			tabs.Controls.Add(classTab);
			
			classTab.ClassChanged += delegate
			{
				if (Changed != null) Changed(Type.ID);
				RevealIcon();
			};
		}
		
		verbsTab = new VerbsTab("Actions", Type.Verbs);
		tabs.Controls.Add(verbsTab);
		
		if (OS.Vista && Type.BackgroundVerbs != null)
		{
			backVerbTab = new VerbsTab("Background", Type.BackgroundVerbs);
			tabs.Controls.Add(backVerbTab);
		}
		
		if (Type.IsValidExtension(Type.ID))
		{
			iconTab = new IconTab(Type);
			tabs.Controls.Add(iconTab);
			
			iconTab.IconChanged += delegate
			{
				if (Changed != null) Changed(Type.Actual.ID);
				RevealIcon();
			};
			
			miscTab = new MiscTab(Type);
			tabs.Controls.Add(miscTab);
		}
		
		KeyDown += delegate (object o, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape: Close(); break;
				case Keys.F1: About.Display(); break;
			}
		};
		
		RevealIcon();
	}
	
	void RevealIcon ()
	{
		string icon = Type.Icon;
		
		if (icon != null)
		{
			ResIcon res = new ResIcon(icon);
			if (res.Valid) { Icon = res.Get(16); return; }
		}
		
		if (OS.Seven) Icon = new ResIcon("imageres.dll", 2).Get(16);
		else if (OS.Vista) Icon = new ResIcon("imageres.dll", 1).Get(16);
		else Icon = new ResIcon("shell32.dll").Get(16);
	}
}