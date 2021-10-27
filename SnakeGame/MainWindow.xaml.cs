using System.Windows.Shapes;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SnakeGame.Enums;
using SnakeGame.Models;
using SnakeGame.Models.Interfaces;
using System.Linq;
using System.Windows.Threading;

namespace SnakeGame
{
	public partial class MainWindow : Window
	{
		//Fields
		private Timer timer;
		private Snake snake;
		public const int WindowRefreshRate = 2;

		//Properties
		public Rectangle[] Borders { get; set; }

		//Constructors
		public MainWindow()
		{
			InitializeComponent();
			Borders = new Rectangle[4];
			Borders = GetBorders();
		}

		//Methods
		public void Draw(IDrawable element)
		{
			this.Dispatcher.Invoke(() =>
			{
				GameArea.Children.Add(element.UiElement);
				Refresh(element);
			});
		}
		public void Refresh(IDrawable element)
		{
			this.Dispatcher.Invoke(() =>
			{
				Canvas.SetLeft(element.UiElement, element.Position.X);
				Canvas.SetTop(element.UiElement, element.Position.Y);
			});
		}
		public void StopGame()
		{
			timer.Dispose();
			//timer.Stop();
			timer = null;

			MessageBox.Show("You lost((", "Game Over", MessageBoxButton.OK);

			ResetGame();
		}
		public void ResetGame()
		{
			this.Dispatcher.Invoke(() =>
			{
				GameArea.Children.Clear();
			});
			snake = new Snake(this);
		}

		private Rectangle[] GetBorders()
		{
			var array = new Rectangle[4];
			var enumerator = this.GameArea.Children.GetEnumerator();

			int i = 0;
			while (enumerator.MoveNext() && i < 4)
			{
				array[i] = (Rectangle)enumerator.Current;
				i++;
			}

			return array;
		}

		//Event Handlers
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			snake = new Snake(this);
		}
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (snake.Head.CurrentDirection == Direction.None && e.Key == Key.Down)
				return;

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
				//timer = new DispatcherTimer();
				//timer.Interval = new System.TimeSpan(0, 0, 0, 0, WindowRefreshRate);
				//timer.Tick += (a, b) => snake.Move();
				//timer.Start();
				timer = new Timer((obj) => snake.Move(), null, 0, WindowRefreshRate);
			}
		}
	}
}
