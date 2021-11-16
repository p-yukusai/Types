using System;
using System.Drawing;
using System.Windows.Forms;

class VerbIconTab : Tab
{
	public event Action IconChanged;
	IconSelector icosel = new IconSelector();
	
	public VerbIconTab (Verb verb) : base("Icon")
	{
		Controls.Add(icosel);
		
		Enter += delegate
		{
			if (verb.Icon != null) icosel.Path = verb.Icon;
			else {
				if (verb.Command != null) {
					if (!string.IsNullOrEmpty(verb.Program)) {
						icosel.DefaultPath = verb.Program;
					}
				}
				
				icosel.Path = null;
			}
		};
		
		icosel.Changed += delegate
		{
			try { verb.Icon = icosel.Path; IconChanged(); }
			catch (Exception x) { Tip.Boo(icosel, x); }
		};
		
		icosel.Removed += delegate
		{
			try { verb.Icon = null; IconChanged(); }
			catch (Exception x) { Tip.Boo(icosel, x); }
		};
	}
}