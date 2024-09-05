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
			toolStrip1 = new ToolStrip();
			rtsPoint = new ToolStripDropDownButton();
			rtsPointClosestPoint = new ToolStripMenuItem();
			rtsPointBelongsTo = new ToolStripMenuItem();
			rtsPointCircle = new ToolStripMenuItem();
			rtsPointRect = new ToolStripMenuItem();
			rtsPointTriangle = new ToolStripMenuItem();
			ts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)drawingBox).BeginInit();
			ss.SuspendLayout();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// ts
			// 
			ts.Items.AddRange(new ToolStripItem[] { tsDraw, tsSelection, tsSnap });
			ts.Location = new Point(0, 0);
			ts.Name = "ts";
			ts.Size = new Size(1184, 25);
			ts.TabIndex = 0;
			ts.Text = "toolStrip1";
			// 
			// tsDraw
			// 
			tsDraw.Image = (Image)resources.GetObject("tsDraw.Image");
			tsDraw.ImageTransparentColor = Color.Magenta;
			tsDraw.Name = "tsDraw";
			tsDraw.Size = new Size(54, 22);
			tsDraw.Text = "Draw";
			tsDraw.Click += tsDraw_Click;
			// 
			// tsSelection
			// 
			tsSelection.Image = (Image)resources.GetObject("tsSelection.Image");
			tsSelection.ImageTransparentColor = Color.Magenta;
			tsSelection.Name = "tsSelection";
			tsSelection.Size = new Size(58, 22);
			tsSelection.Text = "Select";
			tsSelection.Click += tsSelection_Click;
			// 
			// tsSnap
			// 
			tsSnap.Image = (Image)resources.GetObject("tsSnap.Image");
			tsSnap.ImageTransparentColor = Color.Magenta;
			tsSnap.Name = "tsSnap";
			tsSnap.Size = new Size(77, 22);
			tsSnap.Text = "Snapping";
			tsSnap.Click += tsSnap_Click;
			// 
			// drawingBox
			// 
			drawingBox.Dock = DockStyle.Fill;
			drawingBox.Location = new Point(0, 25);
			drawingBox.Name = "drawingBox";
			drawingBox.Size = new Size(1184, 636);
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
			ss.Location = new Point(0, 639);
			ss.Name = "ss";
			ss.Size = new Size(1184, 22);
			ss.TabIndex = 3;
			ss.Text = "statusStrip1";
			// 
			// ssCoordinates
			// 
			ssCoordinates.Name = "ssCoordinates";
			ssCoordinates.Size = new Size(42, 17);
			ssCoordinates.Text = "X:0 Y:0";
			// 
			// ssInfo
			// 
			ssInfo.Name = "ssInfo";
			ssInfo.Size = new Size(1105, 17);
			ssInfo.Spring = true;
			ssInfo.Text = "NaN";
			// 
			// ssInfoActivated
			// 
			ssInfoActivated.Name = "ssInfoActivated";
			ssInfoActivated.Size = new Size(22, 17);
			ssInfoActivated.Text = "???";
			// 
			// toolStrip1
			// 
			toolStrip1.Dock = DockStyle.Right;
			toolStrip1.Items.AddRange(new ToolStripItem[] { rtsPoint });
			toolStrip1.Location = new Point(1135, 25);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new Size(49, 614);
			toolStrip1.TabIndex = 4;
			toolStrip1.Text = "toolStrip1";
			// 
			// rtsPoint
			// 
			rtsPoint.DisplayStyle = ToolStripItemDisplayStyle.Text;
			rtsPoint.DropDownItems.AddRange(new ToolStripItem[] { rtsPointClosestPoint, rtsPointBelongsTo, rtsPointCircle, rtsPointRect, rtsPointTriangle });
			rtsPoint.Image = (Image)resources.GetObject("rtsPoint.Image");
			rtsPoint.ImageTransparentColor = Color.Magenta;
			rtsPoint.Name = "rtsPoint";
			rtsPoint.Size = new Size(46, 19);
			rtsPoint.Text = "Point";
			// 
			// rtsPointClosestPoint
			// 
			rtsPointClosestPoint.Name = "rtsPointClosestPoint";
			rtsPointClosestPoint.Size = new Size(180, 22);
			rtsPointClosestPoint.Text = "Closest Point";
			rtsPointClosestPoint.Click += rtsPointClosestPoint_Click;
			// 
			// rtsPointBelongsTo
			// 
			rtsPointBelongsTo.Name = "rtsPointBelongsTo";
			rtsPointBelongsTo.Size = new Size(180, 22);
			rtsPointBelongsTo.Text = "Belongs To Object";
			rtsPointBelongsTo.Click += rtsPointBelongsTo_Click;
			// 
			// rtsPointCircle
			// 
			rtsPointCircle.Name = "rtsPointCircle";
			rtsPointCircle.Size = new Size(180, 22);
			rtsPointCircle.Text = "Draw Circle";
			rtsPointCircle.Click += rtsPointCircle_Click;
			// 
			// rtsPointRect
			// 
			rtsPointRect.Name = "rtsPointRect";
			rtsPointRect.Size = new Size(180, 22);
			rtsPointRect.Text = "Draw Rectangle";
			rtsPointRect.Click += rtsPointRect_Click;
			// 
			// rtsPointTriangle
			// 
			rtsPointTriangle.Name = "rtsPointTriangle";
			rtsPointTriangle.Size = new Size(180, 22);
			rtsPointTriangle.Text = "Draw Triangle";
			rtsPointTriangle.Click += rtsPointTriangle_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1184, 661);
			Controls.Add(toolStrip1);
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
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
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
		private ToolStrip toolStrip1;
		private ToolStripDropDownButton rtsPoint;
		private ToolStripMenuItem rtsPointClosestPoint;
		private ToolStripMenuItem rtsPointBelongsTo;
		private ToolStripMenuItem rtsPointCircle;
		private ToolStripMenuItem rtsPointRect;
		private ToolStripMenuItem rtsPointTriangle;
	}
}
