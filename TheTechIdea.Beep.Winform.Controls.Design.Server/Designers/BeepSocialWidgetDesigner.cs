using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepSocialWidgetDesigner : BaseWidgetDesigner
    {
        public BeepSocialWidget? SocialWidget => Component as BeepSocialWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepSocialWidgetActionList(this));
            return lists;
        }
    }

    public class BeepSocialWidgetActionList : DesignerActionList
    {
        private readonly BeepSocialWidgetDesigner _designer;

        public BeepSocialWidgetActionList(BeepSocialWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Social")]
        [Description("Visual style of the social widget")]
        public SocialWidgetStyle Style
        {
            get => _designer.GetProperty<SocialWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Social")]
        [Description("Title of the social widget")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        [Category("Social")]
        [Description("Subtitle of the social widget")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? string.Empty;
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Social")]
        [Description("Primary user name")]
        public string UserName
        {
            get => _designer.GetProperty<string>("UserName") ?? string.Empty;
            set => _designer.SetProperty("UserName", value);
        }

        [Category("Social")]
        [Description("Primary user role")]
        public string UserRole
        {
            get => _designer.GetProperty<string>("UserRole") ?? string.Empty;
            set => _designer.SetProperty("UserRole", value);
        }

        [Category("Social")]
        [Description("Primary user status")]
        public string UserStatus
        {
            get => _designer.GetProperty<string>("UserStatus") ?? string.Empty;
            set => _designer.SetProperty("UserStatus", value);
        }

        [Category("Social")]
        [Description("Show the status indicator")]
        public bool ShowStatus
        {
            get => _designer.GetProperty<bool>("ShowStatus");
            set => _designer.SetProperty("ShowStatus", value);
        }

        [Category("Social")]
        [Description("Show the user avatar")]
        public bool ShowAvatar
        {
            get => _designer.GetProperty<bool>("ShowAvatar");
            set => _designer.SetProperty("ShowAvatar", value);
        }

        public void ConfigureAsChatWidget()
        {
            Style = SocialWidgetStyle.ChatWidget;
            ShowAvatar = true;
            ShowStatus = true;
        }

        public void ConfigureAsCommentThread()
        {
            Style = SocialWidgetStyle.CommentThread;
            ShowAvatar = true;
            ShowStatus = false;
        }

        public void ConfigureAsActivityStream()
        {
            Style = SocialWidgetStyle.ActivityStream;
            ShowAvatar = true;
            ShowStatus = true;
        }

        public void ConfigureAsProfileCard()
        {
            Style = SocialWidgetStyle.ProfileCard;
            ShowAvatar = true;
            ShowStatus = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsChatWidget", "Chat Widget", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsCommentThread", "Comment Thread", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsActivityStream", "Activity Stream", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsProfileCard", "Profile Card", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle", "Properties"));
            items.Add(new DesignerActionPropertyItem("UserName", "User Name", "Properties"));
            items.Add(new DesignerActionPropertyItem("UserRole", "User Role", "Properties"));
            items.Add(new DesignerActionPropertyItem("UserStatus", "User Status", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowStatus", "Show Status", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowAvatar", "Show Avatar", "Properties"));
            return items;
        }
    }
}
