using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Brushes = System.Windows.Media.Brushes;

namespace Dock
{
	/// <summary>
	/// Interaction logic for FolderWindow.xaml
	/// </summary>
	public partial class FolderWindow
	{
		private int _columnIndex;
		private int _rowIndex;
		private readonly int _dimension;

		public FolderWindow(string directory)
		{
			InitializeComponent();

			var files = Directory.GetFiles(directory);

			_dimension = (int)Math.Ceiling(Math.Sqrt(files.Length));

			for (var dimensionIndex = 0; dimensionIndex < _dimension; ++dimensionIndex)
			{
				FolderGrid.ColumnDefinitions.Add(new ColumnDefinition());
				FolderGrid.RowDefinitions.Add(new RowDefinition());
			}

			FolderGrid.Width = 72 * _dimension;
			FolderGrid.Height = 72 * _dimension;

			foreach (var file in files)
			{
				AddButton(file);
			}

			Width = FolderGrid.Width + 20;
			Height = FolderGrid.Height + 20;

			var screenWidth = SystemParameters.PrimaryScreenWidth;
			Left = (screenWidth / 2) - (Width / 2);
			Top = 80;
		}

		private void AddButton(string shortcut)
		{
			var icon = IconGetter.GetLargeIcon(shortcut);
			
			var newBtn = new DockButton
			{
				Content = new Image
				{
					Source = icon,
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
				CommandParameter = shortcut
			};

			Grid.SetColumn(newBtn, _columnIndex++);
			Grid.SetRow(newBtn, _rowIndex);

			if (_columnIndex >= _dimension)
			{
				_columnIndex = 0;
				_rowIndex++;
			}

			RenderOptions.SetBitmapScalingMode(newBtn, BitmapScalingMode.Fant);
			newBtn.Click += ShortcutClicked;
			newBtn.MouseEnter += ButtonMouseEnter;
			newBtn.MouseLeave += ButtonMouseLeave;
			FolderGrid.Children.Add(newBtn);
		}

		private static void ButtonMouseLeave(object sender, MouseEventArgs e)
		{
			var button = (DockButton) sender;
			((DropShadowEffect) ((Image) button.Content).Effect).Opacity = 0.0;
		}

		private static void ButtonMouseEnter(object sender, MouseEventArgs e)
		{
			var button = (DockButton) sender;
			((DropShadowEffect) ((Image) button.Content).Effect).Opacity = 0.2;
		}

		protected override void OnDeactivated(EventArgs e)
		{
			Hide();

			base.OnDeactivated(e);
		}

		private void ShortcutClicked(object sender, RoutedEventArgs e)
		{
			Process.Start(((DockButton) sender).CommandParameter.ToString());

			Hide();
		}
	}
}
