using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

class About : Form
{
	Label nameLabel = new Label();
	TongueCombo tongueCombo = new TongueCombo();
	
	public About ()
	{
		Text = Own.Line("About");
		StartPosition = FormStartPosition.CenterParent;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = MinimizeBox = false;
		ClientSize = new Size(240, 120);
		KeyPreview = true;
		
		nameLabel.Top = 10;
		nameLabel.Text = Application.ProductName + " " + Application.ProductVersion.ToString();
		nameLabel.Size = new Size(ClientSize.Width, ClientSize.Height / 2);
		nameLabel.TextAlign = ContentAlignment.MiddleCenter;
		nameLabel.Font = new Font(nameLabel.Font.FontFamily, 20, FontStyle.Bold);
		Controls.Add(nameLabel);
		
		tongueCombo.Left = 20;
		tongueCombo.Width = ClientSize.Width - 40;
		tongueCombo.TongueSelected += ChangeTongue;
		tongueCombo.Top = nameLabel.Top + nameLabel.Height + 5;
		Controls.Add(tongueCombo);
		
		KeyDown += delegate (object o, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape: Close(); break;
				case Keys.F1: About.Display(); break;
			}
		};
	}
	
	void ChangeTongue (string tongue)
	{
		Options.Set("Tongue", tongue);
	}
	
	public static void Display ()
	{
		new About().ShowDialog();
	}
}