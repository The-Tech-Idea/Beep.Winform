using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepMediaWidgetDesigner : BaseWidgetDesigner, IImagePathDesignerHost
    {
        public BeepMediaWidget? MediaWidget => Component as BeepMediaWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new ImagePathDesignerActionList(this));
            lists.Add(new BeepMediaWidgetActionList(this));
            return lists;
        }

        public void SelectImage()
        {
            if (Component == null) return;

            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));
            var currentPath = GetImagePath();

            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly, currentPath);
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
                if (!string.IsNullOrEmpty(newValue))
                {
                    SetImagePath(newValue);
                }
            }
        }

        public void ClearImage()
            => SetImagePath(string.Empty);

        public void EmbedImage()
        {
            if (Component == null) return;

            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));
            var currentPath = GetImagePath();

            using var dialog = new BeepImagePickerDialog(null, embed: true, serviceProvider, Component.GetType().Assembly, currentPath);
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK && !dialog.SelectionResult.IsCancelled)
            {
                var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
                if (!string.IsNullOrEmpty(newValue))
                {
                    SetImagePath(newValue);
                }
            }
        }

        public string GetImagePath()
            => GetProperty<string>("ImagePath") ?? string.Empty;

        public void SetImagePath(string value)
            => SetProperty("ImagePath", value ?? string.Empty);
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

        [Category("Media")]
        [Description("Subtitle text displayed with the media item")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? string.Empty;
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Media")]
        [Description("Path to the primary media image")]
        public string ImagePath
        {
            get => _designer.GetProperty<string>("ImagePath") ?? string.Empty;
            set => _designer.SetProperty("ImagePath", value);
        }

        [Category("Media")]
        [Description("Show overlay content on top of media")]
        public bool ShowOverlay
        {
            get => _designer.GetProperty<bool>("ShowOverlay");
            set => _designer.SetProperty("ShowOverlay", value);
        }

        [Category("Media")]
        [Description("Text displayed in the media overlay")]
        public string OverlayText
        {
            get => _designer.GetProperty<string>("OverlayText") ?? string.Empty;
            set => _designer.SetProperty("OverlayText", value);
        }

        public void ConfigureAsImageCard()
        {
            Style = MediaWidgetStyle.ImageCard;
            ShowOverlay = true;
        }

        public void ConfigureAsMediaGallery()
        {
            Style = MediaWidgetStyle.MediaGallery;
            ShowOverlay = false;
        }

        public void ConfigureAsAvatarList()
        {
            Style = MediaWidgetStyle.AvatarList;
            ShowOverlay = false;
        }

        public void ConfigureAsProfileCard()
        {
            Style = MediaWidgetStyle.ProfileCard;
            ShowOverlay = false;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsImageCard", "Image Card", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMediaGallery", "Media Gallery", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsAvatarList", "Avatar List", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsProfileCard", "Profile Card", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle", "Properties"));
            items.Add(new DesignerActionPropertyItem("ImagePath", "Image Path", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowOverlay", "Show Overlay", "Properties"));
            items.Add(new DesignerActionPropertyItem("OverlayText", "Overlay Text", "Properties"));
            return items;
        }
    }
}
