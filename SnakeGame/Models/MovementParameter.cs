using System;

namespace SnakeGame.Models
{
	public struct MovementParameter
	{
		public int Vertical;
		public int Horizontal;

		public MovementParameter(int vertical, int horizontal)
		{
			Vertical = vertical;
			Horizontal = horizontal;
		}
	}
}
