using System.Runtime.InteropServices;

static class OS
{
	public static readonly ResIcon BlankIcon;
	
	public static readonly int MajorVersion = System.Environment.OSVersion.Version.Major;
	public static readonly int MinorVersion = System.Environment.OSVersion.Version.Minor;
	
	public static readonly bool Vista = MajorVersion > 5;
	public static readonly bool Seven = Vista && MinorVersion > 0;
	
	[DllImport("shell32.dll")]
	static extern void SHChangeNotify (uint wEventId, uint uFlags, int dwItem1, int dwItem2);
	public static void FlushIcons () { SHChangeNotify(0x08000000, 0x0000, 0, 0); }
	
	static OS ()
	{
		if (Seven) BlankIcon = new ResIcon("imageres.dll", 2);
		else if (Vista) BlankIcon = new ResIcon("imageres.dll", 1);
		else BlankIcon = new ResIcon("shell32.dll");
	}
}