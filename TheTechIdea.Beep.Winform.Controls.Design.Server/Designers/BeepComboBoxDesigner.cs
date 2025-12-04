using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepComboBox control
    /// </summary>
    public class BeepComboBoxDesigner : BaseBeepControlDesigner
    {
        public BeepComboBox? ComboBox => Component as BeepComboBox;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepComboBoxActionList(this));
            return lists;
        }
    }

    public class BeepComboBoxActionList : DesignerActionList
    {
        private readonly BeepComboBoxDesigner _designer;

        public BeepComboBoxActionList(BeepComboBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Behavior")]
        public bool IsEditable
        {
            get => _designer.GetProperty<bool>("IsEditable");
            set => _designer.SetProperty("IsEditable", value);
        }

        [Category("Behavior")]
        public bool MultiSelect
        {
            get => _designer.GetProperty<bool>("MultiSelect");
            set => _designer.SetProperty("MultiSelect", value);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("IsEditable", "Editable", "Behavior"));
            items.Add(new DesignerActionPropertyItem("MultiSelect", "Multi-Select", "Behavior"));

            return items;
        }
    }
}

