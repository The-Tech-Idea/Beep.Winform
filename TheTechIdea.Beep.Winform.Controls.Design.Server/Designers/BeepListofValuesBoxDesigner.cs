using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for <see cref="BeepListofValuesBox"/>.
    /// Provides smart-tag presets for the most common LOV configurations.
    /// </summary>
    public class BeepListofValuesBoxDesigner : BaseBeepControlDesigner
    {
        public BeepListofValuesBox? Lov => Component as BeepListofValuesBox;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepListofValuesBoxActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Smart-tag action list for <see cref="BeepListofValuesBox"/>.
    /// Surfaces the most-used design-time properties and preset configurations.
    /// </summary>
    public class BeepListofValuesBoxActionList : DesignerActionList
    {
        private readonly BeepListofValuesBoxDesigner _designer;

        public BeepListofValuesBoxActionList(BeepListofValuesBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepListofValuesBox? Lov => Component as BeepListofValuesBox;

        // ── Core ──────────────────────────────────────────────────────────

        [Category("LOV")]
        [Description("Title shown in the selection popup header.")]
        public string LovTitle
        {
            get => _designer.GetProperty<string>("LovTitle") ?? "Select Value";
            set => _designer.SetProperty("LovTitle", value);
        }

        [Category("LOV")]
        [Description("Label text displayed above the field.")]
        public string LabelText
        {
            get => _designer.GetProperty<string>("LabelText") ?? string.Empty;
            set => _designer.SetProperty("LabelText", value);
        }

        [Category("LOV")]
        [Description("Helper / hint text shown below the field.")]
        public string HelperText
        {
            get => _designer.GetProperty<string>("HelperText") ?? string.Empty;
            set => _designer.SetProperty("HelperText", value);
        }

        [Category("LOV")]
        [Description("Show a coloured pill badge next to the display value.")]
        public bool ShowKeyBadge
        {
            get => _designer.GetProperty<bool>("ShowKeyBadge");
            set => _designer.SetProperty("ShowKeyBadge", value);
        }

        [Category("LOV")]
        [Description("Maximum height of the selection popup (pixels).")]
        public int MaxPopupHeight
        {
            get => _designer.GetProperty<int>("MaxPopupHeight");
            set => _designer.SetProperty("MaxPopupHeight", value);
        }

        // ── Presets ───────────────────────────────────────────────────────

        [Description("Configure as a standard Employee lookup.")]
        public void ApplyEmployeePreset()
        {
            _designer.SetProperty("LovTitle",    "Select Employee");
            _designer.SetProperty("LabelText",   "Employee");
            _designer.SetProperty("HelperText",  "Type a name or ID, or press F9 to browse.");
            _designer.SetProperty("ShowKeyBadge", true);
            _designer.SetProperty("MaxPopupHeight", 360);
        }

        [Description("Configure as a compact key-only LOV (no badge, small popup).")]
        public void ApplyCompactPreset()
        {
            _designer.SetProperty("LovTitle",       "Select");
            _designer.SetProperty("LabelText",      string.Empty);
            _designer.SetProperty("HelperText",     string.Empty);
            _designer.SetProperty("ShowKeyBadge",   false);
            _designer.SetProperty("MaxPopupHeight", 240);
        }

        // ── Action item list ──────────────────────────────────────────────

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("LOV Identity"));
            items.Add(new DesignerActionPropertyItem(nameof(LovTitle),      "Title",        "LOV Identity"));
            items.Add(new DesignerActionPropertyItem(nameof(LabelText),     "Label Text",   "LOV Identity"));
            items.Add(new DesignerActionPropertyItem(nameof(HelperText),    "Helper Text",  "LOV Identity"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowKeyBadge),    "Show Key Badge",   "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(MaxPopupHeight),  "Max Popup Height", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyEmployeePreset), "Employee Preset", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyCompactPreset),  "Compact Preset",  "Presets", true));

            return items;
        }
    }
}
