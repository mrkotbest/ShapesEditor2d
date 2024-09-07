using ShapesEditor2D.Factories;
using ShapesEditor2D.Helpers;
using ShapesEditor2D.Models;
using ShapesEditor2D.Models.Enums;
using ShapesEditor2D.Services;

namespace ShapesEditor2D
{
	public partial class MainForm : Form
	{
		private Graphics _g;
		private MainMode _mode = MainMode.None;
		private Mode _modee = Mode.None;
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

		#region MouseEvents
		private void drawingBox_MouseClick(object sender, MouseEventArgs e)
		{
			switch (_mode)
			{
				case MainMode.Draw:
					HandleDrawingMode(e.Location);
					break;
				case MainMode.Select:
					HandleSelectionMode(e.Location);
					break;
				case MainMode.None:
					if (_modee == Mode.PointCircle || _modee == Mode.PointRect || _modee == Mode.PointTriangle)
					{
						var vertex = ShapeFactory.GetVertexAtPoint(new Point(e.X, e.Y));
						if (vertex != null)
						{
							if (_modee == Mode.PointCircle)
							{
								ShapeFactory.Shapes.Add(vertex.CreateCircleAroundPoint(30));
								ssInfo.Text = "������ ����";
							}
							else if (_modee == Mode.PointRect)
							{
								ShapeFactory.Shapes.Add(vertex.CreateSquareAroundPoint(30));
								ssInfo.Text = "������ �������";
							}
							else if (_modee == Mode.PointTriangle)
							{
								ShapeFactory.Shapes.Add(vertex.CreateTriangleAroundPoint(30));
								ssInfo.Text = "������ �����������";
							}
						}
					}
					break;
			}
			drawingBox.Invalidate();
		}
		private void drawingBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (_mode == MainMode.Select)
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

			if (_modee == Mode.ClosestPoint)
			{
				_selectionService.ClearSelection();
				_selectionService.SelectVertex(Vertex.GetClosestVertex(e.Location));
				drawingBox.Invalidate();
			}

			if (_modee == Mode.PointBelongsTo && ShapeFactory.GetShapeAtLocation(e.Location, 12) is { } shape)
			{
				ssInfo.Text = shape.GetVertices().Any(vertex => vertex.DistanceTo(e.Location) <= 6)
					? $"{e.Location} ����������� ������� {shape}"
					: string.Empty;
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

				var selectedShapes = _selectionService.SelectedShapes;
				if (selectedShapes.Count != 0)
				{
					ssInfo.Text = $"������� ��������:\n{string.Join(" \\ ", selectedShapes
						.GroupBy(shape => shape.GetType())
						.Select(group => $"{group.Key.Name}: {group.Count()}"))}";
				}
			}
		}
		#endregion

		#region Handlers
		private void HandleDrawingMode(Point cursorLocation)
		{
			_currentVertices.Add(new Vertex(cursorLocation.X, cursorLocation.Y));
			int vertexCount = _currentVertices.Count;

			if (vertexCount == 1)
			{
				ShapeFactory.CreateVertex(_currentVertices[0]);
				ssInfo.Text = $"������ ������: ����� {_currentVertices[0]}";
			}
			else if (vertexCount == 2)
			{
				ShapeFactory.RemoveLastShape<Vertex>();
				ShapeFactory.CreateLine(_currentVertices);
				ssInfo.Text = $"������ ������: ����� {_currentVertices[0]} -> {_currentVertices[1]}";
			}
			else
			{
				if (_snapService.ShouldClosePolygon(_currentVertices))
				{
					ShapeFactory.RemoveLastShape<Polyline>();
					ShapeFactory.CreatePolygon(_currentVertices);
					ssInfo.Text = $"������ ������: ������� � {vertexCount} ���������";
					_currentVertices.Clear();
				}
				else
				{
					var existingPolyline = ShapeFactory.GetLastAddedShape<Polyline>();
					if (existingPolyline != null && existingPolyline.Vertices[0].Equals(_currentVertices[0]))
					{
						existingPolyline.AddVertex(_currentVertices.Last());
						ssInfo.Text = $"��������� ���������: {vertexCount} ������.";
					}
					else
					{
						ShapeFactory.RemoveLastShape<Line>();
						ShapeFactory.CreatePolyline(_currentVertices);
						ssInfo.Text = $"������ ������: ��������� � {vertexCount} ���������";
					}
				}
			}
		}
		private void HandleSelectionMode(Point cursorLocation)
		{
			// �����
			if (_modee == Mode.LineIntersection) IntersectionLine();
			if (_modee == Mode.LineLength) LineLength();
			if (_modee == Mode.LineExtend) ExtendLine(cursorLocation);
			if (_modee == Mode.LineRotate) RotateLine(cursorLocation);
			if (_modee == Mode.LineTranslate) TranslateLine(cursorLocation);

			// ���������
			if (_modee == Mode.PolylineIntersection) PolylineIntersection();
			if (_modee == Mode.PolylineLength) PolylineLength();
			if (_modee == Mode.PolylineSmooth) SmoothPolyline();
			if (_modee == Mode.PolylineAngle) PolylineAngle();
			if (_modee == Mode.PolylineRotate) RotatePolyline(cursorLocation);
			if (_modee == Mode.PolylineTranslate) TranslatePolyline(cursorLocation);
			if (_modee == Mode.PolylineCreatePlane) CreatePolylinePlane();
			if (_modee == Mode.PolylineDirection) PolylineDirection();
		}
		#endregion

		#region Line
		private void IntersectionLine()
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();
			if (selectedLines == null || !selectedLines.Any()) return;

			selectedLines.SelectMany(selectedLine => ShapeFactory.Shapes.Where(shape => shape != selectedLine)
						.SelectMany(shape => selectedLine.Intersect(shape) ?? Enumerable.Empty<Vertex>())).ToList()
						.ForEach(intersection => _selectionService.SelectVertex(intersection));

			ssInfo.Text = $"������� {_selectionService.SelectedVertices.Count} �����������";
		}
		private void LineLength()
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();
			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "��� ���������� ����� ��� ��������� �����";
				return;
			}
			ssInfo.Text = $"��������� ����� ��������� �����: {selectedLines.Sum(line => line.GetLength()):F2}";
		}
		private void ExtendLine(Point newEndPoint)
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();
			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "��� ���������� ����� ��� ���������";
				return;
			}

			ShapeFactory.Shapes.OfType<Line>()
				.Where(line => selectedLines.Contains(line))
				.ToList()
				.ForEach(line => line.ExtendEnd(new Vertex(newEndPoint.X, newEndPoint.Y)));
		}
		private void RotateLine(Point cursorLocation)
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();
			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "��� ���������� ����� ��� ��������";
				return;
			}

			ShapeFactory.Shapes.OfType<Line>()
				.Where(line => selectedLines.Contains(line))
				.ToList()
				.ForEach(line => line.Rotate(GetAngleBetweenCursorAndLine(cursorLocation, line)));
		}
		private static double GetAngleBetweenCursorAndLine(Point cursorLocation, Line line)
		{
			var lineVector = new Vertex(line.End.X - line.Start.X, line.End.Y - line.Start.Y);
			var cursorVector = new Vertex(cursorLocation.X - line.Start.X, cursorLocation.Y - line.Start.Y);

			return Math.Atan2(cursorVector.Y, cursorVector.X) - Math.Atan2(lineVector.Y, lineVector.X);
		}
		private void TranslateLine(Point newLocation)
		{
			var selectedLines = _selectionService.GetSelectedShapesOfType<Line>();
			if (selectedLines == null || !selectedLines.Any())
			{
				ssInfo.Text = "��� ���������� ����� ��� �����������";
				return;
			}

			double averageX = selectedLines.Average(line => (line.Start.X + line.End.X) / 2);
			double averageY = selectedLines.Average(line => (line.Start.Y + line.End.Y) / 2);

			double offsetX = newLocation.X - averageX;
			double offsetY = newLocation.Y - averageY;

			ShapeFactory.Shapes.OfType<Line>()
				.Where(line => selectedLines.Contains(line))
				.ToList()
				.ForEach(line => line.Translate((int)offsetX, (int)offsetY));
		}
		#endregion

		#region Polyline
		private void PolylineIntersection()
		{
			_selectionService.GetSelectedShapesOfType<Polyline>().ToList().ForEach(polyline =>
			{
				ShapeFactory.Shapes
					.Where(shape => shape != polyline)
					.SelectMany(shape => polyline.Intersect(shape) ?? Enumerable.Empty<Vertex>())
					.ToList()
					.ForEach(intersection => _selectionService.SelectVertex(intersection));
			});

			ssInfo.Text = $"������� {_selectionService.SelectedVertices.Count} ����������� ��� ���������";
		}
		private void PolylineLength()
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>().ToList();
			if (selectedPolylines.Count == 0)
			{
				ssInfo.Text = $"��� ���������� {selectedPolylines.GetType().Name}";
				return;
			}

			double totalLength = selectedPolylines.Sum(polyline => polyline.GetLength());
			ssInfo.Text = $"��������� �����: {totalLength:F2}";
		}
		private void SmoothPolyline()
		{
			_selectionService.GetSelectedShapesOfType<Polyline>().ToList().ForEach(polyline => polyline.Smooth());
		}
		private void PolylineAngle()
		{
			_selectionService.GetSelectedShapesOfType<Polyline>().ToList().ForEach(polyline =>
			{
				double angle = polyline.GetAngleBetweenThreePoints(1);
				ssInfo.Text = $"���� ����� ������� {polyline.GetType().Name}: {Math.Round(angle * (180 / Math.PI), 2)}�";
			});
		}
		private void RotatePolyline(Point cursorLocation)
		{
			_selectionService.GetSelectedShapesOfType<Polyline>().ToList().ForEach(polyline =>
			{
				var firstVertex = polyline.Vertices.First();
				polyline.RotateAroundPoint(firstVertex, GetAngleBetweenCursorAndPoint(cursorLocation, firstVertex));
			});
		}
		private static double GetAngleBetweenCursorAndPoint(Point cursorLocation, Vertex vertex)
		{
			double x = cursorLocation.X - vertex.X;
			double y = cursorLocation.Y - vertex.Y;
			return Math.Atan2(y, x);
		}
		private void TranslatePolyline(Point cursorLocation)
		{
			var selectedPolylines = _selectionService.GetSelectedShapesOfType<Polyline>()
				.Where(polyline => polyline is not Polygon)
				.ToList();

			if (selectedPolylines.Count == 0)
			{
				ssInfo.Text = "��� ���������� ��������� ��� �����������";
				return;
			}

			var averagePoint = new Vertex(
				(int)selectedPolylines.Average(polyline => polyline.Vertices.Average(v => v.X)),
				(int)selectedPolylines.Average(polyline => polyline.Vertices.Average(v => v.Y)));

			int offsetX = cursorLocation.X - averagePoint.X;
			int offsetY = cursorLocation.Y - averagePoint.Y;

			selectedPolylines.ForEach(polyline => polyline.Translate(offsetX, offsetY));
		}
		private void CreatePolylinePlane()
		{
			var selectedShapes = _selectionService.GetSelectedShapesOfType<Polyline>()
				.Where(shape => shape is not Polygon && shape is Polyline polyline && !polyline.IsPolygonCreated)
				.Cast<Polyline>()
				.ToList();

			foreach (var polyline in selectedShapes)
			{
				if (polyline.Vertices.Count >= 3)
				{
					ShapeFactory.CreatePolygon(polyline.Vertices);
					_selectionService.DeselectShape(polyline);
					ShapeFactory.RemoveShape(polyline);
					ssInfo.Text = "��������� ������� �� ������ ���������";
				}
				else
					ssInfo.Text = "������������ ����� ��� �������� ���������";
			}
		}
		private void PolylineDirection()
		{
			var selectedPolys = _selectionService.GetSelectedShapesOfType<Polyline>();
			foreach (var selectedPoly in selectedPolys)
			{
				bool isClockwise = selectedPoly.IsClockwise();
				ssInfo.Text = isClockwise ? $"{selectedPoly.GetType().Name} ��������� �� ������� �������" : $"{selectedPoly.GetType().Name} ��������� ������ ������� �������";
			}
		}
		#endregion

		#region Snapping
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
		#endregion

		private void drawingBox_Paint(object sender, PaintEventArgs e)
		{
			ShapeFactory.Shapes.ForEach(shape => shape.Draw(e.Graphics));

			if (_mode == MainMode.Select)
			{
				using (Pen pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
				{
					e.Graphics.DrawRectangle(pen, _selectionRectangle);
				}

				_selectionService.SelectedShapes.ForEach(shape => shape.Draw(e.Graphics));
				_selectionService.SelectedVertices.ForEach(vertex => vertex.Draw(e.Graphics));

				if (_modee == Mode.LineLength || _modee == Mode.PolylineLength)
					_selectionService.SelectedShapes.ForEach(shape => shape.Draw(e.Graphics, true));

				if (_modee == Mode.LineIntersection || _modee == Mode.PolylineIntersection)
					_selectionService.SelectedVertices.ForEach(vertex => vertex.Draw(e.Graphics, true));
			}
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
		private void ResetEveryMode()
		{
			_mode = MainMode.None;
			_modee = Mode.None;
			_currentVertices.Clear();
			_selectionService.ClearSelection();
			drawingBox.Invalidate();
			ssInfo.Text = string.Empty;
			ssInfoActivated.Text = "��� ������ ���������";
		}

		#region UpToolStrip
		private void tsDraw_Click(object sender, EventArgs e)
		{
			if (_mode == MainMode.Draw)
			{
				_mode = MainMode.None;
				_currentVertices.Clear();
				ssInfoActivated.Text = "����� ��������� ��������";
			}
			else
			{
				ResetEveryMode();
				_mode = MainMode.Draw;
				_selectionService.ClearSelection();
				ssInfoActivated.Text = "����� ��������� �����������";
			}
		}
		private void tsSelection_Click(object sender, EventArgs e)
		{
			if (_mode == MainMode.Select)
			{
				_mode = MainMode.None;
				_selectionService.ClearSelection();
				ssInfoActivated.Text = "����� ��������� ��������";
			}
			else
			{
				ResetEveryMode();
				_mode = MainMode.Select;
				ssInfoActivated.Text = "����� ��������� �����������";
			}
		}
		private void tsSnap_Click(object sender, EventArgs e)
		{
			_isSnapping = !_isSnapping;
			ssInfoActivated.Text = _isSnapping ? "����� �������� �������" : "����� �������� ��������";
		}
		#endregion

		private void ToggleMode(Mode mode, string msg)
		{
			if (_modee == mode)
			{
				_modee = Mode.None;
				ssInfoActivated.Text = $"{msg} ��������";
			}
			else
			{
				_modee = Mode.None;
				_modee = mode;
				ssInfoActivated.Text = $"{msg} �����";

				_selectionService.ClearSelection();
				ssInfo.Text = string.Empty;
				drawingBox.Invalidate();
			}
		}

		#region RightToolStrip_Point
		private void rtsPointClosestPoint_Click(object sender, EventArgs e) => ToggleMode(Mode.ClosestPoint, "����� ��������� �����");
		private void rtsPointBelongsTo_Click(object sender, EventArgs e) => ToggleMode(Mode.PointBelongsTo, "����� �������������� �����");
		private void rtsPointCircle_Click(object sender, EventArgs e) => ToggleMode(Mode.PointCircle, "����� ����� ������ �����");
		private void rtsPointRect_Click(object sender, EventArgs e) => ToggleMode(Mode.PointRect, "����� �������������� ������ �����");
		private void rtsPointTriangle_Click(object sender, EventArgs e) => ToggleMode(Mode.PointTriangle, "����� ������������ ������ �����");
		#endregion

		#region RightToolStrip_Line
		private void rtsLineIntersection_Click(object sender, EventArgs e) => ToggleMode(Mode.LineIntersection, "����� ����������� �����");
		private void rtsLineLength_Click(object sender, EventArgs e) => ToggleMode(Mode.LineLength, "����� ����� �����");
		private void rtsLineExtend_Click(object sender, EventArgs e) => ToggleMode(Mode.LineExtend, "����� ��������� �����");
		private void rtsLineRotate_Click(object sender, EventArgs e) => ToggleMode(Mode.LineRotate, "����� �������� �����");
		private void rtsLineTransform_Click(object sender, EventArgs e) => ToggleMode(Mode.LineTranslate, "����� ������������� �����");
		#endregion

		#region RightToolStrip_Polyline
		private void rtsPolylineIntersection_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineIntersection, "����� ����������� ���������");
		private void rtsPolylineLength_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineLength, "����� ����� ���������");
		private void rtsPolylineSmooth_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineSmooth, "����� ����������� ���������");
		private void rtsPolylineAngle_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineAngle, "����� ���� ���������");
		private void rtsPolylineRotate_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineRotate, "����� �������� ���������");
		private void rtsPolylineTranslate_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineTranslate, "����� ����������� ���������");
		private void rtsPolylineCreatePlane_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineCreatePlane, "����� �������� ��������� ���������");
		private void rtsPolylineDirection_Click(object sender, EventArgs e) => ToggleMode(Mode.PolylineDirection, "����� ����������� ���������");
		#endregion
	}
}