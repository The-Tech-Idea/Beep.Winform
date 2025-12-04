using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepSwitch control
    /// Provides smart tags for style selection, icon configuration, and behavior presets
    /// </summary>
    public class BeepSwitchDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepSwitch? Switch => Component as BeepSwitch;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Select On Icon...", OnSelectOnIcon),
                        new DesignerVerb("Select Off Icon...", OnSelectOffIcon),
                        new DesignerVerb("Clear Icons", OnClearIcons),
                        new DesignerVerb("Toggle Switch", OnToggleSwitch)
                    };
                }
                return _verbs;
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepSwitchActionList(this));
            return lists;
        }

        private void OnSelectOnIcon(object? sender, EventArgs e)
        {
            SelectIcon("OnIconName");
        }

        private void OnSelectOffIcon(object? sender, EventArgs e)
        {
            SelectIcon("OffIconName");
        }

        private void SelectIcon(string propertyName)
        {
            if (Component == null) return;

            var property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null) return;

            var currentValue = property.GetValue(Component) as string;
            
            using (var dialog = new Editors.IconPickerDialog(currentValue))
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetProperty(propertyName, dialog.SelectedIconPath);
                }
            }
        }

        private void OnClearIcons(object? sender, EventArgs e)
        {
            SetProperty("OnIconName", string.Empty);
            SetProperty("OffIconName", string.Empty);
        }

        private void OnToggleSwitch(object? sender, EventArgs e)
        {
            if (Switch != null)
            {
                Switch.Checked = !Switch.Checked;
            }
        }
    }

    /// <summary>
    /// Action list for BeepSwitch smart tags
    /// Provides quick configuration options
    /// </summary>
    public class BeepSwitchActionList : DesignerActionList
    {
        private readonly BeepSwitchDesigner _designer;

        public BeepSwitchActionList(BeepSwitchDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        private BeepSwitch? Switch => Component as BeepSwitch;

        #region Properties (for smart tags)

        [Category("Appearance")]
        [Description("Label for the On state")]
        public string OnLabel
        {
            get => _designer.GetProperty<string>("OnLabel") ?? "On";
            set => _designer.SetProperty("OnLabel", value);
        }

        [Category("Appearance")]
        [Description("Label for the Off state")]
        public string OffLabel
        {
            get => _designer.GetProperty<string>("OffLabel") ?? "Off";
            set => _designer.SetProperty("OffLabel", value);
        }

        [Category("Behavior")]
        [Description("Whether the switch is checked (On)")]
        public bool Checked
        {
            get => _designer.GetProperty<bool>("Checked");
            set => _designer.SetProperty("Checked", value);
        }

        [Category("Appearance")]
        [Description("Switch orientation")]
        public SwitchOrientation Orientation
        {
            get => _designer.GetProperty<SwitchOrientation>("Orientation");
            set => _designer.SetProperty("Orientation", value);
        }

        [Category("Behavior")]
        [Description("Enable drag to toggle functionality")]
        public bool DragToToggleEnabled
        {
            get => _designer.GetProperty<bool>("DragToToggleEnabled");
            set => _designer.SetProperty("DragToToggleEnabled", value);
        }

        #endregion

        #region Icon Actions

        public void SelectOnIcon()
        {
            _designer.Verbs[0].Invoke(); // "Select On Icon..."
        }

        public void SelectOffIcon()
        {
            _designer.Verbs[1].Invoke(); // "Select Off Icon..."
        }

        public void UseCheckmarkIcons()
        {
            _designer.SetProperty("OnIconName", TheTechIdea.Beep.Icons.SvgsUI.Check);
            _designer.SetProperty("OffIconName", TheTechIdea.Beep.Icons.SvgsUI.X);
            _designer.SetProperty("OnLabel", "");
            _designer.SetProperty("OffLabel", "");
        }

        public void UsePowerIcons()
        {
            _designer.SetProperty("OnIconName", TheTechIdea.Beep.Icons.SvgsUI.Power);
            _designer.SetProperty("OffIconName", TheTechIdea.Beep.Icons.SvgsUI.Power);
            _designer.SetProperty("OnLabel", "");
            _designer.SetProperty("OffLabel", "");
        }

        public void UseToggleIcons()
        {
            _designer.SetProperty("OnIconName", TheTechIdea.Beep.Icons.SvgsUI.ToggleRight);
            _designer.SetProperty("OffIconName", TheTechIdea.Beep.Icons.SvgsUI.ToggleLeft);
            _designer.SetProperty("OnLabel", "");
            _designer.SetProperty("OffLabel", "");
        }

        public void UseLockIcons()
        {
            _designer.SetProperty("OnIconName", TheTechIdea.Beep.Icons.SvgsUI.Unlock);
            _designer.SetProperty("OffIconName", TheTechIdea.Beep.Icons.SvgsUI.Lock);
            _designer.SetProperty("OnLabel", "");
            _designer.SetProperty("OffLabel", "");
        }

        public void ClearIcons()
        {
            _designer.SetProperty("OnIconName", string.Empty);
            _designer.SetProperty("OffIconName", string.Empty);
        }

        #endregion

        #region Style Presets

        public void SetStyleToiOS() => _designer.SetStyle(BeepControlStyle.iOS15);
        public void SetStyleToMaterial3() => _designer.SetStyle(BeepControlStyle.Material3);
        public void SetStyleToFluent2() => _designer.SetStyle(BeepControlStyle.Fluent2);
        public void SetStyleToMinimal() => _designer.SetStyle(BeepControlStyle.Minimal);

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // State & Labels
            items.Add(new DesignerActionHeaderItem("State & Labels"));
            items.Add(new DesignerActionPropertyItem("Checked", "Checked (On)", "State & Labels"));
            items.Add(new DesignerActionPropertyItem("OnLabel", "On Label", "State & Labels"));
            items.Add(new DesignerActionPropertyItem("OffLabel", "Off Label", "State & Labels"));
            items.Add(new DesignerActionPropertyItem("Orientation", "Orientation", "State & Labels"));

            // Icon Selection
            items.Add(new DesignerActionHeaderItem("Icons"));
            items.Add(new DesignerActionMethodItem(this, "SelectOnIcon", "Select On Icon...", "Icons", true));
            items.Add(new DesignerActionMethodItem(this, "SelectOffIcon", "Select Off Icon...", "Icons", true));
            items.Add(new DesignerActionMethodItem(this, "ClearIcons", "Clear Icons", "Icons", false));

            // Icon Presets
            items.Add(new DesignerActionHeaderItem("Icon Presets"));
            items.Add(new DesignerActionMethodItem(this, "UseCheckmarkIcons", "✓ Checkmark Icons", "Icon Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UsePowerIcons", "⚡ Power Icons", "Icon Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseToggleIcons", "⇄ Toggle Icons", "Icon Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseLockIcons", "🔒 Lock Icons", "Icon Presets", false));

            // Style Presets
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToiOS", "iOS Style", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToMaterial3", "Material 3", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToFluent2", "Fluent 2", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetStyleToMinimal", "Minimal", "Style Presets", false));

            // Behavior
            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("DragToToggleEnabled", "Enable Drag to Toggle", "Behavior"));

            return items;
        }
    }
}

