using ShapesEditor2D.Factories;
using ShapesEditor2D.Models;

namespace ShapesEditor2D.Services
{
	public sealed class SnappingService
	{
		private const float _snappingTolerance = 3.5f;
		private float _magnetStrength = 0.25f;

		public static Vertex SnapToVertex(PointF point)
		{
			return ShapeFactory.GetAllVertices()
				.Where(vertex => Math.Abs(vertex.X - point.X) <= _snappingTolerance && Math.Abs(vertex.Y - point.Y) <= _snappingTolerance)
				.OrderBy(vertex => Math.Sqrt(Math.Pow(vertex.X - point.X, 2) + Math.Pow(vertex.Y - point.Y, 2)))
				.FirstOrDefault();
		}

		public Point GetSnappedPosition(PointF originalPos, Vertex snappedVertex)
		{
			int newX = (int)(originalPos.X + (snappedVertex.X - originalPos.X) * _magnetStrength);
			int newY = (int)(originalPos.Y + (snappedVertex.Y - originalPos.Y) * _magnetStrength);
			return new Point(newX, newY);
		}

		public bool ShouldClosePolygon(Vertex first, Vertex last)
			=> Math.Abs(first.X - last.X) <= _snappingTolerance && Math.Abs(first.Y - last.Y) <= _snappingTolerance;
	}
}