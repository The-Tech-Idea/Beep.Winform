using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Shared action list for data-bound controls
    /// Provides common actions for configuring data sources and bindings
    /// </summary>
    public class DataControlActionList : DesignerActionList
    {
        private readonly BaseBeepControlDesigner _designer;

        public DataControlActionList(BaseBeepControlDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BaseControl? BeepControl => Component as BaseControl;

        #region Properties

        [Category("Data")]
        [Description("Data source for the control")]
        public object? DataSource
        {
            get => _designer.GetProperty<object>("DataSource");
            set => _designer.SetProperty("DataSource", value);
        }

        [Category("Data")]
        [Description("Display member for the control")]
        public string DisplayMember
        {
            get => _designer.GetProperty<string>("DisplayMember") ?? string.Empty;
            set => _designer.SetProperty("DisplayMember", value);
        }

        [Category("Data")]
        [Description("Value member for the control")]
        public string ValueMember
        {
            get => _designer.GetProperty<string>("ValueMember") ?? string.Empty;
            set => _designer.SetProperty("ValueMember", value);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Generate sample data for the control
        /// </summary>
        public void GenerateSampleData()
        {
            // This would use DesignTimeDataHelper to populate the control
            // Implementation depends on control type
        }

        /// <summary>
        /// Clear data binding
        /// </summary>
        public void ClearDataBinding()
        {
            _designer.SetProperty("DataSource", null);
            _designer.SetProperty("DisplayMember", string.Empty);
            _designer.SetProperty("ValueMember", string.Empty);
        }

        /// <summary>
        /// Configure data source
        /// </summary>
        public void ConfigureDataSource()
        {
            // In a full implementation, this would show a dialog to configure data source
            // For now, just a placeholder
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Data Binding"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureDataSource", "Configure Data Source...", "Data Binding", true));
            items.Add(new DesignerActionMethodItem(this, "GenerateSampleData", "Generate Sample Data", "Data Binding", true));
            items.Add(new DesignerActionMethodItem(this, "ClearDataBinding", "Clear Data Binding", "Data Binding", true));

            items.Add(new DesignerActionHeaderItem("Data Properties"));
            items.Add(new DesignerActionPropertyItem("DataSource", "Data Source", "Data Properties"));
            items.Add(new DesignerActionPropertyItem("DisplayMember", "Display Member", "Data Properties"));
            items.Add(new DesignerActionPropertyItem("ValueMember", "Value Member", "Data Properties"));

            return items;
        }
    }
}
