using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Labels.Models;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Helpers
{
    internal static class BeepLabelLayoutHelper
    {
        public static TextFormatFlags GetTextFormatFlags(BeepLabelState state)
        {
            TextFormatFlags flags = TextFormatFlags.PreserveGraphicsClipping;

            if (state.Multiline || state.WordWrap)
            {
                flags |= TextFormatFlags.WordBreak;
            }
            else
            {
                flags |= TextFormatFlags.SingleLine;
            }

            if (state.AutoEllipsis)
            {
                flags |= TextFormatFlags.EndEllipsis;
            }

            switch (state.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    return flags | TextFormatFlags.Left | TextFormatFlags.Top;
                case ContentAlignment.TopCenter:
                    return flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.Top;
                case ContentAlignment.TopRight:
                    return flags | TextFormatFlags.Right | TextFormatFlags.Top;
                case ContentAlignment.MiddleLeft:
                    return flags | TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                case ContentAlignment.MiddleCenter:
                    return flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                case ContentAlignment.MiddleRight:
                    return flags | TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                case ContentAlignment.BottomLeft:
                    return flags | TextFormatFlags.Left | TextFormatFlags.Bottom;
                case ContentAlignment.BottomCenter:
                    return flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
                case ContentAlignment.BottomRight:
                    return flags | TextFormatFlags.Right | TextFormatFlags.Bottom;
                default:
                    return flags;
            }
        }

        public static Rectangle AlignRectangle(Rectangle container, Size size, ContentAlignment alignment)
        {
            int x;
            int y;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    x = container.X;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    x = container.X + (container.Width - size.Width) / 2;
                    break;
                default:
                    x = container.Right - size.Width;
                    break;
            }

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = container.Y;
                    break;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    y = container.Y + (container.Height - size.Height) / 2;
                    break;
                default:
                    y = container.Bottom - size.Height;
                    break;
            }

            return new Rectangle(new Point(x, y), size);
        }

        public static void CalculateLayout(
            BeepLabel owner,
            BeepLabelState state,
            Rectangle contentRect,
            Size imageSize,
            Size textSize,
            out Rectangle imageRect,
            out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            int inflateVal = DpiScalingHelper.ScaleValue(2, owner);
            contentRect.Inflate(-inflateVal, -inflateVal);

            bool hasImage = state.HasImage && imageSize != Size.Empty;
            bool hasText = !string.IsNullOrEmpty(state.HeaderText) && !state.HideText;

            if (hasImage && !hasText)
            {
                imageRect = AlignRectangle(contentRect, imageSize, ContentAlignment.MiddleCenter);
                return;
            }

            if (hasText && !hasImage)
            {
                textRect = contentRect;
                return;
            }

            if (!hasImage || !hasText)
            {
                return;
            }

            switch (state.TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    imageRect = AlignRectangle(contentRect, imageSize, state.ImageAlign);
                    textRect = AlignRectangle(contentRect, textSize, state.TextAlign);
                    break;
                case TextImageRelation.ImageBeforeText:
                    imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, imageSize.Width, contentRect.Height), imageSize, state.ImageAlign);
                    textRect = AlignRectangle(new Rectangle(contentRect.Left + imageSize.Width, contentRect.Top, contentRect.Width - imageSize.Width, contentRect.Height), textSize, state.TextAlign);
                    break;
                case TextImageRelation.TextBeforeImage:
                    textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, textSize.Width, contentRect.Height), textSize, state.TextAlign);
                    imageRect = AlignRectangle(new Rectangle(contentRect.Left + textSize.Width, contentRect.Top, contentRect.Width - textSize.Width, contentRect.Height), imageSize, state.ImageAlign);
                    break;
                case TextImageRelation.ImageAboveText:
                    imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, imageSize.Height), imageSize, state.ImageAlign);
                    textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + imageSize.Height, contentRect.Width, contentRect.Height - imageSize.Height), textSize, state.TextAlign);
                    break;
                case TextImageRelation.TextAboveImage:
                    textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, textSize.Height), textSize, state.TextAlign);
                    imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + textSize.Height, contentRect.Width, contentRect.Height - textSize.Height), imageSize, state.ImageAlign);
                    break;
            }
        }
    }
}
