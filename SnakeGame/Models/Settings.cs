using System.Windows.Media;

namespace SnakeGame.Models
{
	static class Settings
	{
		public static SolidColorBrush GameAreaBorderColor = Brushes.White;
		public const int WindowRefreshRate = 5;
		public const int SnakePartSideLength = 18;
		public const int SnakeMovingSpeed = 3;

		public const int FoodSideLength = 15;
		public const int FoodBeatingInterval = 300;
		public const int FoodExtensionLimit = 3;
		public const int BonusFoodLifetime = 5000;
	}
}
