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

		private Point _selectionStart;
		private Rectangle _selectionRectangle;

		public MainForm() => InitializeComponent();

		private void MainForm_Load(object sender, EventArgs e)
			=> InitializeCustomTools();
		private void InitializeCustomTools()
		{
			_g = drawingBox.CreateGraphics();
			_selectionService = new SelectionService();
			_selectionRectangle = new Rectangle();
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
						var vertex = ShapeFactory.GetVertexAtPoint(new Point(e.X, e.Y));
						if (vertex != null)
						{
							if (_pointCircle)
							{
								ShapeFactory.Shapes.Add(vertex.CreateCircleAroundPoint(30));
								ssInfo.Text = "Создан круг";
							}
							else if (_pointRect)
							{
								ShapeFactory.Shapes.Add(vertex.CreateSquareAroundPoint(30));
								ssInfo.Text = "Создан квадрат";
							}
							else if (_pointTriangle)
							{
								ShapeFactory.Shapes.Add(vertex.CreateTriangleAroundPoint(30));
								ssInfo.Text = "Создан треугольник";
							}
						}
					}
					break;
				default:
					break;
			}
			drawingBox.Invalidate();
		}
		private void drawingBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (_mode == Mode.Select)
			{
				_selectionStart = e.Location;
				_isSelecting = true;
			}
		}
		private void drawingBox_MouseMove(object sender, MouseEventArgs e)
		{
			ssCoordinates.Text = $"X: {e.X}, Y: {e.Y}";

			if (_isSelecting)
			{
				_selectionRectangle.X = Math.Min(_selectionStart.X, e.X);
				_selectionRectangle.Y = Math.Min(_selectionStart.Y, e.Y);
				_selectionRectangle.Width = Math.Abs(e.X - _selectionStart.X);
				_selectionRectangle.Height = Math.Abs(e.Y - _selectionStart.Y);
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
			// Линии
			if (_isLineIntersection) IntersectionLine();
			if (_isLineLength) LineLength();
			if (_isLineExtend) ExtendLine(cursorLocation);
			if (_isLineRotate) RotateLine(cursorLocation);
			if (_isLineTranslate) TranslateLine(cursorLocation);

			// Полилинии
			if (_isPolylineIntersection) PolylineIntersection();
			if (_isPolylineLength) PolylineLength();
			if (_isPolylineSmooth) SmoothPolyline();
			if (_isPolylineAngle) PolylineAngle();
			if (_isPolylineRotate) RotatePolyline(cursorLocation);
			if (_isPolylineTranslate) TranslatePolyline(cursorLocation);
			if (_isPolylineCreatePlane) CreatePolylinePlane();
			if (_isPolylineDirection) PolylineDirection();
		}

		private void IntersectionLine()
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();

			foreach (var selectedLine in selectedLines)
			{
				foreach (var shape in ShapeFactory.Shapes.Where(s => s != selectedLine))
				{
					var intersections = selectedLine.Intersect(shape);
					if (intersections != null && intersections.Count != 0)
					{
						foreach (var intersection in intersections)
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
		private static double GetAngleBetweenCursorAndLine(Point cursorLocation, Line line)
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

			double averageX = selectedLines.Average(line => (line.Start.X + line.End.X) / 2);
			double averageY = selectedLines.Average(line => (line.Start.Y + line.End.Y) / 2);

			double offsetX = newLocation.X - averageX;
			double offsetY = newLocation.Y - averageY;

			foreach (var selectedLine in selectedLines)
			{
				var originalLine = ShapeFactory.Shapes.OfType<Line>().FirstOrDefault(line => line.Equals(selectedLine));
				originalLine?.Translate((int)offsetX, (int)offsetY);
			}
		}

		private void PolylineIntersection()
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();
			foreach (var selectedPolyline in selectedPolylines)
			{
				foreach (var shape in ShapeFactory.Shapes.Where(s => s != selectedPolyline))
				{
					var intersections = selectedPolyline.Intersect(shape);
					if (intersections != null && intersections.Any())
					{
						foreach (var intersection in intersections)
						{
							_selectionService.SelectVertex(intersection);
						}
					}
				}
			}
			ssInfo.Text = $"Найдено {_selectionService.SelectedVertices.Count} пересечений для полилинии";
		}
		private void PolylineLength()
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();

			if (selectedPolylines == null || !selectedPolylines.Any())
			{
				ssInfo.Text = "Нет выделенных полилиний.";
				return;
			}
			
			double totalLength = 0;
			totalLength += selectedPolylines.Sum(polyline => polyline.GetLength());

			ssInfo.Text = $"Суммарная длина полилиний: {totalLength:F2}";
		}
		private void SmoothPolyline()
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();
			foreach (var selectedPolyline in selectedPolylines)
				selectedPolyline.Smooth();

			ssInfo.Text = "Полилиния сглажена";
		}
		private void PolylineAngle()
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();
			foreach (var selectedPolyline in selectedPolylines)
			{
				double angle = selectedPolyline.GetAngleBetweenThreePoints(1);
				ssInfo.Text = $"Угол между точками полилинии: {Math.Round(angle * (180 / Math.PI), 2)}°";
			}
		}
		private void RotatePolyline(Point cursorLocation)
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();
			foreach (var selectedPolyline in selectedPolylines)
			{
				var firstVertex = selectedPolyline.Vertices.FirstOrDefault();
				if (firstVertex != null)
				{
					double angle = GetAngleBetweenCursorAndPoint(cursorLocation, firstVertex);
					selectedPolyline.RotateAroundPoint(firstVertex, angle);
				}
			}
			ssInfo.Text = "Полилиния повернута";
		}
		private static double GetAngleBetweenCursorAndPoint(Point cursorLocation, Vertex point)
		{
			var vectorToCursor = new Vertex(cursorLocation.X - point.X, cursorLocation.Y - point.Y);
			double angle = Math.Atan2(vectorToCursor.Y, vectorToCursor.X);
			return angle;
		}
		private void TranslatePolyline(Point cursorLocation)
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();
			if (selectedPolylines == null || !selectedPolylines.Any())
			{
				ssInfo.Text = "Нет выделенных полилиний для перемещения";
				return;
			}

			double averageX = selectedPolylines.Average(polyline => polyline.Vertices.Average(v => v.X));
			double averageY = selectedPolylines.Average(polyline => polyline.Vertices.Average(v => v.Y));

			double offsetX = cursorLocation.X - averageX;
			double offsetY = cursorLocation.Y - averageY;

			foreach (var selectedPolyline in selectedPolylines)
			{
				var originalPolyline = ShapeFactory.Shapes.OfType<Polyline>().FirstOrDefault(polyline => polyline.Equals(selectedPolyline));
				originalPolyline?.Translate((int)offsetX, (int)offsetY);
			}
			ssInfo.Text = "Полилиния перемещена";
		}
		private void CreatePolylinePlane()
		{
			var selectedShapes = _selectionService.GetSelectedShapesOfType<Polyline>();
			var shapesToRemove = new List<Shape>();

			foreach (var shape in selectedShapes)
			{
				if (shape is Polygon)
				{
					ssInfo.Text = "Выделенный объект уже является полигоном";
					continue;
				}

				if (shape is Polyline polyline)
				{
					if (polyline.IsPolygonCreated)
					{
						ssInfo.Text = "Для этой Полилинии уже создан Полигон..";
						continue;
					}

					if (polyline.Vertices.Count >= 3)
					{
						ShapeFactory.CreatePolygon(polyline.Vertices);
						polyline.IsPolygonCreated = true;
						shapesToRemove.Add(polyline);
						ssInfo.Text = "Плоскость создана на основе полилинии";
					}
					else
						ssInfo.Text = "Недостаточно точек для создания плоскости";
				}
			}

			foreach (var shape in shapesToRemove)
			{
				_selectionService.DeselectShape(shape);
				ShapeFactory.RemoveShape(shape);
			}
		}
		private void PolylineDirection()
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>();
			foreach (var selectedPolyline in selectedPolylines)
			{
				bool isClockwise = selectedPolyline.IsClockwise();
				ssInfo.Text = isClockwise ? "Полилиния построена по часовой стрелке" : "Полилиния построена против часовой стрелки";
			}
		}

		private void SnapTo(Point location, bool magnet = false)
		{
			var snappedVertex = SnappingService.SnapToVertex(location);
			if (snappedVertex != null)
			{
				_g.DrawEllipse(Pens.Blue, snappedVertex.X - 6, snappedVertex.Y - 6, 12, 12);
				if (magnet)
					ApplyMagnetEffect(location, snappedVertex);
			}
			else
				drawingBox.Invalidate();
		}
		private void ApplyMagnetEffect(Point location, Vertex snappedVertex)
		{
			var newCursorPos = _snapService.GetSnappedPosition(location, snappedVertex);
			Point screenPoint = drawingBox.PointToScreen(newCursorPos);
			CursorHelper.SetCursorPos(screenPoint.X, screenPoint.Y);
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

				if (_isLineLength || _isPolylineLength)
				{
					foreach (var shape in _selectionService.SelectedShapes)
						shape.Draw(e.Graphics, true);
				}

				if (_isLineIntersection || _isPolylineIntersection)
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
				ResetEveryMode();
				_mode = Mode.Draw;
				_selectionService.ClearSelection();
				ssInfoActivated.Text = "Режим рисования активирован";
			}
			else if (_mode == Mode.Draw)
			{
				_mode = Mode.None;
				_currentVertices.Clear();
				ssInfoActivated.Text = "Режим рисования завершен";
			}
		}
		private void tsSelection_Click(object sender, EventArgs e)
		{
			if (_mode != Mode.Select)
			{
				ResetEveryMode();
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
			ssInfoActivated.Text = _isSnapping ? "Режим привязки включен" : "Режим привязки выключен";
		}
		private void ResetEveryMode()
		{
			_mode = Mode.None;
			_currentVertices.Clear();
			_selectionService.ClearSelection();
			ResetAllPointModes();
			ResetAllLineModes();
			ResetAllPolylineModes();
			drawingBox.Invalidate();
			ssInfo.Text = string.Empty;
			ssInfoActivated.Text = "Все режими завершены";
		}
		
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				ResetEveryMode();

			if (e.Shift && e.KeyCode == Keys.T)
			{
				_selectionService?.ClearSelection();
				ssInfo.Text = string.Empty;
				drawingBox.Invalidate();
			}
		}

		private void ToggleMode(ref bool mode, Action resetModes, string modeName)
		{
			if (mode)
			{
				resetModes();
				ssInfoActivated.Text = $"{modeName} завершен";
			}
			else
			{
				resetModes();
				mode = true;
				_selectionService.ClearSelection();
				ssInfo.Text = string.Empty;
				ssInfoActivated.Text = $"{modeName} начат";
				drawingBox.Invalidate();
			}
		}	
		
		private bool _isClosestPoint, _isPointBelongsTo, _pointCircle, _pointRect, _pointTriangle;
		private void ResetAllPointModes() => _isClosestPoint = _isPointBelongsTo = _pointCircle = _pointRect = _pointTriangle = false;
		private void rtsPointClosestPoint_Click(object sender, EventArgs e) => ToggleMode(ref _isClosestPoint, ResetAllPointModes, "Режим ближайшей точки");
		private void rtsPointBelongsTo_Click(object sender, EventArgs e) => ToggleMode(ref _isPointBelongsTo, ResetAllPointModes, "Режим принадлежности точки");
		private void rtsPointCircle_Click(object sender, EventArgs e) => ToggleMode(ref _pointCircle, ResetAllPointModes, "Режим круга");
		private void rtsPointRect_Click(object sender, EventArgs e) => ToggleMode(ref _pointRect, ResetAllPointModes, "Режим прямоугольника");
		private void rtsPointTriangle_Click(object sender, EventArgs e) => ToggleMode(ref _pointTriangle, ResetAllPointModes, "Режим треугольника");

		private bool _isLineIntersection, _isLineLength, _isLineExtend, _isLineRotate, _isLineTranslate;
		private void ResetAllLineModes() => _isLineIntersection = _isLineLength = _isLineExtend = _isLineRotate = _isLineTranslate = false;
		private void rtsLineIntersection_Click(object sender, EventArgs e) => ToggleMode(ref _isLineIntersection, ResetAllLineModes, "Режим пересечения линий");
		private void rtsLineLength_Click(object sender, EventArgs e) => ToggleMode(ref _isLineLength, ResetAllLineModes, "Режим длины линии");
		private void rtsLineExtend_Click(object sender, EventArgs e) => ToggleMode(ref _isLineExtend, ResetAllLineModes, "Режим продления линии");
		private void rtsLineRotate_Click(object sender, EventArgs e) => ToggleMode(ref _isLineRotate, ResetAllLineModes, "Режим вращения линии");
		private void rtsLineTransform_Click(object sender, EventArgs e) => ToggleMode(ref _isLineTranslate, ResetAllLineModes, "Режим трансформации линии");

		private bool _isPolylineIntersection, _isPolylineLength, _isPolylineSmooth, _isPolylineAngle, _isPolylineRotate, _isPolylineTranslate, _isPolylineCreatePlane, _isPolylineDirection;
		private void ResetAllPolylineModes() => _isPolylineIntersection = _isPolylineLength = _isPolylineSmooth = _isPolylineAngle = _isPolylineRotate = _isPolylineTranslate = _isPolylineCreatePlane = _isPolylineDirection = false;
		private void rtsPolylineIntersection_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineIntersection, ResetAllPolylineModes, "Режим пересечения полилинии");
		private void rtsPolylineLength_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineLength, ResetAllPolylineModes, "Режим длины полилинии");
		private void rtsPolylineSmooth_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineSmooth, ResetAllPolylineModes, "Режим сглаживания полилинии");
		private void rtsPolylineAngle_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineAngle, ResetAllPolylineModes, "Режим угла полилинии");
		private void rtsPolylineRotate_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineRotate, ResetAllPolylineModes, "Режим вращения полилинии");
		private void rtsPolylineTranslate_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineTranslate, ResetAllPolylineModes, "Режим перемещения полилинии");
		private void rtsPolylineCreatePlane_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineCreatePlane, ResetAllPolylineModes, "Режим создания плоскости полилинии");
		private void rtsPolylineDirection_Click(object sender, EventArgs e) => ToggleMode(ref _isPolylineDirection, ResetAllPolylineModes, "Режим направления полилинии");
	}
}