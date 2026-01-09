using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Toggle;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepToggle control
    /// Similar to BeepSwitch but with toggle-specific features
    /// </summary>
    public class BeepToggleDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepToggle? Toggle => Component as BeepToggle;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Select Icon...", OnSelectIcon),
                        new DesignerVerb("Clear Icon", OnClearIcon),
                        new DesignerVerb("Toggle State", OnToggle)
                    };
                }
                return _verbs;
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepToggleActionList(this));
            return lists;
        }

        private void OnSelectIcon(object? sender, EventArgs e)
        {
            if (Component == null) return;

            var property = TypeDescriptor.GetProperties(Component)["IconName"];
            if (property == null) return;

            var currentValue = property.GetValue(Component) as string;
            
            using (var dialog = new Editors.IconPickerDialog(currentValue))
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetProperty("IconName", dialog.SelectedIconPath);
                }
            }
        }

        private void OnClearIcon(object? sender, EventArgs e)
        {
            SetProperty("IconName", string.Empty);
        }

        private void OnToggle(object? sender, EventArgs e)
        {
            if (Toggle != null)
            {
                var currentValue = GetProperty<object>("Value");
                // Toggle implementation depends on value type
            }
        }
    }

    /// <summary>
    /// Action list for BeepToggle smart tags
    /// Provides quick access to common toggle properties and style presets
    /// </summary>
    public class BeepToggleActionList : DesignerActionList
    {
        private readonly BeepToggleDesigner _designer;

        public BeepToggleActionList(BeepToggleDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepToggle? Toggle => Component as BeepToggle;

        #region Properties

        [Category("Toggle")]
        [Description("Whether the toggle is ON")]
        public bool IsOn
        {
            get => _designer.GetProperty<bool>("IsOn");
            set => _designer.SetProperty("IsOn", value);
        }

        [Category("Toggle")]
        [Description("Visual style of the toggle")]
        public ToggleStyle ToggleStyle
        {
            get => _designer.GetProperty<ToggleStyle>("ToggleStyle");
            set => _designer.SetProperty("ToggleStyle", value);
        }

        [Category("Toggle")]
        [Description("Text to display when ON")]
        public string OnText
        {
            get => _designer.GetProperty<string>("OnText") ?? "ON";
            set => _designer.SetProperty("OnText", value);
        }

        [Category("Toggle")]
        [Description("Text to display when OFF")]
        public string OffText
        {
            get => _designer.GetProperty<string>("OffText") ?? "OFF";
            set => _designer.SetProperty("OffText", value);
        }

        [Category("Toggle")]
        [Description("Show ON/OFF labels")]
        public bool ShowLabels
        {
            get => _designer.GetProperty<bool>("ShowLabels");
            set => _designer.SetProperty("ShowLabels", value);
        }

        [Category("Appearance")]
        [Description("Color when toggle is ON")]
        public Color OnColor
        {
            get => _designer.GetProperty<Color>("OnColor");
            set => _designer.SetProperty("OnColor", value);
        }

        [Category("Appearance")]
        [Description("Color when toggle is OFF")]
        public Color OffColor
        {
            get => _designer.GetProperty<Color>("OffColor");
            set => _designer.SetProperty("OffColor", value);
        }

        [Category("Behavior")]
        [Description("Enable smooth animation")]
        public bool AnimateTransition
        {
            get => _designer.GetProperty<bool>("AnimateTransition");
            set => _designer.SetProperty("AnimateTransition", value);
        }

        [Category("Behavior")]
        [Description("Animation duration in milliseconds")]
        public int AnimationDuration
        {
            get => _designer.GetProperty<int>("AnimationDuration");
            set => _designer.SetProperty("AnimationDuration", value);
        }

        #endregion

        #region Actions

        public void ToggleState()
        {
            if (Toggle != null)
            {
                IsOn = !IsOn;
            }
        }

        public void SelectIcon()
        {
            _designer.Verbs[0].Invoke();
        }

        public void UseCheckmarkIcon()
        {
            _designer.SetProperty("OnIconPath", TheTechIdea.Beep.Icons.SvgsUI.Check);
            _designer.SetProperty("OffIconPath", TheTechIdea.Beep.Icons.SvgsUI.X);
        }

        public void UseHeartIcon()
        {
            _designer.SetProperty("OnIconPath", TheTechIdea.Beep.Icons.SvgsUI.Heart);
            _designer.SetProperty("OffIconPath", TheTechIdea.Beep.Icons.SvgsUI.HeartOff ?? TheTechIdea.Beep.Icons.SvgsUI.Heart);
        }

        public void UseLockIcon()
        {
            _designer.SetProperty("OnIconPath", TheTechIdea.Beep.Icons.SvgsUI.Lock);
            _designer.SetProperty("OffIconPath", TheTechIdea.Beep.Icons.SvgsUI.Unlock);
        }

        public void UseEyeIcon()
        {
            _designer.SetProperty("OnIconPath", TheTechIdea.Beep.Icons.SvgsUI.Eye);
            _designer.SetProperty("OffIconPath", TheTechIdea.Beep.Icons.SvgsUI.EyeOff);
        }

        public void ApplyClassicStyle()
        {
            ToggleStyle = ToggleStyle.Classic;
        }

        public void ApplyMaterialStyle()
        {
            ToggleStyle = ToggleStyle.MaterialPill;
        }

        public void ApplyIOSStyle()
        {
            ToggleStyle = ToggleStyle.iOS;
        }

        public void ApplyMinimalStyle()
        {
            ToggleStyle = ToggleStyle.Minimal;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Toggle"));
            items.Add(new DesignerActionPropertyItem("IsOn", "Is On:", "Toggle"));
            items.Add(new DesignerActionPropertyItem("ToggleStyle", "Style:", "Toggle"));
            items.Add(new DesignerActionPropertyItem("OnText", "ON Text:", "Toggle"));
            items.Add(new DesignerActionPropertyItem("OffText", "OFF Text:", "Toggle"));
            items.Add(new DesignerActionPropertyItem("ShowLabels", "Show Labels:", "Toggle"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("OnColor", "ON Color:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("OffColor", "OFF Color:", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("AnimateTransition", "Animate:", "Behavior"));
            items.Add(new DesignerActionPropertyItem("AnimationDuration", "Duration (ms):", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyClassicStyle", "Classic Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ApplyMaterialStyle", "Material Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyIOSStyle", "iOS Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyMinimalStyle", "Minimal Style", "Style Presets", false));

            items.Add(new DesignerActionHeaderItem("Icons"));
            items.Add(new DesignerActionMethodItem(this, "SelectIcon", "Select Icon...", "Icons", true));
            items.Add(new DesignerActionMethodItem(this, "UseCheckmarkIcon", "✓ Checkmark", "Icons", false));
            items.Add(new DesignerActionMethodItem(this, "UseHeartIcon", "❤️ Heart", "Icons", false));
            items.Add(new DesignerActionMethodItem(this, "UseLockIcon", "🔒 Lock", "Icons", false));
            items.Add(new DesignerActionMethodItem(this, "UseEyeIcon", "👁️ Eye", "Icons", false));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, "ToggleState", "Toggle State", "Actions", true));

            return items;
        }
    }
}

