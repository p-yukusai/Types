using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Deployment
{
	class Installer
	{
		public readonly string ID;
		public Installer () { ID = Application.ProductName; }
		public Installer (string id) { ID = id; }
		
		static readonly string ProgramFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
		static readonly string DesktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		static readonly string StartDir = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
		
		const string URKPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
		static readonly RegistryKey URK = Registry.LocalMachine.OpenSubKey(URKPath, true);
		
		static readonly Assembly Ass = Assembly.GetExecutingAssembly();
		static readonly string OwnPath = Ass.Location;
		static readonly string OwnDir = Path.GetDirectoryName(OwnPath);
		
		public readonly string ProductName = Application.ProductName;
		public readonly string ProductVersion = Application.ProductVersion.ToString();
		public readonly string ProductAuthor = Application.CompanyName;
		public string ProductInfoLink = null;
		
		public string MainExe = null;
		public string RemoveCommand = null;
		public string InstallDir = ProgramFilesDir + "\\" + Application.ProductName;
		
		Dictionary<string, string> filesToDeploy = new Dictionary<string, string>();
		Dictionary<string, string> textToDeploy = new Dictionary<string, string>();
		Dictionary<string, string> desktopShortcuts = new Dictionary<string, string>();
		Dictionary<string, string> startShortcuts = new Dictionary<string, string>();
		
		List<string> dirsToRemove = new List<string>();
		List<string> filesToRemove = new List<string>();
		List<string> regsToRemove = new List<string>();
		
		int deployedBytes = 0;
		
		public void AddFileToDeploy (string name) { AddFileToDeploy(name, name); }
		public void AddFileToDeploy (string name, string outName) { filesToDeploy.Add(name, outName); }
		public void AddTextToDeploy (string text, string outName) { textToDeploy.Add(outName, text); }
		public void AddDesktopShortcut (string lnkName, string target) { desktopShortcuts[lnkName] = target; }
		public void AddStartShortcut (string lnkName, string target) { startShortcuts[lnkName] = target; }
		public void RememberKey (string key) { regsToRemove.Add(key); }
		public void RememberValue (string key, string name) { regsToRemove.Add(key + ":" + name); }
		
		public void Go ()
		{
			if (MainExe == null) throw new InvalidOperationException("MainExe is null");
			if (RemoveCommand == null) throw new InvalidOperationException("RemoveCommand is null");
			
			if (!Directory.Exists(InstallDir)) Directory.CreateDirectory(InstallDir);
			dirsToRemove.Add(InstallDir);
			
			foreach (KeyValuePair<string, string> ftd in filesToDeploy) DeployFile(ftd.Key, InstallDir + "\\" + ftd.Value);
			foreach (KeyValuePair<string, string> ttd in textToDeploy) DeployText(ttd.Value, InstallDir + "\\" + ttd.Key);
			foreach (KeyValuePair<string, string> dln in desktopShortcuts) DeployShortcut(DesktopDir, dln.Key, dln.Value);
			foreach (KeyValuePair<string, string> sln in startShortcuts) DeployShortcut(StartDir, sln.Key, sln.Value);
			
			using (RegistryKey ek = URK.CreateSubKey(ID))
			{
				string mainExePath = InstallDir + "\\" + filesToDeploy[MainExe];
				string removeCommandPath = InstallDir + "\\" + filesToDeploy[RemoveCommand];
				
				ek.SetValue("DisplayName", ProductName);
				ek.SetValue("Publisher", ProductAuthor);
				ek.SetValue("DisplayVersion", ProductVersion);
				if (ProductInfoLink != null) ek.SetValue("URLInfoAbout", ProductInfoLink);
				ek.SetValue("DisplayIcon", mainExePath);
				ek.SetValue("UninstallString", removeCommandPath);
				ek.SetValue("EstimatedSize", deployedBytes / 1024);
				ek.SetValue("DirsToRemove", dirsToRemove.ToArray());
				ek.SetValue("FilesToRemove", filesToRemove.ToArray());
				ek.SetValue("RegsToRemove", regsToRemove.ToArray());
				ek.SetValue("NoModify", 1);
				ek.SetValue("NoRepair", 1);
			}
		}
		
		void DeployShortcut (string lnkDir, string lnkName, string targetName)
		{
			string lnkPath = lnkDir + "\\" + lnkName + ".url";
			string targetPath = InstallDir + "\\" + targetName;
			
			string lnk = "[InternetShortcut]\r\n";
			lnk += "URL=" + targetPath + "\r\n";
			lnk += "IconFile=" + targetPath + "\r\n";
			lnk += "IconIndex=0\r\n";
			
			DeployText(lnk, lnkPath);
		}
		
		void DeployFile (string name, string outPath)
		{
			Stream input = Ass.GetManifestResourceStream(name);
			if (input == null) input = new FileStream(OwnDir + "\\" + name, FileMode.Open);
			
			using (Stream output = File.Create(outPath))
			{
				for (int b = input.ReadByte(); b != -1; b = input.ReadByte()) output.WriteByte((byte)b);
				deployedBytes += (int) output.Length;
			}
			
			filesToRemove.Add(outPath);
		}
		
		void DeployText (string text, string outPath)
		{
			using (StreamWriter output = new StreamWriter(outPath))
			{
				output.Write(text); output.Flush();
				deployedBytes += (int) output.BaseStream.Length;
			}
			
			filesToRemove.Add(outPath);
		}
	}
}