﻿
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen
{
	public static class MouseTracker
	{
		/// <summary>
		/// Struct representing a point.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public static implicit operator Point(POINT point)
			{
				return new Point(point.X, point.Y);
			}
		}

		/// <summary>
		/// Retrieves the cursor's position, in screen coordinates.(relative to top left corner of screen: 0,0)
		/// </summary>
		/// <see>See MSDN documentation for further information.</see>
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		public static Point GetCursorPosition()
		{
			POINT lpPoint;
			GetCursorPos(out lpPoint);
			//bool success = User32.GetCursorPos(out lpPoint);
			// if (!success)

			return lpPoint;
		}
	}
}
