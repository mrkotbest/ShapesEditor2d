namespace ShapesEditor2D.Models
{
	public class Vertex : Shape
	{
		public int X { get; private set; }
		public int Y { get; private set; }

		public Vertex(int x, int y)
		{
			X = x;
			Y = y;
		}

		public double DistanceTo(Vertex other)
			=> Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));

		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);
		}

		public override IEnumerable<Vertex> GetVertices()
		{
			yield return this;
		}

		public override void Draw(Graphics g)
		{
			if (IsSelected)
				g.FillRectangle(Brushes.Aqua, X - 3, Y - 3, 9, 9);
			else
				g.FillEllipse(Brushes.Black, X - 3, Y - 3, 6, 6);
		}

		public override bool ContainsPoint(Vertex v)
		{
			return X == v.X && Y == v.Y;
		}

		public override void Transform(int x, int y)
		{
			X += x;
			Y += y;
		}

		public override string ToString()
			=> $"({X}:{Y})";
		public override bool Equals(object obj)
		{
			if (obj is Vertex other)
			{
				return X == other.X && Y == other.Y;
			}
			return false;
		}
		public override int GetHashCode()
			=> HashCode.Combine(X, Y);
	}
}