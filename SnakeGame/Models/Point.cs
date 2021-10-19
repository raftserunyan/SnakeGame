using System;

namespace SnakeGame.Models
{
	public class Point
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point(Point point)
		{
			X = point.X;
			Y = point.Y;
		}
	}
}
