using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using BeepProgressBarStyle = TheTechIdea.Beep.Winform.Controls.ProgressBars.ProgressBarStyle;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepProgressBar control
    /// Provides smart tags for style presets and animation configuration
    /// </summary>
    public class BeepProgressBarDesigner : BaseBeepControlDesigner
    {
        public BeepProgressBar? ProgressBar => Component as BeepProgressBar;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepProgressBarActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepProgressBar smart tags
    /// Provides quick style presets and common property access
    /// </summary>
    public class BeepProgressBarActionList : DesignerActionList
    {
        private readonly BeepProgressBarDesigner _designer;

        public BeepProgressBarActionList(BeepProgressBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepProgressBar? ProgressBar => Component as BeepProgressBar;

        #region Properties (for smart tags)

        [Category("Progress")]
        [Description("Current progress value")]
        public int Value
        {
            get => _designer.GetProperty<int>("Value");
            set => _designer.SetProperty("Value", value);
        }

        [Category("Progress")]
        [Description("Minimum value")]
        public int Minimum
        {
            get => _designer.GetProperty<int>("Minimum");
            set => _designer.SetProperty("Minimum", value);
        }

        [Category("Progress")]
        [Description("Maximum value")]
        public int Maximum
        {
            get => _designer.GetProperty<int>("Maximum");
            set => _designer.SetProperty("Maximum", value);
        }

        [Category("Progress")]
        [Description("Step increment")]
        public int Step
        {
            get => _designer.GetProperty<int>("Step");
            set => _designer.SetProperty("Step", value);
        }

        [Category("Appearance")]
        [Description("Progress bar style")]
        public BeepProgressBarStyle ProgressBarStyle
        {
            get => _designer.GetProperty<BeepProgressBarStyle>("ProgressBarStyle");
            set => _designer.SetProperty("ProgressBarStyle", value);
        }

        [Category("Animation")]
        [Description("Animate value changes")]
        public bool AnimateValueChanges
        {
            get => _designer.GetProperty<bool>("AnimateValueChanges");
            set => _designer.SetProperty("AnimateValueChanges", value);
        }

        [Category("Animation")]
        [Description("Show glow effect")]
        public bool ShowGlowEffect
        {
            get => _designer.GetProperty<bool>("ShowGlowEffect");
            set => _designer.SetProperty("ShowGlowEffect", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Set progress to 0%
        /// </summary>
        public void SetProgressToZero()
        {
            Value = 0;
        }

        /// <summary>
        /// Set progress to 25%
        /// </summary>
        public void SetProgressToQuarter()
        {
            var max = Maximum;
            Value = max / 4;
        }

        /// <summary>
        /// Set progress to 50%
        /// </summary>
        public void SetProgressToHalf()
        {
            var max = Maximum;
            Value = max / 2;
        }

        /// <summary>
        /// Set progress to 75%
        /// </summary>
        public void SetProgressToThreeQuarters()
        {
            var max = Maximum;
            Value = (max * 3) / 4;
        }

        /// <summary>
        /// Set progress to 100%
        /// </summary>
        public void SetProgressToComplete()
        {
            Value = Maximum;
        }

        /// <summary>
        /// Configure as linear progress bar
        /// </summary>
        public void ConfigureAsLinear()
        {
            ProgressBarStyle = BeepProgressBarStyle.Gradient;
            AnimateValueChanges = true;
        }

        /// <summary>
        /// Configure as circular progress bar
        /// </summary>
        public void ConfigureAsCircular()
        {
            // Note: This would require checking if BeepProgressBar supports circular style
            // For now, just set a modern style
            _designer.SetProperty("Style", BeepControlStyle.Material3);
        }

        /// <summary>
        /// Enable animation effects
        /// </summary>
        public void EnableAnimation()
        {
            AnimateValueChanges = true;
            ShowGlowEffect = true;
        }

        /// <summary>
        /// Disable animation effects
        /// </summary>
        public void DisableAnimation()
        {
            AnimateValueChanges = false;
            ShowGlowEffect = false;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick progress presets
            items.Add(new DesignerActionHeaderItem("Quick Progress"));
            items.Add(new DesignerActionMethodItem(this, "SetProgressToZero", "Set to 0%", "Quick Progress", true));
            items.Add(new DesignerActionMethodItem(this, "SetProgressToQuarter", "Set to 25%", "Quick Progress", true));
            items.Add(new DesignerActionMethodItem(this, "SetProgressToHalf", "Set to 50%", "Quick Progress", true));
            items.Add(new DesignerActionMethodItem(this, "SetProgressToThreeQuarters", "Set to 75%", "Quick Progress", true));
            items.Add(new DesignerActionMethodItem(this, "SetProgressToComplete", "Set to 100%", "Quick Progress", true));

            // Style configuration
            items.Add(new DesignerActionHeaderItem("Style Configuration"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsLinear", "Linear Style", "Style Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "EnableAnimation", "Enable Animation", "Style Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "DisableAnimation", "Disable Animation", "Style Configuration", true));

            // Progress properties
            items.Add(new DesignerActionHeaderItem("Progress Properties"));
            items.Add(new DesignerActionPropertyItem("Value", "Value", "Progress Properties"));
            items.Add(new DesignerActionPropertyItem("Minimum", "Minimum", "Progress Properties"));
            items.Add(new DesignerActionPropertyItem("Maximum", "Maximum", "Progress Properties"));
            items.Add(new DesignerActionPropertyItem("Step", "Step", "Progress Properties"));

            // Appearance properties
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("ProgressBarStyle", "Progress Bar Style", "Appearance"));

            // Animation properties
            items.Add(new DesignerActionHeaderItem("Animation"));
            items.Add(new DesignerActionPropertyItem("AnimateValueChanges", "Animate Value Changes", "Animation"));
            items.Add(new DesignerActionPropertyItem("ShowGlowEffect", "Show Glow Effect", "Animation"));

            return items;
        }
    }
}
