namespace ShapesEditor2D.Models.Interfaces
{
    public interface ICompositeShape
    {
        List<Vertex> Vertices { get; }

        void AddVertex(Vertex vertex);
        void RemoveVertex(Vertex vertex);
    }
}