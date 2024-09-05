namespace ShapesEditor2D.Models
{
	public class Polyline : Shape, ICompositeShape
	{
		private const float _tolerance = 3.0f;  // Допустимая погрешность для "попадания" точки на линию

		public List<Vertex> Vertices { get; } = new List<Vertex>();

		public Polyline(IEnumerable<Vertex> vertices)
		{
			Vertices.AddRange(vertices);
		}

		public void AddVertex(Vertex vertex)
		{
			Vertices.Add(vertex);
		}

		public void RemoveVertex(Vertex vertex)
		{
			Vertices.Remove(vertex);
		}

		public override void SetSelected(bool isSelected)
		{
			base.SetSelected(isSelected);
			foreach (var vertex in Vertices)
			{
				vertex.SetSelected(isSelected);
			}
		}

		public override IEnumerable<Vertex> GetVertices()
		{
			return Vertices;
		}

		public override void Draw(Graphics g)
		{
			Pen pen = IsSelected ? new Pen(Color.CadetBlue, 2) : Pens.Black;

			for (int i = 0; i < Vertices.Count - 1; i++)
			{
				g.DrawLine(pen, Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y);
				Vertices[i].Draw(g);
			}
			Vertices[^1].Draw(g);
		}

		public override bool ContainsPoint(Vertex v)
		{
			for (int i = 0; i < Vertices.Count - 1; i++)
			{
				if (IsPointOnLine(Vertices[i], Vertices[i + 1], v))
				{
					return true;
				}
			}
			return false;
		}

		public override void Transform(int x, int y)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertices[i] = new Vertex(Vertices[i].X + x, Vertices[i].Y + y);
			}
		}

		private bool IsPointOnLine(Vertex startLine, Vertex endLine, Vertex v)
		{
			float distance = Math.Abs((endLine.Y - startLine.Y) * v.X - (endLine.X - startLine.X) *
				v.Y + endLine.X * startLine.Y - endLine.Y * startLine.X) /
				(float)Math.Sqrt(Math.Pow(endLine.Y - startLine.Y, 2) + Math.Pow(endLine.X - startLine.X, 2));
			return distance <= _tolerance;
		}

		public override string ToString()
			=> string.Join(" -> ", Vertices.Select(v => $"{v.X}:{v.Y}"));
	}
}