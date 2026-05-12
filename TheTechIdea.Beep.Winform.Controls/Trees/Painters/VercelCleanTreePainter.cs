using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Vercel clean tree painter.
    /// Features: Ultra-minimal, monospace text, subtle borders, clean spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class VercelCleanTreePainter : BaseTreePainter
    {
        private const int BorderWidth = 1;

        private Font _monoFont;

        /// <summary>
        /// Initialize the painter, creating a cached monospace font.
        /// </summary>
        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _monoFont?.Dispose(); } catch { }
            _monoFont = new Font("Consolas", owner?.TextFont?.Size ?? SystemFonts.DefaultFont.Size, FontStyle.Regular);
        }

        /// <summary>
        /// Vercel-specific node painting with ultra-minimal clean design.
        /// Features: Top accent border (2px on selection), flat backgrounds, plus/minus toggles, monospace fonts, compact spacing.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Delegate to base for multi-column support
            if (_owner?.IsMultiColumn == true)
            {
                base.PaintNode(g, node, nodeBounds, isHovered, isSelected);
                return;
            }

            // Enable high-quality rendering for Vercel clean appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Vercel flat background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? GetSelectedBackColor() : GetHoverBackColor();
                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    g.FillRectangle(bgBrush, nodeBounds);

                    // STEP 2: Vercel top accent border (distinctive feature)
                    if (isSelected)
                    {
                        // Top accent line (2px thick)
                        var accentPen = PaintersFactory.GetPen(_theme.AccentColor, 2f);
                        g.DrawLine(accentPen, 
                            nodeBounds.Left, nodeBounds.Top, 
                            nodeBounds.Right, nodeBounds.Top);
                    }
                    else if (isHovered)
                    {
                        // Subtle border on hover
                        var hoverPen = PaintersFactory.GetPen(Color.FromArgb(40, _theme.BorderColor), 1f);
                        g.DrawRectangle(hoverPen, nodeBounds);
                    }
                }

                // STEP 3: Draw Vercel plus/minus toggle (minimalist)
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color iconColor = _theme.TreeForeColor;

                    DrawPlusMinus(g, toggleRect, iconColor, 1.5f, node.Item.IsExpanded);
                }

                // STEP 4: Draw Vercel minimal checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    g.FillRectangle(bgBrush, checkRect);

                    var borderPen = PaintersFactory.GetPen(borderColor, 1f);
                    g.DrawRectangle(borderPen, checkRect);

                    // Minimalist checkmark
                    if (node.Item.IsChecked)
                    {
                        DrawCheckmark(g, checkRect, Color.White, 1.5f);
                    }
                }

                // STEP 5: Draw Vercel minimal outlined icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 6: Draw text with Vercel monospace typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? GetSelectedForeColor() : _theme.TreeForeColor;

                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _monoFont ?? _owner.TextFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: flat with left accent border
                var brush = PaintersFactory.GetSolidBrush(GetSelectedBackColor());
                g.FillRectangle(brush, nodeBounds);

                // Left accent border
                using (var pen = new Pen(_theme.AccentColor, 2f))
                {
                    g.DrawLine(pen, nodeBounds.Left, nodeBounds.Top, nodeBounds.Left, nodeBounds.Bottom);
                }
            }
            else if (isHovered)
            {
                // Hover: very subtle border
                var hoverBrush = PaintersFactory.GetSolidBrush(GetHoverBackColor());
                g.FillRectangle(hoverBrush, nodeBounds);

                using (var pen = new Pen(Color.FromArgb(30, _theme.BorderColor), BorderWidth))
                {
                    g.DrawRectangle(pen, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Vercel Style: minimalist plus/minus
            Color iconColor = _theme.TreeForeColor;

            DrawPlusMinus(g, toggleRect, iconColor, 1.5f, isExpanded);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.VercelClean);
                    return;
                }
                catch { }
            }

            // Default: minimal outlined icon
            PaintDefaultVercelIcon(g, iconRect);
        }

        private void PaintDefaultVercelIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.TreeForeColor;

            // Simple outlined square
            int padding = iconRect.Width / 4;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            var pen = PaintersFactory.GetPen(iconColor, 1f);
            g.DrawRectangle(pen, innerRect);

            // Diagonal line for "folder"
            g.DrawLine(pen, innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Bottom);
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? GetSelectedForeColor() : _theme.TreeForeColor;
            var fontToUse = _monoFont ?? font;
            TextRenderer.DrawText(g, text, fontToUse, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var brush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
            g.FillRectangle(brush, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Vercel compact spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
