using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace SharpAIS.DotSpatialMap
{
    public class AISHelper
    {
        private ProjectionInfo _projectionInfo = KnownCoordinateSystems.Geographic.World.WGS1984;
        private AISDataSet.Message1DataTable _vesselsDataTable;
        private FeatureSet _vesselsFeatureSet;
        private MapPointLayer _vesselsLayer;
        private Parser _aisParser = new Parser();

        public AISHelper()
        { }

        public AISHelper(ProjectionInfo projection)
            : this()
        {
            _projectionInfo = projection;
        }


        public AISDataSet.Message1DataTable VesselsDataTable
        {
            get
            {
                if (_vesselsDataTable == null)
                    _vesselsDataTable = new AISDataSet.Message1DataTable();

                return _vesselsDataTable;
            }
            set { _vesselsDataTable = value; }
        }

        public FeatureSet VesselsFeatureSet
        {
            get
            {
                if (_vesselsFeatureSet == null)
                {
                    _vesselsFeatureSet = new FeatureSet(FeatureType.Point);
                    _vesselsFeatureSet.Projection = _projectionInfo;
                    _vesselsFeatureSet.DataTable = VesselsDataTable;
                }

                return _vesselsFeatureSet;
            }
            set { _vesselsFeatureSet = value; }
        }

        public MapPointLayer VesselsLayer
        {
            get
            {
                if (_vesselsLayer == null)
                {
                    _vesselsLayer = new MapPointLayer(VesselsFeatureSet);
                    _vesselsLayer.Projection = VesselsFeatureSet.Projection;
                    _vesselsLayer.LegendText = "Vessels";

                    ApplySymbologyScheme(_vesselsLayer);
                    CreateLabelsLayer(_vesselsLayer);

                }

                return _vesselsLayer;
            }
            set { _vesselsLayer = value; }
        }


        private void ApplySymbologyScheme(MapPointLayer _vesselsLayer)
        {
            _vesselsLayer.Symbology.Categories.Clear();

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.LightGray, DotSpatial.Symbology.PointShape.Rectangle, 9),
                    "[NavigationalStatus] = 0",
                    "Under way using engine"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.LightGray, DotSpatial.Symbology.PointShape.Diamond, 9),
                    "[NavigationalStatus] = 1",
                    "At anchor"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.Red, DotSpatial.Symbology.PointShape.Rectangle, 9),
                    "[NavigationalStatus] = 2",
                    "Not under command"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.Orange, DotSpatial.Symbology.PointShape.Rectangle, 9),
                    "[NavigationalStatus] = 3",
                    "Restricted manoeuverability"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.DarkOrange, DotSpatial.Symbology.PointShape.Rectangle, 9),
                    "[NavigationalStatus] = 4",
                    "Constrained by her draught"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.Blue, DotSpatial.Symbology.PointShape.Diamond, 9),
                    "[NavigationalStatus] = 5",
                    "Moored"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.DarkBlue, DotSpatial.Symbology.PointShape.Diamond, 9),
                    "[NavigationalStatus] = 6",
                    "Aground"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.LightGreen, DotSpatial.Symbology.PointShape.Rectangle, 9),
                    "[NavigationalStatus] = 7",
                    "Engaged in Fishing"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.LightGoldenrodYellow, DotSpatial.Symbology.PointShape.Rectangle, 9),
                    "[NavigationalStatus] = 8",
                    "Under way sailing"
                )
            );

            _vesselsLayer.Symbology.Categories.Add(
                GetPointCategory(
                    GetPointSymbolizer(Color.LightGray, DotSpatial.Symbology.PointShape.Hexagon, 9),
                    "[NavigationalStatus] > 8",
                    "Not defined"
                )
            );

            _vesselsLayer.ApplyScheme(VesselsLayer.Symbology);
            _vesselsLayer.DataSet.InvalidateVertices();
        }

        private void CreateLabelsLayer(MapPointLayer _vesselsLayer)
        {
            _vesselsLayer.LabelLayer = new MapLabelLayer(VesselsFeatureSet) { Projection = _vesselsLayer.Projection };
            _vesselsLayer.ShowLabels = true;
            //_vesselsLayer.LabelLayer.UseDynamicVisibility = true;
            //_vesselsLayer.LabelLayer.DynamicVisibilityWidth = 0.025F;

            _vesselsLayer.LabelLayer.Symbology.Categories.Clear();
            _vesselsLayer.LabelLayer.Symbology.Categories.Add(
                new LabelCategory()
                {
                    Expression = "[MMSI]",
                    Symbolizer = new LabelSymbolizer()
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
                    },
                    Name = "Vessels' Names"
                }
            );

            _vesselsLayer.LabelLayer.CreateLabels();
        }


        private PointSymbolizer GetPointSymbolizer(Color symbolColor, DotSpatial.Symbology.PointShape symbolShape, double symbolSize)
        {
            IList<ISymbol> symbolsList = new CopyList<ISymbol>();
            symbolsList.Add(new SimpleSymbol(symbolColor, symbolShape, symbolSize) { UseOutline = true, OutlineColor = Color.Black });
            return new PointSymbolizer(symbolsList) { Smoothing = true };
        }

        private IPointCategory GetPointCategory(PointSymbolizer pointSymbolizer, string filterExpression, string legendText)
        {
            return new PointCategory(pointSymbolizer) { FilterExpression = filterExpression, LegendText = legendText };
        }

        public void HandleRow(string textline)
        {
            Hashtable rs = _aisParser.Parse(textline);
            if (rs != null)
            {
                if (rs.ContainsKey("MessageType"))
                {
                    switch ((uint)rs["MessageType"])
                    {
                        case 1:
                        case 2:
                        case 3:
                            double[] pointCoords = { (double)rs["Lontitude"], (double)rs["Lattitude"] };
                            double[] z = { 0 };

                            if (!_projectionInfo.Equals(KnownCoordinateSystems.Geographic.World.WGS1984))
                                Reproject.ReprojectPoints(pointCoords, z, KnownCoordinateSystems.Geographic.World.WGS1984, _projectionInfo, 0, 1);

                            Coordinate coord = new Coordinate(pointCoords[0], pointCoords[1]);
                            IFeature feature;

                            List<int> results = VesselsFeatureSet.Find(string.Format("[MMSI]={0}", rs["MMSI"]));
                            if (results != null && results.Any())
                            {
                                feature = VesselsFeatureSet.GetFeature(results[0]);
                                feature.Coordinates[0] = coord;
                            }
                            else
                            {
                                DotSpatial.Topology.Point point = new DotSpatial.Topology.Point(coord);
                                feature = VesselsFeatureSet.AddFeature(point);
                            }

                            //
                            // Fix values outside range
                            if (double.IsInfinity((double)rs["CourseOverGround"]) || double.IsNaN((double)rs["CourseOverGround"]))
                                rs["CourseOverGround"] = 0F;
                            if (double.IsInfinity((double)rs["SpeedOverGround"]) || double.IsNaN((double)rs["SpeedOverGround"]))
                                rs["SpeedOverGround"] = 0F;

                            foreach (DictionaryEntry item in rs)
                                feature.DataRow[item.Key.ToString()] = item.Value;

                            break;

                        case 5:
                            break;

                        default:
                            break;
                    }
                }
            }
        }

    }
}
