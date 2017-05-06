using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Brushes = System.Windows.Media.Brushes;

namespace Dock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class DockWindow
	{
		private readonly TimeSpan _swellDuration = TimeSpan.FromMilliseconds(100);
		private readonly TimeSpan _popupDuration = TimeSpan.FromMilliseconds(100);

		public DockWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			AddShortcuts();

			Width = DockPanel.Width;

			var screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (Width / 2);
			Top = -64;

			base.OnInitialized(e);
		}

		private void AddShortcuts()
		{
			var fileContents = File.ReadAllText("./shortcuts.txt");

			foreach (var shortcut in fileContents.Split(new [] { Environment.NewLine }, StringSplitOptions.None))
			{
				AddButton(shortcut);
			}
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
			var swellAnimation = new ThicknessAnimation(new Thickness(0, 0, 0, 0), new Thickness(4, 4, 4, 4), _swellDuration);
			var button = (Button) sender;
			button.BeginAnimation(PaddingProperty, swellAnimation);
			((DropShadowEffect) ((Image) button.Content).Effect).Opacity = 0.0;
		}

		private void ButtonMouseEnter(object sender, MouseEventArgs e)
		{
			var swellAnimation = new ThicknessAnimation(new Thickness(4, 4, 4, 4), new Thickness(0, 0, 0, 0), _swellDuration);
			var button = (Button) sender;
			button.BeginAnimation(PaddingProperty, swellAnimation);
			((DropShadowEffect) ((Image) button.Content).Effect).Opacity = 0.2;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			var popupAnimation = new DoubleAnimation(-64, 0, _popupDuration);
			BeginAnimation(TopProperty, popupAnimation);

			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			var popupAnimation = new DoubleAnimation(0, -64, _popupDuration);
			BeginAnimation(TopProperty, popupAnimation);

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
