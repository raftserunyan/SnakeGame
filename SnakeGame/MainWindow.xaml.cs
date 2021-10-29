using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using SnakeGame.Enums;
using SnakeGame.Models;
using SnakeGame.Models.Interfaces;

namespace SnakeGame
{
	public partial class MainWindow : Window
	{
		//Fields
		public const int WindowRefreshRate = 10;
		public Food Food;
		public List<Food> BonusFoods;
		public int Score;

		private Timer timer;
		//private DispatcherTimer timer;
		private Snake snake;

		//Properties
		public Rectangle[] Borders { get; set; }

		//Constructors
		public MainWindow()
		{
			InitializeComponent();

			Borders = new Rectangle[4];
			Borders = GetBorders();

			BonusFoods = new List<Food>();
			Score = 0;
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
			if (timer == null)
				return;

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

			SetUpGameArea();
		}
		public void RespawnFood()
		{
			Food.Dispose();

			Application.Current.Dispatcher.Invoke(() =>
			{
				this.GameArea.Children.Remove(this.Food.UiElement);
			});

			InitializeFood();
			this.Draw(Food);
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
		private Food InitializeFood()
		{
			do
			{
				var rnd = new Random();

				int width = 0;
				int height = 0;
				Application.Current.Dispatcher.Invoke(() => 
				{
					width = (int)this.GameArea.Width;
					height = (int)this.GameArea.Height;
				});

				var maxFoodSize = Food.SideLength + Food.ExtensionLimit;
				var point = new Models.Point(rnd.Next(0 + Food.ExtensionLimit / 2, width - maxFoodSize),
										rnd.Next(0 + Food.ExtensionLimit / 2, height - maxFoodSize));

				Food = new Food(point, false, this);
			}
			while (!IsFoodPositionOk(Food));

			Food.StartBeating();

			return Food;
		}
		private bool IsFoodPositionOk(Food food)
		{
			bool isPositionOk = true;

			Parallel.For(0, this.snake.Length, (i, loopState) =>
			{
				if (this.snake[i].CollidesWith(food))
				{
					isPositionOk = false;
					loopState.Stop();
				}
			});

			return isPositionOk;
		}
		private void SetUpGameArea()
		{
			snake = new Snake(this);
			Food = InitializeFood();
			Draw(Food);
		}
		private void RefreshGame()
		{
			snake.Move();

			if (snake.BitesItself() || snake.CollidesWithWalls())
				this.StopGame();

			if (snake.EatsFood())
			{
				Score++;

				snake.Extend();
				this.RespawnFood();
			}
		}

		//Event Handlers
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			SetUpGameArea();
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
				timer = new Timer((obj) => this.RefreshGame(), null, 0, WindowRefreshRate);
			}
		}		
	}
}
