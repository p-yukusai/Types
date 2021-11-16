using System;
using System.Drawing;
using System.Windows.Forms;

class VerbList : ListBox
{
	public readonly Verbs Verbs;
	
	ImageList icons = new ImageList();
	static readonly Bitmap defaultIcon = new ResIcon("shell32.dll", 2).Get(16).ToBitmap();
	
	public Verb SelectedVerb
	{
		get {
			if (!Selected) return null;
			return Verbs[SelectedName];
		}
		
		set {
			if (!Items.ContainsKey(value.ID)) return;
			Items[value.ID].Selected = true;
		}
	}
	
	public VerbList (Verbs verbs)
	{
		Verbs = verbs;
		
		icons.ColorDepth = ColorDepth.Depth32Bit;
		icons.ImageSize = new Size(16, 16);
		
		View = View.List;
		SmallImageList = icons;
		HideSelection = false;
	}
	
	public void Reload ()
	{
		BeginUpdate();
		
		Clear();
		icons.Images.Clear();
		
		foreach (Verb v in Verbs)
		{
			icons.Images.Add(defaultIcon);
			
			ListViewItem item = Items.Add(v.ID);
			item.ImageIndex = item.Index;
			item.Name = v.ID;
			
			SetVerb(v);
		}
		
		EndUpdate();
	}
	
	void SetVerb (Verb v)
	{
		ListViewItem item = Items[v.ID];
		
		item.Text = v.Title;
		
		if (v.IsExtended) item.ForeColor = Color.Gray;
		else if (v.IsDefault) item.ForeColor = Color.Green;
		
		ResIcon ico = null;
		if (OS.Vista && v.Icon != null) ico = new ResIcon(v.Icon);
		else if (v.Command != null) ico = new ResIcon(v.Program, 0);
		if (ico != null && ico.Valid) icons.Images[item.Index] = ico.Get(16).ToBitmap();
		else icons.Images[item.Index] = defaultIcon;
	}
	
	public void UpdateVerb (Verb v)
	{
		SetVerb(v);
		Refresh();
	}
}