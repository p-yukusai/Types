using System;
using System.Drawing;
using System.Windows.Forms;

class ListBox : ListView
{
	public ListBox ()
	{
		DoubleBuffered = true;
	}
	
	public bool Selected
	{
		get { return SelectedItems.Count > 0; }
	}
	
	public ListViewItem SelectedItem
	{
		get {
			if (Selected) return SelectedItems[0];
			else return null;
		}
	}
	
	public string SelectedName
	{
		get { return Selected ? SelectedItem.Name : null; }
		set { if (Items.ContainsKey(value)) Items[value].Selected = true; }
	}
	
	public void Deselect ()
	{
		if (Selected) SelectedItem.Selected = false;
	}
}