using Microsoft.Win32;

static class Options
{
	static readonly RegistryKey key;
	static Options () { key = Registry.CurrentUser.CreateSubKey("Software\\" + Own.Name); }
	public static bool Has (string valueName) { return key.GetValue(valueName) != null; }
	public static object Get (string valueName) { return key.GetValue(valueName); }
	public static void Set (string name, object value) { key.SetValue(name, value); }
}