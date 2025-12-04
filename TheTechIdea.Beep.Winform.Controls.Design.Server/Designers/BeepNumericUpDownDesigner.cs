using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Numerics;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepNumericUpDown control
    /// </summary>
    public class BeepNumericUpDownDesigner : BaseBeepControlDesigner
    {
        public BeepNumericUpDown? NumericControl => Component as BeepNumericUpDown;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepNumericUpDownActionList(this));
            return lists;
        }
    }

    public class BeepNumericUpDownActionList : DesignerActionList
    {
        private readonly BeepNumericUpDownDesigner _designer;

        public BeepNumericUpDownActionList(BeepNumericUpDownDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Value")]
        public decimal Minimum
        {
            get => _designer.GetProperty<decimal>("Minimum");
            set => _designer.SetProperty("Minimum", value);
        }

        [Category("Value")]
        public decimal Maximum
        {
            get => _designer.GetProperty<decimal>("Maximum");
            set => _designer.SetProperty("Maximum", value);
        }

        [Category("Value")]
        public decimal Increment
        {
            get => _designer.GetProperty<decimal>("Increment");
            set => _designer.SetProperty("Increment", value);
        }

        [Category("Value")]
        public int DecimalPlaces
        {
            get => _designer.GetProperty<int>("DecimalPlaces");
            set => _designer.SetProperty("DecimalPlaces", value);
        }

        #endregion

        #region Preset Actions

        public void ConfigureForCurrency()
        {
            Minimum = 0;
            Maximum = 999999.99m;
            DecimalPlaces = 2;
            Increment = 0.01m;
        }

        public void ConfigureForPercentage()
        {
            Minimum = 0;
            Maximum = 100;
            DecimalPlaces = 2;
            Increment = 0.1m;
        }

        public void ConfigureForInteger()
        {
            Minimum = 0;
            Maximum = 1000;
            DecimalPlaces = 0;
            Increment = 1;
        }

        public void ConfigureForQuantity()
        {
            Minimum = 1;
            Maximum = 9999;
            DecimalPlaces = 0;
            Increment = 1;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Value Range"));
            items.Add(new DesignerActionPropertyItem("Minimum", "Minimum", "Value Range"));
            items.Add(new DesignerActionPropertyItem("Maximum", "Maximum", "Value Range"));
            items.Add(new DesignerActionPropertyItem("Increment", "Increment", "Value Range"));
            items.Add(new DesignerActionPropertyItem("DecimalPlaces", "Decimal Places", "Value Range"));

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForCurrency", "💰 Currency (0.00 - 999,999.99)", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForPercentage", "📊 Percentage (0-100%)", "Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForInteger", "🔢 Integer (0-1000)", "Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForQuantity", "📦 Quantity (1-9999)", "Presets", false));

            return items;
        }
    }
}

