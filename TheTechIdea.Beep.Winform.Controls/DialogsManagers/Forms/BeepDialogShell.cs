using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    /// <summary>
    /// The layout scaffold every dialog composes into: a three-row header / body / footer
    /// <see cref="TableLayoutPanel"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This replaces per-form absolute positioning. Every dialog in this directory used to place its
    /// own controls with <c>Location = new Point(...)</c> — 41 assignments across six forms, and not
    /// one <see cref="TableLayoutPanel"/> anywhere. Coordinates cannot express a dialog whose height
    /// depends on its message length, do not scale with DPI, and clip when a caption is localised
    /// into a longer language.
    /// </para>
    /// <para>
    /// The structure is the one every reference product converges on — Radix and shadcn
    /// (<c>Header</c>/<c>Title</c>/<c>Description</c>/<c>Footer</c>), Material 3 (icon, headline,
    /// supporting text, actions), Ant Design (<c>title</c>/<c>content</c>/<c>footer</c>) and Fluent 2
    /// (<c>DialogTitle</c>/<c>DialogBody</c>/<c>DialogActions</c>). Only the body differs between a
    /// message, a question, an input and a list dialog; that is the separation this exists to give.
    /// </para>
    /// <para>
    /// Composition happens here at runtime rather than in a designer file, so no
    /// <c>InitializeComponent</c> needs the loops that would break the VS designer's parser.
    /// </para>
    /// </remarks>
    [ToolboxItem(false)]
    public sealed class BeepDialogShell : TableLayoutPanel
    {
        private readonly TableLayoutPanel _footer;
        private Control? _body;

        public BeepDialogShell()
        {
            Dock = DockStyle.Fill;
            ColumnCount = 1;
            RowCount = 3;
            Padding = new Padding(16);

            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            RowStyles.Add(new RowStyle(SizeType.AutoSize));            // header
            RowStyles.Add(new RowStyle(SizeType.Percent, 100f));       // body
            RowStyles.Add(new RowStyle(SizeType.AutoSize));            // footer

            // Right-aligned by anchoring the whole footer, not by a spacer column.
            //
            // A Percent(100) spacer was the obvious way to push the actions right, and it is wrong:
            // a percent column claims all remaining width, which starved the AutoSize button columns
            // and left every action laid out past the panel's right edge — OK sat at X=352 with a
            // width of 110 inside a 396px footer. AutoSize on the panel and a percent column inside
            // it are contradictory instructions: one wants to shrink to content, the other to fill.
            _footer = new TableLayoutPanel
            {
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 12, 0, 0),
                RowCount = 1,
                ColumnCount = 0,
            };
            _footer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Controls.Add(_footer, 0, 2);
        }

        /// <summary>The action row. Buttons are added right-aligned via <see cref="AddAction"/>.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TableLayoutPanel Footer => _footer;

        /// <summary>
        /// Places the header content — typically a title, and an icon when the dialog has severity.
        /// </summary>
        public void SetHeader(Control header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            Replace(header, row: 0);
        }

        /// <summary>
        /// Places the body content. This is the only region that differs between dialog kinds.
        /// </summary>
        public void SetBody(Control body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            // The body fills its row and must not size itself from its own content.
            //
            // AutoSize = false is not enough on its own: BeepLabel derives its MinimumSize from the
            // *unwrapped* text, and MinimumSize wins over both AutoSize and the cell. A long message
            // produced MinimumSize.Width = 1788 inside a 428px shell, so the label ran off the form
            // and the text was cut at the right rather than wrapping. Clearing the width floor is
            // what actually lets the cell govern.
            body.AutoSize = false;
            if (body.MinimumSize.Width > 0)
            {
                body.MinimumSize = new Size(0, body.MinimumSize.Height);
            }

            _body = body;
            Replace(body, row: 1);

            // Text assigned after composition (Message, Title, …) re-derives that floor, so clamp
            // again whenever the body reports a size change.
            body.SizeChanged -= OnBodySizeChanged;
            body.SizeChanged += OnBodySizeChanged;
        }

        /// <summary>The current body, or <see langword="null"/> before one is set.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control? Body => _body;

        /// <summary>
        /// Appends an action button to the footer, right-aligned, in call order.
        /// </summary>
        /// <remarks>
        /// Each button gets its own <see cref="SizeType.AutoSize"/> column, so a longer caption — a
        /// localised one, typically 30–40% longer than English — widens its column instead of
        /// clipping. This is the case absolute layout always gets wrong.
        /// </remarks>
        public void AddAction(Control button)
        {
            if (button == null) throw new ArgumentNullException(nameof(button));

            int column = _footer.ColumnCount;
            _footer.ColumnCount = column + 1;
            _footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Size from MinimumSize rather than AutoSize. A custom-painted BeepButton reports a
            // preferred size from its text alone — 44×32 for "OK" — which is narrower and shorter
            // than the 110×34 the dialogs specify, so an AutoSize cell sized the column to the text
            // and the button then overflowed it. Fixing the size makes the cell and the control
            // agree.
            if (!button.MinimumSize.IsEmpty)
            {
                button.AutoSize = false;
                button.Size = button.MinimumSize;
            }

            button.Margin = new Padding(8, 0, 0, 0);
            button.Anchor = AnchorStyles.None;
            _footer.Controls.Add(button, column, 0);
        }

        /// <summary>
        /// Keeps the body's width floor clear so the cell governs its width, not its text.
        /// </summary>
        private void OnBodySizeChanged(object? sender, EventArgs e)
        {
            if (sender is not Control body) return;
            if (body.MinimumSize.Width <= 0) return;

            body.MinimumSize = new Size(0, body.MinimumSize.Height);
        }

        /// <summary>
        /// Caps the body's width at the space actually available before the table measures it.
        /// </summary>
        /// <remarks>
        /// Clearing <c>MinimumSize</c> was not sufficient: a wrapping <c>BeepLabel</c> also reports a
        /// <c>PreferredSize</c> from its unwrapped text — 1781px for a three-sentence message — and a
        /// <c>Percent</c> column grows to that preferred width, taking the label off the form with
        /// it. A width ceiling is the standard remedy for a wrapping label inside a table, and it can
        /// only be applied here, where the available width is finally known.
        /// </remarks>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (_body != null)
            {
                int available = DisplayRectangle.Width - Padding.Horizontal;
                if (available > 0)
                {
                    if (_body.MaximumSize.Width != available)
                    {
                        _body.MaximumSize = new Size(available, 0);
                    }

                    // Assign Width outright. BeepLabel sets its own Width from its unwrapped text and
                    // honours none of AutoSize, MinimumSize or MaximumSize — measured at 1788px wide
                    // inside a 428px shell with all three cleared or capped. A container cannot
                    // constrain a control that sizes itself, so the width has to be imposed.
                    //
                    // This belongs here rather than in each dialog: otherwise every caller that ever
                    // puts a wrapping label in a dialog has to know to repeat it.
                    if (_body.Width != available)
                    {
                        _body.Width = available;
                    }
                }
            }

            base.OnLayout(levent);
        }

        private void Replace(Control content, int row)
        {
            Control? existing = GetControlFromPosition(0, row);
            if (existing != null)
            {
                Controls.Remove(existing);
                existing.Dispose();
            }

            content.Dock = DockStyle.Fill;
            content.Margin = Padding.Empty;
            Controls.Add(content, 0, row);
        }
    }
}
