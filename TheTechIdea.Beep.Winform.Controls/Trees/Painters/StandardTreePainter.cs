using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Standard/classic Windows tree view painter.
    /// Features: Windows Explorer Style with tree lines, plus/minus toggles, standard checkboxes.
    /// </summary>
    public class StandardTreePainter : BaseTreePainter
    {
        /// <summary>
        /// Standard Windows tree painting with classic Explorer Style.
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
                // STEP1: Draw standard background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    g.FillRectangle(bgBrush, nodeBounds);
                }

                // STEP2: Draw plus/minus toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    var boxBrush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
                    g.FillRectangle(boxBrush, toggleRect);
                    var borderPen = PaintersFactory.GetPen(_theme.TreeForeColor, 1f);
                    g.DrawRectangle(borderPen, toggleRect);

                    var pen = PaintersFactory.GetPen(_theme.TreeForeColor, 1f);
                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;
                    g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                    if (!node.Item.IsExpanded)
                    {
                        g.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                    }
                }

                // STEP3: Draw checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    var borderPen = PaintersFactory.GetPen(borderColor, 1f);
                    g.FillRectangle(bgBrush, checkRect);
                    g.DrawRectangle(borderPen, checkRect);

                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White, 2f);
                        var points = new Point[]
                        {
                            new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                            new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4),
                            new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4)
                        };
                        g.DrawLines(checkPen, points);
                    }
                }

                // STEP4: Draw icon
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    if (!string.IsNullOrEmpty(node.Item.ImagePath))
                    {
                        try { StyledImagePainter.Paint(g, iconRect, node.Item.ImagePath); }
                        catch { }
                    }
                    else
                    {
                        Color iconColor = _theme.AccentColor;
                        var bgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(50, iconColor));
                        g.FillRectangle(bgBrush, iconRect);
                        var borderPen = PaintersFactory.GetPen(iconColor, 1f);
                        g.DrawRectangle(borderPen, iconRect);
                    }
                }

                // STEP5: Draw text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
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

            var bgBrush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
            g.FillRectangle(bgBrush, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }
    }
}
