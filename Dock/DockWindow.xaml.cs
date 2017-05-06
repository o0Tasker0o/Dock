using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
			var icon = IconGetter.GetLargeIcon(shortcut);

			var newBtn = new Button
			{
				Style = FindResource("NoChromeButton") as Style,
				Background = Brushes.Transparent,
				Content = new Image
				{
					Source = icon ?? Icon,
					Effect = new DropShadowEffect
					{
						Color = Colors.White,
						BlurRadius = 10.0,
						Opacity = 0.0,
						ShadowDepth = 0.0
					}
				},
				CommandParameter = shortcut,
				Width = 64,
				Padding = new Thickness(4,4,4,4)
			};
			RenderOptions.SetBitmapScalingMode(newBtn, BitmapScalingMode.Fant);
			newBtn.Click += ShortcutClicked;
			newBtn.MouseEnter += ButtonMouseEnter;
			newBtn.MouseLeave += ButtonMouseLeave;
			DockPanel.Width += newBtn.Width;
			DockPanel.Children.Add(newBtn);
		}

		private void ButtonMouseLeave(object sender, MouseEventArgs e)
		{
			((DropShadowEffect) ((Image) ((Button) sender).Content).Effect).Opacity = 0.0;
		}

		private void ButtonMouseEnter(object sender, MouseEventArgs e)
		{
			((DropShadowEffect)((Image)((Button)sender).Content).Effect).Opacity = 0.2;
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
