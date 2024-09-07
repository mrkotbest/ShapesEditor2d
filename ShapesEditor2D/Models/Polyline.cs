namespace ShapesEditor2D.Models
{
	public class Polyline : Shape, ICompositeShape
	{
		public List<Vertex> Vertices { get; } = new List<Vertex>();

		public Polyline(IEnumerable<Vertex> vertices)
		{
			Vertices.AddRange(vertices);
		}

		public void AddVertex(Vertex vertex)
			=> Vertices.Add(vertex);

		public void RemoveVertex(Vertex vertex)
			=> Vertices.Remove(vertex);

		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);

			foreach (var vertex in Vertices)
				vertex.SetSelected(isSelected);
		}

		public override IEnumerable<Vertex> GetVertices()
			=> Vertices;

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
				double totalLength = GetLength();
				var startPoint = Vertices.First();

				using Font font = new("Arial", 10, FontStyle.Bold);
				{
					g.DrawString(totalLength.ToString("F2"), font, Brushes.Red, startPoint.X, startPoint.Y);
				}
			}
		}

		public override void Translate(int x, int y)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertices[i].X += x;
				Vertices[i].Y += y;
			}
		}

		public override bool ContainsPoint(Vertex vertex)
		{
			if (Vertices.Count < 2) return false;
			return Vertices.Zip(Vertices.Skip(1), (start, end) => new Line(start, end))
						   .Any(line => line.ContainsPoint(vertex));
		}

		public IEnumerable<Vertex> Intersect(Shape shape)
		{
			var intersections = new List<Vertex>();

			for (int i = 0; i < Vertices.Count - 1; i++)
			{
				var line = new Line(Vertices[i], Vertices[i + 1]);
				intersections.AddRange(line.Intersect(shape));
			}

			return intersections.Where(intersection => intersection != null).ToList();
		}

		public double GetLength()
		{
			double length = 0.0;
			for (int i = 0; i < Vertices.Count - 1; i++)
			{
				var line = new Line(Vertices[i], Vertices[i + 1]);
				length += line.GetLength();
			}
			return length;
		}

		public void Smooth()
		{
			for (int i = 1; i < Vertices.Count - 1; i++)
			{
				Vertices[i].X = (Vertices[i - 1].X + Vertices[i].X + Vertices[i + 1].X) / 3;
				Vertices[i].Y = (Vertices[i - 1].Y + Vertices[i].Y + Vertices[i + 1].Y) / 3;
			}
		}

		public double GetAngleBetweenThreePoints(int i)
		{
			if (i <= 0 || i >= Vertices.Count - 1)
				MessageBox.Show("Три точки должны находиться внутри полилинии", "Некорректное расположение точек", MessageBoxButtons.OK, MessageBoxIcon.Warning);

			var a = Vertices[i - 1];
			var b = Vertices[i];
			var c = Vertices[i + 1];

			var ab = new Vertex(b.X - a.X, b.Y - a.Y);
			var bc = new Vertex(c.X - b.X, c.Y - b.Y);

			double dotProduct = ab.X * bc.X + ab.Y * bc.Y;
			double magnitudeAB = Math.Sqrt(ab.X * ab.X + ab.Y * ab.Y);
			double magnitudeBC = Math.Sqrt(bc.X * bc.X + bc.Y * bc.Y);

			return Math.Acos(dotProduct / (magnitudeAB * magnitudeBC));
		}

		public void RotateAroundPoint(Vertex point, double angleInRadians)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				double dx = Vertices[i].X - point.X;
				double dy = Vertices[i].Y - point.Y;
				double cos = Math.Cos(angleInRadians);
				double sin = Math.Sin(angleInRadians);
				int newX = (int)(cos * dx - sin * dy);
				int newY = (int)(sin * dx + cos * dy);

				Vertices[i] = new Vertex(newX + point.X, newY + point.Y);
			}
		}

		public bool IsPolygonCreated { get; set; } = false;
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
			double sum = 0;

			for (int i = 0; i < Vertices.Count - 1; i++)
			{
				var v1 = Vertices[i];
				var v2 = Vertices[i + 1];
				sum += (v2.X - v1.X) * (v2.Y + v1.Y);
			}
			return sum > 0;
		}

		public override string ToString()
			=> $"Polyline {(Vertices.Count > 5 ? "... -> " : string.Empty)}{string.Join(" -> ", Vertices.Skip(Math.Max(0, Vertices.Count - 5)))}";
	}
}