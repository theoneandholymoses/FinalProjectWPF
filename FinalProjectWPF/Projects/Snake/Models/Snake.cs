using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWPF.Projects.Snake.Models
{
    public class Snake
    {
        public List<Point> Body { get; private set; }
        public Point Head => Body[Body.Count - 1];

        public Direction direction { get; set; }

        public Snake(int x, int y)
        {
            Body = new List<Point> { new Point(x, y) };
            direction = Direction.Right;
        }

        public void Move()
        {
            Point newHead = new Point(Head.X, Head.Y);

            switch (direction)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            Body.Add(newHead);
            Body.RemoveAt(0);
        }

        public void Grow()
        {
            Point lastPoint = Body[Body.Count - 1];
            Body.Add(new Point(lastPoint.X, lastPoint.Y));
        }

        public void ChangeDirection(Direction newDirection)
        {
            if ((direction == Direction.Up && newDirection != Direction.Down) ||
                (direction == Direction.Down && newDirection != Direction.Up) ||
                (direction == Direction.Left && newDirection != Direction.Right) ||
                (direction == Direction.Right && newDirection != Direction.Left))
            {
                direction = newDirection;
            }
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
