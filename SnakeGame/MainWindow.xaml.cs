using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SnakeGame.Enums;
using SnakeGame.Models;
using SnakeGame.Models.Interfaces;
using System;

namespace SnakeGame
{
	public partial class MainWindow : Window
	{
		//Fields
		//private DispatcherTimer dispatcherTimer;
		private Timer timer;
		private Snake snake;
		public const int WindowRefreshRate = 10;

		//Constructors
		public MainWindow()
		{
			InitializeComponent();
		}

		//Methods
		public void Draw(IDrawable element)
		{
			GameArea.Children.Add(element.UiElement);
			Refresh(element);
		}

		public void Refresh(IDrawable element)
		{
			this.Dispatcher.Invoke(() =>
			{
				Canvas.SetLeft(element.UiElement, element.Position.X);
				Canvas.SetTop(element.UiElement, element.Position.Y);
			});
		}

		//Event Handlers
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			snake = new Snake(this);
		}
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
					snake.Direction = Direction.Left;
					break;
				case Key.Up:
					snake.Direction = Direction.Up;
					break;
				case Key.Right:
					snake.Direction = Direction.Right;
					break;
				case Key.Down:
					snake.Direction = Direction.Down;
					break;
				default:
					return;
			}

			if (timer == null)
			{
				timer = new Timer((obj) => snake.Move(), null, 0, WindowRefreshRate);

				//dispatcherTimer = new DispatcherTimer();
				//dispatcherTimer.Tick += (a, b) => snake.Move();
				//dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, WindowRefreshRate);
				//dispatcherTimer.Start();
			}
		}
	}
}
