using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

class TongueCombo : ComboBox
{
	public event Action <string> TongueSelected;
	
	Dictionary<string, string> tongues = Own.Tongues;
	Dictionary<string, string> seugnot = new Dictionary<string, string>();
	
	public TongueCombo ()
	{
		DropDownStyle = ComboBoxStyle.DropDownList;
		
		foreach (KeyValuePair<string,string> vp in tongues)
		{
			seugnot[vp.Value] = vp.Key;
			Items.Add(vp.Value);
		}
		
		if (tongues.ContainsKey(Own.Tongue)) {
			Text = tongues[Own.Tongue];
		} else Text = Own.DefaultTongueName;
		
		SelectionChangeCommitted += delegate { TongueSelected(Tongue); };
	}
	
	public string Tongue {
		get {
			return seugnot[SelectedItem.ToString()];
		}
	}
}