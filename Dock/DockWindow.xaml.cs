using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

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
			AddButton("::{20d04fe0-3aea-1069-a2d8-08002b30309d}");
			AddButton("C:");
			AddButton(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
			AddButton(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe");

			Width = DockPanel.Width;

			var screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (Width / 2);
			Top = -64;

			base.OnInitialized(e);
		}

		private void AddButton(string shortcut)
		{
			BitmapFrame shortcutImage = null;

			try
			{
				var icon = System.Drawing.Icon.ExtractAssociatedIcon(shortcut);

				if (icon != null)
				{
					using (var bmp = icon.ToBitmap())
					{
						var stream = new MemoryStream();
						bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
						shortcutImage = BitmapFrame.Create(stream);
					}
				}
			}
			catch (Exception)
			{
			}

			var newBtn = new Button
			{
				Style = FindResource("NoChromeButton") as Style,
				Background = Brushes.Transparent,
				Content = new Image
				{
					Source = shortcutImage ?? Icon
				},
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

		private void ExitMenuClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
