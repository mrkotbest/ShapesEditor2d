﻿namespace ShapesEditor2D.Models
{
	public class Polygon : Polyline
	{
		public Polygon(IEnumerable<Vertex> vertices) : base(vertices)
		{
			if (Vertices.Count > 2 && !Vertices.First().Equals(Vertices.Last()))
				Vertices.Add(Vertices.First());
		}

		public override void Draw(Graphics g, bool length = false)
		{
			base.Draw(g);
			if (Vertices.Count > 2)
			{
				Pen pen = IsSelected ? new Pen(Color.Blue, 2) : Pens.Black;
				g.DrawLine(pen, Vertices.Last().X, Vertices.Last().Y, Vertices.First().X, Vertices.First().Y);
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