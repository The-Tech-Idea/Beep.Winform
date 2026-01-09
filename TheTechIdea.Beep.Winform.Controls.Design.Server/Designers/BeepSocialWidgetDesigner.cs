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

        public void ConfigureAsChatWidget() { Style = SocialWidgetStyle.MessageCard; }
        public void ConfigureAsCommentThread() { Style = SocialWidgetStyle.CommentThread; }
        public void ConfigureAsActivityStream() { Style = SocialWidgetStyle.ActivityStream; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsChatWidget", "Chat Widget", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsCommentThread", "Comment Thread", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsActivityStream", "Activity Stream", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
