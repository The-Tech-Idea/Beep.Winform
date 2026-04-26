using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Labels.Helpers;
using TheTechIdea.Beep.Winform.Controls.Labels.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Painters
{
    internal static class BeepLabelPainter
    {
        public static void Paint(
            Graphics g,
            BeepLabel owner,
            BeepLabelState state,
            BeepLabelLayoutContext context,
            Font headerFont,
            Font subHeaderFont,
            Color headerColor,
            Color subHeaderColor,
            bool applyThemeOnImage)
        {
            var bounds = context.ContentBounds;
            if (bounds.IsEmpty) bounds = owner.ClientRectangle;

            if (state.IsPressed && owner.SelectedBackColor != Color.Empty)
            {
                using var bgBrush = new SolidBrush(owner.SelectedBackColor);
                g.FillRectangle(bgBrush, bounds);
            }
            else if (state.IsHovered && owner.HoverBackColor != Color.Empty)
            {
                using var bgBrush = new SolidBrush(owner.HoverBackColor);
                g.FillRectangle(bgBrush, bounds);
            }

            Color effectiveHeaderColor = headerColor;
            Color effectiveSubHeaderColor = subHeaderColor;

            if (state.IsPressed && owner.SelectedForeColor != Color.Empty)
            {
                effectiveHeaderColor = owner.SelectedForeColor;
                effectiveSubHeaderColor = owner.SelectedForeColor;
            }
            else if (state.IsHovered && owner.HoverForeColor != Color.Empty)
            {
                effectiveHeaderColor = owner.HoverForeColor;
                effectiveSubHeaderColor = owner.HoverForeColor;
            }

            if (state.HasImage && !context.ImageBounds.IsEmpty && !string.IsNullOrEmpty(state.ImagePath))
            {
                if (applyThemeOnImage)
                {
                    StyledImagePainter.PaintWithTint(g, context.ImageBounds, state.ImagePath, effectiveHeaderColor, 1f, 0);
                }
                else
                {
                    StyledImagePainter.Paint(g, context.ImageBounds, state.ImagePath);
                }
            }

            if (state.HideText || string.IsNullOrEmpty(state.HeaderText))
            {
                return;
            }

            TextRenderer.DrawText(
                g,
                state.HeaderText,
                headerFont,
                context.HeaderBounds,
                effectiveHeaderColor,
                BeepLabelLayoutHelper.GetTextFormatFlags(state));

            if (context.HasSubHeader && !string.IsNullOrEmpty(state.SubHeaderText))
            {
                TextRenderer.DrawText(
                    g,
                    state.SubHeaderText,
                    subHeaderFont,
                    context.SubHeaderBounds,
                    effectiveSubHeaderColor,
                    BeepLabelLayoutHelper.GetTextFormatFlags(state));
            }
        }
    }
}
