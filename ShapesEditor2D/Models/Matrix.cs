namespace ShapesEditor2D.Models
{
	public class Matrix
	{
		private readonly double[,] _matrix;

		public Matrix(double[,] matrix)
		{
			_matrix = matrix;
		}

		public static Matrix CreateTranslationMatrix(double x, double y)
		{
			double[,] translationMatrix = new double[,]
			{
				{ 1, 0, x },
				{ 0, 1, y },
				{ 0, 0, 1 }
			};
			return new Matrix(translationMatrix);
		}

		public static Matrix CreateRotationMatrix(double angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);

			double[,] rotationMatrix = new double[,]
			{
				{ cos, -sin, 0 },
				{ sin, cos, 0 },
				{ 0, 0, 1 }
			};
			return new Matrix(rotationMatrix);
		}

		public Vertex Apply(Vertex point)
		{
			double[] pointArray = { point.X, point.Y, 1 };
			double[] result = new double[3];

			for (int i = 0; i < 3; i++)
				result[i] = _matrix[i, 0] * pointArray[0] + _matrix[i, 1] * pointArray[1] + _matrix[i, 2] * pointArray[2];

			return new Vertex((int)result[0], (int)result[1]);
		}

		public static double DegreesToRadians(double degrees)
			=> degrees * (Math.PI / 180);
	}
}