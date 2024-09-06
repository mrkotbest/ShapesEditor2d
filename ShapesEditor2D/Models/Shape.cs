namespace ShapesEditor2D.Models
{
	public abstract class Shape
	{
		public const float Epsilon = 0.0001f;
		public bool IsSelected { get; private set; }

		public virtual void SetSelected(bool isSelected) => IsSelected = isSelected;
		public abstract IEnumerable<Vertex> GetVertices();
		public abstract void Draw(Graphics g, bool length = false);
		public abstract void Translate(double distance, double angle);
		public abstract bool ContainsPoint(Vertex vertex);
	}
}