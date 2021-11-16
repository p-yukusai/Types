using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

class VerbsTab : Tab
{
	public readonly Verbs Verbs;
	
	Field verbField = new Field();
	FlatButton addVerbBtn = new FlatButton(Own.Image("Add"));
	FlatButton deleteVerbButton = new FlatButton(Own.Image("Delete"));
	VerbList verbList;
	VerbMenu verbMenu = new VerbMenu();
	Combo commandCombo = new Combo("Commands");
	FlatButton browseCommandBtn = new FlatButton(Own.Image("Browse"));
	OpenFileDialog commandOpener = new OpenFileDialog();
	Dictionary<string, VerbForm> verbForms = new Dictionary<string, VerbForm>();
	
	public VerbsTab (string title, Verbs verbs) : base (title)
	{
		Verbs = verbs;
		
		Enter += delegate
		{
			verbField.Set(null);
			verbList.Reload();
			RevealSelected();
			RevealAdding();
		};
		
		addVerbBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		addVerbBtn.Location = new Point(Width - addVerbBtn.Width - 10, 10);
		addVerbBtn.Clicked += AddVerb;
		Tip.Set(addVerbBtn, "Add menu item");
		Controls.Add(addVerbBtn);
		
		deleteVerbButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		deleteVerbButton.Location = addVerbBtn.Location;
		deleteVerbButton.Visible = false;
		Tip.Set(deleteVerbButton, "Delete item");
		Controls.Add(deleteVerbButton);
		
		verbField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		verbField.Location = new Point(10, 10);
		verbField.Width = addVerbBtn.Left - 20;
		verbField.GotFocus += delegate { verbList.Deselect(); };
		verbField.Changed += RevealAdding;
		verbField.Entered += AddVerb;
		Tip.Set(verbField, "Verb title");
		Controls.Add(verbField);
		
		browseCommandBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		browseCommandBtn.Location = new Point(addVerbBtn.Left, Height - browseCommandBtn.Height - 10);
		browseCommandBtn.Clicked += delegate { commandOpener.ShowDialog(); };
		Tip.Set(browseCommandBtn, "Select program");
		Controls.Add(browseCommandBtn);
		
		commandCombo.LostFocus += delegate { commandCombo.Remember(commandCombo.Text); };
		commandCombo.Changed += ChangeCommand;
		commandCombo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		commandCombo.Location = new Point(10, browseCommandBtn.Top);
		commandCombo.Width = browseCommandBtn.Left - 20;
		Tip.Set(commandCombo, "Command");
		Controls.Add(commandCombo);
		
		verbList = new VerbList(Verbs);
		verbList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
		verbList.Location = new Point(10, verbField.Top + verbField.Height + 10);
		verbList.Size = new Size(Width - 20, commandCombo.Top - verbList.Top - 10);
		verbList.LabelEdit = true;
		verbList.MultiSelect = false;
		verbList.SelectedIndexChanged += delegate { RevealSelected(); };
		Controls.Add(verbList);
		
		verbList.MouseClick += delegate (object o, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && verbList.Selected)
			{
				Verb sa = verbList.SelectedVerb;
				
				verbMenu.DefaultRow.Checked = sa.IsDefault;
				verbMenu.DefaultRow.Enabled = !sa.IsDefault;
				verbMenu.ExtendedRow.Checked = sa.IsExtended;
				
				verbMenu.Display();
			}
		};
		
		verbList.AfterLabelEdit += delegate (object o, LabelEditEventArgs e)
		{
			Verb sa = verbList.SelectedVerb;
			
			if (sa == null) { e.CancelEdit = true; return; }
			else if (e.Label == null) e.CancelEdit = true;
			else if (e.Label == "") e.CancelEdit = true;
			else sa.Title = e.Label;
			
			verbList.UpdateVerb(sa);
		};
		
		verbList.KeyDown += delegate (object o, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F2: BeginRename(); break;
				case Keys.Delete: DeleteVerb(); break;
				case Keys.Enter: ShowProperties(); break;
			}
		};
		
		verbList.DoubleClick += delegate { ShowProperties(); };
		
		verbMenu.DefaultRow.Clicked += SetDefault;
		verbMenu.RenameRow.Clicked += BeginRename;
		verbMenu.DeleteRow.Clicked += DeleteVerb;
		verbMenu.ExtendedRow.Clicked += ToggleExtended;
		verbMenu.EditRow.Clicked += ShowProperties;
		
		commandOpener.Filter = Own.Line("Executables") + " (*.exe)|*.exe";
		commandOpener.FileOk += delegate
		{
			commandCombo.Text = "\"" + commandOpener.FileName + "\" \"%1\"";
			commandCombo.Remember(commandCombo.Text);
		};
		
		verbField.TabIndex = 0;
		addVerbBtn.TabIndex = 1;
		verbList.TabIndex = 2;
		commandCombo.TabIndex = 3;
		browseCommandBtn.TabIndex = 4;
	}
	
	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (
			keyData == Keys.Tab &&
			verbList.Focused &&
			commandCombo.Enabled
		) {
			commandCombo.Focus();
			return true;
		}
		
		return base.ProcessCmdKey(ref msg, keyData);
	}
	
	
	void SetDefault ()
	{
		try {
			
			Verb sa = verbList.SelectedVerb;
			
			sa.IsDefault = true;
			verbList.Reload();
			
			verbList.SelectedVerb = sa;
			
		} catch (Exception x) { Tip.Boo(verbList, x); }
	}
	
	void ToggleExtended ()
	{
		try {
			
			Verb sa = verbList.SelectedVerb;
			
			sa.IsExtended =! sa.IsExtended;
			verbList.Reload();
			
			verbList.SelectedVerb = sa;
			
		} catch (Exception x) { Tip.Boo(verbList, x); }
	}
	
	void BeginRename ()
	{
		if (verbList.Selected)
		{
			verbList.SelectedItem.BeginEdit();
		}
	}
	
	void DeleteVerb ()
	{
		Verb sa = verbList.SelectedVerb;
		if (sa == null) return;
		
		try { Verbs.Delete(sa.ID); }
		catch (Exception x) { Tip.Boo(verbList, x); }
		
		verbList.Deselect();
		verbList.Reload();
	}
	
	void ShowProperties ()
	{
		Verb sa = verbList.SelectedVerb;
		if (sa == null) return;
		
		if (verbForms.ContainsKey(sa.ID)) verbForms[sa.ID].Activate();
		else
		{
			VerbForm af = new VerbForm(sa);
			verbForms.Add(sa.ID, af);
			
			af.Closed += VerbFormClosed;
			af.IconChanged += VerbFormIconChanged;
			
			af.StartPosition = FormStartPosition.Manual;
			af.Location = new Point(Parent.Parent.Left + 10, Parent.Parent.Top + 10);
			af.Show();
		}
	}
	
	void VerbFormClosed (object o, EventArgs e)
	{
		VerbForm af = o as VerbForm;
		verbForms.Remove(af.Verb.ID);
		
		af.Closed -= VerbFormClosed;
		af.IconChanged -= VerbFormIconChanged;
	}
	
	void VerbFormIconChanged (object o, EventArgs e)
	{
		verbList.UpdateVerb((o as VerbForm).Verb);
	}
	
	void RevealSelected ()
	{
		Verb a = verbList.SelectedVerb;
		
		if (a != null) {
			commandCombo.Set(a.Command);
			commandCombo.Enabled = true;
			browseCommandBtn.Enabled = true;
		} else {
			commandCombo.Set(null);
			commandCombo.Enabled = false;
			browseCommandBtn.Enabled = false;
		}
	}
	
	void RevealAdding ()
	{
		addVerbBtn.Enabled = verbField.Text.Length > 0;
	}
	
	void AddVerb ()
	{
		if (verbField.Text.Length == 0) { Tip.Boo(); return; }
		
		try {
			Verb added = Verbs.AddVerbWithTitle(verbField.Text);
			verbField.Text = "";
			verbList.Reload();
			verbList.SelectedVerb = added;
			verbList.Focus();
		} catch (Exception x) { Tip.Boo(addVerbBtn, x); }
	}
	
	void ChangeCommand ()
	{
		Verb act = verbList.SelectedVerb;
		if (act == null) return;
		
		try { act.Command = commandCombo.Text; }
		catch (Exception x) { Tip.Boo(commandCombo, x); }
		
		verbList.UpdateVerb(act);
	}
}