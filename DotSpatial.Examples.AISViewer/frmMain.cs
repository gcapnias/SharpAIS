using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using SharpAIS;
using System.Collections;
using DotSpatial.Symbology;
using SharpAIS.DotSpatialMap;

namespace DotSpatial.Examples.AISViewer
{
    public partial class frmMain : Form
    {
        private Timer refreshTimer = new Timer();

        private PseudoSerial pseudoAisPort = new PseudoSerial();
        private delegate void DataHandler(string row);

        private AISHelper helper;
        private MapPointLayer vesselsLayer;


        BruTileLayer backgroundLayer;

        public frmMain()
        {
            InitializeComponent();

            uxMap.Projection = KnownCoordinateSystems.Projected.World.WebMercator;
            uxMap.BackColor = Color.FromArgb(181, 208, 208);

            backgroundLayer = BruTileLayer.CreateOsmLayer();
            uxMap.Layers.Add(backgroundLayer);

            pseudoAisPort.LogFilePath = Application.StartupPath + "\\AIS.log";
            pseudoAisPort.DataReceived += new EventHandler<SerialDataReceivedEventArgs>(pseudoAisPortDataReceived);

            refreshTimer.Enabled = false;
            refreshTimer.Interval = 1500;
            refreshTimer.Tick += new EventHandler(refreshTimerTick);

            helper = new AISHelper(uxMap.Projection);
            vesselsLayer = helper.VesselsLayer;
            uxMap.MapFrame.Add(vesselsLayer);

            uxMap.ViewExtents = new Extent(1642982.27031471, 4063251.12000095, 3802748.48786722, 5126261.05520257);

            if (DotSpatial.Plugins.AISViewer.Properties.Settings.Default.UsePseudoPort)
                pseudoAisPort.Open();
            else
                aisPort.Open();

            refreshTimer.Enabled = true;
        }

        private void pseudoAisPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataHandler handler = new DataHandler(helper.HandleRow);
            handler.BeginInvoke(pseudoAisPort.ReadLine(), null, null);
        }

        void refreshTimerTick(object sender, EventArgs e)
        {
            vesselsLayer.DataSet.InvalidateVertices();
            uxMap.MapFrame.Invalidate();
        }


        private void uxMap_GeoMouseMove(object sender, GeoMouseArgs e)
        {
            toolStripStatusLabel2.Text = string.Format("X:{0:000.00000}, Y:{1:000.00000}", e.GeographicLocation.X, e.GeographicLocation.Y);
        }

        private void uxMap_ViewExtentsChanged(object sender, DotSpatial.Data.ExtentArgs e)
        {
            Debug.WriteLine("xMin:{0}-yMin:{1}, xMax:{2}-yMax:{3}", uxMap.ViewExtents.MinX, uxMap.ViewExtents.MinY, uxMap.ViewExtents.MaxX, uxMap.ViewExtents.MaxY);
        }

        private void aisPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataHandler handler = new DataHandler(helper.HandleRow);
            handler.BeginInvoke(aisPort.ReadLine(), null, null);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // aisPort.Close();

            if (pseudoAisPort.IsOpen)
                pseudoAisPort.Close();
            pseudoAisPort = null;

            vesselsLayer = null;
            helper = null;
        }

    }
}
