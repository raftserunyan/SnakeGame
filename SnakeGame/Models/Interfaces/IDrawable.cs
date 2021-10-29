using System.Windows;

namespace SnakeGame.Models.Interfaces
{
	public interface IDrawable
	{
		int Width { get; set; }
		int Height { get; set; }
		Point Position { get; set; }
		UIElement UiElement { get; set; }
	}
}
