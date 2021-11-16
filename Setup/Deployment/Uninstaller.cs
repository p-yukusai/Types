using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Deployment
{
	class Uninstaller
	{
		public readonly string ID;
		public Uninstaller () { ID = Application.ProductName; }
		public Uninstaller (string id) { ID = id; }
		
		static readonly Assembly Ass = Assembly.GetExecutingAssembly();
		static readonly string OwnPath = Ass.Location;
		static readonly string OwnDir = Path.GetDirectoryName(OwnPath);
		
		const string URKPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
		static RegistryKey URK = Registry.LocalMachine.OpenSubKey(URKPath, true);
		
		public void Go ()
		{
			RegistryKey uk = URK.OpenSubKey(ID);
			
			string[] files = uk.GetValue("FilesToRemove") as string[];
			string[] dirs = uk.GetValue("DirsToRemove") as string[];
			string[] regs = uk.GetValue("RegsToRemove") as string[];
			
			List<string> fails = new List<string>();
			
			if (files != null) foreach (string path in files)
			{
				try { File.Delete(path); }
				catch { fails.Add(path); }
			}
			
			if (dirs != null) foreach (string path in dirs)
			{
				try { Directory.Delete(path); }
				catch { fails.Add(path); }
			}
			
			if (regs != null) foreach (string path in regs)
			{
				try {
					
					string[] split = path.Split(':');
					
					string key = split.Length == 2 ? split[0] : path;
					string valn = split.Length == 2 ? split[1] : null;
					
					if (Registry.CurrentUser.OpenSubKey(path) != null)
					{
						if (valn == null) Registry.CurrentUser.DeleteSubKeyTree(key);
						else Registry.CurrentUser.OpenSubKey(key, true).DeleteValue(valn, false);
					}
					
					if (Registry.LocalMachine.OpenSubKey(path) != null)
					{
						if (valn == null) Registry.LocalMachine.DeleteSubKeyTree(key);
						else Registry.LocalMachine.OpenSubKey(key, true).DeleteValue(valn, false);
					}
					
				} catch { fails.Add(path); }
			}
			
			URK.DeleteSubKeyTree(ID);
		}
		
		public static void Auto (string id, string[] args)
		{
			if (args.Length == 1) new Deployment.Uninstaller(args[0]).Go();
			else {
				string temp = Path.GetTempPath() + "\\" + id + ".UnDep.exe";
				File.Copy(OwnPath, temp, true);
				Process.Start(temp, id);
			}
		}
	}
}