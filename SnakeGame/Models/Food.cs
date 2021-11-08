using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using SnakeGame.Models.Interfaces;

namespace SnakeGame.Models
{
	public class Food : IDrawable, IDisposable
	{
		private Timer timer;
		private bool IsExpanded;
		private readonly MainWindow mainWindow;

		public int Width { get; set; }
		public int Height { get; set; }
		public Point Position { get; set; }
		public UIElement UiElement { get; set; }

		public Food(Point point, bool isBonus, MainWindow mw)
		{
			mainWindow = mw;
			IsExpanded = false;
			Position = point;

			if (isBonus)
			{
				var size = Settings.FoodSideLength * 2;

				Width = size;
				Height = size;

				Application.Current.Dispatcher.Invoke(() =>
				{
					UiElement = new Ellipse()
					{
						Width = size,
						Height = size,
						Fill = Brushes.Blue
					};
				});

				Task.Run(async () => { await Task.Delay(Settings.BonusFoodLifetime); mainWindow.RemoveBonusFood(this); });
			}
			else
			{
				var size = Settings.FoodSideLength;

				Width = size;
				Height = size;

				Application.Current.Dispatcher.Invoke(() =>
				{
					UiElement = new Ellipse()
					{
						Width = size,
						Height = size,
						Fill = Brushes.Red
					};
				});
			}
		}

		public void StartBeating()
		{
			timer = new Timer((a) => Beat(), null, 0, Settings.FoodBeatingInterval);
		}

		[DebuggerStepThrough]
		private void Beat()
		{
			var uiElement = (Ellipse)UiElement;
			double width = 0;
			double height = 0;

			Application.Current.Dispatcher.Invoke(() =>
			{
				width = uiElement.Width;
				height = uiElement.Height;
			});


			if (IsExpanded)
			{
				width -= Settings.FoodExtensionLimit;
				height -= Settings.FoodExtensionLimit;
				this.Position.X += Settings.FoodExtensionLimit / 2;
				this.Position.Y += Settings.FoodExtensionLimit / 2;

				IsExpanded = false;
			}
			else
			{
				width += Settings.FoodExtensionLimit;
				height += Settings.FoodExtensionLimit;
				this.Position.X -= Settings.FoodExtensionLimit / 2;
				this.Position.Y -= Settings.FoodExtensionLimit / 2;

				IsExpanded = true;
			}

			Application.Current.Dispatcher.Invoke(() =>
			{
				uiElement.Width = width;
				uiElement.Height = height;
			});

			mainWindow.RefreshElement(this);
		}

		//IDisposable pattern
		private bool disposed;
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				this.timer.Dispose();
			}

			disposed = true;
		}

		~Food()
		{
			this.Dispose(false);
		}
	}
}
