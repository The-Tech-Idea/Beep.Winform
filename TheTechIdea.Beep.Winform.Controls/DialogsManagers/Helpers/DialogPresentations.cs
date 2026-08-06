using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Rearranges a dialog's grid into one of the presentations the reference images show.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is <b>arrangement only</b> — which cell each control occupies, how its text is aligned,
    /// and where the actions sit. It sets no colours and paints nothing, because appearance is the
    /// theme's: see the master tracker. The tinted header of `dialog1.png` and the saturated band of
    /// `dialog4.png` are theme work; what is here is the geometry those designs also differ in, and
    /// geometry is what the dialogs own through their grid.
    /// </para>
    /// <para>
    /// The grid itself is declared in each dialog's designer file. A presentation is applied on top of
    /// that declaration rather than replacing it, so the designer stays the source of truth for what
    /// controls exist and the presentation decides only how they are laid out.
    /// </para>
    /// </remarks>
    internal static class DialogPresentations
    {
        /// <summary>Column 0 is the icon gutter; column 1 holds the title and content.</summary>
        private const int GutterColumn = 0;
        private const int TextColumn = 1;

        public static void Apply(Form dialog, DialogConfig config)
        {
            if (dialog == null || config == null) return;
            if (config.Presentation == DialogPresentation.TitleBar) return;   // the declared layout
            if (dialog.Controls.Count == 0 || dialog.Controls[0] is not TableLayoutPanel shell) return;

            Control? icon = shell.GetControlFromPosition(GutterColumn, 0);
            TableLayoutPanel? footer = FindFooter(shell);

            switch (config.Presentation)
            {
                case DialogPresentation.Centred:
                    CentreTextBlock(shell, icon, footer, iconAbove: true);
                    break;

                case DialogPresentation.HeroBand:
                    CentreTextBlock(shell, icon, footer, iconAbove: true);
                    Enlarge(icon, 2.0f);        // dialog4.png's mark is the dominant element
                    break;

                case DialogPresentation.Flat:
                    CentreTextBlock(shell, icon, footer, iconAbove: true);
                    Enlarge(icon, 2.5f);
                    AlignText(shell, ContentAlignment.TopLeft);
                    if (footer != null) footer.Anchor = AnchorStyles.Left;   // text-style action, left
                    break;
            }
        }

        /// <summary>
        /// Moves the icon above the text and centres the whole block, actions included.
        /// </summary>
        /// <remarks>
        /// The icon leaves the gutter and takes a row of its own spanning both columns, which is what
        /// collapses the gutter to zero width and lets the text centre against the full dialog rather
        /// than against the space left beside an icon. Without that the "centred" text would sit
        /// centred in a column that is itself off-centre.
        /// </remarks>
        private static void CentreTextBlock(TableLayoutPanel shell, Control? icon,
                                            TableLayoutPanel? footer, bool iconAbove)
        {
            shell.SuspendLayout();

            if (icon != null && iconAbove)
            {
                // Push everything down one row to make space above the title.
                foreach (Control c in shell.Controls)
                {
                    if (ReferenceEquals(c, icon)) continue;
                    var pos = shell.GetCellPosition(c);
                    shell.SetCellPosition(c, new TableLayoutPanelCellPosition(pos.Column, pos.Row + 1));
                }

                shell.RowCount++;
                shell.RowStyles.Insert(0, new RowStyle(SizeType.AutoSize));

                shell.SetRowSpan(icon, 1);
                shell.SetCellPosition(icon, new TableLayoutPanelCellPosition(GutterColumn, 0));
                shell.SetColumnSpan(icon, 2);
                icon.Anchor = AnchorStyles.None;                 // centred in its own row
                icon.Margin = new Padding(0, 0, 0, 12);
            }

            AlignText(shell, ContentAlignment.TopCenter);

            // Actions centre with the text rather than sitting at the right edge.
            if (footer != null) footer.Anchor = AnchorStyles.None;

            shell.ResumeLayout(true);
        }

        /// <summary>Aligns every label in the text column.</summary>
        private static void AlignText(TableLayoutPanel shell, ContentAlignment alignment)
        {
            foreach (Control c in shell.Controls)
            {
                if (c is not BeepLabel label) continue;
                if (shell.GetCellPosition(c).Column != TextColumn) continue;

                label.TextAlign = alignment;
            }
        }

        /// <summary>
        /// Scales the icon, which is the dominant element in the hero and flat designs.
        /// </summary>
        /// <remarks>
        /// Applied to <c>Size</c> and <c>MaximumSize</c> together: a BeepImage keeps its aspect ratio,
        /// and leaving the maximum where it was would clamp the new size straight back.
        /// </remarks>
        private static void Enlarge(Control? icon, float factor)
        {
            if (icon == null) return;

            var scaled = new Size(
                Math.Max(1, (int)Math.Round(icon.Width * factor)),
                Math.Max(1, (int)Math.Round(icon.Height * factor)));

            if (!icon.MaximumSize.IsEmpty) icon.MaximumSize = scaled;
            icon.MinimumSize = scaled;
            icon.Size = scaled;
        }

        private static TableLayoutPanel? FindFooter(TableLayoutPanel shell)
        {
            foreach (Control c in shell.Controls)
            {
                if (c is TableLayoutPanel panel && panel.Controls.Cast<Control>().Any(x => x is IButtonControl))
                {
                    return panel;
                }
            }

            return null;
        }
    }
}
