using System;
using System.Windows.Forms;

class Field : TextBox
{
	public Field ()
	{
		TextChanged += delegate
		{
			if (allowEvents && Changed != null)
			{
				Changed();
			}
		};
		
		KeyDown += delegate (object o, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				if (Entered != null) Entered();
				e.SuppressKeyPress = true;
			}
		};
	}
	
	bool allowEvents = true;
	
	public void Set (string text)
	{
		allowEvents = false;
		Text = text != null ? text : "";
		allowEvents = true;
	}
	
	public event Action Changed;
	public event Action Entered;
}