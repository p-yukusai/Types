using System;
using System.Windows.Forms;

class VerbMenu : Menu
{
	public Menu.Row EditRow = new Menu.Row("Edit", "Properties");
	public Menu.Row DefaultRow = new Menu.Row("Default");
	public Menu.Row ExtendedRow = new Menu.Row("Hidden");
	public Menu.Row RenameRow = new Menu.Row("Rename", "Rename", Keys.F2);
	public Menu.Row DeleteRow = new Menu.Row("Delete", "Delete", Keys.Delete);
	
	public VerbMenu ()
	{
		EditRow.Bold = true;
		
		Items.Add(DefaultRow);
		Items.Add(ExtendedRow);
		Items.Add(new ToolStripSeparator());
		Items.Add(RenameRow);
		Items.Add(DeleteRow);
		Items.Add(new ToolStripSeparator());
		Items.Add(EditRow);
	}
}