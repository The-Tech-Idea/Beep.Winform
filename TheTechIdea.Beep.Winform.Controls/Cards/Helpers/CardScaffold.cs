using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// The grid every card composes into: an icon gutter, a text column, and rows added in order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cards are composed from Beep controls rather than painted. A card is this panel holding
    /// <c>BeepLabel</c>, <c>BeepImage</c>, <c>BeepButton</c> and the existing chip, rating and progress
    /// controls — one per cell.
    /// </para>
    /// <para>
    /// <b>Two columns, and the second is shared.</b> The gutter sizes to the icon and collapses to
    /// nothing without one; the title, body and every other text row live in the same column, which is
    /// what gives them one left edge with nobody maintaining a margin. Two grids with different column
    /// counts cannot be aligned by adjusting either of them — the dialogs in this library were rebuilt
    /// once for exactly that reason.
    /// </para>
    /// <para>
    /// Nothing here assigns a colour, scales a value, or sets an accessible name. Every Beep control
    /// resolves its own colours from <c>BeepThemesManager</c> and re-applies them on
    /// <c>ThemeChanged</c>; the layout engine handles DPI; and a label showing a person's name already
    /// carries that text. Those three categories of defect are removed by not writing the code, which
    /// is most of why cards stopped painting.
    /// </para>
    /// </remarks>
    public sealed class CardScaffold : TableLayoutPanel
    {
        /// <summary>Column holding the icon; zero-width when no icon is placed.</summary>
        public const int GutterColumn = 0;

        /// <summary>Column holding the title and every content row.</summary>
        public const int TextColumn = 1;

        private int _rows;

        public CardScaffold()
        {
            Dock = DockStyle.Fill;
            ColumnCount = 2;
            RowCount = 0;
            Margin = new Padding(0);

            ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // icon gutter
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));  // text
        }

        /// <summary>
        /// Places the icon in the gutter, spanning every row added so far and after.
        /// </summary>
        /// <remarks>
        /// Top-anchored: the icon belongs beside the first line of text. Centred against a spanned
        /// block it drifts downward as the card's content grows.
        /// </remarks>
        public void SetIcon(Control icon)
        {
            if (icon == null) throw new ArgumentNullException(nameof(icon));

            if (RowCount == 0) AddRow(SizeType.AutoSize);

            icon.Anchor = AnchorStyles.Top;
            icon.Margin = new Padding(0, 0, 8, 0);

            Controls.Add(icon, GutterColumn, 0);
            UpdateIconSpan(icon);
        }

        /// <summary>Appends a row in the text column, in call order.</summary>
        /// <param name="fill">
        /// <see langword="true"/> for the one row that should absorb leftover height — a hosted chart
        /// or list. Everything else hugs its content, which is what makes a card size to what it says.
        /// </param>
        public void AddContent(Control content, bool fill = false)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            int row = AddRow(fill ? SizeType.Percent : SizeType.AutoSize, fill ? 100f : 0f);

            content.Dock = DockStyle.Fill;
            content.Margin = new Padding(0, 0, 0, 2);

            Controls.Add(content, TextColumn, row);
            RestoreIconSpan();
        }

        /// <summary>
        /// Appends a row spanning both columns — media, a chart, or anything full-bleed.
        /// </summary>
        public void AddFullWidth(Control content, bool fill = false)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            int row = AddRow(fill ? SizeType.Percent : SizeType.AutoSize, fill ? 100f : 0f);

            content.Dock = DockStyle.Fill;
            content.Margin = new Padding(0, 0, 0, 2);

            Controls.Add(content, GutterColumn, row);
            SetColumnSpan(content, 2);
            RestoreIconSpan();
        }

        /// <summary>
        /// Appends a right-aligned row of actions.
        /// </summary>
        /// <remarks>
        /// Right-aligned by anchoring the row rather than by a spacer column: a percent spacer claims
        /// the remaining width and starves the AutoSize button columns, which lays the actions out
        /// past the card's edge.
        /// </remarks>
        public TableLayoutPanel AddActions(params Control[] actions)
        {
            if (actions == null || actions.Length == 0) throw new ArgumentException("No actions given.", nameof(actions));

            var strip = new TableLayoutPanel
            {
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 6, 0, 0),
                RowCount = 1,
                ColumnCount = actions.Length,
            };
            strip.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            for (int i = 0; i < actions.Length; i++)
            {
                strip.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                actions[i].Margin = new Padding(i == 0 ? 0 : 6, 0, 0, 0);
                strip.Controls.Add(actions[i], i, 0);
            }

            AddFullWidth(strip);
            return strip;
        }

        /// <summary>Removes every composed control so the card can be recomposed.</summary>
        /// <remarks>
        /// A card recomposes whenever the property that selects its arrangement changes. Disposing
        /// what is displaced matters: a card left holding controls it no longer shows leaks a handle
        /// each time, and cards live in lists.
        /// </remarks>
        public void Clear()
        {
            SuspendLayout();

            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                var c = Controls[i];
                Controls.RemoveAt(i);
                c.Dispose();
            }

            RowStyles.Clear();
            RowCount = 0;
            _rows = 0;

            ResumeLayout(true);
        }

        private int AddRow(SizeType type, float value = 0f)
        {
            RowStyles.Add(new RowStyle(type, value));
            RowCount = ++_rows;
            return _rows - 1;
        }

        private void UpdateIconSpan(Control icon)
        {
            SetRowSpan(icon, Math.Max(1, RowCount));
        }

        private void RestoreIconSpan()
        {
            var icon = GetControlFromPosition(GutterColumn, 0);
            if (icon != null && GetColumnSpan(icon) == 1) UpdateIconSpan(icon);
        }
    }
}
