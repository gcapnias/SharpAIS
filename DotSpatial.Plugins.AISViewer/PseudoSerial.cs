using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace DotSpatial.Plugins.AISViewer
{
    public partial class PseudoSerial : Component
    {
        private StreamReader sr;
        Queue<string> lineBuffer = new Queue<string>();

        public PseudoSerial()
        {
            InitializeComponent();
            flowTimer.Enabled = false;
        }

        public PseudoSerial(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            flowTimer.Enabled = false;
        }


        private string _LogFilePath;

        [Category("Behavior")]
        [DefaultValue("AIS.log")]
        public string LogFilePath
        {
            get { return _LogFilePath; }
            set
            {
                _LogFilePath = value;

                CloseFile();
                OpenFile();
            }
        }

        [Category("Behavior")]
        public int Interval
        {
            get { return flowTimer.Interval; }
            set { flowTimer.Interval = value; }
        }


        public void Open()
        {
            OpenFile();
        }

        private void OpenFile()
        {
            if (File.Exists(this.LogFilePath))
            {
                sr = new StreamReader(this.LogFilePath);

                flowTimer.Enabled = true;
            }
        }

        public void Close()
        {
            CloseFile();
        }

        private void CloseFile()
        {
            if (sr != null)
            {
                flowTimer.Enabled = false;

                sr.Close();
                sr = null;
            }
        }

        public string ReadLine()
        {
            string line = lineBuffer.Dequeue();
            return line;
        }

        public event EventHandler<SerialDataReceivedEventArgs> DataReceived;
        protected virtual void OnDataReceived(SerialDataReceivedEventArgs e)
        {
            EventHandler<SerialDataReceivedEventArgs> handler = DataReceived;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void flowTimer_Tick(object sender, EventArgs e)
        {
            if (sr != null)
                if (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    lineBuffer.Enqueue(line);
                    OnDataReceived(null);
                }
                else
                {
                    CloseFile();
                    OpenFile();
                }
        }

    }
}
