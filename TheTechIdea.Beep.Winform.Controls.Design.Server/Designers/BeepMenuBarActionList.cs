// BeepMenuBarActionList.cs
// Phase 08 — Designer Integration.
//
// Smart-tag action list for BeepMenuBar. Surfaces the verbs and quick
// settings the user expects from a commercial-grade Visual Studio design
// experience:
//
//   Header "Items"
//     ▸ Load Sample Menu Items         (DesignerActionMethod)
//     ▸ Clear All Items                (DesignerActionMethod)
//     ▸ Item Count                     (read-only DesignerActionProperty)
//
//   Header "Style"
//     ▸ Set Material Style             (DesignerActionMethod)
//     ▸ Set Fluent Style               (DesignerActionMethod)
//     ▸ Set Office Style               (DesignerActionMethod)
//     ▸ Control Style                  (DesignerActionProperty)
//
//   Header "Layout"
//     ▸ Standard Height (44 px)        (DesignerActionMethod)
//     ▸ Compact Height (32 px)         (DesignerActionMethod)
//     ▸ Comfortable Height (56 px)     (DesignerActionMethod)
//     ▸ Height                         (DesignerActionProperty)
//
//   Header "Appearance"
//     ▸ Text Font                      (DesignerActionProperty)
//     ▸ Show Image                     (DesignerActionProperty)
//
// All mutations route through the designer's
// ExecuteAction / SetPropertyWithTransaction chokepoints so VS Undo /
// Redo gets descriptive entries.
//
// See .plans/Menus-Phase-08-DesignerIntegration.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Action list (smart-tag content) for <see cref="BeepMenuBar"/>.
    /// </summary>
    public sealed class BeepMenuBarActionList : DesignerActionList
    {
        private readonly BeepMenuBarDesigner _designer;

        public BeepMenuBarActionList(BeepMenuBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        /// <summary>Designer-hosted menubar, or <c>null</c> if not sited.</summary>
        private BeepMenuBar? MenuBar => _designer.MenuBar;

        // ────────────────────────────────────────────────────────────────
        // Properties surfaced through DesignerActionPropertyItem
        // ────────────────────────────────────────────────────────────────

        [Category("Items")]
        [Description("Number of top-level items currently in the menu bar.")]
        [ReadOnly(true)]
        public int ItemCount => MenuBar?.MenuItems?.Count ?? 0;

        [Category("Style")]
        [Description("Visual style of every top-level menu item.")]
        public BeepControlStyle ControlStyle
        {
            get => _designer.GetProperty<BeepControlStyle>(nameof(ControlStyle));
            set => _designer.SetPropertyWithTransaction(
                nameof(ControlStyle), value, $"Set ControlStyle = {value}");
        }

        [Category("Layout")]
        [Description("Height of the menu bar in pixels.")]
        public int Height
        {
            get => _designer.GetProperty<int>(nameof(Height));
            set => _designer.SetPropertyWithTransaction(
                nameof(Height), value, $"Set Height = {value}");
        }

        [Category("Appearance")]
        [Description("Font used to render the menu item text.")]
        public Font? TextFont
        {
            get => _designer.GetProperty<Font>(nameof(TextFont));
            set => _designer.SetPropertyWithTransaction(
                nameof(TextFont), value, "Set TextFont");
        }

        [Category("Appearance")]
        [Description("Show image to the left of each item's text when authored.")]
        public bool ShowImage
        {
            get => _designer.GetProperty<bool>(nameof(ShowImage));
            set => _designer.SetPropertyWithTransaction(
                nameof(ShowImage), value, value ? "Show item images" : "Hide item images");
        }

        // ────────────────────────────────────────────────────────────────
        // Methods surfaced through DesignerActionMethodItem
        //
        // Each runs inside an ExecuteAction transaction so the VS Edit
        // menu lists a descriptive Undo entry.
        // ────────────────────────────────────────────────────────────────

        /// <summary>Populates the menu with a representative sample tree.</summary>
        public void LoadSampleMenuItems()
        {
            _designer.ExecuteAction("Load Sample Menu Items", bar =>
            {
                bar.LoadSampleMenuItems();
            });
        }

        /// <summary>Removes every top-level item from the menu.</summary>
        public void ClearAllItems()
        {
            _designer.ExecuteAction("Clear All Menu Items", bar =>
            {
                bar.MenuItems?.Clear();
            });
        }

        /// <summary>Quick Style preset: Material.</summary>
        public void SetMaterialStyle() =>
            _designer.SetPropertyWithTransaction(
                nameof(ControlStyle), BeepControlStyle.Material3, "Set Style: Material 3");

        /// <summary>Quick Style preset: Fluent.</summary>
        public void SetFluentStyle() =>
            _designer.SetPropertyWithTransaction(
                nameof(ControlStyle), BeepControlStyle.Fluent2, "Set Style: Fluent 2");

        /// <summary>Quick Style preset: Office.</summary>
        public void SetOfficeStyle() =>
            _designer.SetPropertyWithTransaction(
                nameof(ControlStyle), BeepControlStyle.Office, "Set Style: Office");

        /// <summary>Quick height preset: Standard (44 px).</summary>
        public void SetStandardHeight() =>
            _designer.SetPropertyWithTransaction(nameof(Height), 44, "Set Height: Standard (44 px)");

        /// <summary>Quick height preset: Compact (32 px).</summary>
        public void SetCompactHeight() =>
            _designer.SetPropertyWithTransaction(nameof(Height), 32, "Set Height: Compact (32 px)");

        /// <summary>Quick height preset: Comfortable (56 px).</summary>
        public void SetComfortableHeight() =>
            _designer.SetPropertyWithTransaction(nameof(Height), 56, "Set Height: Comfortable (56 px)");

        // ────────────────────────────────────────────────────────────────
        // Smart-tag layout
        // ────────────────────────────────────────────────────────────────

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Items
            items.Add(new DesignerActionHeaderItem("Items"));
            items.Add(new DesignerActionMethodItem(this, nameof(LoadSampleMenuItems),
                "Load Sample Items", "Items",
                "Populate the bar with a representative sample tree.", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllItems),
                "Clear All Items", "Items",
                "Remove every top-level item from the menu.", true));
            items.Add(new DesignerActionPropertyItem(nameof(ItemCount),
                "Item Count", "Items",
                "Read-only count of top-level items."));

            // Style
            items.Add(new DesignerActionHeaderItem("Style"));
            items.Add(new DesignerActionMethodItem(this, nameof(SetMaterialStyle),
                "Set Material 3 Style", "Style", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SetFluentStyle),
                "Set Fluent 2 Style", "Style", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SetOfficeStyle),
                "Set Office Style", "Style", true));
            items.Add(new DesignerActionPropertyItem(nameof(ControlStyle),
                "Control Style", "Style",
                "Visual style applied to each top-level item."));

            // Layout
            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionMethodItem(this, nameof(SetStandardHeight),
                "Standard (44 px)", "Layout", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SetCompactHeight),
                "Compact (32 px)", "Layout", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SetComfortableHeight),
                "Comfortable (56 px)", "Layout", true));
            items.Add(new DesignerActionPropertyItem(nameof(Height),
                "Height", "Layout",
                "Height of the menu bar in pixels."));

            // Appearance
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(TextFont),
                "Text Font", "Appearance",
                "Font used for the item labels."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowImage),
                "Show Image", "Appearance",
                "Toggle the per-item image column."));

            return items;
        }
    }
}
