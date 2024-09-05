using System.Runtime.InteropServices;

namespace ShapesEditor2D.Helpers
{
	public static class CursorHelper
	{
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int x, int y);
	}
}