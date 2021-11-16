using System;
using System.Windows.Forms;

class TickBox : CheckBox
{
	public TickBox ()
	{
		CheckedChanged += delegate
		{
			if (Enabled && Changed != null)
			{
				Changed(Checked);
			}
		};
	}
	
	public TickBox (string label) : this ()
	{
		Text = label;
	}
	
	public void Set (bool on)
	{
		bool e = Enabled;
		Enabled = false;
		Checked = on;
		Enabled = e;
	}
	
	public event Action <bool> Changed;
}