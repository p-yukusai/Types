using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

class SpecialMenu : Menu
{
	void AddRow (string type)
	{
		if (!Type.Exists(type)) return;
		Row row = new Row(type); row.Tag = type;
		row.Clicked += delegate { Selected(type); };
		Items.Add(row);
	}
	
	void ReloadIcons ()
	{
		foreach (Row row in Items)
		{
			string icon = new Type(row.Tag as string).Icon;
			ResIcon ri = (icon == "" || icon == null) ? OS.BlankIcon : new ResIcon(icon);
			if (ri.Valid) row.Image = ri.Get(16).ToBitmap(); else row.Image = null;
		}
	}
	
	public SpecialMenu ()
	{
		AddRow("*");
		AddRow("Unknown");
		AddRow("Folder");
		AddRow("Directory");
		AddRow("LibraryFolder");
		AddRow("Library");
		AddRow("Drive");
		
		Opening += delegate { ReloadIcons(); };
	}
	
	public event Action <string> Selected;
}