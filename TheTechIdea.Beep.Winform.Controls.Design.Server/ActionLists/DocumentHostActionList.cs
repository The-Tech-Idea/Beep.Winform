// DocumentHostActionList.cs
// Smart-tag action list for BeepDocumentHost.
// Exposes all major properties and design-time document actions.
// Supports nested splits (Sprint 19).
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;
using System.ComponentModel.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Designer smart-tag action list for <see cref="BeepDocumentHost"/>.
    /// Provides the full set of tab, appearance, history, session, split,
    /// preview, cross-host-drag properties plus the most useful document actions.
    /// </summary>
    public class DocumentHostActionList : DesignerActionList
    {
        private readonly BeepDocumentHostDesigner _designer;

        public DocumentHostActionList(BeepDocumentHostDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        // ── convenience accessor ──────────────────────────────────────────────
        private BeepDocumentHost? Host => Component as BeepDocumentHost;

        // ─────────────────────────────────────────────────────────────────────
        // Tabs  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("Tabs")]
        [Description("Visual rendering style of the tab strip (Chrome / VSCode / Underline / Pill / Office / Flat).")]
        public DocumentTabStyle TabStyle
        {
            get => _designer.GetProperty<DocumentTabStyle>("TabStyle");
            set => _designer.SetProperty("TabStyle", value);
        }

        [Category("Tabs")]
        [Description("Position of the tab strip relative to the document panels (Top / Bottom / Left / Right / Hidden).")]
        public TabStripPosition TabPosition
        {
            get => _designer.GetProperty<TabStripPosition>("TabPosition");
            set => _designer.SetProperty("TabPosition", value);
        }

        [Category("Tabs")]
        [Description("Controls when the close (×) button is visible: Always, OnHover, or Never.")]
        public TabCloseMode CloseMode
        {
            get => _designer.GetProperty<TabCloseMode>("CloseMode");
            set => _designer.SetProperty("CloseMode", value);
        }

        [Category("Tabs")]
        [Description("Show the new-document (+) button at the end of the tab strip.")]
        public bool ShowAddButton
        {
            get => _designer.GetProperty<bool>("ShowAddButton");
            set => _designer.SetProperty("ShowAddButton", value);
        }

        [Category("Tabs")]
        [Description("How per-document tab colours are rendered: None, AccentBar, FullBackground, or BottomBorder.")]
        public TabColorMode TabColorMode
        {
            get => _designer.GetProperty<TabColorMode>("TabColorMode");
            set => _designer.SetProperty("TabColorMode", value);
        }

        [Category("Tabs")]
        [Description("Enable built-in keyboard shortcuts (Ctrl+Tab, Ctrl+W, Ctrl+1-9) on the tab strip.")]
        public bool KeyboardShortcutsEnabled
        {
            get => _designer.GetProperty<bool>("KeyboardShortcutsEnabled");
            set => _designer.SetProperty("KeyboardShortcutsEnabled", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Appearance  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("Appearance")]
        [Description("Border/elevation style of the document host container: Flat, Thin, or Raised.")]
        public DocumentHostStyle ControlStyle
        {
            get => _designer.GetProperty<DocumentHostStyle>("ControlStyle");
            set => _designer.SetProperty("ControlStyle", value);
        }

        [Category("Appearance")]
        [Description("Beep theme name propagated to the tab strip and all document panels.")]
        public string ThemeName
        {
            get => _designer.GetProperty<string>("ThemeName") ?? string.Empty;
            set => _designer.SetProperty("ThemeName", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Split View  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("Split View")]
        [Description("Maximum number of side-by-side document groups allowed (1–8).")]
        public int MaxGroups
        {
            get => _designer.GetProperty<int>("MaxGroups");
            set => _designer.SetProperty("MaxGroups", value);
        }

        [Category("Split View")]
        [Description("True = side-by-side (horizontal) split; False = top/bottom (vertical) split.")]
        public bool SplitHorizontal
        {
            get => _designer.GetProperty<bool>("SplitHorizontal");
            set => _designer.SetProperty("SplitHorizontal", value);
        }

        [Category("Split View")]
        [Description("Ratio of host space assigned to the first group (0.1 – 0.9). Default 0.5 = equal split.")]
        public float SplitRatio
        {
            get => _designer.GetProperty<float>("SplitRatio");
            set => _designer.SetProperty("SplitRatio", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Nested Splits  group (Sprint 19)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Read-only summary of the current layout tree structure.
        /// Shows the nesting hierarchy and group count at design time.
        /// </summary>
        [Category("Nested Splits")]
        [Description("Current layout tree structure (read-only). Shows nesting hierarchy and group count.")]
        public string LayoutTreeInfo
        {
            get
            {
                if (Host == null) return "No host";
                var root = Host.LayoutRoot;
                if (root == null) return "No layout tree";
                return FormatLayoutNode(root, 0);
            }
        }

        /// <summary>
        /// Per-group tab strip positions for asymmetric layouts.
        /// Format: "GroupId=Position;..." — edit to change individual group tab positions.
        /// </summary>
        [Category("Nested Splits")]
        [Description("Per-group tab strip positions. Format: GroupId=Position;...")]
        public string GroupTabPositions
        {
            get
            {
                if (Host == null) return string.Empty;
                var parts = new System.Collections.Generic.List<string>();
                foreach (var grp in Host.Groups)
                {
                    parts.Add($"{grp.GroupId.Substring(0, 8)}={grp.TabPosition}");
                }
                return string.Join("; ", parts);
            }
            set
            {
                if (Host == null) return;
                var entries = value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    var kv = entry.Split('=');
                    if (kv.Length == 2 && Enum.TryParse<TabStripPosition>(kv[1].Trim(), true, out var pos))
                    {
                        var prefix = kv[0].Trim();
                        foreach (var grp in Host.Groups)
                        {
                            if (grp.GroupId.StartsWith(prefix))
                            {
                                Host.SetGroupTabPosition(grp.GroupId, pos);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static string FormatLayoutNode(TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout.ILayoutNode node, int depth)
        {
            var indent = new string(' ', depth * 2);
            if (node is TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout.GroupLayoutNode g)
            {
                return $"{indent}Group({g.DocumentIds.Count} docs){(g.SelectedDocumentId != null ? $" [{g.SelectedDocumentId}]" : "")}";
            }
            if (node is TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout.SplitLayoutNode s)
            {
                var orient = s.Orientation == System.Windows.Forms.Orientation.Horizontal ? "H" : "V";
                return $"{indent}Split({orient}, {s.Ratio:P0})\n{FormatLayoutNode(s.First, depth + 1)}\n{FormatLayoutNode(s.Second, depth + 1)}";
            }
            return $"{indent}Unknown";
        }

        // ─────────────────────────────────────────────────────────────────────
        // Preview  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("Preview")]
        [Description("Show a panel thumbnail inside the tab rich-tooltip on hover.")]
        public bool TabPreviewEnabled
        {
            get => _designer.GetProperty<bool>("TabPreviewEnabled");
            set => _designer.SetProperty("TabPreviewEnabled", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // History  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("History")]
        [Description("Maximum number of recently-used documents tracked for MRU navigation (Ctrl+Tab popup).")]
        public int MaxRecentHistory
        {
            get => _designer.GetProperty<int>("MaxRecentHistory");
            set => _designer.SetProperty("MaxRecentHistory", value);
        }

        [Category("History")]
        [Description("Maximum number of recently-closed documents available for Reopen Closed Tab (Ctrl+Shift+T).")]
        public int MaxClosedHistory
        {
            get => _designer.GetProperty<int>("MaxClosedHistory");
            set => _designer.SetProperty("MaxClosedHistory", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cross-Host Drag  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("Cross-Host Drag")]
        [Description("Allow tabs to be dragged from this host to another BeepDocumentHost in the same process.")]
        public bool AllowDragBetweenHosts
        {
            get => _designer.GetProperty<bool>("AllowDragBetweenHosts");
            set => _designer.SetProperty("AllowDragBetweenHosts", value);
        }

        // ─────────────────────────────────────────────────────────────────
        // Tab Sizing  group (proxied from BeepDocumentTabStrip)
        // ─────────────────────────────────────────────────────────────────

        [Category("Tab Sizing")]
        [Description("Controls how individual tabs are sized: Auto (content-based), Fixed (equal FixedTabWidth), or Fill (stretch to strip width).")]
        public TheTechIdea.Beep.Winform.Controls.DocumentHost.TabSizeMode TabSizeMode
        {
            get => _designer.GetProperty<TheTechIdea.Beep.Winform.Controls.DocumentHost.TabSizeMode>("TabSizeMode");
            set => _designer.SetProperty("TabSizeMode", value);
        }

        [Category("Tab Sizing")]
        [Description("Fixed pixel width for every tab when TabSizeMode = Fixed.")]
        public int FixedTabWidth
        {
            get => _designer.GetProperty<int>("FixedTabWidth");
            set => _designer.SetProperty("FixedTabWidth", value);
        }

        // ─────────────────────────────────────────────────────────────────
        // Interaction  group (proxied from BeepDocumentTabStrip)
        // ─────────────────────────────────────────────────────────────────

        [Category("Interaction")]
        [Description("Controls what information is displayed in tab tooltips: None, Title, TitleAndPath, or Full.")]
        public TabTooltipMode TabTooltipMode
        {
            get => _designer.GetProperty<TabTooltipMode>("TabTooltipMode");
            set => _designer.SetProperty("TabTooltipMode", value);
        }

        [Category("Interaction")]
        [Description("Allow the user to drag a tab into a free-floating window.")]
        public bool AllowDragFloat
        {
            get => _designer.GetProperty<bool>("AllowDragFloat");
            set => _designer.SetProperty("AllowDragFloat", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Policies  group (P7-002 / P7-005)
        // ─────────────────────────────────────────────────────────────────────

        [Category("Policies")]
        [Description("Allow documents to be floated into their own window via drag or context menu.")]
        public bool AllowFloat
        {
            get => _designer.GetProperty<bool>("AllowFloat");
            set => _designer.SetProperty("AllowFloat", value);
        }

        [Category("Policies")]
        [Description("Allow documents to be split into side-by-side tab groups.")]
        public bool AllowSplit
        {
            get => _designer.GetProperty<bool>("AllowSplit");
            set => _designer.SetProperty("AllowSplit", value);
        }

        [Category("Policies")]
        [Description("Allow documents to be pinned to the left of the tab strip.")]
        public bool AllowPin
        {
            get => _designer.GetProperty<bool>("AllowPin");
            set => _designer.SetProperty("AllowPin", value);
        }

        [Category("Policies")]
        [Description("Allow documents to be minimised to auto-hide side strips.")]
        public bool AllowAutoHide
        {
            get => _designer.GetProperty<bool>("AllowAutoHide");
            set => _designer.SetProperty("AllowAutoHide", value);
        }

        [Category("Policies")]
        [Description("Maximum nesting depth for split groups (1 = no splits allowed).")]
        public int MaxSplitDepth
        {
            get => _designer.GetProperty<int>("MaxSplitDepth");
            set => _designer.SetProperty("MaxSplitDepth", value);
        }

        [Category("Animation")]
        [Description("Hover delay (ms) before an auto-hide strip tab reveals its flyout. 0 = click-only.")]
        public int AutoHideHoverDelay
        {
            get => _designer.GetProperty<int>("AutoHideHoverDelay");
            set => _designer.SetProperty("AutoHideHoverDelay", value);
        }

        /// <summary>Removes every document and resets the host to a blank state at design time.</summary>
        public void ClearAllDocuments()
            => _designer.CloseAllDesignTimeDocuments();

        /// <summary>Copies the current design-time layout snapshot JSON to the clipboard.</summary>
        public void SaveLayoutSnapshot()
        {
            if (Host == null) return;
            var json = Host.SaveLayout();
            System.Windows.Forms.Clipboard.SetText(json);
            System.Windows.Forms.MessageBox.Show(
                "Layout snapshot copied to clipboard.",
                "Save Layout Snapshot",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Session  group
        // ─────────────────────────────────────────────────────────────────────

        [Category("Session")]
        [Description("Automatically save and restore the tab layout using SessionFile.")]
        public bool AutoSaveLayout
        {
            get => _designer.GetProperty<bool>("AutoSaveLayout");
            set => _designer.SetProperty("AutoSaveLayout", value);
        }

        [Category("Session")]
        [Description("File path for the automatic layout save/restore (AutoSaveLayout must be true).")]
        public string SessionFile
        {
            get => _designer.GetProperty<string>("SessionFile") ?? string.Empty;
            set => _designer.SetProperty("SessionFile", value);
        }

        [Category("Session")]
        [Description("Enable context-aware routed command execution for vNext command service APIs.")]
        public bool EnableRoutedCommands
        {
            get => _designer.GetProperty<bool>("EnableRoutedCommands");
            set => _designer.SetProperty("EnableRoutedCommands", value);
        }

        [Category("Session")]
        [Description("Enable transaction-scoped docking operations for split/dock mutations.")]
        public bool EnableTransactionalDocking
        {
            get => _designer.GetProperty<bool>("EnableTransactionalDocking");
            set => _designer.SetProperty("EnableTransactionalDocking", value);
        }

        [Category("Session")]
        [Description("Enable host telemetry event emission for command, docking, and restore flows.")]
        public bool EnableHostTelemetry
        {
            get => _designer.GetProperty<bool>("EnableHostTelemetry");
            set => _designer.SetProperty("EnableHostTelemetry", value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Document Actions
        // ─────────────────────────────────────────────────────────────────────

        [Category("Documents")]
        [Description("Title of the currently active design-time document surface.")]
        public string ActiveDocumentTitle
        {
            get => _designer.GetActiveDocumentTitle();
            set => _designer.SetActiveDocumentTitle(value);
        }

        /// <summary>Adds a placeholder document at design time.</summary>
        public void AddNewDocument()
            => _designer.AddDesignTimeDocument();

        /// <summary>Closes the currently active document.</summary>
        public void CloseActiveDocument()
            => _designer.CloseActiveDesignTimeDocument();

        /// <summary>Closes all documents.</summary>
        public void CloseAllDocuments()
            => _designer.CloseAllDesignTimeDocuments();

        /// <summary>
        /// Reopens the most recently closed document (equivalent to Ctrl+Shift+T).
        /// </summary>
        public void ReopenLastClosed()
            => _designer.ReopenLastClosedDesignTimeDocument();

        /// <summary>Selects the active document panel so toolbox drops target that surface.</summary>
        public void SelectActiveDocumentSurface()
            => _designer.SelectActiveDocumentSurface();

        /// <summary>Opens the Quick-Switch document picker popup.</summary>
        public void ShowQuickSwitch()
        {
            Host?.ShowQuickSwitch();
        }

        /// <summary>Floats the active document into its own window.</summary>
        public void FloatActiveDocument()
            => _designer.FloatActiveDesignTimeDocument();

        /// <summary>Pins the active document (icon-only, no close).</summary>
        public void PinActiveDocument()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.SetActiveDocumentPinned(true);
        }

        /// <summary>Unpins the active document.</summary>
        public void UnpinActiveDocument()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.SetActiveDocumentPinned(false);
        }

        /// <summary>Creates a new side-by-side design surface and activates it.</summary>
        public void SplitActiveHorizontal()
            => _designer.CreateSplitDesignTimeDocument(horizontal: true);

        /// <summary>Creates a new stacked design surface and activates it.</summary>
        public void SplitActiveVertical()
            => _designer.CreateSplitDesignTimeDocument(horizontal: false);

        /// <summary>Opens the guided layout assistant dialog.</summary>
        public void OpenLayoutAssistant()
            => _designer.ShowLayoutAssistant();

        // ─────────────────────────────────────────────────────────────────────        // Design-time Actions
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the DesignTimeDocuments collection editor from the smart-tag.
        /// </summary>
        public void EditDesignTimeDocuments()
            => _designer.OpenDesignTimeDocumentsEditor();

        /// <summary>
        /// Opens the layout preset picker and applies the chosen multi-group template.
        /// Supports nested splits (Sprint 19).
        /// </summary>
        public void ApplyLayoutPreset()
        {
            if (Host == null) return;
            using var dlg = new LayoutPresetPickerDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _designer.ApplyDesignTimeLayoutPreset(dlg.SelectedPreset);
        }

        /// <summary>
        /// Collapses all split groups back to a single group, moving their
        /// documents into the primary group.
        /// </summary>
        public void MergeAllGroups()
            => _designer.MergeAllDesignTimeGroups();

        // ─────────────────────────────────────────────────────────────────        // Style preset shortcuts
        // ─────────────────────────────────────────────────────────────────────

        public void UseChromStyle()     => TabStyle = DocumentTabStyle.Chrome;
        public void UseVSCodeStyle()    => TabStyle = DocumentTabStyle.VSCode;
        public void UseFlatStyle()      => TabStyle = DocumentTabStyle.Flat;
        public void UseOfficeStyle()    => TabStyle = DocumentTabStyle.Office;
        public void UsePillStyle()      => TabStyle = DocumentTabStyle.Pill;
        public void UseUnderlineStyle() => TabStyle = DocumentTabStyle.Underline;

        // ─────────────────────────────────────────────────────────────────────
        // Theme picker
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the visual theme picker dialog and applies the chosen theme.
        /// </summary>
        public void ChooseTheme()
        {
            if (Host == null) return;
            using var dlg = new ThemePickerDialog(ThemeName);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && !string.IsNullOrEmpty(dlg.SelectedThemeName))
            {
                ThemeName = dlg.SelectedThemeName;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // GetSortedActionItems
        // ─────────────────────────────────────────────────────────────────────

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // ── Documents ────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Documents"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddNewDocument),
                "Add New Document", "Documents",
                "Adds a persisted design-time document surface and selects it.", true));
            items.Add(new DesignerActionMethodItem(this, nameof(CloseActiveDocument),
                "Close Active Document", "Documents",
                "Closes the currently active document.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(CloseAllDocuments),
                "Close All Documents", "Documents",
                "Removes every document from the host.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ReopenLastClosed),
                "Reopen Last Closed", "Documents",
                "Reopens the most recently closed design-time document.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectActiveDocumentSurface),
                "Select Active Document Surface", "Documents",
                "Targets the active document panel for toolbox drops and direct authoring.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowQuickSwitch),
                "Quick Switch\u2026", "Documents",
                "Opens the Quick-Switch document picker popup.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(FloatActiveDocument),
                "Float Active Document", "Documents",
                "Floats the active document and persists that dock state into the design-time layout snapshot.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(PinActiveDocument),
                "Pin Active Document", "Documents",
                "Pins the active tab (icon-only, un-closable).", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UnpinActiveDocument),
                "Unpin Active Document", "Documents",
                "Restores the pinned tab to normal.", false));
            items.Add(new DesignerActionPropertyItem(nameof(ActiveDocumentTitle),
                "Active Document Title:", "Documents",
                "Rename the currently active design-time document surface."));

            // ── Design-Time ──────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Design-Time"));
            items.Add(new DesignerActionMethodItem(this, nameof(EditDesignTimeDocuments),
                "Edit Design-Time Documents\u2026", "Design-Time",
                "Opens the collection editor to define documents that appear at design and startup.", true));

            // ── Split View ───────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Split View"));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenLayoutAssistant),
                "Layout Assistant...", "Split View",
                "Guides you through choosing a dock pattern and naming the document surfaces it should create.", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SplitActiveHorizontal),
                "Split Horizontal \u2194", "Split View",
                "Creates a new side-by-side document surface and activates it.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SplitActiveVertical),
                "Split Vertical \u2195", "Split View",
                "Creates a new stacked document surface and activates it.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(MergeAllGroups),
                "Merge All Groups", "Split View",
                "Collapses all split groups back to a single group.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyLayoutPreset),
                "Apply Layout Preset\u2026", "Split View",
                "Opens the visual layout preset picker and creates any missing document surfaces.", true));
            items.Add(new DesignerActionPropertyItem(nameof(MaxGroups),       "Max Groups:",       "Split View", "Maximum number of simultaneous split groups."));
            items.Add(new DesignerActionPropertyItem(nameof(SplitHorizontal), "Horizontal Split:",  "Split View", "True = side-by-side; False = top/bottom."));
            items.Add(new DesignerActionPropertyItem(nameof(SplitRatio),      "Split Ratio:",      "Split View", "Fraction of space for the first group (0.1\u20130.9)."));

            // ── Nested Splits ────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Nested Splits"));
            items.Add(new DesignerActionPropertyItem(nameof(GroupTabPositions), "Group Tab Positions:", "Nested Splits",
                "Per-group tab strip positions for asymmetric layouts."));
            items.Add(new DesignerActionPropertyItem(nameof(LayoutTreeInfo), "Layout Tree:", "Nested Splits",
                "Current layout tree structure (read-only)."));

            // ── Tabs ─────────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Tabs"));
            items.Add(new DesignerActionPropertyItem(nameof(TabStyle),                  "Tab Style:",          "Tabs", "Visual rendering style of the tab strip."));
            items.Add(new DesignerActionPropertyItem(nameof(TabPosition),               "Tab Position:",       "Tabs", "Position of the tab strip relative to the document panels."));
            items.Add(new DesignerActionPropertyItem(nameof(CloseMode),                 "Close Mode:",         "Tabs", "When the close button is visible on each tab."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowAddButton),             "Show Add (+):",       "Tabs", "Shows the new-document (+) button in the strip."));
            items.Add(new DesignerActionPropertyItem(nameof(TabColorMode),              "Tab Color Mode:",     "Tabs", "How per-document colours are applied."));
            items.Add(new DesignerActionPropertyItem(nameof(KeyboardShortcutsEnabled),  "Keyboard Shortcuts:", "Tabs", "Enable built-in keyboard shortcuts."));

            // ── Tab Sizing ───────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Tab Sizing"));
            items.Add(new DesignerActionPropertyItem(nameof(TabSizeMode),   "Tab Size Mode:",  "Tab Sizing", "Auto, Fixed, or Fill sizing for individual tabs."));
            items.Add(new DesignerActionPropertyItem(nameof(FixedTabWidth), "Fixed Tab Width:", "Tab Sizing", "Pixel width for each tab when TabSizeMode = Fixed."));

            // ── Interaction ──────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Interaction"));
            items.Add(new DesignerActionPropertyItem(nameof(TabTooltipMode), "Tooltip Mode:",    "Interaction", "What is shown in tab tooltips."));
            items.Add(new DesignerActionPropertyItem(nameof(AllowDragFloat), "Allow Float Drag:", "Interaction", "User can drag a tab into a free-floating window."));

            // ── Style Presets ────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(UseChromStyle),     "Chrome",    "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UseVSCodeStyle),    "VS Code",   "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UseFlatStyle),      "Flat",      "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UseOfficeStyle),    "Office",    "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UsePillStyle),      "Pill",      "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UseUnderlineStyle), "Underline", "Style Presets", false));

            // ── Appearance ───────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(ControlStyle), "Container Style:", "Appearance", "Border/elevation style of the host container."));
            items.Add(new DesignerActionPropertyItem(nameof(ThemeName),    "Theme:",           "Appearance", "Beep theme name propagated to all child controls."));            items.Add(new DesignerActionMethodItem(this, nameof(ChooseTheme),
                "Choose Theme\u2026", "Appearance",
                "Opens the visual theme picker to select a Beep theme.", true));
            // ── Preview ──────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Preview"));
            items.Add(new DesignerActionPropertyItem(nameof(TabPreviewEnabled), "Tab Preview:", "Preview", "Show panel thumbnails in rich tooltips on tab hover."));

            // ── History ──────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("History"));
            items.Add(new DesignerActionPropertyItem(nameof(MaxRecentHistory), "Max Recent History:", "History", "MRU list depth."));
            items.Add(new DesignerActionPropertyItem(nameof(MaxClosedHistory), "Max Closed History:", "History", "Closed-tab reopen stack depth."));

            // ── Cross-Host Drag ──────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Cross-Host Drag"));
            items.Add(new DesignerActionPropertyItem(nameof(AllowDragBetweenHosts), "Allow Drag Between Hosts:", "Cross-Host Drag", "Allow tabs to be dragged between BeepDocumentHost instances."));

            // ── Policies ─────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Policies"));
            items.Add(new DesignerActionPropertyItem(nameof(AllowFloat),    "Allow Float:",     "Policies", "Allow documents to be floated into their own window."));
            items.Add(new DesignerActionPropertyItem(nameof(AllowSplit),    "Allow Split:",     "Policies", "Allow documents to be split into side-by-side groups."));
            items.Add(new DesignerActionPropertyItem(nameof(AllowPin),      "Allow Pin:",       "Policies", "Allow documents to be pinned to the tab strip."));
            items.Add(new DesignerActionPropertyItem(nameof(AllowAutoHide), "Allow Auto-Hide:", "Policies", "Allow documents to be minimised to auto-hide strips."));
            items.Add(new DesignerActionPropertyItem(nameof(MaxSplitDepth), "Max Split Depth:", "Policies", "Maximum nesting depth for split groups."));

            // ── Animation ────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Animation"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoHideHoverDelay), "Auto-Hide Hover Delay:", "Animation", "Hover delay (ms) before auto-hide flyout opens."));

            // ── Quick Actions ────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Quick Actions"));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllDocuments),
                "Clear All Documents", "Quick Actions",
                "Removes every document from the host and resets to blank state.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SaveLayoutSnapshot),
                "Save Layout Snapshot\u2026", "Quick Actions",
                "Copies the current design-time layout JSON to the clipboard.", false));

            // ── Session ──────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Session"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoSaveLayout), "Auto Save Layout:", "Session", "Automatically save/restore tab layout."));
            items.Add(new DesignerActionPropertyItem(nameof(SessionFile),    "Session File:",     "Session", "File path for layout save/restore."));
            items.Add(new DesignerActionPropertyItem(nameof(EnableRoutedCommands), "Enable Routed Commands:", "Session", "Enable vNext routed command execution."));
            items.Add(new DesignerActionPropertyItem(nameof(EnableTransactionalDocking), "Enable Transactional Docking:", "Session", "Enable transaction-scoped split and docking operations."));
            items.Add(new DesignerActionPropertyItem(nameof(EnableHostTelemetry), "Enable Host Telemetry:", "Session", "Emit profiler telemetry events for host operations."));

            return items;
        }
    }
}
