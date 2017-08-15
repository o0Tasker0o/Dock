using System.Windows.Controls;

namespace Dock
{
	/// <summary>
	/// Interaction logic for DockButton.xaml
	/// </summary>
	public partial class DockButton : Button
	{
		public DockButton()
		{
			InitializeComponent();
		}

		public bool IsSubPanel { get; set; }

		public FolderWindow FolderWindow { get; set; }
	}
}
