using System;
using System.Windows.Forms;

class ExtListMenu : Menu
{
	public Menu.Row AddRow;
	public Menu.Row RefreshRow;
	
	public ExtListMenu()
	{
		AddRow = new Menu.Row("Add", "Create", Keys.Control | Keys.N);
		RefreshRow = new Menu.Row("Refresh", "Refresh", Keys.F5);
		
		Items.Add(AddRow);
		Items.Add(RefreshRow);
	}
}