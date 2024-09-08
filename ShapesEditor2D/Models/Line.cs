using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

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

		public override void Draw(Graphics g, params bool[] parameters)
		{
			using (Pen pen = new(IsSelected ? Color.CadetBlue : Color.Black, IsSelected ? 2 : 1))
			{
				g.DrawLine(pen, Start.X, Start.Y, End.X, End.Y);
			}

			if (parameters.Length > 0 && parameters[0])
			{
				using (Pen pen = new(IsSelected ? Color.RosyBrown : Color.Black, IsSelected ? 2 : 1))
				{
					g.DrawLine(pen, Start.X, Start.Y, End.X, End.Y);
				}
				using (Font font = new("Arial", 10, FontStyle.Bold))
				{
					var mid = GetMidPoint();
					g.DrawString(GetLength().ToString("F2"), font, Brushes.Red, mid.X, mid.Y);
				}
			}
			Start.Draw(g);
			End.Draw(g);
		}

		public override void Translate(int x, int y)
		{
			Matrix translationMatrix = new Matrix();
			translationMatrix.Translate(x, y);
			Point[] points = { new(Start.X, Start.Y), new(End.X, End.Y) };
			translationMatrix.TransformPoints(points);
			Start = new Vertex(points[0].X, points[0].Y);
			End = new Vertex(points[1].X, points[1].Y);
		}

		private Vertex _initialEnd;
		public void Rotate(int mouseX, int mouseY)
		{
			_initialEnd ??= new Vertex(End.X, End.Y);

			var curr_dx = _initialEnd.X - Start.X;
			var curr_dy = _initialEnd.Y - Start.Y;

			var dx = mouseX - Start.X;
			var dy = mouseY - Start.Y;

			var rotangle = Math.Atan2((dy * curr_dx) - (dx * curr_dy), (dx * curr_dx) + (dy * curr_dy));

			var new_x = (int)(Start.X + (curr_dx * Math.Cos(rotangle)) - (curr_dy * Math.Sin(rotangle)));
			var new_y = (int)(Start.Y + (curr_dx * Math.Sin(rotangle)) + (curr_dy * Math.Cos(rotangle)));

			End = new Vertex(new_x, new_y);
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

		public Vertex GetMidPoint()
			=> new((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);

		public List<Vertex> Intersect(Shape someShape)
		{
			var intersections = new List<Vertex>();

			switch (someShape)
			{
				case Line line:
					var intersectionWithLine = IntersectWithLine(line);
					if (intersectionWithLine != null && intersectionWithLine.Any())
						intersections.AddRange(intersectionWithLine);
					break;
				case Polyline polyline:
					var intersectionsWithPolyline = IntersectWithPolyline(polyline);
					if (intersectionsWithPolyline != null && intersectionsWithPolyline.Any())
						intersections.AddRange(intersectionsWithPolyline);
					break;
				default:
					return null;
			}

			return intersections;
		}

		private List<Vertex> IntersectWithLine(Line line)
		{
			var intersections = new List<Vertex>();

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
				intersections.Add(new Vertex(intersectionX, intersectionY));
			}

			return intersections;
		}

		private List<Vertex> IntersectWithPolyline(Polyline polyline)
		{
			var intersections = new List<Vertex>();

			for (int i = 0; i < polyline.Vertices.Count - 1; i++)
			{
				var edge = new Line(polyline.Vertices[i], polyline.Vertices[i + 1]);
				var vertexIntersections = IntersectWithLine(edge);
				intersections.AddRange(vertexIntersections);
			}

			return intersections;
		}

		private List<Vertex> IntersectWithPolygon(Polygon polygon)
		{
			var intersections = new List<Vertex>();

			for (int i = 0; i < polygon.Vertices.Count; i++)
			{
				var start = polygon.Vertices[i];
				var end = polygon.Vertices[(i + 1) % polygon.Vertices.Count];
				var edge = new Line(start, end);
				var vertexIntersections = IntersectWithLine(edge);
				intersections.AddRange(vertexIntersections);
			}

			return intersections;
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