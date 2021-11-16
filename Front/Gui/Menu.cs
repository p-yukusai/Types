using System;
using System.Drawing;
using System.Windows.Forms;

class Menu : ContextMenuStrip
{
	public Menu ()
	{
		Renderer = new SimpleRenderer();
	}
	
	public void Display ()
	{
		Show(Cursor.Position);
	}
	
	class SimpleRenderer : ToolStripProfessionalRenderer
	{
		protected override void OnRenderImageMargin (ToolStripRenderEventArgs e) { }
	}
	
	public class Row : ToolStripMenuItem
	{
		public Row (string text)
		{
			Text = Own.Line(text);
			Click += delegate { Clicked(); };
		}
		
		public Row (string icon, string text)
		{
			Image = Own.Image(icon);
			Text = Own.Line(text);
			Click += delegate { Clicked(); };
		}
		
		public Row (string icon, string text, Keys keys)
		{
			Image = Own.Image(icon);
			Text = Own.Line(text);
			ShortcutKeys = keys;
			Click += delegate { Clicked(); };
		}
		
		public bool Bold {
			set {
				if (value) Font = new Font(Font, FontStyle.Bold);
				else Font = new Font(Font, FontStyle.Regular);
			}
		}
		
		public event Action Clicked;
	}
}