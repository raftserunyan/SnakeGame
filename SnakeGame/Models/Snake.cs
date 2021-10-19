using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
				//If the intput was the same or the opposite
				//direction which the snake currently goes:
				//then ignore the input
				if (value == this.Head.CurrentDirection
					|| (int)value == (int)this.Head.CurrentDirection * -1)
					return;

				var changeDirectionPoint = new ChangeDirectionPoint(null, value);

				foreach (var part in body)
				{
					//If the queue of the part, contains any records
					if (part.DirectionsQueue.Any())
					{
						//Find the latest added record which containsno target point
						//and assign it a one
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
			int posX = (int)(mainWindow.GameArea.Width / 2);
			int posY = (int)(mainWindow.GameArea.Height / 2);

			//Create the snake's head
			var snakeHead = new SnakePart(posX, posY, true);
			body.Add(snakeHead);
			mainWindow.Draw(snakeHead);

			//Create two more snake parts
			this.AddSnakePart(new Point(Head.Position.X,
										Head.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(175, 175), Direction.Up));
			this.AddSnakePart(new Point(Tail.Position.X,
										Tail.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(175, 175), Direction.Up));
			this.AddSnakePart(new Point(Tail.Position.X,
										Tail.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(175, 175), Direction.Up));
			this.AddSnakePart(new Point(Tail.Position.X,
										Tail.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(175, 175), Direction.Up));
			this.AddSnakePart(new Point(Tail.Position.X,
										Tail.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(175, 175), Direction.Up));
			this.AddSnakePart(new Point(Tail.Position.X,
										Tail.Position.Y + SnakePart.SideLength))
											.DirectionsQueue.Enqueue(new ChangeDirectionPoint(new Point(175, 175), Direction.Up));
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

		private SnakePart AddSnakePart(Point point)
		{
			//Create the new Part
			var newPart = new SnakePart(point.X, point.Y);
			body.Add(newPart);

			//Draw the part
			mainWindow.Draw(newPart);

			return newPart;
		}
	}
}