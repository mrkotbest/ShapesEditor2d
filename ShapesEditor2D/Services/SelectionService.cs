using ShapesEditor2D.Factories;
using ShapesEditor2D.Models;

namespace ShapesEditor2D.Services
{
	public class SelectionService
	{
		public List<Shape> SelectedShapes { get; private set; } = new List<Shape>();
		public List<Vertex> SelectedVertices { get; private set; } = new List<Vertex>();

		public void SelectShape(Shape shape)
		{
			if (!SelectedShapes.Contains(shape) && shape != null)
			{
				SelectedShapes.Add(shape);
				shape.SetSelected(true);
			}
		}

		public void DeselectShape(Shape shape)
		{
			if (shape != null)
			{
				SelectedShapes.Remove(shape);
				shape.SetSelected(false);
			}
		}

		public void ClearSelection()
		{
			if (SelectedShapes.Count > 0)
			{
				foreach (var shape in SelectedShapes.ToList())
					DeselectShape(shape);
			}

			SelectedShapes.Clear();
			SelectedVertices.Clear();
		}

		public IEnumerable<Shape> GetShapesInRectangle(Rectangle rectangle)
		{
			foreach (var shape in ShapeFactory.Shapes)
			{
				if (IsShapeInRectangle(shape, rectangle))
				{
					SelectShape(shape);
					yield return shape;
				}
			}
		}

		private bool IsShapeInRectangle(Shape shape, Rectangle rectangle)
		{
			foreach (var vertex in shape.GetVertices())
			{
				if (rectangle.Contains(new Point(vertex.X, vertex.Y)))
					return true;
			}
			return false;
		}
	}
}