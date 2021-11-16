using System;
using System.Drawing;
using System.Runtime.InteropServices;

class ResIcon
{
	[DllImport("shell32.dll")] static extern int ExtractIconEx (string szFileName, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);
	[DllImport("user32.dll")] static extern bool DestroyIcon (IntPtr hIcon);
	
	IntPtr h16, h32;
	Icon i16, i32;
	
	bool valid = true;
	public bool Valid { get { return valid; } }
	
	public Icon Get (int size)
	{
		if (size <= 16) return new Icon(i16, new Size(size, size));
		else return new Icon(i32, new Size(size, size));
	}
	
	public ResIcon (string file, int index)
	{
		Load(file, index);
	}
	
	public ResIcon (string path)
	{
		Load(GetFile(path), GetIndex(path));
	}
	
	void Load (string file, int index)
	{
		try {
			ExtractIconEx(file, index, out h32, out h16, 1);
			i16 = Icon.FromHandle(h16);
			i32 = Icon.FromHandle(h32);
		} catch {
			valid = false;
		}
	}
	
	public static string GetFile (string path)
	{
		return path.Split(',')[0].Trim().Trim('"');
	}
	
	public static int GetIndex (string path)
	{
		if (path.Contains(",")) {
			try { return Convert.ToInt32(path.Split(',')[1].Trim()); }
			catch { return 0; }
		} else return 0;
	}
	
	~ResIcon ()
	{
		DestroyIcon(h16);
		DestroyIcon(h32);
	}
}