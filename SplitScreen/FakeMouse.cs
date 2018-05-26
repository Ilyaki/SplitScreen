using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen.Patchers
{
	public class FakeMouse
	{
		public static int X { set; get; }
		public static int Y { set; get; }

		public static Point GetPoint() => new Point(X, Y);
	}
}
