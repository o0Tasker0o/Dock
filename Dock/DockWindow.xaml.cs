using System;
using System.Windows;

namespace Dock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class DockWindow
	{
		public DockWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			var screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (Width / 2);
			Top = 0;

			base.OnInitialized(e);
		}
	}
}
