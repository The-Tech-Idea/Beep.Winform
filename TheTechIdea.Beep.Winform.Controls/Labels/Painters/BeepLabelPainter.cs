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
            if (state.HasImage && !context.ImageBounds.IsEmpty && !string.IsNullOrEmpty(state.ImagePath))
            {
                if (applyThemeOnImage)
                {
                    StyledImagePainter.PaintWithTint(g, context.ImageBounds, state.ImagePath, headerColor, 1f, 0);
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
                headerColor,
                BeepLabelLayoutHelper.GetTextFormatFlags(state));

            if (context.HasSubHeader && !string.IsNullOrEmpty(state.SubHeaderText))
            {
                TextRenderer.DrawText(
                    g,
                    state.SubHeaderText,
                    subHeaderFont,
                    context.SubHeaderBounds,
                    subHeaderColor,
                    BeepLabelLayoutHelper.GetTextFormatFlags(state));
            }
        }
    }
}
