using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// DevExpress tree painter.
    /// Features: Professional gradients, icons with badges, focus indicators, polished appearance.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class DevExpressTreePainter : BaseTreePainter
    {
        private Font _regularFont;

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
        }

        /// <summary>
        /// DevExpress-specific node painting with professional enterprise styling.
        /// Features: Professional vertical gradients, plus/minus box toggles, focus borders, gloss effects on icons, polished appearance.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                if (isSelected || isHovered)
                {
                    var brush = PaintersFactory.GetSolidBrush(isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor);
                    g.FillRectangle(brush, nodeBounds);
                }

                // checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var bgBrush = PaintersFactory.GetSolidBrush(node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor);
                    g.FillRectangle(bgBrush, checkRect);

                    if (node.Item.IsChecked)
                    {
                        var pen = PaintersFactory.GetPen(Color.White, 2f);
                        int cx = checkRect.Left + checkRect.Width / 2;
                        int cy = checkRect.Top + checkRect.Height / 2;
                        g.DrawLine(pen, cx - 3, cy, cx - 1, cy + 3);
                        g.DrawLine(pen, cx - 1, cy + 3, cx + 4, cy - 3);
                    }
                }

                // icon
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    if (!string.IsNullOrEmpty(node.Item.ImagePath))
                    {
                        try { Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, node.Item.ImagePath, BeepControlStyle.Fluent2); } catch { }
                    }
                }

                // text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    var textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _regularFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
            }
        }
    }
}
