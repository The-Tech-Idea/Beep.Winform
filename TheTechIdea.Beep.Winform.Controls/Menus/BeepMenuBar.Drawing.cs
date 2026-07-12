// BeepMenuBar.Drawing.cs
// Phase 02 — Partial-Class Split.
//
// Owns paint orchestration: the DrawContent override and the per-item
// rendering pipeline. Chrome (shadow → border → background) is delegated
// to BeepStyling.PaintControl which fans out to the established
// per-style factories — no parallel painter framework lives here.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        protected override void DrawContent(Graphics g)
        {
            DrawWithBeepStyling(g);
        }

        /// <summary>
        /// Draws every visible menu item via the established
        /// BeepStyling chrome pipeline (or the HC fallback when High
        /// Contrast mode is active — see <c>BeepMenuBar.HighContrast.cs</c>).
        /// </summary>
        private void DrawWithBeepStyling(Graphics g)
        {
            if (items == null || items.Count == 0) return;

            UpdateDrawingRect();

            var menuRects = CalculateMenuItemRects();
            bool hc = IsHighContrast;
            bool focusVisible = _activation != MenubarActivation.Inactive;

            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];
                string itemName = $"MenuItem_{i}";
                bool isHovered  = _hoveredMenuItemName == itemName;
                bool isSelected = _selectedIndex == i;

                if (hc)
                {
                    DrawMenuItemHighContrast(g, item, rect, isHovered, isSelected);
                }
                else
                {
                    DrawMenuItemWithBeepStyling(g, item, rect, isHovered, isSelected);
                }

                // Phase 07-C — focus rectangle on the keyboard-focused top
                // level item. HC uses a 3 px Highlight ring; non-HC uses
                // the standard ControlPaint dotted rectangle.
                if (focusVisible && isSelected)
                {
                    if (hc)
                    {
                        PaintFocusRectIfHC(g, rect);
                    }
                    else
                    {
                        var focusRect = Rectangle.Inflate(rect, -ScaleUi(2), -ScaleUi(2));
                        ControlPaint.DrawFocusRectangle(g, focusRect);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a single menu item. Hover takes visual priority over
        /// selection so a hover transition stays crisp.
        /// </summary>
        private void DrawMenuItemWithBeepStyling(
            Graphics g, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected)
        {
            if (item == null) return;

            try
            {
                var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();

                var itemPath = BeepStyling.CreateControlStylePath(rect, ControlStyle);
                if (itemPath == null) return;

                var itemState = ControlState.Normal;
                if (isHovered)       itemState = ControlState.Hovered;
                else if (isSelected) itemState = ControlState.Selected;

                var contentPath = BeepStyling.PaintControl(
                    g,
                    itemPath,
                    ControlStyle,
                    theme,
                    UseThemeColors,
                    itemState,
                    false,
                    ShowAllBorders
                );

                // Use the chrome-inset content path for text/icon placement when
                // available; fall back to the item rect itself for compact bars.
                if (contentPath != null)
                {
                    var boundsF = contentPath.GetBounds();
                    var cr = Rectangle.Round(boundsF);
                    // Guard: if the chrome path is virtually empty (e.g. card padding
                    // collapsed on a short bar), use a sensible fallback inset.
                    if (cr.Width > 4 && cr.Height > 4)
                        DrawMenuItemContent(g, item, cr, ControlStyle, theme, contentPath);
                    else
                    {
                        int pad = ScaleUi(4);
                        var fallback = new Rectangle(rect.X + pad, rect.Y, rect.Width - pad * 2, rect.Height);
                        DrawMenuItemContent(g, item, fallback, ControlStyle, theme, null);
                    }
                    contentPath.Dispose();
                }
                else
                {
                    int pad = ScaleUi(4);
                    var fallback = new Rectangle(rect.X + pad, rect.Y, rect.Width - pad * 2, rect.Height);
                    DrawMenuItemContent(g, item, fallback, ControlStyle, theme, null);
                }

                itemPath.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MenuBar drawing error: {ex.Message}");
                DrawMenuItemFallback(g, item, rect, isHovered, isSelected);
            }
        }

        /// <summary>
        /// Fallback drawing used only when the BeepStyling pipeline throws.
        /// </summary>
        private void DrawMenuItemFallback(
            Graphics g, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected)
        {
            var brush = new SolidBrush(isHovered ? Color.LightBlue : Color.White);
            g.FillRectangle(brush, rect);
            brush.Dispose();

            var pen = new Pen(Color.Gray, Math.Max(1, ScaleUi(1)));
            g.DrawRectangle(pen, rect);
            pen.Dispose();

            using (var textBrush = new SolidBrush(_currentTheme.MenuItemForeColor))
            using (var format    = new StringFormat())
            {
                format.Alignment    = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming     = StringTrimming.EllipsisCharacter;
                format.FormatFlags  = StringFormatFlags.NoWrap;

                RectangleF textRectF = rect;
                g.DrawString(item.Text ?? "", _textFont, textBrush, textRectF, format);
            }
        }

        /// <summary>
        /// Draws image + text inside the content rect returned by the
        /// chrome pipeline (so we never paint into border/shadow).
        /// </summary>
        private void DrawMenuItemContent(
            Graphics g,
            SimpleItem item,
            Rectangle rect,
            BeepControlStyle style,
            IBeepTheme theme,
            GraphicsPath contentPath)
        {
            Rectangle contentRect = rect;
            if (contentPath != null && contentPath.PointCount > 0)
            {
                var boundsF = contentPath.GetBounds();
                contentRect = Rectangle.Round(boundsF);
            }

            int gap             = ScaleUi(6);
            int scaledImageSize = ScaledImageSize;
            int imageAreaWidth  = !string.IsNullOrEmpty(item.ImagePath) ? scaledImageSize + gap : 0;
            int textStartX      = contentRect.X + imageAreaWidth;
            int textWidth       = Math.Max(0, contentRect.Width - imageAreaWidth);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imageRect = new Rectangle(
                    contentRect.X,
                    contentRect.Y + (contentRect.Height - scaledImageSize) / 2,
                    scaledImageSize,
                    scaledImageSize);

                var imagePath = BeepStyling.CreateControlStylePath(imageRect, style);
                BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, style);
                imagePath.Dispose();
            }

            var textRect = new Rectangle(textStartX, contentRect.Y, textWidth, contentRect.Height);

            Color textColor = theme != null
                ? theme.MenuItemForeColor
                : BeepStyling.GetForegroundColor(style);

            var baseFlags = TextFormatFlags.VerticalCenter
                          | TextFormatFlags.Left
                          | TextFormatFlags.EndEllipsis;

            var flags = _drawMnemonics
                ? baseFlags
                : baseFlags | TextFormatFlags.HidePrefix;

            TextRenderer.DrawText(
                g,
                item.Text ?? "",
                _textFont,
                textRect,
                textColor,
                flags);
        }
    }
}
