using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Labels.Helpers;
using TheTechIdea.Beep.Winform.Controls.Labels.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Painters
{
    internal static class BeepLabelPainter
    {
        // Process-lifetime shared brush cache (UI-thread only). Never dispose these.
        private static readonly Dictionary<int, SolidBrush> _brushCache = new();

        private static SolidBrush GetBrush(Color color)
        {
            int key = color.ToArgb();
            if (_brushCache.TryGetValue(key, out var b) && b != null) return b;
            b = new SolidBrush(color);
            _brushCache[key] = b;
            return b;
        }

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
                g.FillRectangle(GetBrush(owner.SelectedBackColor), bounds);
            }
            else if (state.IsHovered && owner.HoverBackColor != Color.Empty)
            {
                g.FillRectangle(GetBrush(owner.HoverBackColor), bounds);
            }

            // Style variant shape background (rounded, pill, badge, chip, code block)
            owner.DrawStyleShape(g, bounds);

            // Visual effect overlay (glow, raised, gradient, accent bar, shimmer)
            owner.DrawStyleEffect(g, bounds);

            Color effectiveHeaderColor = owner.ApplyAutoContrast(headerColor);
            Color effectiveSubHeaderColor = owner.ApplyAutoContrast(subHeaderColor);

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

            var headerDrawRect = context.HasSubHeader
                ? context.HeaderBounds
                : context.TextBounds;

            TextRenderer.DrawText(
                g,
                state.HeaderText,
                headerFont,
                headerDrawRect,
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
