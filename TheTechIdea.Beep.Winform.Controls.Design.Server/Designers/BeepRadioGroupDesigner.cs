using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepRadioGroup control
    /// </summary>
    public class BeepRadioGroupDesigner : BaseBeepControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepRadioGroupActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepRadioGroup smart tags
    /// Provides quick access to common radio group properties and render style presets
    /// </summary>
    public class BeepRadioGroupActionList : DesignerActionList
    {
        private readonly BeepRadioGroupDesigner _designer;

        public BeepRadioGroupActionList(BeepRadioGroupDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepRadioGroup? RadioGroup => Component as BeepRadioGroup;

        #region Properties

        [Category("Radio Group")]
        [Description("Visual render style of the radio group")]
        public RadioGroupRenderStyle RadioGroupStyle
        {
            get => _designer.GetProperty<RadioGroupRenderStyle>("RadioGroupStyle");
            set => _designer.SetProperty("RadioGroupStyle", value);
        }

        [Category("Behavior")]
        [Description("Allow multiple items to be selected")]
        public bool AllowMultipleSelection
        {
            get => _designer.GetProperty<bool>("AllowMultipleSelection");
            set => _designer.SetProperty("AllowMultipleSelection", value);
        }

        [Category("Appearance")]
        [Description("Control style")]
        public BeepControlStyle Style
        {
            get => _designer.GetProperty<BeepControlStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Appearance")]
        [Description("Use theme colors")]
        public bool UseThemeColors
        {
            get => _designer.GetProperty<bool>("UseThemeColors");
            set => _designer.SetProperty("UseThemeColors", value);
        }

        #endregion

        #region Actions

        public void ApplyMaterialStyle()
        {
            RadioGroupStyle = RadioGroupRenderStyle.Material;
        }

        public void ApplyCardStyle()
        {
            RadioGroupStyle = RadioGroupRenderStyle.Card;
        }

        public void ApplyChipStyle()
        {
            RadioGroupStyle = RadioGroupRenderStyle.Chip;
        }

        public void ApplyButtonStyle()
        {
            RadioGroupStyle = RadioGroupRenderStyle.Button;
        }

        public void ApplySegmentedStyle()
        {
            RadioGroupStyle = RadioGroupRenderStyle.Segmented;
        }

        public void UseRecommendedLayout()
        {
            if (RadioGroup != null)
            {
                var style = RadioGroup.RadioGroupStyle;
                // Could set recommended spacing, padding, etc. based on style
            }
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Radio Group"));
            items.Add(new DesignerActionPropertyItem("RadioGroupStyle", "Render Style:", "Radio Group"));
            items.Add(new DesignerActionPropertyItem("AllowMultipleSelection", "Allow Multiple Selection:", "Radio Group"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Style", "Control Style:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("UseThemeColors", "Use Theme Colors:", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMaterialStyle", "Material Design", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ApplyCardStyle", "Card Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyChipStyle", "Chip Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyButtonStyle", "Button Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplySegmentedStyle", "Segmented Style", "Style Presets", false));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionMethodItem(this, "UseRecommendedLayout", "Use Recommended Layout", "Layout", true));

            return items;
        }
    }
}
