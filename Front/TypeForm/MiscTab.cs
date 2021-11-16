using System;
using System.Drawing;
using System.Windows.Forms;

class MiscTab : Tab
{
	public readonly Type Type;
	
	GroupBox extViGroup;
	
	Radio followExtRadio = new Radio("Follow Explorer settings");
	Radio neverExtRadio = new Radio("Never show extension");
	Radio alwaysExtRadio = new Radio("Always show extension");
	
	Field percField = new Field();
	TickBox percCk = new TickBox();
	
	public MiscTab (Type type) : base("Other")
	{
		Type = type;
		Enter += delegate { Reveal(); };
		
		percCk.Text = Own.Line("Perceived type") + ":";
		percCk.Location = new Point(20, 20);
		percCk.Width = Width - 40;
		percCk.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		Controls.Add(percCk);
		
		percField.Location = new Point(20, percCk.Bottom);
		percField.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		percField.Width = Width - 40;
		Controls.Add(percField);
		
		percCk.Changed += delegate (bool on)
		{
			if (on) percField.Enabled = true;
			else try {
				Type.Perceived = null;
				percField.Enabled = false;
				percField.Set(null);
			} catch (Exception e) {
				Tip.Boo(percField, e);
				percCk.Set(true);
			}
		};
		
		percField.Changed += delegate ()
		{
			try { Type.Perceived = percField.Text; }
			catch (Exception e) { Tip.Boo(percField, e); }
		};
		
		extViGroup = new GroupBox();
		extViGroup.Text = Own.Line("Extension visibility");
		extViGroup.Width = Width - 40;
		extViGroup.Location = new Point(20, percField.Bottom + 20);
		extViGroup.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		
		followExtRadio.Location = new Point(20, 23);
		followExtRadio.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		followExtRadio.Width = extViGroup.Width - 40;
		extViGroup.Controls.Add(followExtRadio);
		
		alwaysExtRadio.Location = new Point(20, followExtRadio.Top + followExtRadio.Height);
		alwaysExtRadio.Anchor = followExtRadio.Anchor;
		alwaysExtRadio.Width = followExtRadio.Width;
		extViGroup.Controls.Add(alwaysExtRadio);
		
		neverExtRadio.Location = new Point(20, alwaysExtRadio.Top + alwaysExtRadio.Height);
		neverExtRadio.Anchor = followExtRadio.Anchor;
		neverExtRadio.Width = alwaysExtRadio.Width;
		extViGroup.Controls.Add(neverExtRadio);
		
		followExtRadio.Changed += delegate (bool on) { SetShowExt(followExtRadio, 0); };
		alwaysExtRadio.Changed += delegate (bool on) { SetShowExt(alwaysExtRadio, +1); };
		neverExtRadio.Changed += delegate (bool on) { SetShowExt(neverExtRadio, -1); };
		
		extViGroup.Height = alwaysExtRadio.Top + alwaysExtRadio.Height + 40;
		Controls.Add(extViGroup);
	}
	
	void SetShowExt (Radio sender, int value)
	{
		if (sender.Checked) try { Type.ShowExtension = value; }
		catch (Exception e) { Tip.Boo(sender, e); }
	}
	
	void Reveal ()
	{
		followExtRadio.Set(Type.ShowExtension == 0);
		neverExtRadio.Set(Type.ShowExtension < 0);
		alwaysExtRadio.Set(Type.ShowExtension > 0);
		
		percCk.Set(Type.Perceived != null);
		percField.Enabled = percCk.Checked;
		percField.Set(Type.Perceived);
	}
}