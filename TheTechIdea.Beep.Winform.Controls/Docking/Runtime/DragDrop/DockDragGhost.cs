using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// A translucent, click-through, top-most preview window shown while a panel is dragged. It
    /// snaps to the would-be drop bounds (or follows the cursor for a float) so the user sees where
    /// the panel will land. Painted with the themed accent/border via the docking palette; never
    /// takes focus or mouse input (so guide hit-testing keeps working).
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    internal sealed class DockDragGhost : Form
    {
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        private DockingThemeColors _colors = DockingThemeColors.Default;
        private string _title = string.Empty;

        public DockDragGhost()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            Enabled = false;                 // never interactive
            StartPosition = FormStartPosition.Manual;
            Opacity = 0.55;
            DoubleBuffered = true;
            BackColor = _colors.ActiveTabBackColor;
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        /// <summary>Shows the ghost for a panel using the given palette.</summary>
        public void Begin(DockPanel panel, DockingThemeColors colors, Form owner, Rectangle screenBounds)
        {
            _colors = colors ?? DockingThemeColors.Default;
            _title = panel?.Title ?? string.Empty;
            BackColor = _colors.ActiveTabBackColor;

            if (owner != null && !ReferenceEquals(Owner, owner))
                Owner = owner;

            MoveTo(screenBounds);
            if (!Visible)
                Show();
            Invalidate();
        }

        /// <summary>Moves/resizes the ghost to the given screen rectangle.</summary>
        public void MoveTo(Rectangle screenBounds)
        {
            if (screenBounds.Width < 2 || screenBounds.Height < 2)
                return;
            Bounds = screenBounds;
        }

        /// <summary>Hides the ghost (kept alive for reuse).</summary>
        public void End()
        {
            if (Visible)
                Hide();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = ClientRectangle;

            using (var fill = new SolidBrush(_colors.ActiveTabBackColor))
                g.FillRectangle(fill, rect);

            using (var pen = new Pen(_colors.TabBorderColor, 2f))
                g.DrawRectangle(pen, 1, 1, rect.Width - 2, rect.Height - 2);

            if (!string.IsNullOrEmpty(_title))
            {
                using var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                using var brush = new SolidBrush(_colors.ActiveTabForeColor);
                var font = BeepFontManager.DefaultFont ?? SystemFonts.DefaultFont;
                g.DrawString(_title, font, brush, rect, sf);
            }
        }
    }
}
