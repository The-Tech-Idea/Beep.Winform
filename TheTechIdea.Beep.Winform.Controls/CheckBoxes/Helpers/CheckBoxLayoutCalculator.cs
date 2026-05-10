using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers
{
    /// <summary>
    /// The computed geometry for a single checkbox render pass.
    /// Produced by <see cref="CheckBoxLayoutCalculator"/> and consumed by the draw path and painters.
    /// Storing this as the authoritative output means the draw path and glyph hit-testing always
    /// use the same rectangle rather than recomputing independently.
    /// </summary>
    public readonly struct CheckBoxLayoutContext
    {
        /// <summary>Bounding rectangle of the painted checkbox glyph.</summary>
        public readonly Rectangle CheckBoxRect;

        /// <summary>Bounding rectangle of the text label. Empty when text is hidden or absent.</summary>
        public readonly Rectangle TextRect;

        /// <summary>True when the text label will be rendered.</summary>
        public readonly bool IsTextVisible;

        /// <summary>True when the layout was produced for grid/cell mode (simplified geometry).</summary>
        public readonly bool IsGridMode;

        /// <summary>True when right-to-left layout was applied.</summary>
        public readonly bool IsRtl;

        public CheckBoxLayoutContext(
            Rectangle checkBoxRect,
            Rectangle textRect,
            bool isTextVisible,
            bool isGridMode,
            bool isRtl)
        {
            CheckBoxRect = checkBoxRect;
            TextRect = textRect;
            IsTextVisible = isTextVisible;
            IsGridMode = isGridMode;
            IsRtl = isRtl;
        }
    }

    /// <summary>
    /// Computes checkbox layout geometry from control state.
    /// All rectangle math lives here so the draw path, grid path, and glyph hit-testing
    /// share one authoritative geometry path rather than each maintaining their own calculation.
    /// </summary>
    public static class CheckBoxLayoutCalculator
    {
        /// <summary>
        /// Compute normal (non-grid) layout for a checkbox control.
        /// </summary>
        /// <param name="controlRect">The drawing rectangle passed into Draw().</param>
        /// <param name="scaledCheckBoxSize">DPI-scaled checkbox glyph size.</param>
        /// <param name="scaledSpacing">DPI-scaled spacing between glyph and text.</param>
        /// <param name="padding">Control padding (left/top/right/bottom).</param>
        /// <param name="hideText">Whether text is suppressed regardless of Text value.</param>
        /// <param name="text">Text label; may be null or empty.</param>
        /// <param name="font">Font used for text measurement.</param>
        /// <param name="graphics">Graphics context for TextRenderer.MeasureText.</param>
        /// <param name="textAlignment">Requested glyph-to-text alignment.</param>
        /// <param name="rightToLeft">True when the control is in right-to-left layout mode.</param>
        public static CheckBoxLayoutContext Compute(
            Rectangle controlRect,
            int scaledCheckBoxSize,
            int scaledSpacing,
            Padding padding,
            bool hideText,
            string text,
            Font font,
            Graphics graphics,
            TextAlignment textAlignment,
            bool rightToLeft = false)
        {
            bool hasText = !hideText && !string.IsNullOrEmpty(text);

            // Clamp the glyph so it can never exceed the available drawing area
            int maxGlyphSize = Math.Min(
                controlRect.Width - padding.Horizontal,
                controlRect.Height - padding.Vertical);
            int glyphSize = Math.Max(1, Math.Min(scaledCheckBoxSize, maxGlyphSize));

            // Measure text only when it will be shown
            Size textSize = hasText
                ? TextRenderer.MeasureText(graphics, text, font, controlRect.Size, TextFormatFlags.SingleLine)
                : Size.Empty;

            Rectangle checkBoxRect;
            Rectangle textRect = Rectangle.Empty;

            if (!hasText)
            {
                // Center the glyph in the available space when there is no text
                checkBoxRect = new Rectangle(
                    controlRect.X + (controlRect.Width - glyphSize) / 2,
                    controlRect.Y + (controlRect.Height - glyphSize) / 2,
                    glyphSize, glyphSize);

                return new CheckBoxLayoutContext(checkBoxRect, Rectangle.Empty, false, false, rightToLeft);
            }

            // When RTL is active, mirror Left/Right alignment semantics
            TextAlignment resolvedAlignment = textAlignment;
            if (rightToLeft)
            {
                resolvedAlignment = textAlignment switch
                {
                    TextAlignment.Right => TextAlignment.Left,
                    TextAlignment.Left => TextAlignment.Right,
                    _ => textAlignment
                };
            }

            switch (resolvedAlignment)
            {
                case TextAlignment.Right:
                    // [glyph] [text →]
                    checkBoxRect = new Rectangle(
                        controlRect.X + padding.Left,
                        controlRect.Y + (controlRect.Height - glyphSize) / 2,
                        glyphSize, glyphSize);
                    textRect = new Rectangle(
                        checkBoxRect.Right + scaledSpacing,
                        controlRect.Y + padding.Top,
                        Math.Max(0, controlRect.Right - padding.Right - (checkBoxRect.Right + scaledSpacing)),
                        Math.Max(0, controlRect.Height - padding.Vertical));
                    break;

                case TextAlignment.Left:
                    // [← text] [glyph]
                    int glyphXLeft = controlRect.Right - padding.Right - glyphSize;
                    checkBoxRect = new Rectangle(
                        glyphXLeft,
                        controlRect.Y + (controlRect.Height - glyphSize) / 2,
                        glyphSize, glyphSize);
                    textRect = new Rectangle(
                        controlRect.X + padding.Left,
                        controlRect.Y + padding.Top,
                        Math.Max(0, glyphXLeft - scaledSpacing - (controlRect.X + padding.Left)),
                        Math.Max(0, controlRect.Height - padding.Vertical));
                    break;

                case TextAlignment.Above:
                    // [↑ text]
                    // [glyph]
                    int aboveTextW = Math.Max(0, controlRect.Width - padding.Horizontal);
                    textRect = new Rectangle(
                        controlRect.X + padding.Left,
                        controlRect.Y + padding.Top,
                        aboveTextW, textSize.Height);
                    checkBoxRect = new Rectangle(
                        controlRect.X + (controlRect.Width - glyphSize) / 2,
                        textRect.Bottom + scaledSpacing,
                        glyphSize, glyphSize);
                    break;

                case TextAlignment.Below:
                    // [glyph]
                    // [↓ text]
                    checkBoxRect = new Rectangle(
                        controlRect.X + (controlRect.Width - glyphSize) / 2,
                        controlRect.Y + padding.Top,
                        glyphSize, glyphSize);
                    int belowTextW = Math.Max(0, controlRect.Width - padding.Horizontal);
                    textRect = new Rectangle(
                        controlRect.X + padding.Left,
                        checkBoxRect.Bottom + scaledSpacing,
                        belowTextW, textSize.Height);
                    break;

                default:
                    // Fallback: glyph at origin
                    checkBoxRect = new Rectangle(
                        controlRect.X + padding.Left,
                        controlRect.Y + (controlRect.Height - glyphSize) / 2,
                        glyphSize, glyphSize);
                    textRect = new Rectangle(
                        checkBoxRect.Right + scaledSpacing,
                        controlRect.Y + padding.Top,
                        Math.Max(0, controlRect.Right - padding.Right - (checkBoxRect.Right + scaledSpacing)),
                        Math.Max(0, controlRect.Height - padding.Vertical));
                    break;
            }

            return new CheckBoxLayoutContext(checkBoxRect, textRect, true, false, rightToLeft);
        }

        /// <summary>
        /// Compute simplified grid-cell layout for a checkbox rendered inside a grid/table cell.
        /// Text is always positioned to the right of the glyph when present.
        /// </summary>
        /// <param name="cellRect">The drawing rectangle of the grid cell.</param>
        /// <param name="scaledGlyphSize">DPI-scaled glyph size; clamped to a grid-appropriate max (16px).</param>
        /// <param name="hideText">Whether text is suppressed.</param>
        /// <param name="text">Text label; may be null or empty.</param>
        /// <param name="scaledSpacing">DPI-scaled spacing between glyph and text.</param>
        public static CheckBoxLayoutContext ComputeGrid(
            Rectangle cellRect,
            int scaledGlyphSize,
            bool hideText,
            string text,
            int scaledSpacing)
        {
            // Grid cells cap the glyph at 16 DIP to keep row height stable
            int glyphSize = Math.Min(scaledGlyphSize, Math.Min(cellRect.Width, cellRect.Height) - 4);
            glyphSize = Math.Max(1, glyphSize);

            bool hasText = !hideText && !string.IsNullOrEmpty(text);

            Rectangle checkBoxRect;

            if (!hasText)
            {
                checkBoxRect = new Rectangle(
                    cellRect.X + (cellRect.Width - glyphSize) / 2,
                    cellRect.Y + (cellRect.Height - glyphSize) / 2,
                    glyphSize, glyphSize);

                return new CheckBoxLayoutContext(checkBoxRect, Rectangle.Empty, false, true, false);
            }

            // Glyph flush-left with a small inset; text fills remaining width
            checkBoxRect = new Rectangle(
                cellRect.X + 2,
                cellRect.Y + (cellRect.Height - glyphSize) / 2,
                glyphSize, glyphSize);

            Rectangle textRect = new Rectangle(
                checkBoxRect.Right + scaledSpacing,
                cellRect.Y,
                Math.Max(0, cellRect.Right - (checkBoxRect.Right + scaledSpacing)),
                cellRect.Height);

            return new CheckBoxLayoutContext(checkBoxRect, textRect, true, true, false);
        }
    }
}
