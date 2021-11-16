using System;
using System.Drawing;
using System.Windows.Forms;

class VerbForm : Form
{
	public readonly Verb Verb;
	
	TabControl tabs;
	DDETab ddeTab;
	VerbIconTab iconTab;
	
	public event EventHandler IconChanged;
	
	public VerbForm (Verb verb)
	{
		Verb = verb;
		
		MinimizeBox = false;
		MaximizeBox = false;
		
		KeyPreview = true;
		SizeGripStyle = SizeGripStyle.Show;
		
		Icon = Own.Icon("Types");
		Text = Verb.Title + " (" + verb.Verbs.Type.ID + ")";
		
		ClientSize = new Size(300, 340);
		
		tabs = new TabControl();
		tabs.Location = new Point(10, 10);
		tabs.Size = new Size(ClientSize.Width - 20, ClientSize.Height - 20);
		tabs.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
		Controls.Add(tabs);
		
		ddeTab = new DDETab(Verb);
		tabs.Controls.Add(ddeTab);
		
		if (OS.Vista)
		{
			iconTab = new VerbIconTab(verb);
			iconTab.IconChanged += delegate { IconChanged(this, null); };
			tabs.Controls.Add(iconTab);
		}
		
		KeyDown += delegate (object o, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape: Close(); break;
				case Keys.F1: About.Display(); break;
			}
		};
	}
}