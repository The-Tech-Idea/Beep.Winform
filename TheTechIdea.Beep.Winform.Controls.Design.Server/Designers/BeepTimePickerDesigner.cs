using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepTimePicker control
    /// </summary>
    public class BeepTimePickerDesigner : BaseBeepControlDesigner
    {
        public BeepTimePicker? TimePicker => Component as BeepTimePicker;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepTimePickerActionList(this));
            return lists;
        }
    }

    public class BeepTimePickerActionList : DesignerActionList
    {
        private readonly BeepTimePickerDesigner _designer;

        public BeepTimePickerActionList(BeepTimePickerDesigner designer)
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
        public int MinuteInterval
        {
            get => _designer.GetProperty<int>("MinuteInterval");
            set => _designer.SetProperty("MinuteInterval", value);
        }

        #endregion

        #region Business Presets

        public void ConfigureForMeetingTime()
        {
            _designer.GetProperty<BeepTimePicker>("Component")?.ConfigureForMeetingTime();
        }

        public void ConfigureForAppointment()
        {
            _designer.GetProperty<BeepTimePicker>("Component")?.ConfigureForAppointment();
        }

        public void ConfigureForShiftTime()
        {
            _designer.GetProperty<BeepTimePicker>("Component")?.ConfigureForShiftTime();
        }

        public void ConfigureForDeadline()
        {
            _designer.GetProperty<BeepTimePicker>("Component")?.ConfigureForDeadline();
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("MinuteInterval", "Minute Interval", "Behavior"));
            items.Add(new DesignerActionPropertyItem("AllowEmpty", "Allow Empty", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Business Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForMeetingTime", "📅 Meeting Time", "Business Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForAppointment", "🕐 Appointment", "Business Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForShiftTime", "⏰ Shift Time", "Business Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ConfigureForDeadline", "⏳ Deadline", "Business Presets", false));

            return items;
        }
    }
}

