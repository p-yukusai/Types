using System;
using System.Drawing;
using System.Media;
using System.Security;
using System.Windows.Forms;
using System.Collections.Generic;

class Tip : ToolTip
{
	static List<ToolTip> tips = new List<ToolTip>();
	
	public static void Set (Control target, string text, string shortcut)
	{
		ToolTip tip = new ToolTip();
		
		text = Own.Line(text);
		if (shortcut != null) text += " (" + shortcut + ")";
		
		tip.SetToolTip(target, text);
		
		tips.Add(tip);
	}
	
	public static void Set (Control target, string text)
	{
		Set(target, text, null);
	}
	
	public static void Boo () { SystemSounds.Beep.Play(); }
	public static void Boo (Control c, Exception e)
	{
		ToolTip ot = new ToolTip();
		
		ot.IsBalloon = true;
		ot.ToolTipIcon = ToolTipIcon.Error;
		ot.ToolTipTitle = e.Message;
		string text = e.StackTrace;
		
		if (e is SecurityException)
		{
			ot.ToolTipTitle = Own.Line("Access denied");
			text = Own.Line("Run as administrator");
		}
		
		Point pos = new Point(c.Width / 2, c.Height / 2);
		
		ot.Show(null, c, pos);
		ot.Show(text, c, pos, 3333);
		
		SystemSounds.Beep.Play();
		Console.WriteLine(e);
	}
}