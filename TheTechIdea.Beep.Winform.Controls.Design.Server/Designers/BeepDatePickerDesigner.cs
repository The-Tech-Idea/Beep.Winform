using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Dates;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepDatePicker control
    /// </summary>
    public class BeepDatePickerDesigner : BaseBeepControlDesigner
    {
        public BeepDatePicker? DatePicker => Component as BeepDatePicker;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepDatePickerActionList(this));
            return lists;
        }
    }

    public class BeepDatePickerActionList : DesignerActionList
    {
        private readonly BeepDatePickerDesigner _designer;

        public BeepDatePickerActionList(BeepDatePickerDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Behavior")]
        public bool AllowEmpty
        {
            get => _designer.GetProperty<bool>("AllowEmpty");
            set => _designer.SetProperty("AllowEmpty", value);
        }

        [Category("Behavior")]
        public bool ReadOnly
        {
            get => _designer.GetProperty<bool>("ReadOnly");
            set => _designer.SetProperty("ReadOnly", value);
        }

        #endregion

        #region Business Configuration Presets

        public void ConfigureForDueDate()
        {
            _designer.GetProperty<BeepDatePicker>("Component")?.ConfigureForDueDate();
        }

        public void ConfigureForCreationDate()
        {
            _designer.GetProperty<BeepDatePicker>("Component")?.ConfigureForCreationDate();
        }

        public void ConfigureForEventScheduling()
        {
            _designer.GetProperty<BeepDatePicker>("Component")?.ConfigureForEventScheduling();
        }

        public void ConfigureForBirthDate()
        {
            _designer.GetProperty<BeepDatePicker>("Component")?.ConfigureForBirthDate();
        }

        public void ConfigureForAppointment()
        {
            _designer.GetProperty<BeepDatePicker>("Component")?.ConfigureForAppointment();
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("AllowEmpty", "Allow Empty", "Behavior"));
            items.Add(new DesignerActionPropertyItem("ReadOnly", "Read Only", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Business Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForDueDate", "📅 Due Date", "Business Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForCreationDate", "📝 Creation Date", "Business Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForEventScheduling", "🎉 Event Scheduling", "Business Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForBirthDate", "🎂 Birth Date", "Business Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForAppointment", "🕐 Appointment", "Business Presets", false));

            return items;
        }
    }
}

