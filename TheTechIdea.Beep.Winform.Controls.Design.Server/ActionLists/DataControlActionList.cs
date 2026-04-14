using System;
using System.ComponentModel;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Shared action list for data-bound controls
    /// Provides the small shared surface that is genuinely common across data-bound controls.
    /// Control-specific sample-data and configuration flows stay on the individual designer.
    /// </summary>
    public class DataControlActionList : DesignerActionList
    {
        private readonly IBeepDesignerActionHost _designer;

        public DataControlActionList(IBeepDesignerActionHost designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

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
        /// Clear data binding
        /// </summary>
        public void ClearDataBinding()
        {
            _designer.SetProperty("DataSource", null);
            _designer.SetProperty("DisplayMember", string.Empty);
            _designer.SetProperty("ValueMember", string.Empty);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Data Binding"));
            items.Add(new DesignerActionMethodItem(this, "ClearDataBinding", "Clear Data Binding", "Data Binding", true));

            items.Add(new DesignerActionHeaderItem("Data Properties"));
            items.Add(new DesignerActionPropertyItem("DataSource", "Data Source", "Data Properties"));
            items.Add(new DesignerActionPropertyItem("DisplayMember", "Display Member", "Data Properties"));
            items.Add(new DesignerActionPropertyItem("ValueMember", "Value Member", "Data Properties"));

            return items;
        }
    }
}
