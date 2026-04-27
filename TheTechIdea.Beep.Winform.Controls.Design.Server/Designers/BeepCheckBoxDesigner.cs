using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;
using CheckBoxStyle = TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers.CheckBoxStyle;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepCheckBox control
    /// Provides smart tags for checkbox configuration and styling
    /// </summary>
    public class BeepCheckBoxDesigner : BaseBeepControlDesigner, IImagePathDesignerHost
    {
        public BeepCheckBox<bool>? CheckBox => Component as BeepCheckBox<bool>;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new ImagePathDesignerActionList(this));
            lists.Add(new BeepCheckBoxActionList(this));
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

    /// <summary>
    /// Action list for BeepCheckBox smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepCheckBoxActionList : DesignerActionList
    {
        private readonly BeepCheckBoxDesigner _designer;

        public BeepCheckBoxActionList(BeepCheckBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepCheckBox<bool>? CheckBox => Component as BeepCheckBox<bool>;

        #region Properties (for smart tags)

        [Category("Appearance")]
        [Description("Checkbox visual style")]
        public CheckBoxStyle CheckBoxStyle
        {
            get => _designer.GetProperty<CheckBoxStyle>("CheckBoxStyle");
            set => _designer.SetProperty("CheckBoxStyle", value);
        }

        [Category("Appearance")]
        [Description("Text displayed next to the checkbox")]
        public string Text
        {
            get => _designer.GetProperty<string>("Text");
            set => _designer.SetProperty("Text", value);
        }

        [Category("Layout")]
        [Description("Size of the checkbox in pixels")]
        public int CheckBoxSize
        {
            get => _designer.GetProperty<int>("CheckBoxSize");
            set => _designer.SetProperty("CheckBoxSize", value);
        }

        [Category("Layout")]
        [Description("Spacing between checkbox and text")]
        public int Spacing
        {
            get => _designer.GetProperty<int>("Spacing");
            set => _designer.SetProperty("Spacing", value);
        }

        [Category("Appearance")]
        [Description("Hide text label")]
        public bool HideText
        {
            get => _designer.GetProperty<bool>("HideText");
            set => _designer.SetProperty("HideText", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Apply Material3 style preset
        /// </summary>
        public void ApplyMaterial3Style()
        {
            ApplyStylePreset(CheckBoxStyle.Material3);
        }

        /// <summary>
        /// Apply Modern style preset
        /// </summary>
        public void ApplyModernStyle()
        {
            ApplyStylePreset(CheckBoxStyle.Modern);
        }

        /// <summary>
        /// Apply Classic style preset
        /// </summary>
        public void ApplyClassicStyle()
        {
            ApplyStylePreset(CheckBoxStyle.Classic);
        }

        /// <summary>
        /// Apply Minimal style preset
        /// </summary>
        public void ApplyMinimalStyle()
        {
            ApplyStylePreset(CheckBoxStyle.Minimal);
        }

        /// <summary>
        /// Apply iOS style preset
        /// </summary>
        public void ApplyiOSStyle()
        {
            ApplyStylePreset(CheckBoxStyle.iOS);
        }

        /// <summary>
        /// Apply Fluent2 style preset
        /// </summary>
        public void ApplyFluent2Style()
        {
            ApplyStylePreset(CheckBoxStyle.Fluent2);
        }

        /// <summary>
        /// Apply Switch style preset
        /// </summary>
        public void ApplySwitchStyle()
        {
            ApplyStylePreset(CheckBoxStyle.Switch);
        }

        /// <summary>
        /// Apply Button style preset
        /// </summary>
        public void ApplyButtonStyle()
        {
            ApplyStylePreset(CheckBoxStyle.Button);
        }

        private void ApplyStylePreset(CheckBoxStyle style)
        {
            CheckBoxStyle = style;
            CheckBoxSize = CheckBoxStyleHelpers.GetRecommendedCheckBoxSize(style);
            Spacing = CheckBoxStyleHelpers.GetRecommendedSpacing(style);
        }

        /// <summary>
        /// Set recommended checkbox size for current style
        /// </summary>
        public void SetRecommendedCheckBoxSize()
        {
            if (CheckBox != null)
            {
                ApplyStylePreset(CheckBox.CheckBoxStyle);
            }
        }

        #endregion

        #region DesignerActionItemCollection

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Header
            items.Add(new DesignerActionHeaderItem("Style"));
            items.Add(new DesignerActionPropertyItem("CheckBoxStyle", "CheckBox Style", "Style", "Visual style of the checkbox"));

            // Style presets
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMaterial3Style", "Material3 Style", "Style Presets", "Apply Material Design 3 style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyModernStyle", "Modern Style", "Style Presets", "Apply modern flat design style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyClassicStyle", "Classic Style", "Style Presets", "Apply classic bordered style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMinimalStyle", "Minimal Style", "Style Presets", "Apply minimal clean style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyiOSStyle", "iOS Style", "Style Presets", "Apply iOS-style rounded design"));
            items.Add(new DesignerActionMethodItem(this, "ApplyFluent2Style", "Fluent2 Style", "Style Presets", "Apply Fluent Design 2 style"));
            items.Add(new DesignerActionMethodItem(this, "ApplySwitchStyle", "Switch Style", "Style Presets", "Apply switch-style toggle appearance"));
            items.Add(new DesignerActionMethodItem(this, "ApplyButtonStyle", "Button Style", "Style Presets", "Apply button-style appearance"));

            // Appearance
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Text", "Text", "Appearance", "Text label"));
            items.Add(new DesignerActionPropertyItem("HideText", "Hide Text", "Appearance", "Hide text label"));

            // Layout
            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("CheckBoxSize", "CheckBox Size", "Layout", "Size of the checkbox"));
            items.Add(new DesignerActionPropertyItem("Spacing", "Spacing", "Layout", "Spacing between checkbox and text"));

            // Quick actions
            items.Add(new DesignerActionHeaderItem("Quick Actions"));
            items.Add(new DesignerActionMethodItem(this, "SetRecommendedCheckBoxSize", "Set Recommended Size", "Quick Actions", "Set checkbox size and spacing based on current style"));

            return items;
        }

        #endregion
    }
}
