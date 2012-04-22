namespace DotSpatial.Examples.AISViewer
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.appManager1 = new DotSpatial.Controls.AppManager();
            this.uxMap = new DotSpatial.Controls.Map();
            this.uxSpatialStatusStrip = new DotSpatial.Controls.SpatialStatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.layoutMenuStrip1 = new DotSpatial.Controls.LayoutMenuStrip();
            this.aisPort = new System.IO.Ports.SerialPort(this.components);
            this.spatialToolStrip1 = new DotSpatial.Controls.SpatialToolStrip();
            this.uxSpatialStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // appManager1
            // 
            this.appManager1.CompositionContainer = null;
            this.appManager1.Directories = ((System.Collections.Generic.List<string>)(resources.GetObject("appManager1.Directories")));
            this.appManager1.DockManager = null;
            this.appManager1.HeaderControl = null;
            this.appManager1.Legend = null;
            this.appManager1.Map = this.uxMap;
            this.appManager1.ProgressHandler = null;
            this.appManager1.ShowExtensionsDialog = DotSpatial.Controls.ShowExtensionsDialog.Default;
            // 
            // uxMap
            // 
            this.uxMap.AllowDrop = true;
            this.uxMap.BackColor = System.Drawing.Color.White;
            this.uxMap.CollectAfterDraw = false;
            this.uxMap.CollisionDetection = false;
            this.uxMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxMap.ExtendBuffer = false;
            this.uxMap.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.uxMap.IsBusy = false;
            this.uxMap.Legend = null;
            this.uxMap.Location = new System.Drawing.Point(0, 24);
            this.uxMap.Name = "uxMap";
            this.uxMap.ProgressHandler = this.uxSpatialStatusStrip;
            this.uxMap.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.uxMap.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.uxMap.RedrawLayersWhileResizing = true;
            this.uxMap.SelectionEnabled = true;
            this.uxMap.Size = new System.Drawing.Size(873, 652);
            this.uxMap.TabIndex = 2;
            this.uxMap.GeoMouseMove += new System.EventHandler<DotSpatial.Controls.GeoMouseArgs>(this.uxMap_GeoMouseMove);
            this.uxMap.ViewExtentsChanged += new System.EventHandler<DotSpatial.Data.ExtentArgs>(this.uxMap_ViewExtentsChanged);
            // 
            // uxSpatialStatusStrip
            // 
            this.uxSpatialStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.uxSpatialStatusStrip.Location = new System.Drawing.Point(0, 676);
            this.uxSpatialStatusStrip.Name = "uxSpatialStatusStrip";
            this.uxSpatialStatusStrip.ProgressBar = this.toolStripProgressBar1;
            this.uxSpatialStatusStrip.ProgressLabel = this.toolStripStatusLabel1;
            this.uxSpatialStatusStrip.Size = new System.Drawing.Size(873, 22);
            this.uxSpatialStatusStrip.TabIndex = 1;
            this.uxSpatialStatusStrip.Text = "Idle";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(200, 17);
            this.toolStripStatusLabel2.Text = "X:0, Y:0";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(300, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(35, 17);
            this.toolStripStatusLabel1.Text = "Idle...";
            // 
            // layoutMenuStrip1
            // 
            this.layoutMenuStrip1.LayoutControl = null;
            this.layoutMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.layoutMenuStrip1.Name = "layoutMenuStrip1";
            this.layoutMenuStrip1.Size = new System.Drawing.Size(873, 24);
            this.layoutMenuStrip1.TabIndex = 3;
            this.layoutMenuStrip1.Text = "layoutMenuStrip1";
            // 
            // aisPort
            // 
            this.aisPort.BaudRate = 4800;
            this.aisPort.PortName = "COM4";
            this.aisPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.aisPort_DataReceived);
            // 
            // spatialToolStrip1
            // 
            this.spatialToolStrip1.ApplicationManager = this.appManager1;
            this.spatialToolStrip1.Location = new System.Drawing.Point(0, 24);
            this.spatialToolStrip1.Map = this.uxMap;
            this.spatialToolStrip1.Name = "spatialToolStrip1";
            this.spatialToolStrip1.Size = new System.Drawing.Size(873, 25);
            this.spatialToolStrip1.TabIndex = 4;
            this.spatialToolStrip1.Text = "uxSpatialToolStrip";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 698);
            this.Controls.Add(this.spatialToolStrip1);
            this.Controls.Add(this.uxMap);
            this.Controls.Add(this.uxSpatialStatusStrip);
            this.Controls.Add(this.layoutMenuStrip1);
            this.MainMenuStrip = this.layoutMenuStrip1;
            this.Name = "frmMain";
            this.Text = "AIS Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.uxSpatialStatusStrip.ResumeLayout(false);
            this.uxSpatialStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DotSpatial.Controls.Map uxMap;
        private DotSpatial.Controls.SpatialStatusStrip uxSpatialStatusStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private DotSpatial.Controls.LayoutMenuStrip layoutMenuStrip1;
        private DotSpatial.Controls.AppManager appManager1;
        private System.IO.Ports.SerialPort aisPort;
        private DotSpatial.Controls.SpatialToolStrip spatialToolStrip1;

    }
}