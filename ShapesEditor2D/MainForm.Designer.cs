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
			ts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)drawingBox).BeginInit();
			ss.SuspendLayout();
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
			// 
			// tsSelection
			// 
			tsSelection.Image = (Image)resources.GetObject("tsSelection.Image");
			tsSelection.ImageTransparentColor = Color.Magenta;
			tsSelection.Name = "tsSelection";
			tsSelection.Size = new Size(58, 22);
			tsSelection.Text = "Select";
			// 
			// tsSnap
			// 
			tsSnap.Image = (Image)resources.GetObject("tsSnap.Image");
			tsSnap.ImageTransparentColor = Color.Magenta;
			tsSnap.Name = "tsSnap";
			tsSnap.Size = new Size(77, 22);
			tsSnap.Text = "Snapping";
			// 
			// drawingBox
			// 
			drawingBox.Dock = DockStyle.Fill;
			drawingBox.Location = new Point(0, 25);
			drawingBox.Name = "drawingBox";
			drawingBox.Size = new Size(1184, 636);
			drawingBox.TabIndex = 2;
			drawingBox.TabStop = false;
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
			ssInfo.Size = new Size(31, 17);
			ssInfo.Text = "NaN";
			// 
			// ssInfoActivated
			// 
			ssInfoActivated.Name = "ssInfoActivated";
			ssInfoActivated.Size = new Size(22, 17);
			ssInfoActivated.Text = "???";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1184, 661);
			Controls.Add(ss);
			Controls.Add(drawingBox);
			Controls.Add(ts);
			Name = "MainForm";
			Text = "Shapes Editor 2D";
			ts.ResumeLayout(false);
			ts.PerformLayout();
			((System.ComponentModel.ISupportInitialize)drawingBox).EndInit();
			ss.ResumeLayout(false);
			ss.PerformLayout();
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
	}
}
