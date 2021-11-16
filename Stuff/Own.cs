using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

static class Own
{
	public const string DefaultTongueCode = "en";
	public const string DefaultTongueName = "English";
	
	static Assembly ass;
	static string tongue;
	
	public static readonly string Name;
	public static readonly string Place;
	
	public static string Tongue
	{
		get { return tongue; }
		set { tongue = value; Prepare(); }
	}
	
	static Own ()
	{
		ass = Assembly.GetExecutingAssembly();
		
		tongue = CultureInfo.CurrentUICulture.ToString().Split('-')[0];
		Place = Path.GetDirectoryName(ass.CodeBase).Replace("file:\\", "");
		
		foreach (object att in ass.GetCustomAttributes(false))
		{
			if (att.GetType() == typeof(AssemblyProductAttribute))
			{
				Name = (att as AssemblyProductAttribute).Product;
			}
		}
		
		Prepare();
	}
	
	public static string[] Items
	{
		get
		{
			List<string> items = new List<string>();
			
			foreach (string name in ass.GetManifestResourceNames())
			{
				items.Add(name);
			}
			
			foreach (FileInfo file in new DirectoryInfo(Place).GetFiles())
			{
				string name = file.Name;
				if (!items.Contains(name)) items.Add(name);
			}
			
			return items.ToArray();
		}
	}
	
	public static Dictionary<string, string> Tongues
	{
		get
		{
			Dictionary<string, string> tongues = new Dictionary<string, string>();
			
			tongues.Add(DefaultTongueCode, DefaultTongueName);
			
			string[] all = Items;
			
			foreach (string one in all)
			{
				if (one.EndsWith(".tongue"))
				{
					using (StreamReader r = new StreamReader(Get(one)))
					{
						tongues[one.Substring(0, one.Length - 7)] = r.ReadLine().Trim();
					}
				}
			}
			
			return tongues;
		}
	}
	
	static Stream Load (string name)
	{
		Stream str = ass.GetManifestResourceStream(name);
		if (str != null) return str;
		
		try { return new FileStream(Place + "\\" + name, FileMode.Open, FileAccess.Read); }
		catch { return null; }
	}
	
	public static Stream Get (string name, bool local)
	{
		Stream s = null;
		if (local) s = Load(tongue + "." + name);
		if (s == null) s = Load(name);
		return s;
	}
	
	public static Stream Get (string name)
	{
		return Get(name, true);
	}
	
	static string Fix (string name, string type)
	{
		if (name.Contains(".")) return name;
		return name + "." + type;
	}
	
	static Stream Get (string name, string type)
	{
		return Get(Fix(name, type));
	}
	
	public static Icon Icon (string name)
	{
		return new Icon(Get(name, "ico"));
	}
	
	public static Image Image (string name)
	{
		return System.Drawing.Image.FromStream(Get(name, "png"));
	}
	
	public static string Read (string name, bool local)
	{
		using (StreamReader r = new StreamReader(Get(name, local))) return r.ReadToEnd();
	}
	
	public static string Read (string name)
	{
		return Read(name, true);
	}
	
	public static string Text (string name)
	{
		using (StreamReader r = new StreamReader(Get(name, "txt"))) return r.ReadToEnd();
	}
	
	static Dictionary<string, string> lines;
	
	static string[] SplitToLines (string s)
	{
		string[] lfs = new string[] {"\r\n", "\n", "\r"};
		return s.Split(lfs, StringSplitOptions.None);
	}
	
	static void Prepare ()
	{
		lines = new Dictionary<string, string>();
		List<string> list = new List<string>();
		
		try { list.AddRange(SplitToLines(Read("tongue", true))); } catch {}
		
		foreach (string line in list)
		{
			if (line.Contains("="))
			{
				string[] pair = line.Split('=');
				lines[pair[0].Trim()] = pair[1].Trim();
			}
		}
	}
	
	public static string Line (string key) { return Line(key, key); }
	public static string Line (string key, string def)
	{
		if (lines.ContainsKey(key)) return lines[key];
		else return def;
	}
}