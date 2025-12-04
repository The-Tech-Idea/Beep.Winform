using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Charts;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepChart control
    /// </summary>
    public class BeepChartDesigner : BaseBeepControlDesigner
    {
        public BeepChart? Chart => Component as BeepChart;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepChartActionList(this));
            return lists;
        }
    }

    public class BeepChartActionList : DesignerActionList
    {
        private readonly BeepChartDesigner _designer;

        public BeepChartActionList(BeepChartDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Appearance")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? "";
            set => _designer.SetProperty("Title", value);
        }

        [Category("Behavior")]
        public bool ShowLegend
        {
            get => _designer.GetProperty<bool>("ShowLegend");
            set => _designer.SetProperty("ShowLegend", value);
        }

        [Category("Behavior")]
        public bool ShowGrid
        {
            get => _designer.GetProperty<bool>("ShowGrid");
            set => _designer.SetProperty("ShowGrid", value);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Title", "Chart Title", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Display"));
            items.Add(new DesignerActionPropertyItem("ShowLegend", "Show Legend", "Display"));
            items.Add(new DesignerActionPropertyItem("ShowGrid", "Show Grid", "Display"));

            return items;
        }
    }
}

