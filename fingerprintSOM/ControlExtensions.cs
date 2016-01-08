using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SOMOrganizing;

namespace _2DOrganizing
{
    public static class ControlExtensions
    {
        [DllImport("gdi32")]
        private static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        private static Color GetPixelColor(int x, int y)
        {
            IntPtr dc = GetWindowDC(IntPtr.Zero);

            long color = GetPixel(dc, x, y);
            Color cc = Color.FromArgb((int)color);
            return Color.FromArgb(cc.B, cc.G, cc.R);
        }

        public static Color GetPixelColor(this BufferedPanel c, int x, int y)
        {
            var screenCoords = c.PointToScreen(new Point(x, y));
            return GetPixelColor(screenCoords.X, screenCoords.Y);
        }
    }
}
