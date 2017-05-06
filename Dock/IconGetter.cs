using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Dock
{
	public class IconGetter
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			private readonly int _Left;
			private readonly int _Top;
			private readonly int _Right;
			private readonly int _Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			public int X;
			public int Y;

			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}

			public static implicit operator System.Drawing.Point(Point p)
			{
				return new System.Drawing.Point(p.X, p.Y);
			}

			public static implicit operator Point(System.Drawing.Point p)
			{
				return new Point(p.X, p.Y);
			}
		}
		// Constants that we need in the function call
		private const int ShilJumbo = 0x4;

		public struct Shfileinfo
		{
			public IntPtr HIcon;
			public int Icon;
			public uint DwAttributes;

			// Path to the file

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string SzDisplayName;

			// File type

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string SzTypeName;

		};

		[DllImport("Kernel32.dll")]
		public static extern bool CloseHandle(IntPtr handle);

		#region Private ImageList COM Interop (XP)
		[ComImport]
		[Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IImageList
		{
			int Add();
			int ReplaceIcon();
			int SetOverlayImage();
			int Replace();
			int AddMasked();
			int Draw();
			int Remove();
			int GetIcon(int i, int flags, ref IntPtr picon);
			
		};
		#endregion

		///
		/// SHGetImageList is not exported correctly in XP.  See KB316931
		/// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
		/// Apparently (and hopefully) ordinal 727 isn't going to change.
		///
		[DllImport("shell32.dll", EntryPoint = "#727")]
		private static extern int SHGetImageList(
			int iImageList,
			ref Guid riid,
			out IImageList ppv
			);

		// The signature of SHGetFileInfo (located in Shell32.dll)
		[DllImport("Shell32.dll")]
		public static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref Shfileinfo psfi, int cbFileInfo, uint uFlags);

		[DllImport("Shell32.dll")]
		public static extern int SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, ref Shfileinfo psfi, int cbFileInfo, uint uFlags);
	
		[DllImport("user32")]
		public static extern int DestroyIcon(IntPtr hIcon);

		private static BitmapSource bitmap_source_of_icon(System.Drawing.Icon ic)
		{
			var ic2 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(ic.Handle,
													System.Windows.Int32Rect.Empty,
													BitmapSizeOptions.FromEmptyOptions());
			ic2.Freeze();
			return ic2;
		}

		public static BitmapSource GetLargeIcon(string fileName)
		{
			var shinfo = new Shfileinfo();

			const uint shgfiSysiconindex = 0x4000;
			const int fileAttributeNormal = 0x80;

			var res = SHGetFileInfo(fileName, fileAttributeNormal, ref shinfo, Marshal.SizeOf(shinfo), shgfiSysiconindex);
			if (res == 0)
			{
				return null;
			}
			var iconIndex = shinfo.Icon;

			// Get the System IImageList object from the Shell:
			var iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

			IImageList iml;
			SHGetImageList(ShilJumbo, ref iidImageList, out iml);

			var hIcon = IntPtr.Zero;
			const int ildTransparent = 1;
			iml.GetIcon(iconIndex, ildTransparent, ref hIcon);

			var myIcon = System.Drawing.Icon.FromHandle(hIcon);
			var bs = bitmap_source_of_icon(myIcon);
			myIcon.Dispose();
			bs.Freeze(); // very important to avoid memory leak
			DestroyIcon(hIcon);

			return bs;
		}
	}
}
