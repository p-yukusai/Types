using System;
using System.Collections.Generic;
using System.Windows.Forms;

class Combo : ComboBox
{
	const int toKeep = 16;
	
	public event Action Changed;
	public event Action Switched;
	
	public readonly string ID;
	
	public Combo () : this (null) {}
	public Combo (string id)
	{
		ID = id;
		
		if (Options.Has(id))
		{
			foreach (string line in Options.Get(ID) as string[])
			{
				if (!string.IsNullOrEmpty(line)) Items.Add(line);
			}
		}
		
		TextChanged += delegate
		{
			if (Enabled && Changed != null)
			{
				Changed();
			}
		};
		
		SelectedIndexChanged += delegate
		{
			if (Enabled && Switched != null)
			{
				Switched();
			}
		};
	}
	
	public void Remember (string s)
	{
		if (String.IsNullOrEmpty(s)) return;
		List<string> lines = new List<string>();
		if (Options.Has(ID)) lines.AddRange(Options.Get(ID) as string[]);
		if (!lines.Contains(s)) lines.Insert(0, s); else return;
		if (lines.Count > toKeep) lines.RemoveRange(toKeep, lines.Count - toKeep);
		Options.Set(ID, lines.ToArray());
	}
	
	public void Set (string text)
	{
		bool e = Enabled;
		Enabled = false;
		Text = text != null ? text : "";
		Enabled = e;
	}
	
	public void SetIndex (int i)
	{
		bool e = Enabled;
		Enabled = false;
		SelectedIndex = i;
		Enabled = e;
	}
}