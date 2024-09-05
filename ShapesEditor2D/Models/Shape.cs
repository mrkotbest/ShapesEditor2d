namespace ShapesEditor2D.Models
{
	public abstract class Shape
	{
		public bool IsSelected { get; private set; }
		public virtual void SetSelected(bool isSelected)
			=> IsSelected = isSelected;
		public abstract IEnumerable<Vertex> GetVertices();
		public abstract void Draw(Graphics g);
		public abstract bool ContainsPoint(Vertex v);
		public abstract void Transform(int x, int y);
	}
}