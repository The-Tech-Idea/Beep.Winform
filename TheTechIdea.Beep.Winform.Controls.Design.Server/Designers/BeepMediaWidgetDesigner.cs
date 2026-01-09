using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepMediaWidgetDesigner : BaseWidgetDesigner
    {
        public BeepMediaWidget? MediaWidget => Component as BeepMediaWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMediaWidgetActionList(this));
            return lists;
        }
    }

    public class BeepMediaWidgetActionList : DesignerActionList
    {
        private readonly BeepMediaWidgetDesigner _designer;

        public BeepMediaWidgetActionList(BeepMediaWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Media")]
        [Description("Visual style of the media widget")]
        public MediaWidgetStyle Style
        {
            get => _designer.GetProperty<MediaWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Media")]
        [Description("Title of the media widget")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsImageCard() { Style = MediaWidgetStyle.ImageCard; }
        public void ConfigureAsMediaGallery() { Style = MediaWidgetStyle.MediaGallery; }
        public void ConfigureAsAvatarList() { Style = MediaWidgetStyle.AvatarGroup; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsImageCard", "Image Card", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMediaGallery", "Media Gallery", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsAvatarList", "Avatar List", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
