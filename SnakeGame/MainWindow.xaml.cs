using System;
using System.Collections.Generic;
using System.Collections;
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
using SnakeGame.Extensions;
using System.IO;

namespace SnakeGame
{
	public partial class MainWindow : Window
	{
		//Fields
		public const int WindowRefreshRate = 5;
		public Food Food;
		public HashSet<Food> BonusFoods;
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

			BonusFoods = new HashSet<Food>();
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
		public void Refresh(IEnumerable<IDrawable> elements)
		{
			this.Dispatcher.Invoke(() =>
			{
				foreach (var element in elements)
				{
					Canvas.SetLeft(element.UiElement, element.Position.X);
					Canvas.SetTop(element.UiElement, element.Position.Y);
				}
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

			Score = 0;
			BonusFoods.Clear();

			SetUpGameArea();
		}
		public void RespawnFood()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				this.GameArea.Children.Remove(this.Food.UiElement);
			});

			Food.Dispose();
			Food = null;

			InitializeFood();
			this.Draw(Food);
		}
		public void RemoveBonusFood(Food bonusFood)
		{
			this.BonusFoods.Remove(bonusFood);

			Application.Current.Dispatcher.Invoke(() =>
			{
				this.GameArea.Children.Remove(bonusFood.UiElement);
			});
		}

		private Rectangle[] GetBorders()
		{
			var array = new Rectangle[4];

			IEnumerator enumerator = null;
			Application.Current.Dispatcher.Invoke(() =>
			{
				enumerator = this.GameArea.Children.GetEnumerator();
			});

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
			Food food;
			do
			{
				var rnd = new Random();

				int gameAreaWidth = 0;
				int gameAreaHeight = 0;
				Application.Current.Dispatcher.Invoke(() =>
				{
					gameAreaWidth = (int)this.GameArea.Width;
					gameAreaHeight = (int)this.GameArea.Height;
				});

				var maxFoodSize = Food.SideLength + Food.ExtensionLimit;
				var point = new Models.Point(rnd.Next(0 + Food.ExtensionLimit / 2, gameAreaWidth - maxFoodSize),
										rnd.Next(0 + Food.ExtensionLimit / 2, gameAreaHeight - maxFoodSize));

				food = new Food(point, false, this);
			}
			while (!IsFoodPositionOk(food));

			Food = food;
			Food.StartBeating();

			return Food;
		}
		private bool IsFoodPositionOk(Food food)
		{
			foreach (var bonusFood in BonusFoods)
			{
				if (bonusFood.CollidesWith(food))
				{
					return false;
				}
			}

			if (this.Food != null)
			{
				if (food.CollidesWith(this.Food))
					return false;
			}

			if (food.CollidesWithAnyParallel(snake.Body.ToArray()))
				return false;

			return true;
		}
		private void SetUpGameArea()
		{
			snake = new Snake(this);
			Food = InitializeFood();
			Draw(Food);
		}
		private void RefreshGame()
		{
			try
			{
				snake.Move();

				if (snake.BitesItself() || snake.CollidesWithWalls())
					this.StopGame();

				if (snake.EatsFood())
				{
					this.RespawnFood();
					snake.Extend();

					Score++;
					if (Score % 5 == 0)
						SpawnBonusFood();
				}
				else if (snake.EatsBonusFood(out Food bonusFood))
				{
					this.Score += 5;
					RemoveBonusFood(bonusFood);
				}
			}
			catch (Exception e)
			{
				using (StreamWriter sw = new StreamWriter("C:\\Users\\rtserunyan\\Desktop\\Logs.txt"))
				{
					sw.WriteLine(e.Message + "----------- Object -----" + e.Source + "-----Inner---" + e.InnerException);
				}
			}
		}
		private void SpawnBonusFood()
		{
			int gameAreaWidth = 0;
			int gameAreaHeight = 0;
			Application.Current.Dispatcher.Invoke(() =>
			{
				gameAreaWidth = (int)this.GameArea.Width;
				gameAreaHeight = (int)this.GameArea.Height;
			});

			var rnd = new Random();
			var maxFoodSize = Food.SideLength * 2 + Food.ExtensionLimit;

			Food bonusFood;
			do
			{
				var point = new Models.Point(rnd.Next(0 + Food.ExtensionLimit / 2, gameAreaWidth - maxFoodSize),
										rnd.Next(0 + Food.ExtensionLimit / 2, gameAreaHeight - maxFoodSize));

				bonusFood = new Food(point, true, this);
			}
			while (!IsFoodPositionOk(bonusFood));

			this.BonusFoods.Add(bonusFood);
			this.Draw(bonusFood);

			bonusFood.StartBeating();
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
				//timer.Tick += (a, b) => RefreshGame();
				//timer.Start();
				timer = new Timer((obj) => this.RefreshGame(), null, 0, WindowRefreshRate);
			}
		}
	}
}
