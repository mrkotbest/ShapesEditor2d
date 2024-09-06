using ShapesEditor2D.Factories;
using ShapesEditor2D.Helpers;
using ShapesEditor2D.Models;
using ShapesEditor2D.Services;

namespace ShapesEditor2D
{
	public partial class MainForm : Form
	{
		private Graphics _g;
		private Mode _mode = Mode.None;
		private SelectionService _selectionService;
		private SnappingService _snapService;
		private List<Vertex> _currentVertices;
		private bool _isSelecting = false;
		private bool _isSnapping = false;

		private Rectangle _selectionRectangle;

		public MainForm() => InitializeComponent();

		private void MainForm_Load(object sender, EventArgs e)
		{
			InitializeCustomTools();
		}

		private void InitializeCustomTools()
		{
			_g = drawingBox.CreateGraphics();
			_selectionService = new SelectionService();
			_snapService = new SnappingService();
			_currentVertices = new List<Vertex>();
		}

		private void drawingBox_MouseClick(object sender, MouseEventArgs e)
		{
			switch (_mode)
			{
				case Mode.Draw:
					HandleDrawingMode(e.Location);
					break;
				case Mode.Select:
					HandleSelectionMode(e.Location);
					break;
				case Mode.None:
					if (_pointCircle is true || _pointRect is true || _pointTriangle is true)
					{
						var v = ShapeFactory.GetVertexAtPoint(new Point(e.X, e.Y));
						if (v != null)
						{
							if (_pointCircle)
							{
								var circle = v.CreateCircleAroundPoint(30);
								ShapeFactory.Shapes.Add(circle);
								ssInfo.Text = "Создан круг";
							}
							else if (_pointRect)
							{
								var rect = v.CreateSquareAroundPoint(30);
								ShapeFactory.Shapes.Add(rect);
								ssInfo.Text = "Создан квадрат";
							}
							else if (_pointTriangle)
							{
								var triangle = v.CreateTriangleAroundPoint(30);
								ShapeFactory.Shapes.Add(triangle);
								ssInfo.Text = "Создан треугольник";
							}
						}
					}
					break;
			}
			drawingBox.Invalidate();
		}

		private void HandleDrawingMode(Point cursorLocation)
		{
			_currentVertices.Add(new Vertex(cursorLocation.X, cursorLocation.Y));
			int vertexCount = _currentVertices.Count;

			if (vertexCount == 1)
			{
				ShapeFactory.CreateVertex(_currentVertices[0]);
				ssInfo.Text = $"Создан объект: Точка {_currentVertices[0]}";
			}
			else if (vertexCount == 2)
			{
				ShapeFactory.RemoveLastShape<Vertex>();
				ShapeFactory.CreateLine(_currentVertices);
				ssInfo.Text = $"Создан объект: Линия {_currentVertices[0]} -> {_currentVertices[1]}";
			}
			else
			{
				if (_snapService.ShouldClosePolygon(_currentVertices))
				{
					ShapeFactory.RemoveLastShape<Polyline>();
					ShapeFactory.CreatePolygon(_currentVertices);
					ssInfo.Text = $"Создан объект: Полигон с {vertexCount} вершинами";
					_currentVertices.Clear();
				}
				else
				{
					var existingPolyline = ShapeFactory.GetLastAddedShape<Polyline>();
					if (existingPolyline != null && existingPolyline.Vertices[0].Equals(_currentVertices[0]))
					{
						existingPolyline.AddVertex(_currentVertices.Last());
						ssInfo.Text = $"Полилиния обновлена: {vertexCount} вершин.";
					}
					else
					{
						ShapeFactory.RemoveLastShape<Line>();
						ShapeFactory.CreatePolyline(_currentVertices);
						ssInfo.Text = $"Создан объект: Полилиния с {vertexCount} вершинами";
					}
				}
			}
		}

		private void HandleSelectionMode(Point cursorLocation)
		{
			if (_isLineIntersection)
			{
				IntersectionLine();
			}

			if (_isLineLength)
			{
				LineLength();
			}

			if (_isLineExtend)
			{
				ExtendLine(cursorLocation);
			}

			if (_isLineRotate)
			{
				RotateLine(cursorLocation);
			}

			if (_isLineTranslate)
			{
				TranslateLine(cursorLocation);
			}
		}

		private void IntersectionLine()
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();

			foreach (var selectedLine in selectedLines)
			{
				foreach (var shape in ShapeFactory.Shapes)
				{
					if (shape != selectedLine)
					{
						var intersection = selectedLine.Intersect(shape);
						if (intersection != null)
						{
							_selectionService.SelectVertex(intersection);
						}
					}
				}
			}
			ssInfo.Text = $"Найдено {_selectionService.SelectedVertices.Count} пересечений";
		}

		private void LineLength()
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();

			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "Нет выделенных линий для получения длины";
				return;
			}

			double totalLength = selectedLines.Sum(line => line.GetLength());
			ssInfo.Text = $"Суммарная длина выбранных линий: {totalLength:F2}";
		}

		private void ExtendLine(Point newEndPoint)
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();

			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "Нет выделенных линий для продления";
				return;
			}

			foreach (var selectedLine in selectedLines)
			{
				var originalLine = ShapeFactory.Shapes.OfType<Line>().FirstOrDefault(line => line.Equals(selectedLine));
				originalLine?.ExtendEnd(new Vertex(newEndPoint.X, newEndPoint.Y));
			}
		}

		private void RotateLine(Point cursorLocation)
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();

			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "Нет выделенных линий для поворота";
				return;
			}

			foreach (var selectedLine in selectedLines)
			{
				var originalLine = ShapeFactory.Shapes.OfType<Line>()
					.FirstOrDefault(line => line.Equals(selectedLine));

				double angle = GetAngleBetweenCursorAndLine(cursorLocation, originalLine);
				originalLine?.Rotate(angle);
			}
		}

		private double GetAngleBetweenCursorAndLine(Point cursorLocation, Line line)
		{
			var lineVector = new Vertex(line.End.X - line.Start.X, line.End.Y - line.Start.Y);
			var cursorVector = new Vertex(cursorLocation.X - line.Start.X, cursorLocation.Y - line.Start.Y);

			double lineAngle = Math.Atan2(lineVector.Y, lineVector.X);
			double cursorAngle = Math.Atan2(cursorVector.Y, cursorVector.X);

			return cursorAngle - lineAngle;
		}

		private void TranslateLine(Point newLocation)
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();

			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "Нет выделенных линий для перемещения";
				return;
			}

			var averageX = selectedLines.Average(line => (line.Start.X + line.End.X) / 2);
			var averageY = selectedLines.Average(line => (line.Start.Y + line.End.Y) / 2);

			var offsetX = newLocation.X - averageX;
			var offsetY = newLocation.Y - averageY;

			foreach (var selectedLine in selectedLines)
			{
				var originalLine = ShapeFactory.Shapes.OfType<Line>()
					.FirstOrDefault(line => line.Equals(selectedLine));

				originalLine?.Translate(offsetX, offsetY);
			}
		}

		private void SnapTo(Point location, bool magnet = false)
		{
			var snappedVertex = SnappingService.SnapToVertex(location);
			if (snappedVertex != null)
				_g.DrawEllipse(Pens.Blue, snappedVertex.X - 6, snappedVertex.Y - 6, 12, 12);
			else
				drawingBox.Invalidate();
			if (magnet && snappedVertex != null)
				ApplyMagnetEffect(location, snappedVertex);
		}
		private void ApplyMagnetEffect(Point location, Vertex snappedVertex)
		{
			var newCursorPos = _snapService.GetSnappedCursorPosition(location, snappedVertex);
			Point screenPoint = drawingBox.PointToScreen(new Point((int)newCursorPos.X, (int)newCursorPos.Y));
			CursorHelper.SetCursorPos(screenPoint.X, screenPoint.Y);
		}

		private void drawingBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (_mode == Mode.Select)
			{
				_isSelecting = true;
				_selectionRectangle = new Rectangle(e.Location, new Size(0, 0));
			}
		}
		private void drawingBox_MouseMove(object sender, MouseEventArgs e)
		{
			ssCoordinates.Text = $"X: {e.X}, Y: {e.Y}";

			if (_isSelecting)
			{
				_selectionRectangle.Size = new Size(e.X - _selectionRectangle.Left, e.Y - _selectionRectangle.Top);
				drawingBox.Invalidate();
			}

			if (_isSnapping)
				SnapTo(e.Location, true);

			if (_isClosestPoint)
			{
				_selectionService.ClearSelection();
				_selectionService.SelectVertex(Vertex.GetClosestVertex(e.Location));
				drawingBox.Invalidate();
			}

			if (_isPointBelongsTo && ShapeFactory.GetShapeAtLocation(e.Location, 12) is { } shape)
			{
				bool isWithinRadius = shape.GetVertices().Any(vertex => vertex.DistanceTo(e.Location) <= 6);
				ssInfo.Text = isWithinRadius ? $"{e.Location} принадлежит объекту {shape}" : string.Empty;
				SnapTo(e.Location);
			}
		}
		private void drawingBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (!_isSelecting) return;

			_isSelecting = false;
			if (_selectionRectangle.Width > 1 && _selectionRectangle.Height > 1)
			{
				_selectionService.AddSelectedShapesFrom(_selectionRectangle);
				_selectionRectangle = Rectangle.Empty;
				if (_selectionService.SelectedShapes.Count != 0)
				{
					var info = string.Join(" \\ ", _selectionService.SelectedShapes
						.GroupBy(shape => shape.GetType())
						.Select(group => $"{group.Key.Name}: {group.Count()}"));
					ssInfo.Text = $"Найдено объектов:\n{info}";
				}
			}
		}

		private void FinishEveryMode()
		{
			_mode = Mode.None;
			_isSnapping = false;
			_currentVertices.Clear();
			_selectionService.ClearSelection();
			ResetAllPointModes();
			ResetAllLineModes();
			drawingBox.Invalidate();
			ssInfo.Text = string.Empty;
			ssInfoActivated.Text = "Все режими завершены";
		}
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				FinishEveryMode();

			if (e.Shift && e.KeyCode == Keys.T)
			{
				_selectionService?.ClearSelection();
				ssInfo.Text = string.Empty;
				drawingBox.Invalidate();
			}
		}

		private void drawingBox_Paint(object sender, PaintEventArgs e)
		{
			foreach (var shape in ShapeFactory.Shapes)
				shape.Draw(e.Graphics);

			if (_mode == Mode.Select)
			{
				using (Pen pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
				{
					e.Graphics.DrawRectangle(pen, _selectionRectangle);
				}

				foreach (var shape in _selectionService.SelectedShapes)
					shape.Draw(e.Graphics);

				foreach (var vertex in _selectionService.SelectedVertices)
					vertex.Draw(e.Graphics);

				if (_isLineLength)
				{
					foreach (var shape in _selectionService.SelectedShapes)
						shape.Draw(e.Graphics, true);
				}

				if (_isLineIntersection)
				{
					foreach (var vertex in _selectionService.SelectedVertices)
						vertex.Draw(e.Graphics, true);
				}
			}
		}
		private void tsDraw_Click(object sender, EventArgs e)
		{
			if (_mode != Mode.Draw)
			{
				_mode = Mode.Draw;
				ssInfoActivated.Text = "Режим рисования активирован";
				_selectionService.ClearSelection();
			}
			else if (_mode == Mode.Draw)
			{
				_mode = Mode.None;
				ssInfoActivated.Text = "Режим рисования завершен";
				_currentVertices.Clear();
			}
		}
		private void tsSelection_Click(object sender, EventArgs e)
		{
			if (_mode != Mode.Select)
			{
				_mode = Mode.Select;
				ssInfoActivated.Text = "Режим выделения активирован";
			}
			else if (_mode == Mode.Select)
			{
				_mode = Mode.None;
				_selectionService.ClearSelection();
				ssInfoActivated.Text = "Режим выделения закончен";
			}
		}
		private void tsSnap_Click(object sender, EventArgs e)
		{
			_isSnapping = !_isSnapping;
			ssInfoActivated.Text = _isSnapping ? "Привязка включена" : "Привязка выключена";
		}

		private void ToggleMode(ref bool mode, Action resetModes)
		{
			if (mode)
			{
				resetModes();
				ssInfoActivated.Text = $"Режим завершен";
			}
			else
			{
				resetModes();
				mode = true;
				ssInfoActivated.Text = $"Режим начат";
			}
		}

		private bool _isClosestPoint, _isPointBelongsTo, _pointCircle, _pointRect, _pointTriangle;
		private void ResetAllPointModes() => _isClosestPoint = _isPointBelongsTo = _pointCircle = _pointRect = _pointTriangle = false;
		private void rtsPointClosestPoint_Click(object sender, EventArgs e) => ToggleMode(ref _isClosestPoint, ResetAllPointModes);
		private void rtsPointBelongsTo_Click(object sender, EventArgs e) => ToggleMode(ref _isPointBelongsTo, ResetAllPointModes);
		private void rtsPointCircle_Click(object sender, EventArgs e) => ToggleMode(ref _pointCircle, ResetAllPointModes);
		private void rtsPointRect_Click(object sender, EventArgs e) => ToggleMode(ref _pointRect, ResetAllPointModes);
		private void rtsPointTriangle_Click(object sender, EventArgs e) => ToggleMode(ref _pointTriangle, ResetAllPointModes);

		private bool _isLineIntersection, _isLineLength, _isLineExtend, _isLineRotate, _isLineTranslate;
		private void ResetAllLineModes() => _isLineIntersection = _isLineLength = _isLineExtend = _isLineRotate = _isLineTranslate = false;
		private void rtsLineIntersection_Click(object sender, EventArgs e) => ToggleMode(ref _isLineIntersection, ResetAllLineModes);
		private void rtsLineLength_Click(object sender, EventArgs e) => ToggleMode(ref _isLineLength, ResetAllLineModes);
		private void rtsLineExtend_Click(object sender, EventArgs e) => ToggleMode(ref _isLineExtend, ResetAllLineModes);
		private void rtsLineRotate_Click(object sender, EventArgs e) => ToggleMode(ref _isLineRotate, ResetAllLineModes);
		private void rtsLineTransform_Click(object sender, EventArgs e) => ToggleMode(ref _isLineTranslate, ResetAllLineModes);

	}
}