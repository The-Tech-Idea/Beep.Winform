using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace TheTechIdea.Beep.Winform.IDE.Extensions.ToolWindows
{
    /// <summary>
    /// Tool window for the Beep Data Block Designer
    /// Provides Oracle Forms-like visual design capabilities for BeepDataBlock components
    /// </summary>
    [Guid("4150e5d3-2e03-4bee-bbca-1314eb84ddb5")]
    public class BeepDataBlockDesignerToolWindow : ToolWindowPane
    {
        private Controls.BeepDataBlockDesignerControl designerControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDataBlockDesignerToolWindow"/> class.
        /// </summary>
        public BeepDataBlockDesignerToolWindow() : base(null)
        {
            this.Caption = "Beep Data Block Designer";

            // Create the designer control
            designerControl = new Controls.BeepDataBlockDesignerControl();
            
            // Set up event handlers for inter-tool communication
            designerControl.OnDataBlockSaved += OnDataBlockSaved;
            designerControl.OnCodeGenerationRequested += OnCodeGenerationRequested;
            designerControl.PropertyChanged += OnPropertyChanged;

            this.Content = designerControl;
        }

        /// <summary>
        /// Gets the data block designer control
        /// </summary>
        public Controls.BeepDataBlockDesignerControl DesignerControl => designerControl;

        /// <summary>
        /// Sets the entity to design in the data block designer
        /// </summary>
        /// <param name="entity">The entity structure to design</param>
        public void SetEntityForDesign(TheTechIdea.Beep.DataBase.EntityStructure entity)
        {
            if (designerControl != null)
            {
                designerControl.CurrentEntity = entity;
            }
        }

        #region Event Handlers
        private void OnDataBlockSaved(object sender, EventArgs e)
        {
            // Handle data block save event
            // This could trigger updates to other tool windows or save to project
            var package = this.Package as TheTechIdea.Beep.Winform.IDE.ExtensionsPackage;
            // TODO: Implement data block save handling
        }

        private void OnCodeGenerationRequested(object sender, EventArgs e)
        {
            // Handle code generation request
            var package = this.Package as TheTechIdea.Beep.Winform.IDE.ExtensionsPackage;
            // TODO: Implement code generation handling
        }

        private void OnPropertyChanged(object sender, EventArgs e)
        {
            // Handle property changes for properties window synchronization
            // TODO: Implement properties window updates
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (designerControl != null)
                {
                    designerControl.OnDataBlockSaved -= OnDataBlockSaved;
                    designerControl.OnCodeGenerationRequested -= OnCodeGenerationRequested;
                    designerControl.PropertyChanged -= OnPropertyChanged;
                }
            }
            base.Dispose(disposing);
        }
    }
}
