using System.Windows;

namespace SnakeGame.Models.Interfaces
{
	public interface IDrawable
	{
		Point Position { get; set; }
		UIElement UiElement { get; set; }
	}
}
