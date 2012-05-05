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
using SharpAIS.DotSpatialMap;

    public class SharpAISPlugin : Extension
    {
        private Timer refreshTimer = new Timer();
        private SimpleActionItem mnuDeviceStart;
        private SimpleActionItem mnuDeviceStop;

        private PseudoSerial pseudoAisPort = new PseudoSerial();
        private delegate void DataHandler(string row);

        private const string AisMenuKey = "SharpAIS";
        private const string AisGroupCaption = "SharpAIS";

        private AISHelper helper;
        private MapPointLayer vesselsLayer;

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

            helper = new AISHelper(App.Map.Projection);
            vesselsLayer = helper.VesselsLayer;

            base.Activate();
        }

        public override void Deactivate()
        {
            if (pseudoAisPort.IsOpen)
                pseudoAisPort.Close();
            pseudoAisPort = null;

            vesselsLayer = null;
            helper = null;

            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }


        void refreshTimerTick(object sender, EventArgs e)
        {
            vesselsLayer.DataSet.InvalidateVertices();
            App.Map.MapFrame.Invalidate();
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

        private void pseudoAisPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataHandler handler = new DataHandler(helper.HandleRow);
            handler.BeginInvoke(pseudoAisPort.ReadLine(), null, null);
        }


    }
}
