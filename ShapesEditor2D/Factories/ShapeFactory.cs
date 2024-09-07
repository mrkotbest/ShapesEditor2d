using ShapesEditor2D.Models;

namespace ShapesEditor2D.Factories
{
	public static class ShapeFactory
	{
		public static List<Shape> Shapes { get; private set; } = new List<Shape>();

		public static void CreateVertex(Vertex v)
			=> Shapes.Add(v);

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

			var existingPolyline = GetLastAddedShape<Polygon>();
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
			var polygonVertices = new List<Vertex>(vertices);
			if (!polygonVertices.First().Equals(polygonVertices.Last()))
			{
				polygonVertices.Add(polygonVertices.First());
			}

			var polygon = new Polygon(polygonVertices);
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
					yield return vertex;
			}
		}

		public static T GetLastAddedShape<T>() where T : Shape
			=> Shapes.OfType<T>().LastOrDefault();

		public static void RemoveLastShape<T>() where T : Shape
		{
			var lastShape = GetLastAddedShape<T>();
			if (lastShape != null)
				Shapes.Remove(lastShape);
		}

		public static Shape GetShapeAtLocation(Point location, float radius = 10)
			=> Shapes.FirstOrDefault(shape => shape.GetVertices().Any(vertex => vertex.DistanceTo(location) <= radius));

		public static Vertex GetVertexAtPoint(Point point, float radius = 10)
			=> GetAllVertices().FirstOrDefault(v => v.DistanceTo(point) <= radius);
	}
}