using System;
using System.Drawing;
using System.Windows.Forms;

class DDETab : Tab
{
	public readonly Verb Verb;
	
	TickBox useCheck; GroupBox ddeGroup;
	Label messageLabel; Field messageField;
	Label appLabel; Field appField;
	Label notRunningLabel; Field notRunningField;
	Label topicLabel; Field topicField;
	
	public DDETab (Verb verb) : base("DDE")
	{
		Verb = verb;
		
		useCheck = new TickBox();
		useCheck.Text = Own.Line("Use DDE");
		useCheck.Location = new Point(20, 20);
		useCheck.Width = ClientSize.Width - 40;
		useCheck.Changed += UseChanged;
		Controls.Add(useCheck);
		
		ddeGroup = new GroupBox();
		ddeGroup.Location = new Point(useCheck.Left, useCheck.Top + useCheck.Height + 10);
		ddeGroup.Size = new Size(useCheck.Width, ClientSize.Height - ddeGroup.Top - 20);
		ddeGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
		Controls.Add(ddeGroup);
		
		messageLabel = new Label();
		messageLabel.Text = Own.Line("DDE Message") + ":";
		messageLabel.Location = new Point(10, 15);
		messageLabel.Size = new Size(ddeGroup.ClientSize.Width - 20, 20);
		messageLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		ddeGroup.Controls.Add(messageLabel);
		
		messageField = new Field();
		messageField.Location = new Point(10, messageLabel.Top + messageLabel.Height);
		messageField.Width = messageLabel.Width;
		messageField.Anchor = messageLabel.Anchor;
		messageField.Changed += MessageChanged;
		ddeGroup.Controls.Add(messageField);
		
		appLabel = new Label();
		appLabel.Text = Own.Line("Application") + ":";
		appLabel.Location = new Point(messageLabel.Left, messageField.Top + messageField.Height + 10);
		appLabel.Size = messageLabel.Size;
		appLabel.Anchor = messageLabel.Anchor;
		ddeGroup.Controls.Add(appLabel);
		
		appField = new Field();
		appField.Location = new Point(appLabel.Left, appLabel.Top + appLabel.Height);
		appField.Width = appLabel.Width;
		appField.Anchor = appLabel.Anchor;
		appField.Changed += AppChanged;
		ddeGroup.Controls.Add(appField);
		
		notRunningLabel = new Label();
		notRunningLabel.Text = Own.Line("Application not running") + ":";
		notRunningLabel.Location = new Point(appLabel.Left, appField.Top + appField.Height + 10);
		notRunningLabel.Size = appLabel.Size;
		notRunningLabel.Anchor = appLabel.Anchor;
		ddeGroup.Controls.Add(notRunningLabel);
		
		notRunningField = new Field();
		notRunningField.Location = new Point(notRunningLabel.Left, notRunningLabel.Top + notRunningLabel.Height);
		notRunningField.Width = notRunningLabel.Width;
		notRunningField.Anchor = notRunningLabel.Anchor;
		notRunningField.Changed += NotRunningChanged;
		ddeGroup.Controls.Add(notRunningField);
		
		topicLabel = new Label();
		topicLabel.Text = Own.Line("Topic") + ":";
		topicLabel.Location = new Point(notRunningField.Left, notRunningField.Top + notRunningField.Height + 10);
		topicLabel.Size = notRunningLabel.Size;
		topicLabel.Anchor = notRunningLabel.Anchor;
		ddeGroup.Controls.Add(topicLabel);
		
		topicField = new Field();
		topicField.Location = new Point(topicLabel.Left, topicLabel.Top + topicLabel.Height);
		topicField.Width = topicLabel.Width;
		topicField.Anchor = topicLabel.Anchor;
		topicField.Changed += TopicChanged;
		ddeGroup.Controls.Add(topicField);
		
		Reveal();
	}
	
	void Reveal ()
	{
		useCheck.Set(Verb.HasDDE);
		ddeGroup.Enabled = Verb.HasDDE;
		
		if (Verb.HasDDE)
		{
			messageField.Set(Verb.DDEMessage);
			appField.Set(Verb.DDEApp);
			notRunningField.Set(Verb.DDENotRunning);
			topicField.Set(Verb.DDETopic);
		}
	}
	
	void UseChanged (bool on)
	{
		try { Verb.HasDDE = on; }
		catch (Exception x) { Tip.Boo(useCheck, x); }
		Reveal();
	}
	
	void MessageChanged ()
	{
		try { Verb.DDEMessage = messageField.Text; }
		catch (Exception x) { Tip.Boo(messageField, x); }
	}
	
	void AppChanged ()
	{
		try { Verb.DDEApp = appField.Text; }
		catch (Exception x) { Tip.Boo(appField, x); }
	}
	
	void NotRunningChanged ()
	{
		try { Verb.DDENotRunning = notRunningField.Text; }
		catch (Exception x) { Tip.Boo(notRunningField, x); }
	}
	
	void TopicChanged ()
	{
		try { Verb.DDETopic = topicField.Text; }
		catch (Exception x) { Tip.Boo(topicField, x); }
	}
}