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
				g.DrawLine(Pens.Black, Start.X, Start.Y, End.X, End.Y);

			Start.Draw(g);
			End.Draw(g);
		}

		public override void Transform(int x, int y)
		{
			Start = new Vertex(Start.X + x, Start.Y + y);
			End = new Vertex(End.X + x, End.Y + y);
		}

		public override bool ContainsPoint(Vertex v)
		{
			var totalLength = Start.DistanceTo(End);
			var lengthToStart = Start.DistanceTo(v);
			var lengthToEnd = End.DistanceTo(v);

			return Math.Abs(totalLength - (lengthToStart + lengthToEnd)) < Epsilon;
		}

		public override string ToString()
			=> $"Line [{Start} -> {End}]";
	}
}