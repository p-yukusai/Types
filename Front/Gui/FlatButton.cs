using System;
using System.Drawing;
using System.Windows.Forms;

class FlatButton : Button
{
	Image icon;
	Image fade;
	
	public FlatButton (Image image)
	{
		Icon = image;
		Size = new Size(icon.Width + 6, icon.Height + 6);
		
		FlatStyle = FlatStyle.Flat;
		FlatAppearance.BorderSize = 0;
		BackgroundImageLayout = ImageLayout.Center;
		
		EnabledChanged += delegate { ShowIcon(); };
		Click += delegate { if (Clicked != null) Clicked(); };
		MouseEnter += delegate { FlatStyle = FlatStyle.Popup; };
		MouseLeave += delegate { FlatStyle = FlatStyle.Flat; };
		
		ShowIcon();
	}
	
	public Image Icon
	{
		get { return icon; }
		set {
			icon = value;
			fade = Fade(value);
		}
	}
	
	Image Fade (Image from)
	{
		Bitmap to = new Bitmap(from);
		
		for (int y = 0; y < to.Height; y++)
		{
			for (int x = 0; x < to.Width; x++)
			{
				Color fc = to.GetPixel(x, y);
				to.SetPixel(x, y, Color.FromArgb(fc.A / 3, fc.R, fc.G, fc.B));
			}
		}
		
		return to;
	}
	
	void ShowIcon ()
	{
		if (Enabled) BackgroundImage = icon;
		else BackgroundImage = fade;
	}
	
	public event Action Clicked;
}