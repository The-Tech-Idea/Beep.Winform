// DocumentHostActionList.cs
// Smart-tag action list for BeepDocumentHost.
// Exposes all major properties and design-time document actions.
// ─────────────────────────────────────────────────────────────────────────────
using System;
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

        // ─────────────────────────────────────────────────────────────────────
        // Document Actions
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Adds a placeholder document at design time.</summary>
        public void AddNewDocument()
        {
            if (Host == null) return;
            int idx   = Host.DocumentCount + 1;
            string id    = $"doc{idx}";
            string title = $"Document {idx}";
            _designer.ExecuteAction($"Add Document '{title}'",
                host => host.AddDocument(id, title, activate: true));
        }

        /// <summary>Closes the currently active document.</summary>
        public void CloseActiveDocument()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.ExecuteAction($"Close Document '{activeId}'",
                host => host.CloseDocument(activeId));
        }

        /// <summary>Closes all documents.</summary>
        public void CloseAllDocuments()
        {
            if (Host == null) return;
            _designer.ExecuteAction("Close All Documents", host =>
            {
                int safety = host.DocumentCount + 8;
                while (host.DocumentCount > 0 && safety-- > 0)
                {
                    var id = host.ActiveDocumentId;
                    if (string.IsNullOrEmpty(id)) break;
                    host.CloseDocument(id);
                }
            });
        }

        /// <summary>
        /// Reopens the most recently closed document (equivalent to Ctrl+Shift+T).
        /// </summary>
        public void ReopenLastClosed()
        {
            _designer.ExecuteAction("Reopen Last Closed Document",
                host => host.ReopenLastClosed());
        }

        /// <summary>Opens the Quick-Switch document picker popup.</summary>
        public void ShowQuickSwitch()
        {
            Host?.ShowQuickSwitch();
        }

        /// <summary>Floats the active document into its own window.</summary>
        public void FloatActiveDocument()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.ExecuteAction($"Float Document '{activeId}'",
                host => host.FloatDocument(activeId));
        }

        /// <summary>Pins the active document (icon-only, no close).</summary>
        public void PinActiveDocument()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.ExecuteAction($"Pin Document '{activeId}'",
                host => host.PinDocument(activeId, true));
        }

        /// <summary>Unpins the active document.</summary>
        public void UnpinActiveDocument()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.ExecuteAction($"Unpin Document '{activeId}'",
                host => host.PinDocument(activeId, false));
        }

        /// <summary>Splits the active document horizontally (side-by-side).</summary>
        public void SplitActiveHorizontal()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.ExecuteAction("Split Horizontal",
                host => host.SplitDocumentHorizontal(activeId));
        }

        /// <summary>Splits the active document vertically (top / bottom).</summary>
        public void SplitActiveVertical()
        {
            if (Host == null) return;
            var activeId = Host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return;
            _designer.ExecuteAction("Split Vertical",
                host => host.SplitDocumentVertical(activeId));
        }

        // ─────────────────────────────────────────────────────────────────────        // Design-time Actions
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the DesignTimeDocuments collection editor from the smart-tag.
        /// </summary>
        public void EditDesignTimeDocuments()
        {
            if (Host == null) return;
            var prop = System.ComponentModel.TypeDescriptor.GetProperties(Host)["DesignTimeDocuments"];
            if (prop == null) return;

            var designerHost = _designer.Component?.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            var services     = (IServiceProvider?)designerHost ?? _designer.Component?.Site;
            if (services == null) return;

            var ctx = new DesignTimeDocumentsEditorContext(Host, prop, services);
            var editor = new Editors.DesignTimeDocumentsEditor(
                typeof(System.Collections.ObjectModel.Collection<DocumentDescriptor>));

            var current = prop.GetValue(Host);
            var newVal  = editor.EditValue(ctx, ctx, current);
            if (!ReferenceEquals(newVal, current))
                _designer.SetProperty("DesignTimeDocuments", newVal);
        }

        /// <summary>
        /// Opens the layout preset picker and applies the chosen multi-group template.
        /// </summary>
        public void ApplyLayoutPreset()
        {
            if (Host == null) return;
            using var dlg = new LayoutPresetPickerDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _designer.ExecuteAction($"Apply Layout Preset: {dlg.SelectedPreset}", h =>
            {
                switch (dlg.SelectedPreset)
                {
                    case LayoutPreset.SideBySide:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.Stacked:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentVertical(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.ThreeWay:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentVertical(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.FourUp:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentVertical(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId)) h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        break;
                    default:
                        h.MergeAllGroups();
                        break;
                }
            });
        }

        /// <summary>
        /// Collapses all split groups back to a single group, moving their
        /// documents into the primary group.
        /// </summary>
        public void MergeAllGroups()
        {
            _designer.ExecuteAction("Merge All Groups", host => host.MergeAllGroups());
        }

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
                "Adds a placeholder document tab at design time.", true));
            items.Add(new DesignerActionMethodItem(this, nameof(CloseActiveDocument),
                "Close Active Document", "Documents",
                "Closes the currently active document.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(CloseAllDocuments),
                "Close All Documents", "Documents",
                "Removes every document from the host.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ReopenLastClosed),
                "Reopen Last Closed", "Documents",
                "Reopens the most recently closed document (Ctrl+Shift+T).", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowQuickSwitch),
                "Quick Switch\u2026", "Documents",
                "Opens the Quick-Switch document picker popup.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(FloatActiveDocument),
                "Float Active Document", "Documents",
                "Detaches the active document into a floating window.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(PinActiveDocument),
                "Pin Active Document", "Documents",
                "Pins the active tab (icon-only, un-closable).", false));
            items.Add(new DesignerActionMethodItem(this, nameof(UnpinActiveDocument),
                "Unpin Active Document", "Documents",
                "Restores the pinned tab to normal.", false));

            // ── Design-Time ──────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Design-Time"));
            items.Add(new DesignerActionMethodItem(this, nameof(EditDesignTimeDocuments),
                "Edit Design-Time Documents\u2026", "Design-Time",
                "Opens the collection editor to define documents that appear at design and startup.", true));

            // ── Split View ───────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Split View"));
            items.Add(new DesignerActionMethodItem(this, nameof(SplitActiveHorizontal),
                "Split Horizontal \u2194", "Split View",
                "Moves the active document into a new side-by-side pane.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(SplitActiveVertical),
                "Split Vertical \u2195", "Split View",
                "Moves the active document into a new stacked pane.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(MergeAllGroups),
                "Merge All Groups", "Split View",
                "Collapses all split groups back to a single group.", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyLayoutPreset),
                "Apply Layout Preset\u2026", "Split View",
                "Opens the visual layout preset picker to seed a multi-group layout.", true));
            items.Add(new DesignerActionPropertyItem(nameof(MaxGroups),       "Max Groups:",       "Split View", "Maximum number of simultaneous split groups."));
            items.Add(new DesignerActionPropertyItem(nameof(SplitHorizontal), "Horizontal Split:",  "Split View", "True = side-by-side; False = top/bottom."));
            items.Add(new DesignerActionPropertyItem(nameof(SplitRatio),      "Split Ratio:",      "Split View", "Fraction of space for the first group (0.1\u20130.9)."));

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

            // ── Session ──────────────────────────────────────────────────────
            items.Add(new DesignerActionHeaderItem("Session"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoSaveLayout), "Auto Save Layout:", "Session", "Automatically save/restore tab layout."));
            items.Add(new DesignerActionPropertyItem(nameof(SessionFile),    "Session File:",     "Session", "File path for layout save/restore."));

            return items;
        }
    }
}
