using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Chips;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepMultiChipGroup control
    /// </summary>
    public class BeepMultiChipGroupDesigner : BaseBeepControlDesigner
    {
        public BeepMultiChipGroup? ChipGroup => Component as BeepMultiChipGroup;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMultiChipGroupActionList(this));
            return lists;
        }
    }

    public class BeepMultiChipGroupActionList : DesignerActionList
    {
        private readonly BeepMultiChipGroupDesigner _designer;

        public BeepMultiChipGroupActionList(BeepMultiChipGroupDesigner designer)
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
        public bool AllowUserAddition
        {
            get => _designer.GetProperty<bool>("AllowUserAddition");
            set => _designer.SetProperty("AllowUserAddition", value);
        }

        [Category("Appearance")]
        public int ChipSpacing
        {
            get => _designer.GetProperty<int>("ChipSpacing");
            set => _designer.SetProperty("ChipSpacing", value);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("MultiSelect", "Multi-Select", "Behavior"));
            items.Add(new DesignerActionPropertyItem("AllowUserAddition", "Allow User Addition", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ChipSpacing", "Chip Spacing", "Layout"));

            return items;
        }
    }
}

