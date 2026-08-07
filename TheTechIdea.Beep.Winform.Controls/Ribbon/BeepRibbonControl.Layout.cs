using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        /// <summary>Metadata key a caller can set on a command to declare its ribbon size.</summary>
        /// <remarks>
        /// Accepts "Large"/"Small" in <see cref="SimpleItem.Data"/>. Fluent's model is that a command
        /// declares its own prominence; the group then degrades as a whole when it runs out of room.
        /// The old code instead decided large-versus-small for the entire group from a width estimate
        /// derived from the group's own provisional width, which is circular and resolved to "small"
        /// almost everywhere.
        /// </remarks>
        public const string RibbonItemSizeKey = "RibbonItemSize";

        /// <summary>Width reserved for a group's overflow button when one is needed.</summary>
        private const int OverflowButtonWidth = 68;

        /// <summary>Narrowest budget a group is ever given: one column plus its overflow button.</summary>
        private const int MinGroupBudget = 120;

        /// <summary>Budget used before the ribbon has a width to divide - design time, mostly.</summary>
        private const int DefaultGroupBudget = 480;

        private bool ShouldRenderLargeButtons() =>
            _layoutMode == RibbonLayoutMode.Classic && _density != RibbonDensity.Compact;

        /// <summary>
        /// The size one command occupies in its group's column grid.
        /// </summary>
        /// <remarks>
        /// An explicit declaration wins. Otherwise the leading command of a group is the large one and
        /// everything after it stacks three-high, which is the arrangement Office uses for a group with
        /// one primary action (Paste, then Cut/Copy/Format Painter).
        /// </remarks>
        private RibbonItemSize ResolveItemSize(SimpleItem command, bool isLeadingCommand)
        {
            if (command.IsSeparator) return RibbonItemSize.Small;
            if (!ShouldRenderLargeButtons()) return RibbonItemSize.Small;

            if (command.Data != null &&
                command.Data.TryGetValue(RibbonItemSizeKey, out var declared) &&
                declared != null &&
                Enum.TryParse<RibbonItemSize>(declared.ToString(), ignoreCase: true, out var size))
            {
                // Medium is laid out as Small: it is one row of a column, the same as Small, and the
                // group grid has no third row height to give it.
                return size == RibbonItemSize.Large ? RibbonItemSize.Large : RibbonItemSize.Small;
            }

            // A gallery is never one 22px row - it needs the whole content band to show its tiles.
            if (IsGalleryCommand(command)) return RibbonItemSize.Large;

            return isLeadingCommand ? RibbonItemSize.Large : RibbonItemSize.Small;
        }

        /// <summary>
        /// How much width the tab can still give this group.
        /// </summary>
        /// <remarks>
        /// Groups are docked left, so each one is as wide as its own columns and the tab's remaining
        /// width is what the siblings have not already taken. On a first build the later groups do not
        /// exist yet, so the leading group sees the whole tab; a rebuild - which is what a resize runs -
        /// sees every sibling's real width and settles. This is deliberately not
        /// <c>DisplayRectangle</c>: on a ToolStrip that property subtracted the grip and the overflow
        /// chevron, and carrying the expression over to a Panel would have changed its meaning
        /// silently.
        /// </remarks>
        private int GetGroupWidthBudget(BeepRibbonGroup group)
        {
            // The content host, not the group's own page panel. Every page is docked Fill inside the
            // host, so they are the same width - but a page created during a build has not been laid
            // out yet and still reports Panel's default 200, which starved every group on a new tab
            // and sent almost all of its commands to the overflow menu.
            int total = _ribbonContentHost.ClientSize.Width;
            if (total <= 0) total = ClientSize.Width;
            if (total <= 0) total = Width;
            if (total <= 0) return DefaultGroupBudget;

            int used = 0;
            if (group.Parent != null)
            {
                foreach (var sibling in group.Parent.Controls.OfType<BeepRibbonGroup>())
                {
                    if (!ReferenceEquals(sibling, group)) used += sibling.Width;
                }
            }

            return Math.Max(MinGroupBudget, total - used - 8);
        }

        /// <summary>Icon edge for a menu, quick access or minimized-popup image.</summary>
        /// <remarks>
        /// Delegates to <see cref="BeepRibbonGroup"/> so the density metrics live in exactly one place.
        /// The duplicate switch statements here had already drifted from the group's own.
        /// </remarks>
        private int GetIconSize(bool small) => small
            ? BeepRibbonGroup.SmallIconFor(_density)
            : BeepRibbonGroup.LargeIconFor(_density);

        /// <summary>
        /// Rasterises an SVG for a <see cref="ToolStripItem"/>.
        /// </summary>
        /// <remarks>
        /// Only the paths that still need a <see cref="Image"/> use this: the quick access toolbar, the
        /// drop-down menus and the minimized popup. Commands inside a group hand their path to
        /// <c>BeepButton</c>, which renders and themes the SVG directly and scales it with DPI.
        /// </remarks>
        private Image? CreateCommandImage(string? imagePath, bool small)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return null;
            int size = GetIconSize(small);
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _generatedImages.Add(bmp);
            return bmp;
        }

        private void ClearRibbonTree()
        {
            HideMinimizedPopup();
            _commandMap.Clear();
            _controlCommandMap.Clear();
            _groupCommandNodes.Clear();
            _commandLookup.Clear();
            _tabStrip.Clear();

            // Controls.Clear() orphans the tab panels without disposing them, and each one now owns a
            // tree of real command controls. Dispose the snapshot rather than the live collection.
            var pages = _ribbonContentHost.Controls.Cast<Control>().ToList();
            _ribbonContentHost.Controls.Clear();
            foreach (var page in pages) page.Dispose();

            DisposeGeneratedImages();
        }

        private void DisposeGeneratedImages()
        {
            foreach (var image in _generatedImages) image.Dispose();
            _generatedImages.Clear();
        }

        /// <summary>Drops the command-map entries for a group that is about to be emptied.</summary>
        private void ForgetGroupCommandControls(BeepRibbonGroup group)
        {
            foreach (var control in group.ItemControls) _controlCommandMap.Remove(control);
        }

        /// <summary>
        /// Rebuilds every group against the width the tab can currently give it.
        /// </summary>
        /// <remarks>
        /// This used to run immediately after <c>BuildFromSimpleItems</c> had already built every group
        /// from the same cached nodes, so every command was constructed twice and every icon rasterised
        /// twice per rebuild. It is now what a resize calls, which is what its name always claimed.
        /// Groups are rebuilt one at a time and not cleared up front, so each one measures against its
        /// siblings' real widths.
        /// </remarks>
        private void ApplyResponsiveLayout()
        {
            if (_isApplyingResponsiveLayout || _groupCommandNodes.Count == 0) return;
            _isApplyingResponsiveLayout = true;
            try
            {
                var groups = _groupCommandNodes.Keys.Where(g => !g.IsDisposed).ToList();
                foreach (var group in groups)
                {
                    if (_groupCommandNodes.TryGetValue(group, out var commands))
                        BuildGroupCommands(group, commands);
                }
                RebuildQuickAccessToolbar();
            }
            finally { _isApplyingResponsiveLayout = false; }
        }

        private int EstimateGalleryWidth(SimpleItem command, bool large)
        {
            int itemCount = command.Children.Count(c => !c.IsSeparator && c.IsVisible);
            itemCount = Math.Max(2, itemCount);
            int tileWidth = _density switch { RibbonDensity.Compact => large ? 88 : 68, RibbonDensity.Touch => large ? 110 : 92, _ => large ? 96 : 78 };
            int visibleTiles = large ? Math.Min(3, itemCount) : Math.Min(4, itemCount);
            return Math.Max(128, visibleTiles * tileWidth + 10);
        }

        public RibbonTab AddPage(string title)
        {
            var tab = _tabStrip.AddTab(title);
            tab.ContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _theme.TabActiveBack,
                Visible = _tabStrip.Tabs.Count == 1
            };
            _ribbonContentHost.Controls.Add(tab.ContentPanel);
            return tab;
        }

        public BeepRibbonGroup AddGroup(RibbonTab tab, string title)
        {
            if (tab.ContentPanel == null)
            {
                tab.ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = _theme.TabActiveBack };
                _ribbonContentHost.Controls.Add(tab.ContentPanel);
            }
            var group = new BeepRibbonGroup { Text = title, Density = _density };
            group.ApplyTheme(_theme);
            tab.ContentPanel.Controls.Add(group);

            // A left-docked child docks nearest the edge in reverse child order, so appending would put
            // the LAST group added at the far left and reverse the whole tab. Sending each new group to
            // index 0 keeps the first one added leftmost, which is the order the caller wrote.
            tab.ContentPanel.Controls.SetChildIndex(group, 0);
            return group;
        }
    }
}
