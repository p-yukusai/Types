using System;
using System.Drawing;
using System.Windows.Forms;

class ClassTab : Tab
{
	public readonly Type Type;
	
	Label titleLabel;
	Field titleField;
	TickBox classCheck;
	Combo classCombo;
	FlatButton addClassBtn;
	FlatButton delClassBtn;
	
	public ClassTab (Type type) : base("Class")
	{
		Type = type;
		
		Enter += delegate
		{
			RevealTitle();
			RevealClass();
			SwitchStates();
		};
		
		titleLabel = new Label();
		titleLabel.Text = Own.Line("Name") + ":";
		titleLabel.Location = new Point(20, 20);
		titleLabel.Size = new Size(Width - 40, 20);
		titleLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		Controls.Add(titleLabel);
		
		titleField = new Field();
		titleField.Location = new Point(20, titleLabel.Top + titleLabel.Height);
		titleField.Width = Width - 40;
		titleField.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		
		titleField.Changed += delegate
		{
			try { Type.Title = titleField.Text; }
			catch (Exception e) { Tip.Boo(titleField, e); }
		};
		
		Controls.Add(titleField);
		
		classCheck = new TickBox(Own.Line("Class") + ":");
		classCheck.Location = new Point(20, titleField.Top + titleField.Height + 20);
		classCheck.Width = Width - 40;
		classCheck.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		
		classCheck.Changed += delegate (bool on)
		{
			if (on) classCombo.Enabled = true;
			else
			{
				try { Type.Class = null; }
				catch (Exception x) { Tip.Boo(classCheck, x); }
				
				ClassChanged();
				RevealClass();
				RevealTitle();
				SwitchStates();
			}
		};
		
		Controls.Add(classCheck);
		
		classCombo = new Combo();
		classCombo.Items.AddRange(Type.Classes);
		classCombo.Location = new Point(20, classCheck.Top + classCheck.Height);
		classCombo.Width = Width - 40;
		classCombo.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		
		classCombo.Changed += delegate
		{
			try
			{
				Type.Class = classCombo.Text;
				ClassChanged();
				RevealTitle();
				SwitchStates();
			}
			catch (Exception e)
			{
				Tip.Boo(classCombo, e);
			}
		};
		
		classCombo.KeyDown += delegate (object o, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter: AddClass(); e.SuppressKeyPress = true; break;
			}
		};
		
		Controls.Add(classCombo);
		
		addClassBtn = new FlatButton(Own.Image("Add"));
		addClassBtn.Location = new Point(Width - 40, classCombo.Top);
		addClassBtn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
		addClassBtn.Click += delegate { AddClass(); };
		Tip.Set(addClassBtn, "Add class");
		Controls.Add(addClassBtn);
		
		delClassBtn = new FlatButton(Own.Image("Delete"));
		delClassBtn.Location = addClassBtn.Location;
		delClassBtn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
		delClassBtn.Click += delegate { DelClass(); };
		Tip.Set(delClassBtn, "Delete class");
		Controls.Add(delClassBtn);
		
		classCombo.Width = Width - 40 - addClassBtn.Width - 5;
	}
	
	void RevealTitle ()
	{
		titleField.Set(Type.Title);
	}
	
	void RevealClass ()
	{
		classCheck.Set(Type.Class != null);
		classCombo.Enabled = Type.Class != null;
		classCombo.Set(Type.Class);
	}
	
	void SwitchStates ()
	{
		if (classCombo.Text == "") {
			addClassBtn.Enabled = false;
			addClassBtn.Visible = true;
			delClassBtn.Visible = false;
		} else if (Type.Exists(classCombo.Text)) {
			classCombo.ForeColor = SystemColors.WindowText;
			addClassBtn.Visible = false;
			delClassBtn.Visible = true;
		} else if (!Type.IsValidClass(classCombo.Text)) {
			classCombo.ForeColor = SystemColors.GrayText;
			addClassBtn.Enabled = false;
			addClassBtn.Visible = true;
			delClassBtn.Visible = false;
		} else {
			classCombo.ForeColor = SystemColors.GrayText;
			addClassBtn.Enabled = true;
			addClassBtn.Visible = true;
			delClassBtn.Visible = false;
		}
	}
	
	void AddClass ()
	{
		if (!Type.IsValidClass(classCombo.Text)) return;
		
		try {
			Type.Create(classCombo.Text);
			Type.Class = classCombo.Text;
			ClassChanged();
			RevealTitle();
			SwitchStates();
		} catch (Exception e) {
			Tip.Boo(addClassBtn, e);
		}
	}
	
	void DelClass ()
	{
		try {
			Type.Delete(classCombo.Text);
			Type.Class = null;
			ClassChanged();
			RevealClass();
			RevealTitle();
			SwitchStates();
		} catch (Exception e) {
			Tip.Boo(delClassBtn, e);
		}
	}
	
	public event Action ClassChanged;
}