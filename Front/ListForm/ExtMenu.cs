using System;
using System.Windows.Forms;

class ExtMenu : Menu
{
	public Menu.Row EditRow;
	public Menu.Row DeleteRow;
	
	public ExtMenu()
	{
		EditRow = new Menu.Row("Edit", "Properties"); EditRow.Bold = true;
		DeleteRow = new Menu.Row("Delete", "Delete", Keys.Delete);
		
		Items.Add(EditRow);
		Items.Add(DeleteRow);
	}
}