using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A ribbon group: a container whose commands fill columns of three rows, with a caption strip
    /// beneath them and a rule on its trailing edge.
    /// </summary>
    /// <remarks>
    /// This derived from <see cref="ToolStrip"/>, and that was the defect. A ToolStrip is a toolbar:
    /// one horizontal flow of items with an overflow chevron. It has no notion of a column, so every
    /// group rendered as a single row of bare text captions followed by a "More" button, and no amount
    /// of height tuning could change that — the height was never what was wrong.
    ///
    /// The layout here is Fluent.Ribbon's RibbonGroupBox: small commands stack three-high (22px each)
    /// and then start a new column; a large command takes a whole column at the full 66px content
    /// height. The group is exactly as wide as its columns need, and groups flow left to right across
    /// the tab.
    /// </remarks>
    [ToolboxItem(false)]
    [DesignerCategory("Code")]
    public class BeepRibbonGroup : Panel
    {
        /// <summary>One placed item, or one separator rule, in the column grid.</summary>
        private sealed class GroupSlot
        {
            /// <summary>The hosted control, or null when this slot is a separator rule.</summary>
            public Control? Item;
            public RibbonItemSize Size;
            public bool IsSeparator;

            /// <summary>Measured natural width, taken once when the slot is added.</summary>
            public int NaturalWidth;

            /// <summary>Where the layout put it, relative to the group.</summary>
            public Rectangle Bounds;
        }

        private readonly List<GroupSlot> _slots = [];
        private RibbonDensity _density = RibbonDensity.Comfortable;
        private RibbonTheme? _theme;
        private BeepRibbonPainter? _painter;
        private int _minTouchTargetWidth = 44;
        private bool _popupOpen;
        private ToolStripDropDown? _openPopup;

        public event EventHandler? PopupOpened;
        public event EventHandler? PopupClosed;

        /// <summary>Height of the item area: three 22px rows, the standard ribbon group content.</summary>
        /// <remarks>
        /// From Fluent.Ribbon's group template, whose recurring row unit is 22. The control previously
        /// used 40/48/56 by density — all below the 66px three rows need, which is why every command
        /// collapsed to a single line of text and most of them went to the overflow menu.
        /// </remarks>
        public const int ContentHeight = 66;

        /// <summary>Height of the caption strip beneath the items, carrying <see cref="Control.Text"/>.</summary>
        public const int CaptionHeight = 22;

        /// <summary>Icon edge for a one-row command.</summary>
        public const int SmallIconSize = 16;

        /// <summary>Icon edge for a whole-column command.</summary>
        public const int LargeIconSize = 32;

        /// <summary>Left and right inset of the item band.</summary>
        private const int HorizontalInset = 3;

        /// <summary>Width of the slot a separator rule occupies between two columns.</summary>
        private const int SeparatorSlotWidth = 9;

        /// <summary>Narrowest a group may be, so an empty one is still visibly a group.</summary>
        public const int MinGroupWidth = 48;

        /// <summary>Item-area height for a density. One place, so nothing can disagree about it.</summary>
        public static int ContentFor(RibbonDensity density) => density switch
        {
            RibbonDensity.Compact => 44,   // two rows
            RibbonDensity.Touch => 90,     // three roomier rows
            _ => ContentHeight,            // three 22px rows
        };

        /// <summary>How many small commands stack in one column.</summary>
        public static int RowsFor(RibbonDensity density) =>
            density == RibbonDensity.Compact ? 2 : 3;

        /// <summary>Height of one small-command row.</summary>
        public static int RowHeightFor(RibbonDensity density) =>
            ContentFor(density) / RowsFor(density);

        /// <summary>Icon edge for a one-row command at a density.</summary>
        public static int SmallIconFor(RibbonDensity density) => density switch
        {
            RibbonDensity.Compact => 14,
            RibbonDensity.Touch => 20,
            _ => SmallIconSize,
        };

        /// <summary>
        /// Icon edge for a whole-column command at a density. 32 at the default, which is what a
        /// ribbon large button has always wanted — the old GetIconSize topped out at 20.
        /// </summary>
        public static int LargeIconFor(RibbonDensity density) => density switch
        {
            RibbonDensity.Compact => 24,
            RibbonDensity.Touch => 36,
            _ => LargeIconSize,
        };

        /// <summary>Narrowest a whole-column command may be.</summary>
        public static int LargeWidthFor(RibbonDensity density) => density switch
        {
            RibbonDensity.Compact => 64,
            RibbonDensity.Touch => 84,
            _ => 72,
        };

        public BeepRibbonGroup()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;

            // Left, not Top. Groups sit side by side across a ribbon tab; docked Top they stacked
            // vertically, each stretched to the full width of the ribbon, so on a tab with three
            // groups only the first was inside the content panel's height and the rest were pushed
            // off the bottom entirely.
            Dock = DockStyle.Left;
            Padding = Padding.Empty;
            Margin = Padding.Empty;
            Height = ContentHeight + CaptionHeight;
            Width = MinGroupWidth;
        }

        public RibbonDensity Density
        {
            get => _density;
            set
            {
                if (_density == value) return;
                _density = value;
                ApplyDensity();
            }
        }

        [Category("Layout")]
        [Description("Minimum width of a one-row command when the ribbon is at Touch density.")]
        [DefaultValue(44)]
        public int MinTouchTargetWidth
        {
            get => _minTouchTargetWidth;
            set
            {
                int clamped = Math.Max(32, value);
                if (_minTouchTargetWidth == clamped) return;
                _minTouchTargetWidth = clamped;
                RemeasureItems();
                PerformGroupLayout();
            }
        }

        [Browsable(false)]
        public bool IsPopupOpen
        {
            get => _popupOpen;
            set
            {
                if (_popupOpen == value) return;
                _popupOpen = value;
                if (value) OnPopupOpened(EventArgs.Empty);
                else OnPopupClosed(EventArgs.Empty);
            }
        }

        /// <summary>The command controls this group hosts, in layout order.</summary>
        [Browsable(false)]
        public IReadOnlyList<Control> ItemControls =>
            _slots.Where(s => s.Item != null).Select(s => s.Item!).ToList();

        /// <summary>The item band, excluding the caption strip.</summary>
        [Browsable(false)]
        public int ContentBandHeight => ContentFor(_density);

        public void CloseChildPopup()
        {
            _openPopup?.Close();
            IsPopupOpen = false;
        }

        protected virtual void OnPopupOpened(EventArgs e) => PopupOpened?.Invoke(this, e);
        protected virtual void OnPopupClosed(EventArgs e) => PopupClosed?.Invoke(this, e);

        /// <summary>
        /// Follows a popup opened by one of this group's commands, so the group's own
        /// <see cref="IsPopupOpen"/> and <see cref="CloseChildPopup"/> mean something. On the
        /// ToolStrip version those three members and the two events had no writer anywhere.
        /// </summary>
        public void TrackPopup(ToolStripDropDown popup)
        {
            if (popup == null) return;
            popup.Opened += (_, _) =>
            {
                _openPopup = popup;
                IsPopupOpen = true;
            };
            popup.Closed += (_, _) =>
            {
                if (ReferenceEquals(_openPopup, popup)) _openPopup = null;
                IsPopupOpen = false;
            };
        }

        public void ApplyTheme(RibbonTheme theme)
        {
            if (theme == null) return;

            _theme = theme;
            _painter = null;   // rebuilt on next paint against the new theme
            BackColor = theme.GroupBack;
            ForeColor = theme.Text;
            Font = BeepThemesManager.ToFont(theme.CommandTypography);

            foreach (var slot in _slots)
            {
                if (slot.Item != null) ApplyItemTheme(slot.Item);
            }

            RemeasureItems();
            PerformGroupLayout();
        }

        #region Items

        /// <summary>
        /// Creates a command button sized for this group without adding it, so the caller can finish
        /// configuring it — a chevron changes its width — before the group measures it.
        /// </summary>
        public BeepRibbonCommandButton NewCommandButton(string text, string? imagePath, RibbonItemSize size)
        {
            var button = new BeepRibbonCommandButton
            {
                ItemSize = size,
                Text = text ?? string.Empty,
                ImagePath = imagePath ?? string.Empty,
                MaxImageSize = size == RibbonItemSize.Large
                    ? new Size(LargeIconFor(_density), LargeIconFor(_density))
                    : new Size(SmallIconFor(_density), SmallIconFor(_density))
            };

            ApplyItemTheme(button);
            return button;
        }

        public BeepRibbonCommandButton AddLargeButton(string text, string? imagePath = null)
        {
            var button = NewCommandButton(text, imagePath, RibbonItemSize.Large);
            AddItem(button, RibbonItemSize.Large);
            return button;
        }

        public BeepRibbonCommandButton AddSmallButton(string text, string? imagePath = null)
        {
            var button = NewCommandButton(text, imagePath, RibbonItemSize.Small);
            AddItem(button, RibbonItemSize.Small);
            return button;
        }

        /// <summary>Adds an already-configured control to the column grid.</summary>
        public void AddItem(Control item, RibbonItemSize size)
        {
            if (item == null) return;

            var slot = new GroupSlot { Item = item, Size = size };
            slot.NaturalWidth = MeasureSlot(slot);
            _slots.Add(slot);
            Controls.Add(item);
        }

        /// <summary>
        /// Adds a vertical rule between two columns. A separator in a group is a column boundary, not
        /// a gap in a row — which is what it meant while the group was a toolbar. It is painted, not
        /// hosted: it is not interactive, so it must not appear as a control, a key tip or an
        /// accessibility target.
        /// </summary>
        public void AddSeparator()
        {
            _slots.Add(new GroupSlot { IsSeparator = true, NaturalWidth = SeparatorSlotWidth });
        }

        /// <summary>
        /// Removes the item added last, disposing it. Used to back out of a placement once the group
        /// has measured it and found it does not fit the width the tab can give this group.
        /// </summary>
        /// <returns>
        /// The control that was removed, or null when the last slot was a separator. The caller needs
        /// this to drop the right command-map entry: taking "the last hosted control" instead would
        /// unmap a command that is still on screen whenever the slot being removed is a separator.
        /// </returns>
        public Control? RemoveLastItem()
        {
            if (_slots.Count == 0) return null;

            var slot = _slots[^1];
            _slots.RemoveAt(_slots.Count - 1);
            if (slot.Item == null) return null;

            Controls.Remove(slot.Item);
            slot.Item.Dispose();
            return slot.Item;
        }

        /// <summary>Empties the group, disposing every hosted control.</summary>
        /// <remarks>
        /// <c>Controls.Clear()</c> orphans children without disposing them. These are real controls
        /// with window handles and cached SVG state, so the ToolStrip version's <c>Items.Clear()</c>
        /// leaked a hosted gallery on every rebuild.
        /// </remarks>
        public void ClearItems()
        {
            CloseChildPopup();

            foreach (var slot in _slots)
            {
                if (slot.Item == null) continue;
                Controls.Remove(slot.Item);
                slot.Item.Dispose();
            }

            _slots.Clear();
            Width = MinGroupWidth;
            Invalidate();
        }

        #endregion

        #region Layout

        /// <summary>
        /// Width the current items need, without moving anything. The caller uses this to decide
        /// whether the item it just added has to go to the overflow menu instead.
        /// </summary>
        public int MeasureContentWidth() => ComputeLayout(apply: false);

        /// <summary>Places every item and sizes the group to its columns.</summary>
        public void PerformGroupLayout() => ComputeLayout(apply: true);

        private int ComputeLayout(bool apply)
        {
            int rows = RowsFor(_density);
            int rowHeight = RowHeightFor(_density);
            int band = ContentFor(_density);
            int gap = Math.Max(1, _theme?.ItemSpacing ?? 4);
            int minItemWidth = MinItemWidth;

            int columnLeft = HorizontalInset;
            int columnWidth = 0;
            int row = 0;
            int right = HorizontalInset;
            var column = new List<GroupSlot>();

            // Closing a column is what makes this a ribbon rather than a toolbar: the members share
            // the widest member's width, and the next item starts a new column at the far side.
            void CloseColumn()
            {
                if (column.Count == 0)
                {
                    row = 0;
                    columnWidth = 0;
                    return;
                }

                foreach (var member in column)
                {
                    member.Bounds = new Rectangle(columnLeft, member.Bounds.Y, columnWidth, member.Bounds.Height);
                }

                column.Clear();
                right = columnLeft + columnWidth;
                columnLeft = right + gap;
                columnWidth = 0;
                row = 0;
            }

            foreach (var slot in _slots)
            {
                if (slot.IsSeparator)
                {
                    CloseColumn();
                    slot.Bounds = new Rectangle(columnLeft, 0, SeparatorSlotWidth, band);
                    right = columnLeft + SeparatorSlotWidth;
                    columnLeft = right + gap;
                    continue;
                }

                if (slot.Item == null) continue;

                if (slot.Size == RibbonItemSize.Large)
                {
                    CloseColumn();
                    int wide = Math.Max(LargeWidthFor(_density), slot.NaturalWidth);
                    slot.Bounds = new Rectangle(columnLeft, 0, wide, band);
                    right = columnLeft + wide;
                    columnLeft = right + gap;
                    continue;
                }

                columnWidth = Math.Max(columnWidth, Math.Max(minItemWidth, slot.NaturalWidth));
                slot.Bounds = new Rectangle(columnLeft, row * rowHeight, columnWidth, rowHeight);
                column.Add(slot);
                row++;
                if (row >= rows) CloseColumn();
            }

            CloseColumn();

            int total = Math.Max(MinGroupWidth, right + HorizontalInset);

            if (apply)
            {
                SuspendLayout();
                try
                {
                    foreach (var slot in _slots)
                    {
                        if (slot.Item != null) slot.Item.Bounds = slot.Bounds;
                    }

                    // Width only. Height is the parent's business: the group is docked Left, so the
                    // content panel decides it, and assigning it here would be undone on the next
                    // layout pass anyway. ApplyDensity sets it for the undocked case.
                    Width = total;
                }
                finally
                {
                    ResumeLayout();
                }

                Invalidate();
            }

            return total;
        }

        /// <summary>
        /// Floor for a one-row command. At Touch density this is where
        /// <see cref="MinTouchTargetWidth"/> takes effect — the property existed with no reader at all
        /// while the group was a toolbar.
        /// </summary>
        private int MinItemWidth =>
            _density == RibbonDensity.Touch ? Math.Max(44, _minTouchTargetWidth) : 44;

        private void RemeasureItems()
        {
            foreach (var slot in _slots)
            {
                if (slot.IsSeparator) continue;
                slot.NaturalWidth = MeasureSlot(slot);
            }
        }

        private int MeasureSlot(GroupSlot slot)
        {
            if (slot.IsSeparator) return SeparatorSlotWidth;
            if (slot.Item == null) return 0;

            if (slot.Item is BeepRibbonCommandButton command) return MeasureCommand(command);

            // A hosted control (a gallery) already knows how wide it wants to be.
            return slot.Item.Width > 0 ? slot.Item.Width : slot.Item.PreferredSize.Width;
        }

        /// <summary>
        /// Real measurement of a command, against the font it will actually be drawn with. The
        /// ToolStrip version guessed this — a pile of magic numbers standing in for the toolbar's
        /// internal item padding — and then never reconciled the guess with the width WinForms
        /// computed, which is how the overflow decision could be made on a number that was wrong.
        /// </summary>
        private int MeasureCommand(BeepRibbonCommandButton command)
        {
            var font = command.Font ?? Font;
            int text = string.IsNullOrEmpty(command.Text)
                ? 0
                : TextRenderer.MeasureText(command.Text, font).Width;

            if (command.ItemSize == RibbonItemSize.Large)
            {
                return Math.Max(LargeWidthFor(_density), text + 14);
            }

            int icon = string.IsNullOrWhiteSpace(command.ImagePath) ? 0 : SmallIconFor(_density) + 6;
            int arrow = command.ShowDropDownArrow ? 14 : 0;
            return Math.Max(MinItemWidth, icon + text + arrow + 14);
        }

        private void ApplyDensity()
        {
            Height = ContentFor(_density) + CaptionHeight;

            foreach (var slot in _slots)
            {
                if (slot.Item is not BeepRibbonCommandButton command) continue;
                int icon = command.ItemSize == RibbonItemSize.Large
                    ? LargeIconFor(_density)
                    : SmallIconFor(_density);
                command.MaxImageSize = new Size(icon, icon);
            }

            RemeasureItems();
            PerformGroupLayout();
        }

        /// <summary>
        /// Colours and font for a hosted command, resolved from the ribbon theme.
        /// </summary>
        /// <remarks>
        /// This sets properties on the child; it never calls the child's own <c>ApplyTheme</c>.
        /// A container that re-themes its children re-enters theming and hangs construction.
        /// <c>IsChild</c> makes the button take the group's background, so even a stray global theme
        /// change leaves it sitting on the right colour.
        /// </remarks>
        private void ApplyItemTheme(Control item)
        {
            if (_theme == null || item == null) return;

            item.Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            if (item is not BeepButton button) return;

            // RibbonTheme has a single Text, but the ribbon has three surfaces: the app-bar-derived
            // quick access strip, the tab strip and the group background. The mapper takes Text from
            // the app bar's title colour, which on a light GroupBack is a pale grey on pale blue - a
            // contrast ratio of about 1.4. EnsureReadable keeps the theme's colour whenever it is
            // legible and only substitutes past that point.
            Color ink = Helpers.ColorUtils.EnsureReadable(_theme.Text, _theme.GroupBack);

            button.IsChild = true;
            button.UseThemeFont = false;
            button.BackColor = _theme.GroupBack;
            button.ForeColor = ink;
            button.HoverBackColor = _theme.HoverBack;
            button.HoverForeColor = Helpers.ColorUtils.EnsureReadable(_theme.Text, _theme.HoverBack);
            button.PressedBackColor = _theme.PressedBack;
            button.PressedForeColor = Helpers.ColorUtils.EnsureReadable(_theme.Text, _theme.PressedBack);
            button.SelectedBackColor = _theme.SelectionBack;
            button.SelectedForeColor = Helpers.ColorUtils.EnsureReadable(_theme.Text, _theme.SelectionBack);
            button.DisabledBackColor = _theme.DisabledBack;
            button.DisabledForeColor = _theme.DisabledText;
            button.BorderColor = _theme.GroupBorder;

            if (button is BeepRibbonCommandButton command) command.ArrowColor = ink;
        }

        #endregion

        private BeepRibbonPainter Painter => _painter ??= new BeepRibbonPainter(_theme ??= new RibbonTheme(), this);

        /// <summary>
        /// Draws the group's caption strip, its column separators and its trailing rule.
        /// </summary>
        /// <remarks>
        /// <see cref="Control.Text"/> is set on every group by <c>AddGroup</c> and was never rendered
        /// anywhere, so a ribbon tab showed three unlabelled boxes of commands. The caption is what
        /// makes a group read as a group, and the trailing rule is what stops adjacent ones merging
        /// into one grey band.
        /// </remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width <= 0 || Height <= CaptionHeight) return;

            var g = e.Graphics;
            var painter = Painter;
            int band = ContentBandHeight;

            foreach (var slot in _slots)
            {
                if (!slot.IsSeparator) continue;
                painter.PaintGroupSeparator(g, slot.Bounds.X + slot.Bounds.Width / 2, 0, band);
            }

            painter.PaintGroupTitle(g, new Rectangle(0, band, Width, CaptionHeight), Text ?? string.Empty);

            // Trailing edge, inset so it stops short of the caption baseline - the same treatment
            // Fluent.Ribbon gives its inter-group separator.
            painter.PaintGroupSeparator(g, Width - 1, 0, band);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _openPopup = null;
                _slots.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
