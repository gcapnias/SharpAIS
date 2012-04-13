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

namespace AISMap
{
    public partial class frmMain : Form
    {
        public delegate void RowHandler(string row);
        private DateTime currentTime = new DateTime();
        private Parser parser = new Parser();
        MapPointLayer shipsLayer;
        private FeatureSet ships;
        private AISDataSet.Message1DataTable featureTable;

        public frmMain()
        {
            InitializeComponent();
            appManager1.LoadExtensions();

            uxMap.BackColor = Color.FromArgb(181, 208, 208);
            uxMap.Layers.Add(BruTileLayer.CreateOsmLayer());

            featureTable = new AISDataSet.Message1DataTable();
            ships = new FeatureSet(FeatureType.Point);
            ships.Projection = uxMap.Layers[0].Projection; //KnownCoordinateSystems.Geographic.World.WGS1984;
            ships.DataTable = featureTable;

            shipsLayer = new MapPointLayer(ships);
            shipsLayer.Symbolizer = new PointSymbolizer(Color.Blue, DotSpatial.Symbology.PointShape.Ellipse, 10);
            shipsLayer.LegendText = "AIS Features";
            uxMap.MapFrame.DrawingLayers.Add(shipsLayer);

            uxMap.ViewExtents = new Extent(1642982.27031471, 4063251.12000095, 3802748.48786722, 5126261.05520257);

            aisPort.Open();
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
            BeginInvoke(new RowHandler(HandleRow), aisPort.ReadLine());
        }

        private void HandleRow(string textline)
        {
            Hashtable rs = parser.Parse(textline);
            if (rs != null)
            {
                if (rs.ContainsKey("MessageID"))
                {
                    switch ((int)rs["MessageID"])
                    {
                        case 1:
                        case 2:
                        case 3:
                            double[] pointCoords = { (double)rs["Longitude"], (double)rs["Latitude"] };
                            double[] z = { 0 };
                            Reproject.ReprojectPoints(pointCoords, z, KnownCoordinateSystems.Geographic.World.WGS1984, shipsLayer.Projection, 0, 1);

                            Coordinate coord = new Coordinate(pointCoords[0], pointCoords[1]);

                            IFeature feature;

                            List<int> results = ships.Find(string.Format("UserID={0}", rs["UserID"]));
                            if (results != null && results.Count > 0)
                            {
                                feature = ships.GetFeature(results[0]);
                                feature.Coordinates[0] = coord;
                            }
                            else
                            {
                                DotSpatial.Topology.Point point = new DotSpatial.Topology.Point(coord);
                                feature = ships.AddFeature(point);
                            }

                            foreach (DictionaryEntry item in rs)
                            {
                                feature.DataRow[item.Key.ToString()] = item.Value;
                            }
                            uxMap.MapFrame.Invalidate();

                            Debug.WriteLine("Ship {0}: X:{1:.00000}, Y:{2:.00000}", rs["UserID"], coord.X, coord.Y);
                            break;

                        case 4:
                        case 11:
                            currentTime = new DateTime((int)rs["UTCYear"], (int)rs["UTCMonth"], (int)rs["UTCDay"], (int)rs["UTCHour"], (int)rs["UTCMinute"], (int)rs["UTCSecond"]);
                            break;

                        case 5:
                            break;

                        default:
                            break;
                    }
                }
            }

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            aisPort.Close();
        }

    }
}
