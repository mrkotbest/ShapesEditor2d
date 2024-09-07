using ShapesEditor2D.Models.Interfaces;

namespace ShapesEditor2D.Models
{
    public class Polyline : Shape, ICompositeShape
	{
		public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

		public Polyline(IEnumerable<Vertex> vertices)
		{
			Vertices.AddRange(vertices);
		}

		public void AddVertex(Vertex vertex) => Vertices.Add(vertex);
		public void RemoveVertex(Vertex vertex) => Vertices.Remove(vertex);

		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);
			foreach (var vertex in Vertices)
				vertex.SetSelected(isSelected);
		}

		public override IEnumerable<Vertex> GetVertices() => Vertices;

		public override void Draw(Graphics g, bool length = false)
		{
			Pen pen = IsSelected ? new Pen(Color.CadetBlue, 2) : Pens.Black;

			for (int i = 0; i < Vertices.Count - 1; i++)
			{
				g.DrawLine(pen, Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y);
				Vertices[i].Draw(g);
			}
			Vertices[^1].Draw(g);

			if (length)
			{
				var startPoint = Vertices.First();
				using Font font = new("Arial", 10, FontStyle.Bold);
				{
					g.DrawString(GetLength().ToString("F2"), font, Brushes.Red, startPoint.X, startPoint.Y);
				}
			}
		}

		public override void Translate(int x, int y)
			=> Vertices.ForEach(v => { v.X += x; v.Y += y; });

		public override bool ContainsPoint(Vertex vertex)
		{
			return Vertices.Count >= 2 && Vertices
				.Zip(Vertices.Skip(1), (start, end) => new Line(start, end))
				.Any(line => line.ContainsPoint(vertex));
		}

		public IEnumerable<Vertex> Intersect(Shape shape)
		{
			return Vertices
				.Zip(Vertices.Skip(1), (start, end) => new Line(start, end))
				.SelectMany(line => line.Intersect(shape))
				.Where(intersection => intersection != null);
		}

		public double GetLength()
		{
			return Vertices
				.Zip(Vertices.Skip(1), (start, end) => new Line(start, end).GetLength())
				.Sum();
		}

		public void Smooth()
		{
			if (Vertices.Count < 3) return;
			Vertices = Vertices.Select((v, i) => i == 0 || i == Vertices.Count - 1 ? v : new Vertex(
				(Vertices[i - 1].X + 4 * v.X + Vertices[i + 1].X) / 6,
				(Vertices[i - 1].Y + 4 * v.Y + Vertices[i + 1].Y) / 6
			)).ToList();
		}

		public double GetAngleBetweenThreePoints(int i)
		{
			if (i <= 0 || i >= Vertices.Count - 1)
			{
				MessageBox.Show("Три точки должны находиться внутри полилинии", "Некорректное расположение точек", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return double.NaN;
			}

			var a = Vertices[i - 1];
			var b = Vertices[i];
			var c = Vertices[i + 1];

			var ab = new Vertex(b.X - a.X, b.Y - a.Y);
			var bc = new Vertex(c.X - b.X, c.Y - b.Y);

			double dotProduct = ab.X * bc.X + ab.Y * bc.Y;
			double abLength = Math.Sqrt(ab.X * ab.X + ab.Y * ab.Y);
			double bcLength = Math.Sqrt(bc.X * bc.X + bc.Y * bc.Y);

			return Math.Acos(dotProduct / (abLength * bcLength));
		}

		public void RotateAroundPoint(Vertex point, double angleInRadians)
		{
			double cos = Math.Cos(angleInRadians);
			double sin = Math.Sin(angleInRadians);

			Vertices = Vertices.Select(v =>
			{
				int newX = (int)(cos * (v.X - point.X) - sin * (v.Y - point.Y) + point.X);
				int newY = (int)(sin * (v.X - point.X) + cos * (v.Y - point.Y) + point.Y);
				return new Vertex(newX, newY);
			}).ToList();
		}

		public bool IsPolygonCreated { get; private set; } = false;
		public IEnumerable<Vertex> CreatePlane()
		{
			if (Vertices.Count < 3)
			{
				MessageBox.Show("Недостаточно точек для создания плоскости", "Недостаточно точек", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return null;
			}
			IsPolygonCreated = true;
			return Vertices;
		}

		public bool IsClockwise()
		{
			return Vertices
				.Zip(Vertices.Skip(1), (v1, v2) => (v2.X - v1.X) * (v2.Y + v1.Y))
				.Sum() > 0;
		}

		public override string ToString()
			=> $"Polyline {(Vertices.Count > 5 ? "... -> " : string.Empty)}{string.Join(" -> ", Vertices.Skip(Math.Max(0, Vertices.Count - 5)))}";
	}
}