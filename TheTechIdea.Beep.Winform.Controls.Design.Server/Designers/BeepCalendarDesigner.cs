using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepCalendar control
    /// </summary>
    public class BeepCalendarDesigner : BaseBeepControlDesigner
    {
        public BeepCalendar? Calendar => Component as BeepCalendar;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepCalendarActionList(this));
            return lists;
        }
    }

    public class BeepCalendarActionList : DesignerActionList
    {
        private readonly BeepCalendarDesigner _designer;

        public BeepCalendarActionList(BeepCalendarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Appearance")]
        public bool ShowWeekNumbers
        {
            get => _designer.GetProperty<bool>("ShowWeekNumbers");
            set => _designer.SetProperty("ShowWeekNumbers", value);
        }

        [Category("Appearance")]
        public bool ShowTodayButton
        {
            get => _designer.GetProperty<bool>("ShowTodayButton");
            set => _designer.SetProperty("ShowTodayButton", value);
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowWeekNumbers", "Show Week Numbers", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowTodayButton", "Show Today Button", "Appearance"));

            return items;
        }
    }
}

