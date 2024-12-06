using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell.Design;

namespace TheTechIdea.Beep.Winform.VSIX.Extensions
{
    [ProvideToolboxItems(1)] // Define version
    [Guid("170475A2-1E3A-4E23-8623-35ED47304769")]
    public sealed class ToolboxInstallerPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            AddToolboxItems();
        }

        private void AddToolboxItems()
        {
            // Get the toolbox service
            IToolboxService toolboxService = GetService(typeof(IToolboxService)) as IToolboxService;
            if (toolboxService == null)
            {
                throw new InvalidOperationException("Unable to get the Toolbox service.");
            }

            // Add Toolbox Items
            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepCard>(
                toolboxService,
                "Beep Card",
                "A custom card control for WinForms.",
                "Resources.BeepCardIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepButton>(
                toolboxService,
                "Beep Button",
                "A custom button control for WinForms.",
                "Resources.BeepButtonIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepTree>(
                toolboxService,
                "Beep Tree",
                "A custom tree control for hierarchical data.",
                "Resources.BeepTreeIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepLabel>(
                toolboxService,
                "Beep Label",
                "A custom label control with enhanced features.",
                "Resources.BeepLabelIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepTextBox>(
                toolboxService,
                "Beep TextBox",
                "A custom TextBox control with advanced formatting.",
                "Resources.BeepTextBoxIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepBubbleChart>(
                toolboxService,
                "Beep Chart",
                "A versatile charting control for data visualization.",
                "Resources.BeepChartIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepSimpleGrid>(
                toolboxService,
                "Beep Simple Grid",
                "A simplified data grid control.",
                "Resources.BeepSimpleGridIcon.bmp");

            AddToolboxControl<TheTechIdea.Beep.Winform.Controls.BeepPanel>(
                toolboxService,
                "Beep Panel",
                "A customizable panel for WinForms layouts.",
                "Resources.BeepPanelIcon.bmp");
        }

        /// <summary>
        /// Adds a control to the Toolbox under the specified category.
        /// </summary>
        private void AddToolboxControl<T>(
            IToolboxService toolboxService,
            string displayName,
            string description,
            string iconResource)
        {
            // Create a new ToolboxItem
            var toolboxItem = new ToolboxItem(typeof(T));
            toolboxItem.DisplayName = displayName;
            toolboxItem.Description = description;

            // Set the icon
            toolboxItem.Bitmap = new Bitmap(typeof(ToolboxInstallerPackage), iconResource);

            // Add to toolbox category
            toolboxService.AddToolboxItem(toolboxItem, "Beep Controls");
        }
    }
}
