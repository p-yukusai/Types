using System.Windows.Forms;

class Tab : TabPage
{
	public Tab (string title)
	{
		AutoScroll = true;
		UseVisualStyleBackColor = true;
		Text = Own.Line(title);
	}
}