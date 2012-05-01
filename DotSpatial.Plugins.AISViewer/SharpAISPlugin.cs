namespace DotSpatial.Plugins.AISViewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using DotSpatial.Plugins.AISViewer.Properties;
    using System.IO.Ports;
    using System.Collections;
    using SharpAIS;
    using DotSpatial.Projections;
    using DotSpatial.Topology;
    using DotSpatial.Data;
    using DotSpatial.Symbology;
    using System.Drawing;
    using System.Windows.Forms;

    public class SharpAISPlugin : Extension
    {
        private Timer refreshTimer = new Timer();
        private SimpleActionItem mnuDeviceStart;
        private SimpleActionItem mnuDeviceStop;

        private delegate void RowHandler(string row);
        private PseudoSerial pseudoAisPort = new PseudoSerial();
        private Parser parser = new Parser();
        private const string AisMenuKey = "SharpAIS";
        private const string AisGroupCaption = "SharpAIS";

        MapPointLayer vesselsLayer;
        private FeatureSet vesselsFeatureSet;
        private AISDataSet.Message1DataTable vesselsDataTable;

        public override void Activate()
        {
            App.HeaderControl.Add(new RootItem(AisMenuKey, Resources.MenuAIS));
            App.HeaderControl.Add(mnuDeviceStart = new SimpleActionItem(AisMenuKey, Resources.DeviceStart, DeviceStart)
            {
                LargeImage = Resources.playback_start_32,
                SmallImage = Resources.playback_start_16,
                ToolTipText = "Start Decoding...",
                GroupCaption = AisGroupCaption
            });
            App.HeaderControl.Add(mnuDeviceStop = new SimpleActionItem(AisMenuKey, Resources.DeviceStop, DeviceStop)
            {
                LargeImage = Resources.playback_stop_32,
                SmallImage = Resources.playback_stop_16,
                ToolTipText = "Stop Decoding",
                GroupCaption = AisGroupCaption,
                Enabled = false
            });

            pseudoAisPort.LogFilePath = Application.StartupPath + "\\AIS.log";
            pseudoAisPort.DataReceived += new EventHandler<SerialDataReceivedEventArgs>(pseudoAisPortDataReceived);

            refreshTimer.Enabled = false;
            refreshTimer.Interval = 1500;
            refreshTimer.Tick += new EventHandler(refreshTimerTick);

            PrepareAisMap();

            base.Activate();
        }

        void refreshTimerTick(object sender, EventArgs e)
        {
            vesselsLayer.DataSet.InvalidateVertices();
            App.Map.MapFrame.Invalidate();
        }


        private void PrepareAisMap()
        {
            vesselsDataTable = new AISDataSet.Message1DataTable();
            vesselsFeatureSet = new FeatureSet(FeatureType.Point);
            vesselsFeatureSet.Projection = App.Map.Projection;
            vesselsFeatureSet.DataTable = vesselsDataTable;

            PointSymbolizer vesselsSymbolizerUnderWay = getPointSymbolizer(Color.LightGray, Symbology.PointShape.Rectangle, 8);
            IPointCategory vesselsUnderWayCategory = getPointCategory(vesselsSymbolizerUnderWay, "[NavigationalStatus] = 0", "Under Way");

            PointSymbolizer vesselsSymbolizerAtAnchor = getPointSymbolizer(Color.LightGoldenrodYellow, Symbology.PointShape.Ellipse, 8);
            IPointCategory vesselsAtAnchorCategory = getPointCategory(vesselsSymbolizerAtAnchor, "[NavigationalStatus] = 1", "At Anchor");

            PointSymbolizer vesselsSymbolizerMoored = getPointSymbolizer(Color.LightGreen, Symbology.PointShape.Rectangle, 8);
            IPointCategory vesselsMooredCategory = getPointCategory(vesselsSymbolizerMoored, "[NavigationalStatus] = 5", "Moored");

            PointSymbolizer vesselsSymbolizerFishing = getPointSymbolizer(Color.LightSalmon, Symbology.PointShape.Rectangle, 8);
            IPointCategory vesselsFishingCategory = getPointCategory(vesselsSymbolizerFishing, "[NavigationalStatus] = 7", "Engaged in Fishing");

            PointSymbolizer vesselsSymbolizerSailing = getPointSymbolizer(Color.LightSteelBlue, Symbology.PointShape.Rectangle, 8);
            IPointCategory vesselsSailingCategory = getPointCategory(vesselsSymbolizerSailing, "[NavigationalStatus] = 8", "Under Way Sailing");

            PointSymbolizer vesselsSymbolizerNotDefined = getPointSymbolizer(Color.LightPink, Symbology.PointShape.Diamond, 8);
            IPointCategory vesselsNotDefinedCategory = getPointCategory(vesselsSymbolizerNotDefined, "[NavigationalStatus] = 15", "Not defined");


            vesselsLayer = new MapPointLayer(vesselsFeatureSet);
            vesselsLayer.Projection = vesselsFeatureSet.Projection;
            vesselsLayer.LegendText = "Vessels";
            //vesselsLayer.Symbolizer = new PointSymbolizer(Color.LightGreen, Symbology.PointShape.Rectangle, 10);

            vesselsLayer.Symbology.Categories.Clear();
            vesselsLayer.Symbology.Categories.Add(vesselsUnderWayCategory);
            vesselsLayer.Symbology.Categories.Add(vesselsAtAnchorCategory);
            vesselsLayer.Symbology.Categories.Add(vesselsMooredCategory);
            vesselsLayer.Symbology.Categories.Add(vesselsFishingCategory);
            vesselsLayer.Symbology.Categories.Add(vesselsSailingCategory);
            vesselsLayer.Symbology.Categories.Add(vesselsNotDefinedCategory);

            vesselsLayer.ApplyScheme(vesselsLayer.Symbology);
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

            vesselsLayer.LabelLayer = new MapLabelLayer(vesselsFeatureSet) { Projection = vesselsLayer.Projection };
            vesselsLayer.ShowLabels = true;
            //vesselsLayer.LabelLayer.UseDynamicVisibility = true;
            //vesselsLayer.LabelLayer.DynamicVisibilityWidth = 0.025F;
            vesselsLayer.LabelLayer.Symbology.Categories.Clear();
            vesselsLayer.LabelLayer.Symbology.Categories.Add(vesselsLabelCategory);
            vesselsLayer.LabelLayer.CreateLabels();

        }

        private PointSymbolizer getPointSymbolizer(Color symbolColor, DotSpatial.Symbology.PointShape symbolShape, double symbolSize)
        {
            IList<ISymbol> symbolsList = new CopyList<ISymbol>();
            ISimpleSymbol symbol = new SimpleSymbol(symbolColor, symbolShape, symbolSize) { UseOutline = true, OutlineColor = Color.Black };
            symbolsList.Add(symbol);
            PointSymbolizer pointSymbolizer = new PointSymbolizer(symbolsList)
            {
                Smoothing = true
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

                            if (!App.Map.Projection.Equals(KnownCoordinateSystems.Geographic.World.WGS1984))
                                Reproject.ReprojectPoints(pointCoords, z, KnownCoordinateSystems.Geographic.World.WGS1984, App.Map.Projection, 0, 1);

                            Coordinate coord = new Coordinate(pointCoords[0], pointCoords[1]);

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

                            //vesselsLayer.DataSet.InvalidateVertices();
                            //App.Map.MapFrame.Invalidate();
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

        private void pseudoAisPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RowHandler handler = new RowHandler(HandleRow);
            handler.BeginInvoke(pseudoAisPort.ReadLine(), null, null);
        }

        private void DeviceStart(object sender, EventArgs e)
        {
            App.Map.MapFrame.Add(vesselsLayer);

            pseudoAisPort.Open();
            mnuDeviceStart.Enabled = false;
            mnuDeviceStop.Enabled = true;

            refreshTimer.Enabled = true;
        }

        private void DeviceStop(object sender, EventArgs e)
        {
            App.Map.MapFrame.Remove(vesselsLayer);

            pseudoAisPort.Close();
            mnuDeviceStart.Enabled = true;
            mnuDeviceStop.Enabled = false;

            refreshTimer.Enabled = false;
        }


        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

    }
}
