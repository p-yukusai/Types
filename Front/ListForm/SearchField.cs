using System;
using System.Drawing;
using System.Windows.Forms;

class SearchField : Field
{
	PictureBox btn;
	
	Font emptyFont;
	Font normalFont;
	
	bool empty = true;
	
	public SearchField ()
	{
		normalFont = Font;
		emptyFont = new Font(Font, FontStyle.Italic);
		
		GotFocus += delegate { if (empty) Set(""); else SelectAll(); empty = false; SwitchState(); };
		LostFocus += delegate { empty = Text.Length < 1; SwitchState(); };
		
		btn = new PictureBox();
		btn.Size = new Size(16, 16);
		btn.Cursor = Cursors.Arrow;
		Controls.Add(btn);
		
		Resize += delegate {
			btn.Location = new Point(ClientSize.Width - btn.Width, 0);
		};
		
		btn.Click += delegate {
			if (!empty) Text = "";
			if (!Focused) Focus();
			if (Cleared != null) Cleared();
		};
		
		SwitchState();
	}
	
	void SwitchState ()
	{
		if (empty) {
			Font = emptyFont;
			ForeColor = SystemColors.GrayText;
			Set(Own.Line("Search"));
			btn.Image = Own.Image("Search");
		} else {
			Font = normalFont;
			ForeColor = SystemColors.WindowText;
			btn.Image = Own.Image("Clear");
		}
	}
	
	public void Reset ()
	{
		Set("");
		SwitchState();
	}
	
	public event Action Cleared;
}