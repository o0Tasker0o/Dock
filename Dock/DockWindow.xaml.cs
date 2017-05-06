using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
			AddButton("Chrome",
			          @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");

			Width = DockPanel.Width;

			var screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (Width / 2);
			Top = -64;

			base.OnInitialized(e);
		}

		private void AddButton(string text, string shortcut)
		{
			var newBtn = new Button
			{
				Content = text,
				CommandParameter = shortcut,
				Width = 64
			};

			newBtn.Click += ShortcutClicked;
			DockPanel.Width += newBtn.Width;
			DockPanel.Children.Add(newBtn);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			Top = 0;

			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Top = -64;

			base.OnMouseLeave(e);
		}

		protected override void OnDeactivated(EventArgs e)
		{
			Topmost = true;

			base.OnDeactivated(e);
		}

		private static void ShortcutClicked(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(((Button) sender).CommandParameter.ToString());
		}
	}
}
