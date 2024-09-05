using ShapesEditor2D.Models;

namespace ShapesEditor2D.Services
{
	public class SnappingService
	{
		private const float _snappingTolerance = 3.0f;
		private float _magnetStrength = 1f;

		public static Vertex SnapToVertex(IEnumerable<Vertex> vertices, PointF point)
		{
			return vertices.FirstOrDefault(vertex =>
				Math.Abs(vertex.X - point.X) <= _snappingTolerance &&
				Math.Abs(vertex.Y - point.Y) <= _snappingTolerance);
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