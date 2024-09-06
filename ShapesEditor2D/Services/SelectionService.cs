using ShapesEditor2D.Factories;
using ShapesEditor2D.Models;

namespace ShapesEditor2D.Services
{
	public sealed class SelectionService
	{
		public List<Shape> SelectedShapes { get; private set; } = new List<Shape>();
		public List<Vertex> SelectedVertices { get; private set; } = new List<Vertex>();

		public void SelectShape(Shape shape)
		{
			if (shape != null && !SelectedShapes.Contains(shape))
			{
				SelectedShapes.Add(shape);
				shape.SetSelected(true);
			}
		}

		public bool DeselectShape(Shape shape)
		{
			if (shape != null)
			{
				shape.SetSelected(false);
				return SelectedShapes.Remove(shape);
			}
			return false;
		}

		public void SelectVertex(Vertex vertex)
		{
			if (vertex != null && !SelectedVertices.Contains(vertex))
			{
				SelectedVertices.Add(vertex);
				vertex.SetSelected(true);
			}
		}

		public bool DeselectVertex(Vertex vertex)
		{
			if (vertex != null)
			{
				vertex.SetSelected(false);
				return SelectedVertices.Remove(vertex);
			}
			return false;
		}

		public void ClearSelection()
		{
			foreach (var shape in SelectedShapes.ToList())
				DeselectShape(shape);

			foreach (var vertex in SelectedVertices.ToList())
				DeselectVertex(vertex);

			SelectedShapes.Clear();
			SelectedVertices.Clear();
		}

		public void AddSelectedShapesFrom(Rectangle rectangle)
		{
			foreach (var shape in ShapeFactory.Shapes)
			{
				if (IsShapeInRectangle(shape, rectangle))
				{
					SelectShape(shape);
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

		public IEnumerable<T> GetSelectedShapesOfType<T>() where T : Shape
			=> SelectedShapes.Where(shape => shape is T).Cast<T>();

		public T FindOriginalShape<T>(T selectedShape) where T : Shape
		{
			return ShapeFactory.Shapes
				.OfType<T>()
				.FirstOrDefault(shape => shape.Equals(selectedShape));
		}
	}
}