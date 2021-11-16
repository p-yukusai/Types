using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

class ListForm : Form
{
	FlatButton addBtn;
	FlatButton deleteBtn;
	FlatButton editBtn;
	
	FlatButton specialBtn;
	SpecialMenu specialMenu;
	
	FlatButton helpBtn;
	
	SearchField searchField;
	ExtList extList;
	ExtMenu extMenu;
	ExtListMenu noExtMenu;
	
	Field addField;
	Button doAddBtn;
	Button cancelAddBtn;
	
	Dictionary<string, TypeForm> typeForms;
	
	public ListForm ()
	{
		SuspendLayout();
		
		Text = Own.Line("Types");
		Icon = Own.Icon("Types");
		ClientSize = new Size(520, 400);
		StartPosition = FormStartPosition.CenterScreen;
		SizeGripStyle = SizeGripStyle.Show;
		KeyPreview = true;
		
		Shown += delegate { extList.LoadTypes(); };
		
		KeyDown += delegate (object o, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F5: Reload(); break;
				case Keys.F1: About.Display(); break;
				case Keys.N: if (e.Control) ShowAdd(); break;
				case Keys.Escape: if (!addField.Focused) Dispose(); break;
				case Keys.F: if (e.Control) searchField.Focus(); break;
			}
		};
		
		addBtn = new FlatButton(Own.Image("Add"));
		addBtn.Location = new Point(10, 10);
		addBtn.Clicked += ShowAdd;
		Tip.Set(addBtn, "Add", "Ctrl + N");
		Controls.Add(addBtn);
		
		deleteBtn = new FlatButton(Own.Image("Delete"));
		deleteBtn.Location = new Point(addBtn.Left + addBtn.Width + 10, 10);
		deleteBtn.Enabled = false;
		deleteBtn.Clicked += Delete;
		Tip.Set(deleteBtn, "Delete", "Del");
		Controls.Add(deleteBtn);
		
		editBtn = new FlatButton(Own.Image("Edit"));
		editBtn.Location = new Point(deleteBtn.Left + deleteBtn.Width + 10, 10);
		editBtn.Enabled = false;
		editBtn.Clicked += Edit;
		Tip.Set(editBtn, "Properties", "Enter");
		Controls.Add(editBtn);
		
		Resize += delegate { RepositionButtons(); };
		
		searchField = new SearchField();
		searchField.Width = 160;
		searchField.Location = new Point(ClientSize.Width - searchField.Width - 10, 10);
		searchField.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		searchField.Cleared += delegate { extList.Focus(); };
		Tip.Set(searchField, "Search", "Ctrl + F");
		Controls.Add(searchField);
		
		extMenu = new ExtMenu();
		extMenu.EditRow.Clicked += Edit;
		extMenu.DeleteRow.Clicked += Delete;
		
		noExtMenu = new ExtListMenu();
		noExtMenu.AddRow.Clicked += ShowAdd;
		noExtMenu.RefreshRow.Clicked += Reload;
		
		extList = new ExtList();
		extList.Location = new Point(10, searchField.Top + searchField.Height + 10);
		extList.Size = new Size(ClientSize.Width - 20, ClientSize.Height - extList.Top - 10);
		extList.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		extList.MultiSelect = false;
		extList.LabelEdit = false;
		extList.HideSelection = false;
		
		extList.SelectedIndexChanged += delegate {
			editBtn.Enabled = deleteBtn.Enabled = extList.SelectedItems.Count > 0;
		};
		
		extList.DoubleClick += delegate { if (extList.SelectedItems.Count > 0) Edit(); };
		
		extList.KeyDown += delegate (object o, KeyEventArgs e)
		{
			if (extList.SelectedItems.Count > 0)
			{
				switch (e.KeyCode)
				{
					case Keys.Enter: Edit(); break;
					case Keys.Delete: Delete(); break;
				}
			}
		};
		
		extList.MouseClick += delegate (object o, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && extList.SelectedItems.Count > 0) extMenu.Display();
		};
		
		extList.MouseUp += delegate (object o, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && extList.SelectedItems.Count == 0) noExtMenu.Display();
		};
		
		Controls.Add(extList);
		
		searchField.Changed += delegate { extList.List(searchField.Text); };
		
		addField = new Field();
		addField.Location = new Point(10, searchField.Top);
		addField.Width = 64;
		addField.Visible = false;
		
		addField.KeyDown += delegate (object o, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) Add();
			else if (e.KeyCode == Keys.Escape) CancelAdd();
			else return;
			
			e.SuppressKeyPress = true;
		};
		
		addField.Changed += delegate
		{
			string text = "." + addField.Text.TrimStart('.');
			if (Type.IsValidExtension(text) && !Type.Exists(text))
				addField.ForeColor = Color.Black;
			else addField.ForeColor = Color.Red;
		};
		
		Controls.Add(addField);
		
		doAddBtn = new Button();
		doAddBtn.Text = Own.Line("Add");
		doAddBtn.Location = new Point(addField.Left + addField.Width + 10, addField.Top);
		doAddBtn.Height = addField.Height;
		doAddBtn.Visible = false;
		Controls.Add(doAddBtn);
		
		cancelAddBtn = new Button();
		cancelAddBtn.Text = Own.Line("Cancel");
		cancelAddBtn.Location = new Point(doAddBtn.Left + doAddBtn.Width + 10, doAddBtn.Top);
		cancelAddBtn.Height = doAddBtn.Height;
		cancelAddBtn.Visible = false;
		cancelAddBtn.Click += delegate { CancelAdd(); };
		Controls.Add(cancelAddBtn);
		
		specialMenu = new SpecialMenu();
		specialMenu.Selected += Edit;
		
		specialBtn = new FlatButton(Own.Image("Special"));
		specialBtn.Top = 10;
		specialBtn.Click += delegate { specialMenu.Display(); };
		Tip.Set(specialBtn, "Special types");
		Controls.Add(specialBtn);
		
		helpBtn = new FlatButton(Own.Image("Help"));
		helpBtn.Top = 10;
		helpBtn.Click += delegate { About.Display(); };
		Tip.Set(helpBtn, "About");
		Controls.Add(helpBtn);
		
		RepositionButtons();
		
		extList.TabIndex = 0;
		searchField.TabIndex = 1;
		addBtn.TabIndex = 2;
		deleteBtn.TabIndex = 3;
		editBtn.TabIndex = 4;
		specialBtn.TabIndex = 5;
		
		typeForms = new Dictionary<string, TypeForm>();
		
		ResumeLayout();
	}
	
	bool addVisible
	{
		get { return addField.Visible; }
		
		set {
			editBtn.Visible = deleteBtn.Visible = addBtn.Visible = specialBtn.Visible = helpBtn.Visible = !value;
			addField.Visible = doAddBtn.Visible = cancelAddBtn.Visible = value;
		}
	}
	
	void ShowAdd ()
	{
		addVisible = true;
		addField.Focus();
	}
	
	void CancelAdd ()
	{
		addField.Text = "";
		addVisible = false;
		addBtn.Focus();
	}
	
	bool Add ()
	{
		string add = "." + addField.Text.TrimStart('.');
		
		if (!Type.IsValidExtension(add) || Type.Exists(add))
		{
			Tip.Boo();
			return false;
		}
		
		Type.Create(add);
		extList.CacheType(add);
		extList.ShowExt(add);
		
		addField.Text = "";
		addVisible = false;
		extList.Items[add].Selected = true;
		extList.SelectedItems[0].EnsureVisible();
		extList.Focus();
		
		return true;
	}
	
	void Edit ()
	{
		Edit(extList.SelectedItems[0].Name);
	}
	
	void Edit (string type)
	{
		Cursor = Cursors.WaitCursor;
		
		if (Type.Exists(type))
		{
			if (!typeForms.ContainsKey(type))
			{
				TypeForm editor = new TypeForm(type);
				editor.Changed += extList.UpdateType;
				editor.Closed += delegate (object o, EventArgs e) { typeForms.Remove(((TypeForm)o).Type.ID); };
				typeForms.Add(type, editor);
				editor.Show();
			}
			
			else typeForms[type].Activate();
		}
		
		Cursor = Cursors.Default;
	}
	
	void Delete ()
	{
		Delete(extList.SelectedItems[0].Name);
	}
	
	void Delete (string type)
	{
		try { Type.Delete(type); extList.Remove(type); }
		catch (Exception x) { Tip.Boo(extList, x); }
	}
	
	void Reload ()
	{
		extList.Clear();
		extList.LoadTypes();
		searchField.Reset();
	}
	
	void RepositionButtons ()
	{
		specialBtn.Left = editBtn.Left + (searchField.Left - editBtn.Left) / 2;
		helpBtn.Left = specialBtn.Right + 5;
	}
}