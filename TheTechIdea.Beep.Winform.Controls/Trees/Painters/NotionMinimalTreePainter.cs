using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Notion minimal tree painter.
    /// Features: Clean design, subtle hover states, emoji icons, minimal spacing.
    /// </summary>
    public class NotionMinimalTreePainter : BaseTreePainter
    {
        private const int MinimalPadding = 2;

        private Font _regularFont;
        private Font _emojiFont;

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
            try { _emojiFont?.Dispose(); } catch { }
            _emojiFont = new Font("Segoe UI Emoji", Math.Max(6f, _regularFont.Size * 0.6f), FontStyle.Regular);
        }

        /// <summary>
        /// Notion-specific node painting with minimal clean design.
        /// Features: Bottom accent line (2px on selection), flat backgrounds, simple arrow toggles, emoji-Style icons, compact spacing.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Notion clean appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Notion flat background (very subtle)
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    g.FillRectangle(bgBrush, nodeBounds);

                    // STEP 2: Notion bottom accent line (distinctive feature)
                    if (isSelected)
                    {
                        // Bottom accent line (2px thick)
                        var accentPen = PaintersFactory.GetPen(_theme.AccentColor, 2f);
                        g.DrawLine(accentPen,
                            nodeBounds.Left, nodeBounds.Bottom - 1,
                            nodeBounds.Right, nodeBounds.Bottom - 1);
                    }
                }

                // STEP 3: Draw Notion simple arrow toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    var pen = PaintersFactory.GetPen(_theme.TreeForeColor, 1.5f);
                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                    if (node.Item.IsExpanded)
                    {
                        // Arrow down (simple V shape)
                        g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                        g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                    }
                    else
                    {
                        // Arrow right (simple > shape)
                        g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                        g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                    }
                }

                // STEP 4: Draw Notion minimal checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var bgBrush = PaintersFactory.GetSolidBrush(node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor);
                    g.FillRectangle(bgBrush, checkRect);
                    var borderPen = PaintersFactory.GetPen(node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor, 1f);
                    g.DrawRectangle(borderPen, checkRect);

                    // Simple checkmark
                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White, 1.5f);
                        var points = new Point[]
                        {
                            new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                            new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4),
                            new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4)
                        };
                        g.DrawLines(checkPen, points);
                    }
                }

                // STEP 5: Draw Notion emoji-Style icon
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }
                else if (node.IconRectContent != Rectangle.Empty)
                {
                    // Draw default emoji-Style icon
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    var bgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, _theme.AccentColor));
                    g.FillRectangle(bgBrush, iconRect);

                    var textBrush = PaintersFactory.GetSolidBrush(_theme.TreeForeColor);
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("📄", _emojiFont, textBrush, iconRect, sf);
                }

                // STEP 6: Draw text with Notion clean typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    var renderFont = _regularFont ?? SystemFonts.DefaultFont;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
            var renderFont = _regularFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
    }
}
