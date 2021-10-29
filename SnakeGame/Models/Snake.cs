using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SnakeGame.Enums;

namespace SnakeGame.Models
{
	public class Snake
	{
		//Fields
		public const int MovingSpeed = 2;
		private List<SnakePart> body;
		private MainWindow mainWindow;

		//Properties
		public Direction Direction
		{
			set
			{
				//If the input was the same or the opposite
				//direction which the snake currently goes:
				//then ignore the input
				if (value == this.Head.CurrentDirection
					|| (int)value == (int)this.Head.CurrentDirection * -1)
					return;

				var changeDirectionPoint = new ChangeDirectionPoint(null, value);

				foreach (var part in body)
				{
					//If the queue of the part contains any records
					if (part.DirectionsQueue.Any())
					{
						//Find the latest added record which contains no target point
						//and assign it one
						using (var enumerator = part.DirectionsQueue.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								var current = enumerator.Current;
								if (current.Point == null)
								{
									current.Point = new Point(this.Head.Position);
								}
							}
						}
					}
					else
					{
						if (part.CurrentTargetPoint == null)
						{
							part.CurrentTargetPoint = new Point(this.Head.Position);
						}
					}

					part.DirectionsQueue.Enqueue(changeDirectionPoint);
				}
			}
		}
		public SnakePart Head => body[0];
		public SnakePart Tail => body[Length - 1];
		public int Length => body.Count;

		//Indexers
		public SnakePart this[int index] => body[index];

		//Constructors
		public Snake(MainWindow _mw)
		{
			mainWindow = _mw;
			body = new List<SnakePart>(4);

			//Get the position for the snake's head
			int posX = 0;
			int posY = 0;

			mainWindow.Dispatcher.Invoke(() =>
			{
				posX = (int)(mainWindow.GameArea.Width / 2);
				posY = (int)(mainWindow.GameArea.Height / 2);
			});

			//Create the snake's head
			var snakeHead = new SnakePart(posX, posY, true);
			snakeHead.CurrentDirection = Direction.None;
			body.Add(snakeHead);
			mainWindow.Draw(snakeHead);
			Application.Current.Dispatcher.Invoke(() => { Panel.SetZIndex(snakeHead.UiElement, 100); });

			//Create two more snake parts
			this.AddSnakePart(new Point(Head.Position.X,
										Head.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(posX, posY), Direction.Up));
			this.AddSnakePart(new Point(Tail.Position.X,
										Tail.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(posX, posY), Direction.Up));
		}

		//Methods
		public void Move()
		{
			foreach (var part in body)
			{
				part.Move();
				mainWindow.Refresh(part);
			}
		}
		public void AddPart()
		{
			//Not implemented

			//AddSnakePart();
		}
		public bool BitesItself()
		{
			bool snakeBytesItself = this.Head.CollidesWithAnyParalell(body.Skip(2).ToArray());

			return snakeBytesItself;
		}
		public bool CollidesWithWalls()
		{
			var array = mainWindow.Borders;

			bool collidesWithWalls = this.Head.CollidesWithAnyParalell(array);

			return collidesWithWalls;
		}
		public void Extend()
		{
			//Adding a new part to the snake
			int x = 0;
			int y = 0;

			switch (this.Tail.CurrentDirection)
			{
				case Direction.None:
					break;
				case Direction.Left:
					x = 1;
					break;
				case Direction.Up:
					y = 1;
					break;
				case Direction.Right:
					x = -1;
					break;
				case Direction.Down:
					y = -1;
					break;
				default:
					break;
			}

			var currentTail = this.Tail;
			var newPart = AddSnakePart(new Point(currentTail.Position.X + x * SnakePart.SideLength,
												currentTail.Position.Y + y * SnakePart.SideLength));

			newPart.CurrentDirection = currentTail.CurrentDirection;
			newPart.CurrentTargetPoint = currentTail.CurrentTargetPoint != null ? new Point(currentTail.CurrentTargetPoint) : null;
			newPart.DirectionsQueue = CloneQueue(currentTail.DirectionsQueue);
		}
		public bool EatsFood()
		{
			return this.Head.CollidesWith(mainWindow.Food);
		}

		private SnakePart AddSnakePart(Point point)
		{
			//Create the new Part
			var newPart = new SnakePart(point.X, point.Y);
			body.Add(newPart);

			//Draw the part
			mainWindow.Draw(newPart);

			return newPart;
		}
		private static Queue<ChangeDirectionPoint> CloneQueue(Queue<ChangeDirectionPoint> source)
		{
			var queue = new Queue<ChangeDirectionPoint>();

			using (var enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var point = enumerator.Current.Point != null ? new Point(enumerator.Current.Point) : null;
					var changeDirPoint = new ChangeDirectionPoint(point,
																	enumerator.Current.Direction);
					queue.Enqueue(changeDirPoint);
				}
			}

			return queue;
		}
	}
}