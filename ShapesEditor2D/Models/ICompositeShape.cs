namespace ShapesEditor2D.Models
{
	public interface ICompositeShape
	{
		List<Vertex> Vertices { get; }

		void AddVertex(Vertex vertex);
		void RemoveVertex(Vertex vertex);
		bool ContainsPoint(Vertex vertex);
	}
}