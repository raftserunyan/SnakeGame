using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using SnakeGame.Enums;
using SnakeGame.Models.Interfaces;

namespace SnakeGame.Models
{
	public class SnakePart : IDrawable
	{
		public Point CurrentTargetPoint;
		public Direction CurrentDirection;

		public bool IsHead { get; set; }
		public UIElement UiElement { get; set; }
		public Point Position { get; set; }
		public Queue<ChangeDirectionPoint> DirectionsQueue { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public SnakePart(int x, int y) : this(x, y, false)
		{
		}
		public SnakePart(int x, int y, bool isHead)
		{
			Position = new Point(x, y);
			IsHead = isHead;

			Application.Current.Dispatcher.Invoke(() =>
			{
				Width = Settings.SnakePartSideLength;
				Height = Settings.SnakePartSideLength;
				UiElement = new Ellipse()
				{
					Width = Settings.SnakePartSideLength,
					Height = Settings.SnakePartSideLength,
					Fill = isHead ? Brushes.Red : Brushes.Yellow
				};
			});

			DirectionsQueue = new Queue<ChangeDirectionPoint>();
		}

		public void Move()
		{
			if (this.CurrentTargetPoint != null)
			{
				if (this.HasReachedTarget())
				{
					CurrentTargetPoint = null;

					ChangeDirectionPoint changeDirPoint;
					this.DirectionsQueue.TryDequeue(out changeDirPoint);

					if (changeDirPoint != null)
					{
						CurrentTargetPoint = changeDirPoint.Point;
						CurrentDirection = changeDirPoint.Direction;
					}
				}
			}
			else
			{
				if (this.CurrentDirection == Direction.None)
				{
					//This snake part is moving for the first time

					ChangeDirectionPoint changeDirPoint;
					this.DirectionsQueue.TryDequeue(out changeDirPoint);

					if (changeDirPoint != null)
					{
						if (!this.IsHead)
							CurrentTargetPoint = changeDirPoint.Point;
						CurrentDirection = changeDirPoint.Direction;
					}
					else
					{
						throw new InvalidOperationException("Directions queue was empty!");
					}
				}
				if (this.IsHead && this.DirectionsQueue.Any())
				{
					var changeDirPoint = this.DirectionsQueue.Dequeue();
					CurrentDirection = changeDirPoint.Direction;
				}
			}

			MoveToDirection();
		}

		private bool HasReachedTarget()
		{
			return this.Position.X == CurrentTargetPoint.X
					&& this.Position.Y == CurrentTargetPoint.Y;
		}
		private void MoveToDirection()
		{
			switch (this.CurrentDirection)
			{
				case Direction.Left:
					this.Position.X -= Settings.SnakeMovingSpeed;
					break;
				case Direction.Up:
					this.Position.Y -= Settings.SnakeMovingSpeed;
					break;
				case Direction.Right:
					this.Position.X += Settings.SnakeMovingSpeed;
					break;
				case Direction.Down:
					this.Position.Y += Settings.SnakeMovingSpeed;
					break;
				default:
					return;
			}
		}
	}
}
