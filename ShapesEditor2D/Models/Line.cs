namespace ShapesEditor2D.Models
{
	public class Line : Shape
	{
		private const float _tolerance = 3.0f;	// Допустимая погрешность для "попадания" точки на линию

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

		public override void Draw(Graphics g)
		{
			if (IsSelected)
			{
				using (Pen pen = new(Color.CadetBlue, 2))
				{
					g.DrawLine(pen, Start.X, Start.Y, End.X, End.Y);
				}
			}
			else
			{
				g.DrawLine(Pens.Black, Start.X, Start.Y, End.X, End.Y);
			}

			Start.Draw(g);
			End.Draw(g);
		}

		public override bool ContainsPoint(Vertex v)
		{
			return IsPointOnLine(Start, End, v);
		}

		public override void Transform(int x, int y)
		{
			Start = new Vertex(Start.X + x, Start.Y + y);
			End = new Vertex(End.X + x, End.Y + y);
		}

		private bool IsPointOnLine(Vertex startLine, Vertex endLine, Vertex v)
		{
			float distance = Math.Abs((endLine.Y - startLine.Y) * v.X - (endLine.X - startLine.X) *
				v.Y + endLine.X * startLine.Y - endLine.Y * startLine.X) /
				(float)Math.Sqrt(Math.Pow(endLine.Y - startLine.Y, 2) + Math.Pow(endLine.X - startLine.X, 2));
			return distance <= _tolerance;
		}

		public override string ToString()
			=> $"{Start} -> {End}";
	}
}
