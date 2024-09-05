using ShapesEditor2D.Models;

namespace ShapesEditor2D.Factories
{
	public static class ShapeFactory
	{
		public static List<Shape> Shapes { get; private set; } = new List<Shape>();

		public static Vertex CreateVertex(Vertex v)
		{
			Shapes.Add(v);
			return v;
		}

		public static Line CreateLine(List<Vertex> vertices)
		{
			var line = new Line(vertices[0], vertices[1]);
			Shapes.Add(line);
			return line;
		}

		public static Polyline CreatePolyline(List<Vertex> vertices)
		{
			if (vertices.First().Equals(vertices.Last()))
				return null;

			var existingPolyline = GetLastAddedShape<Polyline>();
			if (existingPolyline?.Vertices[0].Equals(vertices[0]) == true)
			{
				existingPolyline.AddVertex(vertices.Last());
				return existingPolyline;
			}

			var polyline = new Polyline(vertices);
			Shapes.Add(polyline);
			return polyline;
		}

		public static Polygon CreatePolygon(List<Vertex> vertices)
		{
			if (!vertices.First().Equals(vertices.Last()))
				vertices.Add(vertices.First());

			var polygon = new Polygon(vertices);
			Shapes.Add(polygon);
			return polygon;
		}

		public static void RemoveShape(Shape shape)
		{
			Shapes.Remove(shape);
		}

		public static IEnumerable<Vertex> GetAllVertices()
		{
			foreach (var shape in Shapes)
			{
				foreach (var vertex in shape.GetVertices())
				{
					yield return vertex;
				}
			}
		}

		public static T GetLastAddedShape<T>() where T : Shape
		{
			for (int i = Shapes.Count - 1; i >= 0; i--)
			{
				if (Shapes[i] is T shape)
				{
					return shape;
				}
			}
			return null;
		}
	}
}