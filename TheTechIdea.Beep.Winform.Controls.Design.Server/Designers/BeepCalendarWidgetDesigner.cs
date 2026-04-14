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

        [Category("Calendar")]
        [Description("Subtitle shown beneath the calendar title")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? string.Empty;
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Calendar")]
        [Description("Select the overall calendar view mode")]
        public CalendarViewMode ViewMode
        {
            get => _designer.GetProperty<CalendarViewMode>("ViewMode");
            set => _designer.SetProperty("ViewMode", value);
        }

        [Category("Calendar")]
        [Description("Show or hide weekend days")]
        public bool ShowWeekends
        {
            get => _designer.GetProperty<bool>("ShowWeekends");
            set => _designer.SetProperty("ShowWeekends", value);
        }

        [Category("Calendar")]
        [Description("Highlight today's date")]
        public bool ShowToday
        {
            get => _designer.GetProperty<bool>("ShowToday");
            set => _designer.SetProperty("ShowToday", value);
        }

        [Category("Calendar")]
        [Description("Show or hide event rendering")]
        public bool ShowEvents
        {
            get => _designer.GetProperty<bool>("ShowEvents");
            set => _designer.SetProperty("ShowEvents", value);
        }

        [Category("Calendar")]
        [Description("Allow selecting multiple dates")]
        public bool AllowMultiSelect
        {
            get => _designer.GetProperty<bool>("AllowMultiSelect");
            set => _designer.SetProperty("AllowMultiSelect", value);
        }

        [Category("Calendar")]
        [Description("Start of the working-hours range")]
        public int WorkingHoursStart
        {
            get => _designer.GetProperty<int>("WorkingHoursStart");
            set => _designer.SetProperty("WorkingHoursStart", value);
        }

        [Category("Calendar")]
        [Description("End of the working-hours range")]
        public int WorkingHoursEnd
        {
            get => _designer.GetProperty<int>("WorkingHoursEnd");
            set => _designer.SetProperty("WorkingHoursEnd", value);
        }

        public void ConfigureAsMonthView()
        {
            Style = CalendarWidgetStyle.CalendarView;
            ViewMode = CalendarViewMode.Month;
            ShowWeekends = true;
            ShowToday = true;
            ShowEvents = true;
            AllowMultiSelect = false;
        }

        public void ConfigureAsWeekView()
        {
            Style = CalendarWidgetStyle.WeekView;
            ViewMode = CalendarViewMode.Week;
            ShowWeekends = false;
            ShowToday = true;
            ShowEvents = true;
            AllowMultiSelect = false;
            WorkingHoursStart = 8;
            WorkingHoursEnd = 18;
        }

        public void ConfigureAsDayView()
        {
            Style = CalendarWidgetStyle.DateGrid;
            ViewMode = CalendarViewMode.Day;
            ShowWeekends = true;
            ShowToday = true;
            ShowEvents = true;
            AllowMultiSelect = false;
            WorkingHoursStart = 8;
            WorkingHoursEnd = 18;
        }

        public void ConfigureAsDatePicker()
        {
            Style = CalendarWidgetStyle.DatePicker;
            ViewMode = CalendarViewMode.Month;
            ShowEvents = false;
            ShowToday = true;
            AllowMultiSelect = false;
        }

        public void ConfigureAsAvailabilityPlanner()
        {
            Style = CalendarWidgetStyle.AvailabilityGrid;
            ViewMode = CalendarViewMode.Week;
            ShowWeekends = false;
            ShowToday = true;
            ShowEvents = false;
            AllowMultiSelect = true;
            WorkingHoursStart = 8;
            WorkingHoursEnd = 17;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMonthView", "Month View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsWeekView", "Week View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDayView", "Day View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDatePicker", "Date Picker", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsAvailabilityPlanner", "Availability Planner", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle", "Properties"));
            items.Add(new DesignerActionPropertyItem("ViewMode", "View Mode", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowWeekends", "Show Weekends", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowToday", "Show Today", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowEvents", "Show Events", "Properties"));
            items.Add(new DesignerActionPropertyItem("AllowMultiSelect", "Allow Multi Select", "Properties"));
            items.Add(new DesignerActionPropertyItem("WorkingHoursStart", "Working Hours Start", "Properties"));
            items.Add(new DesignerActionPropertyItem("WorkingHoursEnd", "Working Hours End", "Properties"));
            return items;
        }
    }
}
