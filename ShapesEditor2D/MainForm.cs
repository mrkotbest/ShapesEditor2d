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
		private bool _snappingEnabled = false;

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
						var v = ShapeFactory.GetVertexAtPoint(new Point(e.X, e.Y), 20);
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
							drawingBox.Invalidate();
						}
						else
							ssInfo.Text = string.Empty;
					}
					break;
			}
			drawingBox.Invalidate();
		}

		private void HandleDrawingMode(Point location)
		{
			_currentVertices.Add(new Vertex(location.X, location.Y));

			switch (_currentVertices.Count)
			{
				case 1:
					ShapeFactory.CreateVertex(_currentVertices[0]);
					ssInfo.Text = $"Создан объект: Точка {_currentVertices[0]}";
					break;
				case 2:
					var lastVertex = ShapeFactory.GetLastAddedShape<Vertex>();
					if (lastVertex != null)
						ShapeFactory.RemoveShape(lastVertex);

					ShapeFactory.CreateLine(_currentVertices);
					ssInfo.Text = $"Создан объект: Линия {_currentVertices[0]} -> {_currentVertices[1]}";
					break;
				default:
					if (_snapService.ShouldClosePolygon(_currentVertices))
					{
						var lastPolyline = ShapeFactory.GetLastAddedShape<Polyline>();
						if (lastPolyline != null)
							ShapeFactory.RemoveShape(lastPolyline);

						ShapeFactory.CreatePolygon(_currentVertices);
						ssInfo.Text = $"Создан объект: Полигон с {_currentVertices.Count} вершинами";
						_currentVertices.Clear();
					}
					else
					{
						var existingPolyline = ShapeFactory.GetLastAddedShape<Polyline>();
						if (existingPolyline != null && existingPolyline.Vertices[0].Equals(_currentVertices[0]))
						{
							existingPolyline.AddVertex(_currentVertices.Last());
							ssInfo.Text = $"Полилиния обновлена: {_currentVertices.Count} вершин.";
						}
						else
						{
							var lastLine = ShapeFactory.GetLastAddedShape<Line>();
							if (lastLine != null)
								ShapeFactory.RemoveShape(lastLine);

							ShapeFactory.CreatePolyline(_currentVertices);
							ssInfo.Text = $"Создан объект: Полилиния с {_currentVertices.Count} вершинами";
						}
					}
					break;
			}
		}

		private void HandleSelectionMode(Point location)
		{

		}

		private void SnapVertex(Point location, bool magnit = false)
		{
			var snappedVertex = SnappingService.SnapToVertex(ShapeFactory.GetAllVertices(), location);
			if (snappedVertex != null)
				_g.DrawEllipse(Pens.Blue, snappedVertex.X - 5, snappedVertex.Y - 5, 10, 10);
			if (magnit && snappedVertex != null)
				ApplyMagnetEffect(location, snappedVertex);
			drawingBox.Invalidate();
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
				_selectionService.ClearSelection();
				ssInfo.Text = "Нет выделенных объектов";
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

			if (_snappingEnabled)
			{
				SnapVertex(e.Location, true);
			}

			if (_isClosestPoint)
			{
				Vertex closestVertex = Vertex.GetClosestVertex(new Vertex(e.X, e.Y));
				foreach (var shape in ShapeFactory.Shapes)
				{
					shape.SetSelected(shape.Equals(closestVertex));
				}
				drawingBox.Invalidate();
			}

			if (_isPointBelongsTo)
			{
				var shape = ShapeFactory.GetShapeAtLocation(e.Location, 10);
				if (shape == null) return;

				var mousePos = new Vertex(e.X, e.Y);
				bool isWithinRadius = shape.GetVertices().Any(vertex => vertex.DistanceTo(mousePos) <= 10);

				if (isWithinRadius)
					ssInfo.Text = $"{mousePos} принадлежит объекту {shape}";
				else
					ssInfo.Text = string.Empty;

				SnapVertex(e.Location);
			}
		}
		private void drawingBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (_isSelecting)
			{
				_isSelecting = false;
				if (_selectionRectangle.Width > 1 && _selectionRectangle.Height > 1)
				{
					var shapes = _selectionService.GetShapesInRectangle(_selectionRectangle).ToList();
					_selectionRectangle = Rectangle.Empty;

					if (shapes.Count != 0)
					{
						var shapeGroups = shapes
							.GroupBy(shape => shape.GetType())
							.Select(group => new
							{
								Type = group.Key,
								Count = group.Count()
							});

						var info = string.Join(" \\ ", shapeGroups.Select(group =>
							$"{group.Type.Name}: {group.Count}"));
						ssInfo.Text = $"Найдено объектов:\n{info}";
					}
					drawingBox.Invalidate();
				}
			}
		}

		private void FinishAnyMode()
		{
			_mode = Mode.None;
			_isClosestPoint = false;
			_isPointBelongsTo = false;
			_pointCircle = false;
			_pointRect = false;
			_pointTriangle = false;
			_currentVertices.Clear();
			ResetAllPointModes();
			drawingBox.Invalidate();
			ssInfo.Text = string.Empty;
			ssInfoActivated.Text = "Все режими завершены";
		}
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				FinishAnyMode();
		}

		private void drawingBox_Paint(object sender, PaintEventArgs e)
		{
			if (_mode == Mode.Select)
			{
				using (Pen pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
				{
					e.Graphics.DrawRectangle(pen, _selectionRectangle);
				}
			}

			foreach (var shape in ShapeFactory.Shapes)
			{
				shape.Draw(e.Graphics);
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
			_snappingEnabled = !_snappingEnabled;
			ssInfoActivated.Text = _snappingEnabled ? "Привязка включена" : "Привязка выключена";
		}


		private void ResetAllPointModes()
			=> _isClosestPoint = _isPointBelongsTo = _pointCircle = _pointRect = _pointTriangle = false;
		private void ToggleMode(ref bool mode)
		{
			if (mode)
				ResetAllPointModes();
			else
			{
				ResetAllPointModes();
				mode = true;
			}
		}

		private bool _isClosestPoint, _isPointBelongsTo, _pointCircle, _pointRect, _pointTriangle;
		private void rtsPointClosestPoint_Click(object sender, EventArgs e) => ToggleMode(ref _isClosestPoint);
		private void rtsPointBelongsTo_Click(object sender, EventArgs e) => ToggleMode(ref _isPointBelongsTo);
		private void rtsPointCircle_Click(object sender, EventArgs e) => ToggleMode(ref _pointCircle);
		private void rtsPointRect_Click(object sender, EventArgs e) => ToggleMode(ref _pointRect);
		private void rtsPointTriangle_Click(object sender, EventArgs e) => ToggleMode(ref _pointTriangle);
	}
}