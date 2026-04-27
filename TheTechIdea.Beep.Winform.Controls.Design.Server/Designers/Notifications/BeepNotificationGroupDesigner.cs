using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Notifications;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.Notifications
{
    public class BeepNotificationGroupDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepNotificationGroup? NotificationGroup => Component as BeepNotificationGroup;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Toggle Expand", OnToggleExpand),
                        new DesignerVerb("Add Sample Notification", OnAddSample),
                        new DesignerVerb("Clear Notifications", OnClear)
                    };
                }
                return _verbs;
            }
        }

        public override DesignerVerbCollection Verbs => CustomVerbs;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepNotificationGroupActionList(this));
            return lists;
        }

        private void OnToggleExpand(object? sender, EventArgs e) => ToggleExpand();
        private void OnAddSample(object? sender, EventArgs e) => AddSampleNotification();
        private void OnClear(object? sender, EventArgs e) => ClearNotifications();

        public void ToggleExpand()
        {
            if (NotificationGroup == null) return;
            NotificationGroup.ToggleExpand();
        }

        public void AddSampleNotification()
        {
            if (NotificationGroup == null) return;
            var data = new global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationData
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Sample Notification",
                Message = "This is a sample notification message",
                Type = global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info,
                Timestamp = DateTime.Now
            };
            NotificationGroup.AddNotification(data);
        }

        public void ClearNotifications()
        {
            if (NotificationGroup == null) return;
            NotificationGroup.Clear();
        }
    }

    public class BeepNotificationGroupActionList : DesignerActionList
    {
        private readonly BeepNotificationGroupDesigner _designer;

        public BeepNotificationGroupActionList(BeepNotificationGroupDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepNotificationGroup? NotificationGroup => Component as BeepNotificationGroup;

        #region Properties

        [Category("Data")]
        [Description("Unique key for grouping notifications")]
        public string GroupKey
        {
            get => _designer.GetProperty<string>("GroupKey") ?? "";
            set => _designer.SetProperty("GroupKey", value);
        }

        [Category("Appearance")]
        [Description("Title displayed for the notification group")]
        public string GroupTitle
        {
            get => _designer.GetProperty<string>("GroupTitle") ?? "Notifications";
            set => _designer.SetProperty("GroupTitle", value);
        }

        [Category("Appearance")]
        [Description("Notification type for the group")]
        public global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType GroupType
        {
            get => _designer.GetProperty<global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType>("GroupType");
            set => _designer.SetProperty("GroupType", value);
        }

        [Category("Layout")]
        [Description("Minimum touch target width in pixels")]
        public int MinTouchTargetWidth
        {
            get => _designer.GetProperty<int>("MinTouchTargetWidth");
            set => _designer.SetProperty("MinTouchTargetWidth", value);
        }

        [Browsable(false)]
        public bool IsExpanded
        {
            get => _designer.GetProperty<bool>("IsExpanded");
            set => _designer.SetProperty("IsExpanded", value);
        }

        [Browsable(false)]
        public int Count
        {
            get => NotificationGroup?.Count ?? 0;
        }

        #endregion

        #region Actions

        public void ToggleExpand() => _designer.ToggleExpand();
        public void AddSampleNotification() => _designer.AddSampleNotification();
        public void ClearNotifications() => _designer.ClearNotifications();

        public void SetTypeInfo() => GroupType = global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Info;
        public void SetTypeSuccess() => GroupType = global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Success;
        public void SetTypeWarning() => GroupType = global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Warning;
        public void SetTypeError() => GroupType = global::TheTechIdea.Beep.Winform.Controls.Notifications.NotificationType.Error;

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Group"));
            items.Add(new DesignerActionPropertyItem("GroupKey", "Group Key:", "Group"));
            items.Add(new DesignerActionPropertyItem("GroupTitle", "Group Title:", "Group"));
            items.Add(new DesignerActionPropertyItem("GroupType", "Group Type:", "Group"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("MinTouchTargetWidth", "Min Touch Width:", "Layout"));

            items.Add(new DesignerActionHeaderItem("Type Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetTypeInfo", "Info", "Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetTypeSuccess", "Success", "Type Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetTypeWarning", "Warning", "Type Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetTypeError", "Error", "Type Presets", false));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, "ToggleExpand", "Toggle Expand", "Actions", true));
            items.Add(new DesignerActionMethodItem(this, "AddSampleNotification", "Add Sample Notification", "Actions", false));
            items.Add(new DesignerActionMethodItem(this, "ClearNotifications", "Clear Notifications", "Actions", false));

            return items;
        }
    }
}
