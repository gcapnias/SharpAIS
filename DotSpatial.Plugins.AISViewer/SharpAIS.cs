namespace DotSpatial.Plugins.AISViewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;

    public class SharpAIS : Extension
    {
        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem("My Button Caption", ButtonClick));
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        public void ButtonClick(object sender, EventArgs e)
        {
            // Your logic goes here.
        }
    }
}
