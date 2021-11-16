using System;
using System.Reflection;
using System.Windows.Forms;

[assembly: AssemblyTitle("Types")]

static class Types
{
	[STAThread] static void Main (string[] args)
	{
		Application.EnableVisualStyles();
		
		if (Options.Has("Tongue"))
		{
			Own.Tongue = (string) Options.Get("Tongue");
		}
		
		if (args.Length > 0)
		{
			string[] sep = args[0].Split('.');
			
			string type;
			if (sep.Length > 1) type = "." + sep[sep.Length - 1];
			else type = "Unknown";
			
			Application.Run(new TypeForm(type));
		}
		
		else Application.Run(new ListForm());
		
		OS.FlushIcons();
	}
}