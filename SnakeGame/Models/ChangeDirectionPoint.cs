using SnakeGame.Enums;

namespace SnakeGame.Models
{
	public class ChangeDirectionPoint
	{
		//If we reach this point
		public Point Point;
		//We take this direction
		public Direction Direction;

		public ChangeDirectionPoint(Point point, Direction direction)
		{
			Point = point;
			Direction = direction;
		}
	}
}
