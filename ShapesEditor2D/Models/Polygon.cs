namespace ShapesEditor2D.Models
{
	public class Polygon : Polyline
	{
		public Polygon(IEnumerable<Vertex> vertices) : base(vertices) { }

		public override void Draw(Graphics g, params bool[] parameters)
		{
			base.Draw(g);
			if (Vertices.Count <= 2) return;

			if (parameters.Length > 0 && parameters[0])
			{
				using (Font font = new("Arial", 10, FontStyle.Bold))
				{
					g.DrawString(CalculateArea().ToString(), font, Brushes.Red, Vertices.First().X, Vertices.First().Y);
					return;
				}
			}

			var center = CalculateCentroid();
			if (parameters.Length > 1 && parameters[1])
				DrawCircle(g, center, GetInradius(), Color.Red);

			if (parameters.Length > 2 && parameters[2])
				DrawCircle(g, center, GetCircumradius(), Color.Red);

			using (Pen pen = new(IsSelected ? Color.CadetBlue : Color.Black, IsSelected ? 2 : 1))
			{
				g.DrawLine(pen, Vertices.Last().X, Vertices.Last().Y, Vertices.First().X, Vertices.First().Y);
			}
		}

		private void DrawCircle(Graphics g, Vertex center, double radius, Color color)
		{
			var r = Convert.ToSingle(radius);
			using (Pen pen = new(color, 2))
			{
				g.DrawEllipse(pen, center.X - r, center.Y - r, 2 * r, 2 * r);
			}
		}

		public override bool ContainsPoint(Vertex point)
		{
			bool isInside = false;
			for (int i = 0, j = Vertices.Count - 1; i < Vertices.Count; j = i++)
			{
				if ((Vertices[i].Y > point.Y) != (Vertices[j].Y > point.Y) &&
					point.X < (Vertices[j].X - Vertices[i].X) * (point.Y - Vertices[i].Y) / (Vertices[j].Y - Vertices[i].Y) + Vertices[i].X)
				{
					isInside = !isInside;
				}
			}
			return isInside;
		}

		public double CalculateArea()
		{
			double area = 0;
			int j = Vertices.Count - 1;
			for (int i = 0; i < Vertices.Count; i++)
			{
				area += (Vertices[j].X + Vertices[i].X) * (Vertices[j].Y - Vertices[i].Y);
				j = i;
			}
			return Math.Abs(area / 2);
		}

		public double GetPerimeter()
		{
			double perimeter = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				var current = Vertices[i];
				var next = Vertices[(i + 1) % Vertices.Count];
				perimeter += Math.Sqrt(Math.Pow(next.X - current.X, 2) + Math.Pow(next.Y - current.Y, 2));
			}
			return perimeter;
		}

		public double GetInradius()
		{
			double area = CalculateArea();
			double perimeter = GetPerimeter();
			return 2 * area / perimeter;
		}
		public double GetCircumradius()
		{
			double maxDistance = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				for (int j = i + 1; j < Vertices.Count; j++)
				{
					double distance = Math.Sqrt(Math.Pow(Vertices[j].X - Vertices[i].X, 2) + Math.Pow(Vertices[j].Y - Vertices[i].Y, 2));
					if (distance > maxDistance)
					{
						maxDistance = distance;
					}
				}
			}
			return maxDistance / 2;
		}

		public Vertex CalculateCentroid()
		{
			double signedArea = 0.0;
			double x0 = 0.0;
			double y0 = 0.0;

			int i, j;
			for (i = 0, j = Vertices.Count - 1; i < Vertices.Count; i++)
			{
				double xi = Vertices[i].X;
				double yi = Vertices[i].Y;
				double xj = Vertices[j].X;
				double yj = Vertices[j].Y;

				double a = (xi * yj - xj * yi);
				signedArea += a;
				x0 += (xi + xj) * a;
				y0 += (yi + yj) * a;
				j = i;
			}

			signedArea *= 0.5;
			x0 /= (6.0 * signedArea);
			y0 /= (6.0 * signedArea);

			return new Vertex((int)x0, (int)y0);
		}

		public override string ToString()
		{
			var endPoints = Vertices.Skip(Math.Max(0, Vertices.Count - 4)).Take(3);
			var result = Vertices.Count > 4
				? $"{Vertices[0]} ... {string.Join(" -> ", endPoints)}"
				: string.Join(" -> ", Vertices);
			return $"Polygon {result}";
		}
	}
}