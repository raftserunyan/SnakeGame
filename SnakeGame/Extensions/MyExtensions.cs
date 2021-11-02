using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using SnakeGame.Models.Interfaces;

namespace SnakeGame.Extensions
{
	public static class MyExtensions
	{
		public static bool CollidesWith(this IDrawable current, IDrawable element)
		{
			double offset = 0.01;
			var rect1 = new Rect(current.Position.X, current.Position.Y, current.Width - offset, current.Height- offset);
			var rect2 = new Rect(element.Position.X, element.Position.Y, element.Width - offset, element.Height - offset);

			return rect1.IntersectsWith(rect2);
		}
		public static bool CollidesWith(this IDrawable current, Rectangle element)
		{
			double offset = 0.01;
			var rect1 = new Rect(current.Position.X, current.Position.Y, current.Width - offset, current.Height - offset);

			Rect rect2;
			Application.Current.Dispatcher.Invoke(() =>
			{
				rect2 = new Rect(Canvas.GetLeft(element), Canvas.GetTop(element), element.Width - offset, element.Height - offset);
			});

			return rect1.IntersectsWith(rect2);
		}
		public static bool CollidesWithAnyParallel(this IDrawable current, IDrawable[] elements)
		{
			double offset = 0.01;
			var rect1 = new Rect(current.Position.X, current.Position.Y, current.Width - offset, current.Height - offset);

			bool result = false;

			Rect rect2;
			_ = Parallel.For(0, elements.Length, (i, loopState) =>
			{
				rect2 = new Rect(elements[i].Position.X, elements[i].Position.Y, elements[i].Width - offset, elements[i].Height - offset);

				if (rect1.IntersectsWith(rect2))
				{
					result = true;
					loopState.Stop();
				}
			});

			return result;
		}
		public static bool CollidesWithAnyParallel(this IDrawable current, Rectangle[] elements)
		{
			double offset = 0.01;
			var rect1 = new Rect(current.Position.X, current.Position.Y, current.Width - offset, current.Height - offset);

			bool result = false;

			Rect rect2;
			_ = Parallel.For(0, elements.Length, (i, loopState) =>
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					rect2 = new Rect(Canvas.GetLeft(elements[i]), Canvas.GetTop(elements[i]), elements[i].Width - offset, elements[i].Height - offset);
				});

				if (rect1.IntersectsWith(rect2))
				{
					result = true;
					loopState.Stop();
				}
			});

			return result;
		}
	}
}
