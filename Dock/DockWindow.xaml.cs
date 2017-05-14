using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace Dock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class DockWindow
	{
		private readonly TimeSpan _popupDuration = TimeSpan.FromMilliseconds(100);
		private const int HiddenPosition = -72;

		public DockWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			AddShortcuts();

			Width = DockPanel.Width + 20;

			var screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (Width / 2);
			Top = HiddenPosition;

			base.OnInitialized(e);
		}

		private void AddShortcuts()
		{
			var fileContents = File.ReadAllText("./shortcuts.txt");

			foreach (var shortcut in fileContents.Split(new [] { Environment.NewLine }, StringSplitOptions.None))
			{
				if (shortcut == "|")
				{
					AddSeparator();
				}
				else
				{
					AddButton(shortcut);
				}
			}
		}

		private void AddSeparator()
		{
			var separator = new Image
			{
				Width = 20,
				Source = new BitmapImage(new Uri("/Images/Separator.png", UriKind.Relative))
			};

			DockPanel.Width += separator.Width;
			DockPanel.Children.Add(separator);
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
					},
					Width = 64,
					Height = 64
				},
				ToolTip = new ToolTip
				{
					Content = ShortcutParser.GetName(shortcut),
					IsOpen = false,
					Background = Brushes.Transparent,
					BorderBrush = Brushes.Transparent,
					Placement = PlacementMode.Center,
					HorizontalOffset = 0,
					VerticalOffset = 50,
					Foreground = Brushes.White,
					FontSize = 30.0,
					Effect = new DropShadowEffect
					{
						Color = Colors.Black,
						BlurRadius = 4.0,
						Opacity = 1.0,
						ShadowDepth = 0.0
					}
				},
				CommandParameter = shortcut,
				Width = 72
			};

			RenderOptions.SetBitmapScalingMode(newBtn, BitmapScalingMode.Fant);
			newBtn.Click += ShortcutClicked;
			newBtn.MouseEnter += ButtonMouseEnter;
			newBtn.MouseLeave += ButtonMouseLeave;
			DockPanel.Width += newBtn.Width;
			DockPanel.Children.Add(newBtn);
		}

		private static void ButtonMouseLeave(object sender, MouseEventArgs e)
		{
			var button = (Button) sender;
			((DropShadowEffect) ((Image) button.Content).Effect).Opacity = 0.0;
		}

		private static void ButtonMouseEnter(object sender, MouseEventArgs e)
		{
			var button = (Button) sender;
			((DropShadowEffect) ((Image) button.Content).Effect).Opacity = 0.2;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			var popupAnimation = new DoubleAnimation(HiddenPosition, 0, _popupDuration);
			BeginAnimation(TopProperty, popupAnimation);

			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			var collapseAnimation = new DoubleAnimation(0, HiddenPosition, _popupDuration);
			BeginAnimation(TopProperty, collapseAnimation);

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
