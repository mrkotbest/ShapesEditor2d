namespace ShapesEditor2D.Models
{
	public class Polygon : Polyline
	{
		public Polygon(IEnumerable<Vertex> vertices) : base(vertices)
		{
			if (Vertices.Count > 2 && !Vertices.First().Equals(Vertices.Last()))
			{
				Vertices.Add(Vertices.First());
			}
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);

			if (Vertices.Count > 2)
			{
				Pen pen = IsSelected ? new Pen(Color.Blue, 2) : Pens.Black;
				g.DrawLine(pen, Vertices.Last().X, Vertices.Last().Y, Vertices.First().X, Vertices.First().Y);
			}
		}
	}
}