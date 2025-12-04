using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepListBox control
    /// </summary>
    public class BeepListBoxDesigner : BaseBeepControlDesigner
    {
        public BeepListBox? ListBox => Component as BeepListBox;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepListBoxActionList(this));
            return lists;
        }
    }

    public class BeepListBoxActionList : DesignerActionList
    {
        private readonly BeepListBoxDesigner _designer;

        public BeepListBoxActionList(BeepListBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Behavior")]
        public bool MultiSelect
        {
            get => _designer.GetProperty<bool>("MultiSelect");
            set => _designer.SetProperty("MultiSelect", value);
        }

        [Category("Behavior")]
        public bool EnableSearch
        {
            get => _designer.GetProperty<bool>("EnableSearch");
            set => _designer.SetProperty("EnableSearch", value);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("MultiSelect", "Multi-Select", "Behavior"));
            items.Add(new DesignerActionPropertyItem("EnableSearch", "Enable Search", "Behavior"));

            return items;
        }
    }
}

