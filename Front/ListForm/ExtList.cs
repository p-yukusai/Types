using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

class ExtList : ListView
{
	PictureBox spinner;
	
	ImageList icons;
	
	public Dictionary<string, string> typeCache;
	public Dictionary<string, string> iconCache;
	public Dictionary<string, string> textCache;
	
	public ExtList ()
	{
		typeCache = new Dictionary<string, string>();
		iconCache = new Dictionary<string, string>();
		textCache = new Dictionary<string, string>();
		
		icons = new ImageList();
		icons.ColorDepth = ColorDepth.Depth32Bit;
		icons.ImageSize = new Size(32, 32);
		
		icons.Images.Add("Default", OS.BlankIcon.Get(32));
		icons.Images.Add("%1", Own.Icon("Custom"));
		
		LargeImageList = icons;
		View = View.LargeIcon;
		DoubleBuffered = true;
		
		spinner = new PictureBox();
		spinner.Image = Own.Image("Loading.gif");
		spinner.Size = new Size(spinner.Image.Width, spinner.Image.Height);
		Controls.Add(spinner);
		
		Resize += delegate
		{
			spinner.Location = new Point (
				ClientSize.Width / 2 - spinner.Width / 2,
				ClientSize.Height / 2 - spinner.Height / 2
			);
		};
	}
	
	public void CacheType (string type)
	{
		Type t = new Type(type);
		
		string link = null;
		string icon = null;
		string title = null;
		
		try { link = t.Class; icon = t.Icon; title = t.Title; }
		catch (System.Security.SecurityException) { return; }
		
		if (link == null) typeCache[type] = type;
		else typeCache[type] = link;
		
		if (icon == null || icon == "") icon = "Default";
		else if (icon != "%1") {
			if (!icons.Images.ContainsKey(icon)) {
				ResIcon res = new ResIcon(icon);
				if (res.Valid) icons.Images.Add(icon, res.Get(32));
				else icon = "Default";
			}
		}
		
		iconCache[type] = icon;
		textCache[type] = (type + link + title).ToLower();
	}
	
	public void UpdateByClass (string type)
	{
		string[] keys = new string[typeCache.Keys.Count];
		
		typeCache.Keys.CopyTo(keys, 0);
		
		foreach (string key in keys)
		{
			if (typeCache[key] == type) UpdateExt(key);
		}
	}
	
	public void UpdateExt (string type)
	{
		if (Items.ContainsKey(type))
		{
			CacheType(type);
			Items[type].ImageKey = iconCache[type];
		}
	}
	
	public void UpdateType (string type)
	{
		if (type.StartsWith(".")) UpdateExt(type);
		else UpdateByClass(type);
	}
	
	public void ShowExt (string type)
	{
		string icon;
		
		if (iconCache.ContainsKey(type)) icon = iconCache[type];
		else icon = "error";
		
		Items.Add(type, type.Remove(0, 1), icon);
	}
	
	public void LoadTypes ()
	{
		spinner.Visible = true;
		
		typeCache.Clear();
		iconCache.Clear();
		textCache.Clear();
		
		foreach (string type in Type.Extensions)
		{
			CacheType(type);
			Application.DoEvents();
		}
		
		spinner.Visible = false;
		
		List();
	}
	
	public void List (string search)
	{
		if (SelectedItems.Count > 0)
		{
			foreach (ListViewItem i in SelectedItems)
			{
				i.Selected = false;
			}
		}
		
		Clear();
		
		bool all = String.IsNullOrEmpty(search);
		if (!all) search = search.ToLower();
		
		BeginUpdate();
		
		foreach (KeyValuePair<string, string> pair in textCache)
		{
			if (all) ShowExt(pair.Key);
			else if (pair.Value.Contains(search)) ShowExt(pair.Key);
		}
		
		EndUpdate();
	}
	
	public void List ()
	{
		List(null);
	}
	
	public void Remove (string type)
	{
		typeCache.Remove(type);
		iconCache.Remove(type);
		textCache.Remove(type);
		Items.RemoveByKey(type);
	}
}