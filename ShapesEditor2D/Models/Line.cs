namespace ShapesEditor2D.Models
{
	public class Line : Shape
	{
		public Vertex Start { get; private set; }
		public Vertex End { get; private set; }

		public Line(Vertex start, Vertex end)
		{
			Start = start;
			End = end;
		}

		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);

			Start.SetSelected(isSelected);
			End.SetSelected(isSelected);
		}

		public override IEnumerable<Vertex> GetVertices()
		{
			yield return Start;
			yield return End;
		}

		public override void Draw(Graphics g, bool length = false)
		{
			using (Pen pen = new(IsSelected ? Color.CadetBlue : Color.Black, IsSelected ? 2 : 1))
			{
				g.DrawLine(pen, Start.X, Start.Y, End.X, End.Y);
			}

			Start.Draw(g);
			End.Draw(g);

			if (length)
			{
				using (Font font = new("Arial", 10, FontStyle.Bold))
				{
					g.DrawString(GetLength().ToString("F2"), font, Brushes.Red, GetMid());
				}
			}
		}

		public override void Translate(double x, double y)
		{
			var translationMatrix = Matrix.CreateTranslationMatrix(x, y);
			Start = translationMatrix.Apply(Start);
			End = translationMatrix.Apply(End);
		}

		public void Rotate(double angle)
		{
			var rotationMatrix = Matrix.CreateRotationMatrix(angle);
			End = rotationMatrix.Apply(End);
		}

		public override bool ContainsPoint(Vertex v)
		{
			var totalLength = Start.DistanceTo(End);
			var lengthToStart = Start.DistanceTo(v);
			var lengthToEnd = End.DistanceTo(v);
			return Math.Abs(totalLength - (lengthToStart + lengthToEnd)) < Epsilon;
		}

		public void ExtendEnd(Vertex newV)
			=> End = newV;

		public double GetLength()
			=> Start.DistanceTo(End);

		public PointF GetMid()
			=> new PointF((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);

		public Vertex Intersect(Shape someShape)
		{
			switch (someShape)
			{
				case Line line:
					return IntersectWithLine(line);
				case Polyline polyline:
					return IntersectWithPolyline(polyline);
				default:
					return null;
			}
		}

		private Vertex IntersectWithLine(Line line)
		{
			var dir1 = new Vertex(End.X - Start.X, End.Y - Start.Y);
			var dir2 = new Vertex(line.End.X - line.Start.X, line.End.Y - line.Start.Y);
			var diff = new Vertex(line.Start.X - Start.X, line.Start.Y - Start.Y);

			double determinant = dir1.X * dir2.Y - dir1.Y * dir2.X;

			if (Math.Abs(determinant) < Epsilon)
				return null;

			double t1 = (diff.X * dir2.Y - diff.Y * dir2.X) / determinant;
			double t2 = (diff.X * dir1.Y - diff.Y * dir1.X) / determinant;

			if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
			{
				int intersectionX = (int)(Start.X + t1 * dir1.X);
				int intersectionY = (int)(Start.Y + t1 * dir1.Y);
				return new Vertex(intersectionX, intersectionY);
			}
			return null;
		}

		private Vertex IntersectWithPolyline(Polyline polyline)
		{
			for (int i = 0; i < polyline.Vertices.Count - 1; i++)
			{
				var edge = new Line(polyline.Vertices[i], polyline.Vertices[i + 1]);
				var intersection = IntersectWithLine(edge);
				if (intersection != null)
					return intersection;
			}
			return null;
		}

		private Vertex IntersectWithPolygon(Polygon polygon)
		{
			for (int i = 0; i < polygon.Vertices.Count; i++)
			{
				var start = polygon.Vertices[i];
				var end = polygon.Vertices[(i + 1) % polygon.Vertices.Count];
				var edge = new Line(start, end);
				var intersection = IntersectWithLine(edge);
				if (intersection != null)
					return intersection;
			}
			return null;
		}

		public override string ToString()
			=> $"Line [{Start} -> {End}]";

		public override bool Equals(object obj)
		{
			if (obj is Line otherLine)
			{
				return Start.Equals(otherLine.Start) && End.Equals(otherLine.End);
			}
			return false;
		}

		public override int GetHashCode()
			=> Start.GetHashCode() ^ End.GetHashCode();
	}
}