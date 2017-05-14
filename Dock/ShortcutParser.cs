using System.Globalization;
using System.IO;

namespace Dock
{
	public static class ShortcutParser
	{
		public static string GetName(string shortcut)
		{
			if (shortcut == "::{20d04fe0-3aea-1069-a2d8-08002b30309d}")
			{
				return "My Computer";
			}

			var filename = Path.GetFileNameWithoutExtension(shortcut);

			if (string.IsNullOrEmpty(filename))
			{
				return shortcut;
			}

			return Capitalise(filename);
		}

		private static string Capitalise(string text)
		{
			var textInfo = new CultureInfo("en-GB", false).TextInfo;
			return textInfo.ToTitleCase(text);
		}
	}
}
