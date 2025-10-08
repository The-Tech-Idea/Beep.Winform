using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Standard/classic Windows tree view painter.
    /// Features: Windows Explorer style with tree lines, plus/minus toggles, standard checkboxes.
    /// </summary>
    public class StandardTreePainter : BaseTreePainter
    {
        /// <summary>
        /// Standard Windows tree painting with classic Explorer style.
        /// Features: Tree lines connecting nodes, plus/minus box toggles, standard checkboxes, simple icons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw standard background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, nodeBounds);
                    }
                }

                // STEP 2: Draw plus/minus toggle (classic Windows style)
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    
                    // Draw box
                    using (var boxBrush = new SolidBrush(_theme.TreeBackColor))
                    {
                        g.FillRectangle(boxBrush, toggleRect);
                    }

                    using (var borderPen = new Pen(_theme.TreeForeColor, 1f))
                    {
                        g.DrawRectangle(borderPen, toggleRect);
                    }

                    // Draw plus or minus
                    using (var pen = new Pen(_theme.TreeForeColor, 1f))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        // Horizontal line (both plus and minus have this)
                        g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);

                        // Vertical line (only for plus/collapsed)
                        if (!node.Item.IsExpanded)
                        {
                            g.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                        }
                    }
                }

                // STEP 3: Draw standard checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, checkRect);
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
                    }

                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 2f))
                        {
                            var points = new Point[]
                            {
                                new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                                new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4),
                                new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4)
                            };
                            g.DrawLines(checkPen, points);
                        }
                    }
                }

                // STEP 4: Draw simple icon
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = node.IconRectContent;
                    Color iconColor = _theme.AccentColor;

                    // Simple square icon with border
                    using (var bgBrush = new SolidBrush(Color.FromArgb(50, iconColor)))
                    {
                        g.FillRectangle(bgBrush, iconRect);
                    }

                    using (var borderPen = new Pen(iconColor, 1f))
                    {
                        g.DrawRectangle(borderPen, iconRect);
                    }
                }

                // STEP 5: Draw text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _owner.TextFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }
    }
}
