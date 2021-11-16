using System;
using System.Windows.Forms;

class Radio : RadioButton
{
	public event Action <bool> Changed;
	
	public Radio (string label)
	{
		Text = Own.Line(label);
		
		CheckedChanged += delegate
		{
			if (Enabled && Changed != null)
			{
				Changed(Checked);
			}
		};
	}
	
	public void Set (bool on)
	{
		bool e = Enabled;
		Enabled = false;
		Checked = on;
		Enabled = e;
	}
}