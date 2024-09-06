using System.Drawing;

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
		}

		public override void Translate(double x, double y)
		{

		}

		public override bool ContainsPoint(Vertex vertex)
		{
			if (Vertices.Count < 2)
				return false;

			return Vertices.Zip(Vertices.Skip(1), (start, end) => new Line(start, end))
						   .Any(segment => segment.ContainsPoint(vertex));
		}

		public override string ToString()
			=> $"Polyline {string.Join(" -> ", Vertices.Select(v => $"{v}"))}";
	}
}