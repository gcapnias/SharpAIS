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

namespace DotSpatial.Examples.AISViewer
{
    public partial class frmMain : Form
    {
        public delegate void RowHandler(string row);
        private DateTime currentTime = new DateTime();
        private Parser parser = new Parser();

        BruTileLayer backgroundLayer;
        MapPointLayer vesselsLayer;
        private FeatureSet vesselsFeatureSet;
        private AISDataSet.Message1DataTable vesselsDataTable;

        private PseudoSerial pseudoAisPort = new PseudoSerial();

        public frmMain()
        {
            InitializeComponent();

            uxMap.Projection = KnownCoordinateSystems.Projected.World.WebMercator;
            uxMap.BackColor = Color.FromArgb(181, 208, 208);

            backgroundLayer = BruTileLayer.CreateOsmLayer();
            uxMap.Layers.Add(backgroundLayer);

            vesselsDataTable = new AISDataSet.Message1DataTable();
            vesselsFeatureSet = new FeatureSet(FeatureType.Point);
            vesselsFeatureSet.Projection = KnownCoordinateSystems.Projected.World.WebMercator;
            vesselsFeatureSet.DataTable = vesselsDataTable;

            PointSymbolizer vesselsSymbolizerUnderWay = getPointSymbolizer(Color.LightGray, Symbology.PointShape.Rectangle, 8);
            IPointCategory vesselsUnderWayCategory = getPointCategory(vesselsSymbolizerUnderWay, "[NavigationalStatus] = 0", "Under Way");

            PointSymbolizer vesselsSymbolizerAtAnchor = getPointSymbolizer(Color.LightPink, Symbology.PointShape.Diamond, 8);
            IPointCategory vesselsAtAnchorCategory = getPointCategory(vesselsSymbolizerAtAnchor, "[NavigationalStatus] >= 1", "At Anchor");


            vesselsLayer = new MapPointLayer(vesselsFeatureSet);
            vesselsLayer.LegendText = "Vessels";
            //vesselsLayer.Symbolizer = new PointSymbolizer(Color.LightGreen, Symbology.PointShape.Rectangle, 10);

            //IFeatureScheme vesselsScheme = new PointScheme();
            //vesselsScheme.ClearCategories();
            //vesselsScheme.AddCategory(vesselsUnderWayCategory);
            //vesselsScheme.AddCategory(vesselsAtAnchorCategory);
            //vesselsLayer.ApplyScheme(vesselsScheme);

            //vesselsFeatureSet.FillAttributes();
            vesselsLayer.Symbology.Categories.Clear();
            vesselsLayer.Symbology.Categories.Add(vesselsUnderWayCategory);
            vesselsLayer.Symbology.Categories.Add(vesselsAtAnchorCategory);
            vesselsLayer.ApplyScheme(vesselsLayer.Symbology);
            //vesselsLayer.AssignFastDrawnStates();
            vesselsLayer.DataSet.InvalidateVertices();


            LabelSymbolizer vesselsLabelSymbolizer = new LabelSymbolizer()
            {
                FontFamily = "Tahoma",
                FontColor = Color.Black,
                FontSize = 8,
                BackColor = Color.White,
                BackColorEnabled = true,
                BackColorOpacity = 0.5f,
                Orientation = ContentAlignment.MiddleRight,
                PartsLabelingMethod = PartLabelingMethod.LabelAllParts,
                OffsetX = 5
            };

            ILabelCategory vesselsLabelCategory = new LabelCategory()
            {
                Expression = "[MMSI]",
                Symbolizer = vesselsLabelSymbolizer,
                Name = "Vessels' Names"
            };

            vesselsLayer.LabelLayer = new MapLabelLayer(vesselsFeatureSet);
            vesselsLayer.ShowLabels = true;
            //vesselsLayer.LabelLayer.UseDynamicVisibility = true;
            //vesselsLayer.LabelLayer.DynamicVisibilityWidth = 0.025F;
            vesselsLayer.LabelLayer.Symbology.Categories.Clear();
            vesselsLayer.LabelLayer.Symbology.Categories.Add(vesselsLabelCategory);
            vesselsLayer.LabelLayer.CreateLabels();
            uxMap.MapFrame.Add(vesselsLayer);

            uxMap.ViewExtents = new Extent(1642982.27031471, 4063251.12000095, 3802748.48786722, 5126261.05520257);

            //
            // aisPort.Open();
            pseudoAisPort.LogFilePath = "AIS.log";
            pseudoAisPort.DataReceived += new EventHandler<SerialDataReceivedEventArgs>(pseudoAisPortDataReceived);
            pseudoAisPort.Open();
        }

        void pseudoAisPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            BeginInvoke(new RowHandler(HandleRow), pseudoAisPort.ReadLine());
        }


        private PointSymbolizer getPointSymbolizer(Color symbolColor, DotSpatial.Symbology.PointShape symbolShape, double symbolSize)
        {
            IList<ISymbol> symbolsList = new CopyList<ISymbol>();
            ISimpleSymbol symbol = new SimpleSymbol(symbolColor, symbolShape, symbolSize) { UseOutline = true, OutlineColor = Color.Black };
            symbolsList.Add(symbol);
            PointSymbolizer pointSymbolizer = new PointSymbolizer()
            {
                Smoothing = true,
                Symbols = symbolsList
            };

            return pointSymbolizer;
        }

        private IPointCategory getPointCategory(PointSymbolizer pointSymbolizer, string filterExpression, string legendText)
        {
            return new PointCategory(pointSymbolizer)
            {
                FilterExpression = filterExpression,
                LegendText = legendText
            };
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
                if (rs.ContainsKey("MessageType"))
                {
                    switch ((uint)rs["MessageType"])
                    {
                        case 1:
                        case 2:
                        case 3:
                            double[] pointCoords = { (double)rs["Longitude"], (double)rs["Latitude"] };
                            double[] z = { 0 };
                            Reproject.ReprojectPoints(pointCoords, z, KnownCoordinateSystems.Geographic.World.WGS1984, KnownCoordinateSystems.Projected.World.WebMercator, 0, 1);
                            Coordinate coord = new Coordinate(pointCoords[0], pointCoords[1]);

                            //double transformedLongitude = (double)rs["Longitude"] * 20037508.34 / 180;
                            //double transformedLatitude = Math.Log(Math.Tan((90 + (double)rs["Latitude"]) * Math.PI / 360)) / (Math.PI / 180);
                            //transformedLatitude = transformedLatitude * 20037508.34 / 180;
                            //Coordinate coord = new Coordinate(transformedLongitude, transformedLatitude);

                            // WGS 84
                            //Coordinate coord = new Coordinate((double)rs["Longitude"], (double)rs["Latitude"]);

                            IFeature feature;

                            List<int> results = vesselsFeatureSet.Find(string.Format("[MMSI]={0}", rs["MMSI"]));
                            if (results != null && results.Any())
                            {
                                feature = vesselsFeatureSet.GetFeature(results[0]);
                                feature.Coordinates[0] = coord;
                            }
                            else
                            {
                                DotSpatial.Topology.Point point = new DotSpatial.Topology.Point(coord);
                                feature = vesselsFeatureSet.AddFeature(point);
                            }

                            //
                            // Fix values outside range
                            if (double.IsInfinity((double)rs["CourseOverGround"]) || double.IsNaN((double)rs["CourseOverGround"]))
                                rs["CourseOverGround"] = 0F;

                            if (double.IsInfinity((double)rs["SpeedOverGround"]) || double.IsNaN((double)rs["SpeedOverGround"]))
                                rs["SpeedOverGround"] = 0F;

                            foreach (DictionaryEntry item in rs)
                            {
                                feature.DataRow[item.Key.ToString()] = item.Value;
                            }

                            vesselsLayer.LabelLayer.CreateLabels();
                            uxMap.MapFrame.Invalidate();

                            //Debug.WriteLine("Ship {0}: X:{1:.00000}, Y:{2:.00000}", rs["MMSI"], coord.X, coord.Y);
                            break;

                        case 4:
                        case 11:
                            //currentTime = new DateTime(int.Parse(rs["Year"].ToString()), int.Parse(rs["Month"].ToString()), int.Parse(rs["Day"].ToString()), int.Parse(rs["Hour"].ToString()), int.Parse(rs["Minute"].ToString()), int.Parse(rs["Second"].ToString()));
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
            // aisPort.Close();
            pseudoAisPort.Close();
        }

    }
}
