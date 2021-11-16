using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class ResIconList : ListView
{
	ImageList icons;
	
	string file = null;
	int index = 0;
	
	public string File { get { return file; } }
	public int Index { get { return index; } }
	
	public event Action Changed;
	
	public string Path
	{
		get { return file + "," + index.ToString(); }
		set
		{
			string newfile = ResIcon.GetFile(value);
			
			if (file == newfile) return;
			if (!(new ResIcon(newfile, 0).Valid)) return;
			
			file = newfile;
			index = ResIcon.GetIndex(newfile);
			
			Reveal();
		}
	}
	
	public ResIconList ()
	{
		View = View.Tile;
		MultiSelect = false;
		TileSize = new Size(40, 40);
		DoubleBuffered = true;
		
		icons = new ImageList();
		icons.ColorDepth = ColorDepth.Depth32Bit;
		icons.ImageSize = new Size(32, 32);
		LargeImageList = icons;
		
		SelectedIndexChanged += delegate
		{
			if (
				SelectedItems.Count == 1 &&
				SelectedItems[0].Selected
			) {
				index = SelectedItems[0].Index;
				if (Changed != null) Changed();
			}
		};
	}
	
	void Reveal ()
	{
		BeginUpdate();
		
		Clear();
		icons.Images.Clear();
		
		int i = 0;
		ResIcon res;
		
		do {
			if ((res = new ResIcon(file, i)).Valid)
			{
				icons.Images.Add(res.Get(32));
				Items.Add("", i); i++;
			}
		} while (res.Valid);
		
		EndUpdate();
	}
}