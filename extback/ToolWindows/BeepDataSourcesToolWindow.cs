using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.IDE.Extensions.ToolWindows
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// 
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// 
    /// This class derives from the ToolWindowPane class provided by the Managed Package Framework
    /// and defines the appearance and behavior of the tool window.
    /// </summary>
    [Guid("3140e5d3-2e03-4bee-bbca-1314eb84ddb4")]
    public class BeepDataSourcesToolWindow : ToolWindowPane
    {
        private Controls.BeepDataSourcesControl dataSourcesControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataSourcesToolWindow"/> class.
        /// </summary>
        public BeepDataSourcesToolWindow() : base(null)
        {
            this.Caption = "Beep Data Sources";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            dataSourcesControl = new Controls.BeepDataSourcesControl();
            
            // Set up event handlers to communicate with other tool windows
            dataSourcesControl.OnPropertiesRequested += OnPropertiesRequested;
            dataSourcesControl.OnDataBlockRequested += OnDataBlockRequested;
            dataSourcesControl.OnSelectionChanged += OnSelectionChanged;

            this.Content = dataSourcesControl;
        }

        /// <summary>
        /// Gets the data sources control
        /// </summary>
        public Controls.BeepDataSourcesControl DataSourcesControl => dataSourcesControl;

        #region Event Handlers
        private void OnPropertiesRequested(object sender, object selectedObject)
        {
            // Communicate with Properties tool window
            // This would be handled by the package to show properties
            var package = this.Package as TheTechIdea.Beep.Winform.IDE.ExtensionsPackage;
            // TODO: Implement properties window communication
        }

        private void OnDataBlockRequested(object sender, TheTechIdea.Beep.DataBase.EntityStructure entity)
        {
            // Communicate with Data Block Designer tool window
            var package = this.Package as TheTechIdea.Beep.Winform.IDE.ExtensionsPackage;
            // TODO: Implement data block designer communication
        }

        private void OnSelectionChanged(object sender, object selectedObject)
        {
            // Update other tool windows based on selection
            // TODO: Implement selection synchronization
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dataSourcesControl != null)
                {
                    dataSourcesControl.OnPropertiesRequested -= OnPropertiesRequested;
                    dataSourcesControl.OnDataBlockRequested -= OnDataBlockRequested;
                    dataSourcesControl.OnSelectionChanged -= OnSelectionChanged;
                }
            }
            base.Dispose(disposing);
        }
    }
}
