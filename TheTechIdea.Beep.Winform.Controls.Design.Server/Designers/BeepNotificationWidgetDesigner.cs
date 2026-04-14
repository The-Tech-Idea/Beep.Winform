using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepNotificationWidgetDesigner : BaseWidgetDesigner
    {
        public BeepNotificationWidget? NotificationWidget => Component as BeepNotificationWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepNotificationWidgetActionList(this));
            return lists;
        }
    }

    public class BeepNotificationWidgetActionList : DesignerActionList
    {
        private readonly BeepNotificationWidgetDesigner _designer;

        public BeepNotificationWidgetActionList(BeepNotificationWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Notification")]
        [Description("Visual style of the notification widget")]
        public NotificationWidgetStyle Style
        {
            get => _designer.GetProperty<NotificationWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Notification")]
        [Description("Type of notification")]
        public NotificationType NotificationType
        {
            get => _designer.GetProperty<NotificationType>("NotificationType");
            set => _designer.SetProperty("NotificationType", value);
        }

        [Category("Notification")]
        [Description("Title of the notification")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        [Category("Notification")]
        [Description("Message content")]
        public string Message
        {
            get => _designer.GetProperty<string>("Message") ?? string.Empty;
            set => _designer.SetProperty("Message", value);
        }

        [Category("Notification")]
        [Description("Show an action button")]
        public bool ShowAction
        {
            get => _designer.GetProperty<bool>("ShowAction");
            set => _designer.SetProperty("ShowAction", value);
        }

        [Category("Notification")]
        [Description("Label of the action button")]
        public string ActionText
        {
            get => _designer.GetProperty<string>("ActionText") ?? string.Empty;
            set => _designer.SetProperty("ActionText", value);
        }

        [Category("Notification")]
        [Description("Show the notification icon")]
        public bool ShowIcon
        {
            get => _designer.GetProperty<bool>("ShowIcon");
            set => _designer.SetProperty("ShowIcon", value);
        }

        [Category("Notification")]
        [Description("Allow the notification to be dismissed")]
        public bool IsDismissible
        {
            get => _designer.GetProperty<bool>("IsDismissible");
            set => _designer.SetProperty("IsDismissible", value);
        }

        [Category("Notification")]
        [Description("Progress value used by progress-style notifications")]
        public int Progress
        {
            get => _designer.GetProperty<int>("Progress");
            set => _designer.SetProperty("Progress", value);
        }

        public void ConfigureAsToastNotification()
        {
            Style = NotificationWidgetStyle.ToastNotification;
            ShowIcon = true;
            ShowAction = false;
            IsDismissible = true;
        }

        public void ConfigureAsAlertBanner()
        {
            Style = NotificationWidgetStyle.AlertBanner;
            ShowIcon = true;
            ShowAction = true;
            ActionText = "Review";
            IsDismissible = false;
        }

        public void ConfigureAsProgressAlert()
        {
            Style = NotificationWidgetStyle.ProgressAlert;
            NotificationType = NotificationType.Progress;
            ShowIcon = true;
            ShowAction = false;
            IsDismissible = false;
            Progress = 35;
        }

        public void ConfigureAsSuccessBanner()
        {
            Style = NotificationWidgetStyle.SuccessBanner;
            NotificationType = NotificationType.Success;
            ShowIcon = true;
            ShowAction = false;
            IsDismissible = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsToastNotification", "Toast Notification", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsAlertBanner", "Alert Banner", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsProgressAlert", "Progress Alert", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSuccessBanner", "Success Banner", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("NotificationType", "Notification Type", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Message", "Message", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowAction", "Show Action", "Properties"));
            items.Add(new DesignerActionPropertyItem("ActionText", "Action Text", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowIcon", "Show Icon", "Properties"));
            items.Add(new DesignerActionPropertyItem("IsDismissible", "Is Dismissible", "Properties"));
            items.Add(new DesignerActionPropertyItem("Progress", "Progress", "Properties"));
            return items;
        }
    }
}
