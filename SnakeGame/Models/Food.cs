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

		public const int SideLength = 15;
		public const int BeatingInterval = 300;
		public const int ExtensionLimit = 3;
		public const int BonusFoodLifetime = 5000;

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
				var size = SideLength * 2;

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

				Task.Run(async () => { await Task.Delay(BonusFoodLifetime); mainWindow.RemoveBonusFood(this); });
			}
			else
			{
				var size = SideLength;

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
			timer = new Timer((a) => Beat(), null, 0, BeatingInterval);
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
				width -= ExtensionLimit;
				height -= ExtensionLimit;
				this.Position.X += ExtensionLimit / 2;
				this.Position.Y += ExtensionLimit / 2;

				IsExpanded = false;
			}
			else
			{
				width += ExtensionLimit;
				height += ExtensionLimit;
				this.Position.X -= ExtensionLimit / 2;
				this.Position.Y -= ExtensionLimit / 2;

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
