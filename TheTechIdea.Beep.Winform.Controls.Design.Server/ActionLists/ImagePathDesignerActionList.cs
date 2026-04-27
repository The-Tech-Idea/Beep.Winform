using System;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Interface implemented by designers that expose select/clear/embed image commands for smart tags.
    /// </summary>
    internal interface IImagePathDesignerHost
    {
        void SelectImage();
        void ClearImage();
        void EmbedImage();
        string GetImagePath();
        void SetImagePath(string value);
    }

    /// <summary>
    /// Shared action list that surfaces image-related commands on smart tags.
    /// Provides Select, Clear, and Embed actions for any control with ImagePath property.
    /// </summary>
    internal sealed class ImagePathDesignerActionList : DesignerActionList
    {
        private readonly IImagePathDesignerHost _host;

        public ImagePathDesignerActionList(ControlDesigner designer)
            : base(designer.Component)
        {
            if (designer is null)
            {
                throw new ArgumentNullException(nameof(designer));
            }

            _host = designer as IImagePathDesignerHost ?? throw new ArgumentException("Designer must implement IImagePathDesignerHost.", nameof(designer));
        }

        public string ImagePath
        {
            get => _host.GetImagePath() ?? string.Empty;
            set => _host.SetImagePath(value);
        }

        public void SelectImage() => _host.SelectImage();

        public void ClearImage() => _host.ClearImage();

        public void EmbedImage() => _host.EmbedImage();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection
            {
                new DesignerActionHeaderItem("Image"),
                new DesignerActionPropertyItem(nameof(ImagePath), "Image Path"),
                new DesignerActionMethodItem(this, nameof(SelectImage), "Select / Link Image..."),
                new DesignerActionMethodItem(this, nameof(EmbedImage), "Browse and Embed Image..."),
                new DesignerActionMethodItem(this, nameof(ClearImage), "Clear Image")
            };

            return items;
        }
    }
}