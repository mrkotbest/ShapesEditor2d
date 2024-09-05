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
		private bool _isSelecting = false;
		private bool _snappingEnabled = false;
		private List<Vertex> _currentVertices = new List<Vertex>();

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
		}

		private void drawingBox_MouseClick(object sender, MouseEventArgs e)
		{
			switch (_mode)
			{
				case Mode.None:
					break;
				case Mode.Snap:
					break;
				case Mode.Draw:
					HandleDrawingMode(e.Location);
					break;
				case Mode.Select:
					HandleSelectionMode(e.Location);
					break;
				default:
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
					// Создание точки
					ShapeFactory.CreateVertex(_currentVertices[0]);
					ssInfo.Text = $"Создан объект: Точка {_currentVertices[0]}";
					break;

				case 2:
					// Создание линии
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

		private void DrawSnappedVertex(Point location)
		{
			var snappedVertex = SnappingService.SnapToVertex(ShapeFactory.GetAllVertices(), location);
			if (snappedVertex != null)
			{
				ApplyMagnetEffect(location, snappedVertex);
				_g.DrawEllipse(Pens.Blue, snappedVertex.X - 5, snappedVertex.Y - 5, 10, 10);
			}
			else
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
				DrawSnappedVertex(e.Location);
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

					if (shapes.Any())
					{
						// Группируем объекты по типу и считаем количество каждого типа
						var shapeGroups = shapes
							.GroupBy(shape => shape.GetType())
							.Select(group => new
							{
								Type = group.Key,
								Count = group.Count()
							});

						// Формируем строку с количеством объектов каждого типа
						var info = string.Join("\n", shapeGroups.Select(group =>
							$"{group.Type.Name}: {group.Count}"));

						// Отображаем информацию в ssInfo.Text
						ssInfo.Text = $"Найдено объектов:\n{info}";
					}
					else
					{
						ssInfo.Text = "Нет объектов в выделенной области.";
					}

					drawingBox.Invalidate();
				}
			}
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
				FinishDrawingMode();
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

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				FinishDrawingMode();
		}

		private void FinishDrawingMode()
		{
			_mode = Mode.None;
			_currentVertices.Clear();
			drawingBox.Invalidate();
			ssInfoActivated.Text = "Режим рисования завершен.";
		}
	}
}