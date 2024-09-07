using ShapesEditor2D.Factories;

namespace ShapesEditor2D.Models
{
	public class Vertex : Shape
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Vertex(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override void SetSelected(bool isSelected)
			=> base.SetSelected(isSelected);

		public override IEnumerable<Vertex> GetVertices() { yield return this; }

		public override void Draw(Graphics g, bool length = false)
		{
			if (IsSelected)
				g.FillRectangle(Brushes.Aqua, X - 5, Y - 5, 10, 10);
			else
				g.FillEllipse(Brushes.Black, X - 3, Y - 3, 6, 6);

			if (length)
			{
				g.FillEllipse(Brushes.Red, X - 5, Y - 5, 10, 10);
				using (Font font = new Font("Arial", 10, FontStyle.Bold))
				{
					g.DrawString(ToString(), font, Brushes.Red, new PointF(X, Y));
				}
			}
		}

		public override void Translate(int x, int y) {  }

		public override bool ContainsPoint(Vertex vertex)
			=> Equals(vertex);

		public double DistanceTo(Point other)
			=> Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));

		public double DistanceTo(Vertex other)
			=> Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));

		public static Vertex GetClosestVertex(Point point)
			=> ShapeFactory.GetAllVertices()
				.OrderBy(p => Math.Sqrt(Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2)))
				.FirstOrDefault();

		public Polygon CreateCircleAroundPoint(double radius, int numSegments = 15)
		{
			double angleStep = 2 * Math.PI / numSegments;
			var points = Enumerable.Range(0, numSegments).Select(i =>
			{
				double angle = i * angleStep;
				return new Vertex((int)(X + radius * Math.Cos(angle)), (int)(Y + radius * Math.Sin(angle)));
			}).ToList();
			return new Polygon(points);
		}

		public Polygon CreateSquareAroundPoint(double diameter)
		{
			double halfDiameter = diameter / 2;
			var points = new List<Vertex>
			{
				new Vertex((int)(X - halfDiameter), (int)(Y - halfDiameter)),
				new Vertex((int)(X + halfDiameter), (int)(Y - halfDiameter)),
				new Vertex((int)(X + halfDiameter), (int)(Y + halfDiameter)),
				new Vertex((int)(X - halfDiameter), (int)(Y + halfDiameter))
			};
			return new Polygon(points);
		}

		public Polygon CreateTriangleAroundPoint(double height)
		{
			double radius = height / Math.Sqrt(3);
			double[] angles = { 0, 2 * Math.PI / 3, 4 * Math.PI / 3 };
			
			var vertices = angles.Select(angle => new Vertex((int)(X + radius * Math.Cos(angle)), (int)(Y + radius * Math.Sin(angle)))).ToList();
			return new Polygon(vertices);
		}

		public override string ToString()
			=> $"V:({X}:{Y})";
		public override bool Equals(object obj)
		{
			if (obj is Vertex otherVertex)
			{
				return X == otherVertex.X && Y == otherVertex.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}