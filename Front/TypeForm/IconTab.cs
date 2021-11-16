using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

class IconTab : Tab
{
	public readonly Type Type;
	IconSelector icosel = new IconSelector();
	public event Action IconChanged;
	
	public IconTab (Type type) : base("Icon")
	{
		Type = type;
		Controls.Add(icosel);
		
		Enter += delegate
		{
			icosel.Path = Type.Icon;
		};
		
		icosel.Changed += delegate
		{
			try { Type.Icon = icosel.Path; IconChanged(); }
			catch (Exception e) { Tip.Boo(this, e); }
		};
		
		icosel.Removed += delegate
		{
			try { Type.Icon = null; IconChanged(); }
			catch (Exception e) { Tip.Boo(this, e); }
		};
	}
}