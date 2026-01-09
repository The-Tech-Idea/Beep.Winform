using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepCalendarWidgetDesigner : BaseWidgetDesigner
    {
        public BeepCalendarWidget? CalendarWidget => Component as BeepCalendarWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepCalendarWidgetActionList(this));
            return lists;
        }
    }

    public class BeepCalendarWidgetActionList : DesignerActionList
    {
        private readonly BeepCalendarWidgetDesigner _designer;

        public BeepCalendarWidgetActionList(BeepCalendarWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Calendar")]
        [Description("Visual style of the calendar widget")]
        public CalendarWidgetStyle Style
        {
            get => _designer.GetProperty<CalendarWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Calendar")]
        [Description("Title of the calendar")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsMonthView() { Style = CalendarWidgetStyle.CalendarView; }
        public void ConfigureAsWeekView() { Style = CalendarWidgetStyle.TimeSlots; }
        public void ConfigureAsDayView() { Style = CalendarWidgetStyle.DateGrid; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMonthView", "Month View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsWeekView", "Week View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDayView", "Day View", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
