namespace ShapesEditor2D
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			ts = new ToolStrip();
			tsDraw = new ToolStripButton();
			tsSelection = new ToolStripButton();
			tsSnap = new ToolStripButton();
			drawingBox = new PictureBox();
			ss = new StatusStrip();
			ssCoordinates = new ToolStripStatusLabel();
			ssInfo = new ToolStripStatusLabel();
			ssInfoActivated = new ToolStripStatusLabel();
			rts = new ToolStrip();
			rtsPoint = new ToolStripDropDownButton();
			rtsPointClosestPoint = new ToolStripMenuItem();
			rtsPointBelongsTo = new ToolStripMenuItem();
			rtsPointCircle = new ToolStripMenuItem();
			rtsPointRect = new ToolStripMenuItem();
			rtsPointTriangle = new ToolStripMenuItem();
			rtsLine = new ToolStripDropDownButton();
			rtsLineIntersection = new ToolStripMenuItem();
			rtsLineLength = new ToolStripMenuItem();
			rtsLineExtend = new ToolStripMenuItem();
			rtsLineRotate = new ToolStripMenuItem();
			rtsLineTransform = new ToolStripMenuItem();
			rtsPolyline = new ToolStripDropDownButton();
			rtsPolylineIntersection = new ToolStripMenuItem();
			rtsPolylineLength = new ToolStripMenuItem();
			rtsPolylineSmooth = new ToolStripMenuItem();
			rtsPolylineAngle = new ToolStripMenuItem();
			rtsPolylineRotate = new ToolStripMenuItem();
			rtsPolylineTranslate = new ToolStripMenuItem();
			rtsPolylineCreatePlane = new ToolStripMenuItem();
			rtsPolylineDirection = new ToolStripMenuItem();
			ts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)drawingBox).BeginInit();
			ss.SuspendLayout();
			rts.SuspendLayout();
			SuspendLayout();
			// 
			// ts
			// 
			ts.GripStyle = ToolStripGripStyle.Hidden;
			ts.Items.AddRange(new ToolStripItem[] { tsDraw, tsSelection, tsSnap });
			ts.Location = new Point(0, 0);
			ts.Name = "ts";
			ts.Size = new Size(1284, 27);
			ts.TabIndex = 0;
			ts.Text = "toolStrip1";
			// 
			// tsDraw
			// 
			tsDraw.Font = new Font("Segoe UI", 11.25F);
			tsDraw.Image = (Image)resources.GetObject("tsDraw.Image");
			tsDraw.ImageTransparentColor = Color.Magenta;
			tsDraw.Name = "tsDraw";
			tsDraw.Size = new Size(64, 24);
			tsDraw.Text = "Draw";
			tsDraw.Click += tsDraw_Click;
			// 
			// tsSelection
			// 
			tsSelection.Font = new Font("Segoe UI", 11.25F);
			tsSelection.Image = (Image)resources.GetObject("tsSelection.Image");
			tsSelection.ImageTransparentColor = Color.Magenta;
			tsSelection.Name = "tsSelection";
			tsSelection.Size = new Size(69, 24);
			tsSelection.Text = "Select";
			tsSelection.Click += tsSelection_Click;
			// 
			// tsSnap
			// 
			tsSnap.Font = new Font("Segoe UI", 11.25F);
			tsSnap.Image = (Image)resources.GetObject("tsSnap.Image");
			tsSnap.ImageTransparentColor = Color.Magenta;
			tsSnap.Name = "tsSnap";
			tsSnap.Size = new Size(92, 24);
			tsSnap.Text = "Snapping";
			tsSnap.Click += tsSnap_Click;
			// 
			// drawingBox
			// 
			drawingBox.Dock = DockStyle.Fill;
			drawingBox.Location = new Point(0, 27);
			drawingBox.Name = "drawingBox";
			drawingBox.Size = new Size(1284, 684);
			drawingBox.TabIndex = 2;
			drawingBox.TabStop = false;
			drawingBox.Paint += drawingBox_Paint;
			drawingBox.MouseClick += drawingBox_MouseClick;
			drawingBox.MouseDown += drawingBox_MouseDown;
			drawingBox.MouseMove += drawingBox_MouseMove;
			drawingBox.MouseUp += drawingBox_MouseUp;
			// 
			// ss
			// 
			ss.Items.AddRange(new ToolStripItem[] { ssCoordinates, ssInfo, ssInfoActivated });
			ss.Location = new Point(0, 685);
			ss.Name = "ss";
			ss.Size = new Size(1284, 26);
			ss.TabIndex = 3;
			ss.Text = "statusStrip1";
			// 
			// ssCoordinates
			// 
			ssCoordinates.BorderStyle = Border3DStyle.Sunken;
			ssCoordinates.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
			ssCoordinates.Name = "ssCoordinates";
			ssCoordinates.Size = new Size(56, 21);
			ssCoordinates.Text = "X:0 Y:0";
			// 
			// ssInfo
			// 
			ssInfo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
			ssInfo.Name = "ssInfo";
			ssInfo.Size = new Size(1151, 21);
			ssInfo.Spring = true;
			ssInfo.Text = "NaN";
			// 
			// ssInfoActivated
			// 
			ssInfoActivated.Font = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Point, 204);
			ssInfoActivated.Name = "ssInfoActivated";
			ssInfoActivated.Size = new Size(31, 21);
			ssInfoActivated.Text = "???";
			// 
			// rts
			// 
			rts.Dock = DockStyle.Right;
			rts.GripStyle = ToolStripGripStyle.Hidden;
			rts.Items.AddRange(new ToolStripItem[] { rtsPoint, rtsLine, rtsPolyline });
			rts.Location = new Point(1220, 27);
			rts.Name = "rts";
			rts.Size = new Size(64, 658);
			rts.TabIndex = 4;
			rts.Text = "toolStrip1";
			// 
			// rtsPoint
			// 
			rtsPoint.DisplayStyle = ToolStripItemDisplayStyle.Text;
			rtsPoint.DropDownItems.AddRange(new ToolStripItem[] { rtsPointClosestPoint, rtsPointBelongsTo, rtsPointCircle, rtsPointRect, rtsPointTriangle });
			rtsPoint.Font = new Font("Candara", 9.75F);
			rtsPoint.Image = (Image)resources.GetObject("rtsPoint.Image");
			rtsPoint.ImageTransparentColor = Color.Magenta;
			rtsPoint.Name = "rtsPoint";
			rtsPoint.Size = new Size(61, 19);
			rtsPoint.Text = "Point";
			// 
			// rtsPointClosestPoint
			// 
			rtsPointClosestPoint.Name = "rtsPointClosestPoint";
			rtsPointClosestPoint.Size = new Size(174, 22);
			rtsPointClosestPoint.Text = "Closest Point";
			rtsPointClosestPoint.Click += rtsPointClosestPoint_Click;
			// 
			// rtsPointBelongsTo
			// 
			rtsPointBelongsTo.Name = "rtsPointBelongsTo";
			rtsPointBelongsTo.Size = new Size(174, 22);
			rtsPointBelongsTo.Text = "Belongs To Object";
			rtsPointBelongsTo.Click += rtsPointBelongsTo_Click;
			// 
			// rtsPointCircle
			// 
			rtsPointCircle.Name = "rtsPointCircle";
			rtsPointCircle.Size = new Size(174, 22);
			rtsPointCircle.Text = "Draw Circle";
			rtsPointCircle.Click += rtsPointCircle_Click;
			// 
			// rtsPointRect
			// 
			rtsPointRect.Name = "rtsPointRect";
			rtsPointRect.Size = new Size(174, 22);
			rtsPointRect.Text = "Draw Rectangle";
			rtsPointRect.Click += rtsPointRect_Click;
			// 
			// rtsPointTriangle
			// 
			rtsPointTriangle.Name = "rtsPointTriangle";
			rtsPointTriangle.Size = new Size(174, 22);
			rtsPointTriangle.Text = "Draw Triangle";
			rtsPointTriangle.Click += rtsPointTriangle_Click;
			// 
			// rtsLine
			// 
			rtsLine.DisplayStyle = ToolStripItemDisplayStyle.Text;
			rtsLine.DropDownItems.AddRange(new ToolStripItem[] { rtsLineIntersection, rtsLineLength, rtsLineExtend, rtsLineRotate, rtsLineTransform });
			rtsLine.Font = new Font("Candara", 9.75F);
			rtsLine.Image = (Image)resources.GetObject("rtsLine.Image");
			rtsLine.ImageTransparentColor = Color.Magenta;
			rtsLine.Name = "rtsLine";
			rtsLine.Size = new Size(61, 19);
			rtsLine.Text = "Line";
			// 
			// rtsLineIntersection
			// 
			rtsLineIntersection.Name = "rtsLineIntersection";
			rtsLineIntersection.Size = new Size(174, 22);
			rtsLineIntersection.Text = "Intersection Point";
			rtsLineIntersection.Click += rtsLineIntersection_Click;
			// 
			// rtsLineLength
			// 
			rtsLineLength.Name = "rtsLineLength";
			rtsLineLength.Size = new Size(174, 22);
			rtsLineLength.Text = "Length";
			rtsLineLength.Click += rtsLineLength_Click;
			// 
			// rtsLineExtend
			// 
			rtsLineExtend.Name = "rtsLineExtend";
			rtsLineExtend.Size = new Size(174, 22);
			rtsLineExtend.Text = "Extend";
			rtsLineExtend.Click += rtsLineExtend_Click;
			// 
			// rtsLineRotate
			// 
			rtsLineRotate.Name = "rtsLineRotate";
			rtsLineRotate.Size = new Size(174, 22);
			rtsLineRotate.Text = "Rotate";
			rtsLineRotate.Click += rtsLineRotate_Click;
			// 
			// rtsLineTransform
			// 
			rtsLineTransform.Name = "rtsLineTransform";
			rtsLineTransform.Size = new Size(174, 22);
			rtsLineTransform.Text = "Transform";
			rtsLineTransform.Click += rtsLineTransform_Click;
			// 
			// rtsPolyline
			// 
			rtsPolyline.DisplayStyle = ToolStripItemDisplayStyle.Text;
			rtsPolyline.DropDownItems.AddRange(new ToolStripItem[] { rtsPolylineIntersection, rtsPolylineLength, rtsPolylineSmooth, rtsPolylineAngle, rtsPolylineRotate, rtsPolylineTranslate, rtsPolylineCreatePlane, rtsPolylineDirection });
			rtsPolyline.Font = new Font("Candara", 9.75F);
			rtsPolyline.Image = (Image)resources.GetObject("rtsPolyline.Image");
			rtsPolyline.ImageTransparentColor = Color.Magenta;
			rtsPolyline.Name = "rtsPolyline";
			rtsPolyline.Size = new Size(61, 19);
			rtsPolyline.Text = "Polyline";
			// 
			// rtsPolylineIntersection
			// 
			rtsPolylineIntersection.Name = "rtsPolylineIntersection";
			rtsPolylineIntersection.Size = new Size(144, 22);
			rtsPolylineIntersection.Text = "Intersection";
			rtsPolylineIntersection.Click += rtsPolylineIntersection_Click;
			// 
			// rtsPolylineLength
			// 
			rtsPolylineLength.Name = "rtsPolylineLength";
			rtsPolylineLength.Size = new Size(144, 22);
			rtsPolylineLength.Text = "Length";
			rtsPolylineLength.Click += rtsPolylineLength_Click;
			// 
			// rtsPolylineSmooth
			// 
			rtsPolylineSmooth.Name = "rtsPolylineSmooth";
			rtsPolylineSmooth.Size = new Size(144, 22);
			rtsPolylineSmooth.Text = "Smooth";
			rtsPolylineSmooth.Click += rtsPolylineSmooth_Click;
			// 
			// rtsPolylineAngle
			// 
			rtsPolylineAngle.Name = "rtsPolylineAngle";
			rtsPolylineAngle.Size = new Size(144, 22);
			rtsPolylineAngle.Text = "Angle";
			rtsPolylineAngle.Click += rtsPolylineAngle_Click;
			// 
			// rtsPolylineRotate
			// 
			rtsPolylineRotate.Name = "rtsPolylineRotate";
			rtsPolylineRotate.Size = new Size(144, 22);
			rtsPolylineRotate.Text = "Rotate";
			rtsPolylineRotate.Click += rtsPolylineRotate_Click;
			// 
			// rtsPolylineTranslate
			// 
			rtsPolylineTranslate.Name = "rtsPolylineTranslate";
			rtsPolylineTranslate.Size = new Size(144, 22);
			rtsPolylineTranslate.Text = "Translate";
			rtsPolylineTranslate.Click += rtsPolylineTranslate_Click;
			// 
			// rtsPolylineCreatePlane
			// 
			rtsPolylineCreatePlane.Name = "rtsPolylineCreatePlane";
			rtsPolylineCreatePlane.Size = new Size(144, 22);
			rtsPolylineCreatePlane.Text = "Create Plane";
			rtsPolylineCreatePlane.Click += rtsPolylineCreatePlane_Click;
			// 
			// rtsPolylineDirection
			// 
			rtsPolylineDirection.Name = "rtsPolylineDirection";
			rtsPolylineDirection.Size = new Size(144, 22);
			rtsPolylineDirection.Text = "Direction";
			rtsPolylineDirection.Click += rtsPolylineDirection_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1284, 711);
			Controls.Add(rts);
			Controls.Add(ss);
			Controls.Add(drawingBox);
			Controls.Add(ts);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Shapes Editor 2D";
			Load += MainForm_Load;
			KeyDown += MainForm_KeyDown;
			ts.ResumeLayout(false);
			ts.PerformLayout();
			((System.ComponentModel.ISupportInitialize)drawingBox).EndInit();
			ss.ResumeLayout(false);
			ss.PerformLayout();
			rts.ResumeLayout(false);
			rts.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ToolStrip ts;
		private ToolStripButton tsDraw;
		private ToolStripButton tsSelection;
		private ToolStripButton tsSnap;
		private PictureBox drawingBox;
		private StatusStrip ss;
		private ToolStripStatusLabel ssCoordinates;
		private ToolStripStatusLabel ssInfo;
		private ToolStripStatusLabel ssInfoActivated;
		private ToolStrip rts;
		private ToolStripDropDownButton rtsPoint;
		private ToolStripMenuItem rtsPointClosestPoint;
		private ToolStripMenuItem rtsPointBelongsTo;
		private ToolStripMenuItem rtsPointCircle;
		private ToolStripMenuItem rtsPointRect;
		private ToolStripMenuItem rtsPointTriangle;
		private ToolStripDropDownButton rtsLine;
		private ToolStripMenuItem rtsLineIntersection;
		private ToolStripMenuItem rtsLineLength;
		private ToolStripMenuItem rtsLineExtend;
		private ToolStripMenuItem rtsLineRotate;
		private ToolStripMenuItem rtsLineTransform;
		private ToolStripDropDownButton rtsPolyline;
		private ToolStripMenuItem rtsPolylineIntersection;
		private ToolStripMenuItem rtsPolylineLength;
		private ToolStripMenuItem rtsPolylineSmooth;
		private ToolStripMenuItem rtsPolylineAngle;
		private ToolStripMenuItem rtsPolylineRotate;
		private ToolStripMenuItem rtsPolylineTranslate;
		private ToolStripMenuItem rtsPolylineCreatePlane;
		private ToolStripMenuItem rtsPolylineDirection;
	}
}
