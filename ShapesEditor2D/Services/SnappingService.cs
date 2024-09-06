using ShapesEditor2D.Factories;
using ShapesEditor2D.Models;

namespace ShapesEditor2D.Services
{
	public class SnappingService
	{
		private const float _snappingTolerance = 5.0f;
		private float _magnetStrength = 0.3f;

		public static Vertex SnapToVertex(PointF point)
		{
			return ShapeFactory.GetAllVertices()
				.Where(vertex => Math.Abs(vertex.X - point.X) <= _snappingTolerance && Math.Abs(vertex.Y - point.Y) <= _snappingTolerance)
				.OrderBy(vertex => Math.Sqrt(Math.Pow(vertex.X - point.X, 2) + Math.Pow(vertex.Y - point.Y, 2)))
				.FirstOrDefault();
		}

		public PointF GetSnappedCursorPosition(PointF originalPos, Vertex snappedVertex)
		{
			float newX = originalPos.X + (snappedVertex.X - originalPos.X) * _magnetStrength;
			float newY = originalPos.Y + (snappedVertex.Y - originalPos.Y) * _magnetStrength;
			return new PointF(newX, newY);
		}

		public bool ShouldClosePolygon(List<Vertex> vertices)
		{
			if (vertices.Count > 2)
			{
				var firstVertex = vertices.First();
				var lastVertex = vertices.Last();
				return Math.Abs(firstVertex.X - lastVertex.X) <= _snappingTolerance &&
					   Math.Abs(firstVertex.Y - lastVertex.Y) <= _snappingTolerance;
			}
			return false;
		}

		public void DrawSnappingGuide(Graphics g, List<Vertex> vertices, Point location)
		{
			if (ShouldClosePolygon(vertices))
			{
				var firstVertex = vertices.First();
				var lastVertex = vertices.Last();
				g.DrawLine(Pens.Red, lastVertex.X, lastVertex.Y, firstVertex.X, firstVertex.Y);
			}
		}
	}
}