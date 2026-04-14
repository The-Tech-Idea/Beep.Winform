using System;
using System.ComponentModel;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Common smart-tag actions shared by Beep controls that inherit the base designers.
    /// </summary>
    public class CommonBeepControlActionList : DesignerActionList
    {
        private readonly IBeepDesignerActionHost _designer;

        public CommonBeepControlActionList(IBeepDesignerActionHost designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BaseControl? BeepControl => Component as BaseControl;

        [Category("Style")]
        [Description("The visual style of the control")]
        public BeepControlStyle ControlStyle
        {
            get => _designer.GetProperty<BeepControlStyle>("ControlStyle");
            set => _designer.SetProperty("ControlStyle", value);
        }

        [Category("Theme")]
        [Description("Use theme colors instead of style colors")]
        public bool UseThemeColors
        {
            get => _designer.GetProperty<bool>("UseThemeColors");
            set => _designer.SetProperty("UseThemeColors", value);
        }

        [Category("Painting")]
        [Description("Use the BeepStyling system for painting")]
        public bool UseFormStylePaint
        {
            get => _designer.GetProperty<bool>("UseFormStylePaint");
            set => _designer.SetProperty("UseFormStylePaint", value);
        }

        public void ApplyTheme()
        {
            _designer.ApplyTheme();
        }

        public void SelectStyle()
        {
            var dialog = new StyleSelectorDialog(ControlStyle);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ControlStyle = dialog.SelectedStyle;
            }
        }

        public void SetStyleToMaterial3() => ControlStyle = BeepControlStyle.Material3;
        public void SetStyleToiOS15() => ControlStyle = BeepControlStyle.iOS15;
        public void SetStyleToFluent2() => ControlStyle = BeepControlStyle.Fluent2;
        public void SetStyleToMinimal() => ControlStyle = BeepControlStyle.Minimal;
        public void SetStyleToBrutalist() => ControlStyle = BeepControlStyle.Brutalist;
        public void SetStyleToNeumorphism() => ControlStyle = BeepControlStyle.Neumorphism;

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Visual Style"));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectStyle), "Select Style...", "Visual Style", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SetStyleToMaterial3), "Material 3", "Visual Style", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SetStyleToiOS15), "iOS 15", "Visual Style", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SetStyleToFluent2), "Fluent 2", "Visual Style", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SetStyleToMinimal), "Minimal", "Visual Style", false));

            items.Add(new DesignerActionHeaderItem("Theme"));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyTheme), "Apply Current Theme", "Theme", true));
            items.Add(new DesignerActionPropertyItem(nameof(UseThemeColors), "Use Theme Colors", "Theme"));

            items.Add(new DesignerActionHeaderItem("Painting"));
            items.Add(new DesignerActionPropertyItem(nameof(UseFormStylePaint), "Use Style Painting", "Painting"));

            return items;
        }
    }
}