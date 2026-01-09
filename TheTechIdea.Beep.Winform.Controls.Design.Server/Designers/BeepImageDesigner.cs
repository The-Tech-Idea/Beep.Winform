using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Images.Helpers;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepImage control
    /// Provides smart tags for image configuration and clipping
    /// </summary>
    public class BeepImageDesigner : BaseBeepControlDesigner
    {
        public BeepImage? Image => Component as BeepImage;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepImageActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepImage smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepImageActionList : DesignerActionList
    {
        private readonly BeepImageDesigner _designer;

        public BeepImageActionList(BeepImageDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepImage? Image => Component as BeepImage;

        #region Properties (for smart tags)

        [Category("Appearance")]
        [Description("Clip shape for the image")]
        public ImageClipShape ClipShape
        {
            get => _designer.GetProperty<ImageClipShape>("ClipShape");
            set => _designer.SetProperty("ClipShape", value);
        }

        [Category("Appearance")]
        [Description("Corner radius for rounded shapes")]
        public float CornerRadius
        {
            get => _designer.GetProperty<float>("CornerRadius");
            set => _designer.SetProperty("CornerRadius", value);
        }

        [Category("Appearance")]
        [Description("Use region-based clipping for better performance")]
        public bool UseRegionClipping
        {
            get => _designer.GetProperty<bool>("UseRegionClipping");
            set => _designer.SetProperty("UseRegionClipping", value);
        }

        [Category("Appearance")]
        [Description("Image path")]
        public string ImagePath
        {
            get => _designer.GetProperty<string>("ImagePath");
            set => _designer.SetProperty("ImagePath", value);
        }

        [Category("Appearance")]
        [Description("Image embedding context")]
        public ImageEmbededin ImageEmbededin
        {
            get => _designer.GetProperty<ImageEmbededin>("ImageEmbededin");
            set => _designer.SetProperty("ImageEmbededin", value);
        }

        [Category("Effects")]
        [Description("Opacity")]
        public float Opacity
        {
            get => _designer.GetProperty<float>("Opacity");
            set => _designer.SetProperty("Opacity", value);
        }

        [Category("Effects")]
        [Description("Grayscale")]
        public bool Grayscale
        {
            get => _designer.GetProperty<bool>("Grayscale");
            set => _designer.SetProperty("Grayscale", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Apply Circle clip shape
        /// </summary>
        public void ApplyCircleShape()
        {
            ClipShape = ImageClipShape.Circle;
        }

        /// <summary>
        /// Apply RoundedRect clip shape
        /// </summary>
        public void ApplyRoundedRectShape()
        {
            ClipShape = ImageClipShape.RoundedRect;
        }

        /// <summary>
        /// Apply Diamond clip shape
        /// </summary>
        public void ApplyDiamondShape()
        {
            ClipShape = ImageClipShape.Diamond;
        }

        /// <summary>
        /// Apply Triangle clip shape
        /// </summary>
        public void ApplyTriangleShape()
        {
            ClipShape = ImageClipShape.Triangle;
        }

        /// <summary>
        /// Apply Hexagon clip shape
        /// </summary>
        public void ApplyHexagonShape()
        {
            ClipShape = ImageClipShape.Hexagon;
        }

        /// <summary>
        /// Remove clipping
        /// </summary>
        public void RemoveClipping()
        {
            ClipShape = ImageClipShape.None;
        }

        /// <summary>
        /// Enable region-based clipping
        /// </summary>
        public void EnableRegionClipping()
        {
            UseRegionClipping = true;
        }

        /// <summary>
        /// Set recommended corner radius
        /// </summary>
        public void SetRecommendedCornerRadius()
        {
            if (Image != null)
            {
                CornerRadius = ImageClipHelpers.GetRecommendedCornerRadius(Image.Bounds);
            }
        }

        #endregion

        #region DesignerActionItemCollection

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Header
            items.Add(new DesignerActionHeaderItem("Clipping"));
            items.Add(new DesignerActionPropertyItem("ClipShape", "Clip Shape", "Clipping", "Shape used to clip the image"));
            items.Add(new DesignerActionPropertyItem("CornerRadius", "Corner Radius", "Clipping", "Corner radius for rounded shapes"));
            items.Add(new DesignerActionPropertyItem("UseRegionClipping", "Use Region Clipping", "Clipping", "Use region-based clipping for better performance"));

            // Shape presets
            items.Add(new DesignerActionHeaderItem("Shape Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyCircleShape", "Circle Shape", "Shape Presets", "Apply circle clipping"));
            items.Add(new DesignerActionMethodItem(this, "ApplyRoundedRectShape", "Rounded Rectangle", "Shape Presets", "Apply rounded rectangle clipping"));
            items.Add(new DesignerActionMethodItem(this, "ApplyDiamondShape", "Diamond Shape", "Shape Presets", "Apply diamond clipping"));
            items.Add(new DesignerActionMethodItem(this, "ApplyTriangleShape", "Triangle Shape", "Shape Presets", "Apply triangle clipping"));
            items.Add(new DesignerActionMethodItem(this, "ApplyHexagonShape", "Hexagon Shape", "Shape Presets", "Apply hexagon clipping"));
            items.Add(new DesignerActionMethodItem(this, "RemoveClipping", "Remove Clipping", "Shape Presets", "Remove all clipping"));

            // Appearance
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("ImagePath", "Image Path", "Appearance", "Path to the image file"));
            items.Add(new DesignerActionPropertyItem("ImageEmbededin", "Image Embedded In", "Appearance", "Context where image is embedded"));
            items.Add(new DesignerActionPropertyItem("Opacity", "Opacity", "Appearance", "Image opacity"));

            // Effects
            items.Add(new DesignerActionHeaderItem("Effects"));
            items.Add(new DesignerActionPropertyItem("Grayscale", "Grayscale", "Effects", "Convert image to grayscale"));

            // Quick actions
            items.Add(new DesignerActionHeaderItem("Quick Actions"));
            items.Add(new DesignerActionMethodItem(this, "EnableRegionClipping", "Enable Region Clipping", "Quick Actions", "Enable region-based clipping for better performance"));
            items.Add(new DesignerActionMethodItem(this, "SetRecommendedCornerRadius", "Set Recommended Corner Radius", "Quick Actions", "Set corner radius based on image size"));

            return items;
        }

        #endregion
    }
}
