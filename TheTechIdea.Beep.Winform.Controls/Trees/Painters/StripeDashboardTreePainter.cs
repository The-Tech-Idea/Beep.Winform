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
 /// Stripe Dashboard tree painter.
 /// Features: Clean fintech design, data hierarchy, metric badges, professional spacing.
 /// </summary>
 public class StripeDashboardTreePainter : BaseTreePainter
 {
 private const int CornerRadius =6;
 private const int MetricBadgeWidth =40;

 private Font _regularFont;
 private Font _boldFont;

 public override void Initialize(BeepTree owner, IBeepTheme theme)
 {
 base.Initialize(owner, theme);
 _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
 if (_boldFont == null || _boldFont.Size != _regularFont.Size || !_boldFont.FontFamily.Equals(_regularFont.FontFamily))
 {
 try { _boldFont?.Dispose(); } catch { }
 _boldFont = new Font(_regularFont.FontFamily, _regularFont.Size, FontStyle.Bold);
 }
 }

 public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
 {
 if (g == null || node.Item == null) return;

 var oldSmoothing = g.SmoothingMode;
 var oldTextRendering = g.TextRenderingHint;
 g.SmoothingMode = SmoothingMode.AntiAlias;
 g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

 try
 {
 // STEP1: Draw Stripe clean rounded background with padding
 if (isSelected || isHovered)
 {
 Rectangle bgBounds = new Rectangle(
 nodeBounds.X +4,
 nodeBounds.Y +2,
 nodeBounds.Width -8,
 nodeBounds.Height -4);

 using (var bgPath = CreateRoundedRectangle(bgBounds, CornerRadius))
 {
 Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
 var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
 g.FillPath(bgBrush, bgPath);

 // Subtle accent border on selection (Stripe Style)
 if (isSelected)
 {
 var borderPen = PaintersFactory.GetPen(_theme.AccentColor,1f);
 g.DrawPath(borderPen, bgPath);
 }
 }
 }

 // STEP2: Draw Stripe chevron toggle
 bool hasChildren = node.Item.Children != null && node.Item.Children.Count >0;
 if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
 {
 var toggleRect = node.ToggleRectContent;
 Color chevronColor = _theme.TreeForeColor;

 var pen = PaintersFactory.GetPen(chevronColor,1.5f);
 pen.StartCap = LineCap.Round;
 pen.EndCap = LineCap.Round;

 int centerX = toggleRect.Left + toggleRect.Width /2;
 int centerY = toggleRect.Top + toggleRect.Height /2;
 int size = Math.Min(toggleRect.Width, toggleRect.Height) /3;

 if (node.Item.IsExpanded)
 {
 g.DrawLine(pen, centerX - size, centerY - size /2, centerX, centerY + size /2);
 g.DrawLine(pen, centerX, centerY + size /2, centerX + size, centerY - size /2);
 }
 else
 {
 g.DrawLine(pen, centerX - size /2, centerY - size, centerX + size /2, centerY);
 g.DrawLine(pen, centerX + size /2, centerY, centerX - size /2, centerY + size);
 }
 }

 // STEP3: Draw checkbox (clean Stripe Style)
 if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
 {
 var checkRect = node.CheckRectContent;
 var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
 var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

 using (var checkPath = CreateRoundedRectangle(checkRect,3))
 {
 var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
 g.FillPath(bgBrush, checkPath);

 var borderPen = PaintersFactory.GetPen(borderColor,1.5f);
 g.DrawPath(borderPen, checkPath);
 }

 if (node.Item.IsChecked)
 {
 var checkPen = PaintersFactory.GetPen(Color.White,1.5f);
 checkPen.StartCap = LineCap.Round;
 checkPen.EndCap = LineCap.Round;

 var points = new Point[]
 {
 new Point(checkRect.X + checkRect.Width /4, checkRect.Y + checkRect.Height /2),
 new Point(checkRect.X + checkRect.Width /2 -1, checkRect.Y + checkRect.Height *3 /4),
 new Point(checkRect.X + checkRect.Width *3 /4, checkRect.Y + checkRect.Height /4)
 };
 g.DrawLines(checkPen, points);
 }
 }

 // STEP4: Draw Stripe-Style icon (clean rounded with lines)
 if (node.IconRectContent != Rectangle.Empty)
 {
 var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
 Color iconColor = _theme.AccentColor;

 using (var iconPath = CreateRoundedRectangle(iconRect, iconRect.Width /4))
 {
 var fillBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(80, iconColor));
 g.FillPath(fillBrush, iconPath);
 }

 int padding = iconRect.Width /3;
 Rectangle innerRect = new Rectangle(
 iconRect.X + padding,
 iconRect.Y + padding,
 iconRect.Width - padding *2,
 iconRect.Height - padding *2);

 if (innerRect.Width >0 && innerRect.Height >0)
 {
 var pen = PaintersFactory.GetPen(iconColor,2f);
 pen.StartCap = LineCap.Round;
 pen.EndCap = LineCap.Round;

 int lineSpacing = innerRect.Height /3;
 g.DrawLine(pen, innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Top);
 g.DrawLine(pen, innerRect.Left, innerRect.Top + lineSpacing, innerRect.Right, innerRect.Top + lineSpacing);
 }
 }

 // STEP5: Draw text with bold on selection (Stripe typography)
 if (node.TextRectContent != Rectangle.Empty)
 {
 var textRect = node.TextRectContent;
 Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

 Rectangle adjustedTextRect = new Rectangle(
 textRect.X,
 textRect.Y,
 Math.Max(0, textRect.Width - MetricBadgeWidth),
 textRect.Height);

 var renderFont = isSelected ? _boldFont ?? _regularFont : _regularFont;
 TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, adjustedTextRect, textColor,
 TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
 }

 // STEP6: Draw metric badge (Stripe dashboard feature)
 if (node.TextRectContent != Rectangle.Empty)
 {
 Rectangle badgeRect = new Rectangle(
 node.TextRectContent.Right - MetricBadgeWidth,
 node.TextRectContent.Y + node.TextRectContent.Height /4,
 MetricBadgeWidth -4,
 node.TextRectContent.Height /2);

 if (badgeRect.Width >0 && badgeRect.Height >0)
 {
 Color badgeColor = isSelected ? _theme.AccentColor : Color.FromArgb(100, _theme.TreeForeColor);

 using (var badgePath = CreateRoundedRectangle(badgeRect,3))
 {
 var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, badgeColor));
 g.FillPath(brush, badgePath);
 }

 using (var textBrush = PaintersFactory.GetSolidBrush(badgeColor))
 {
 StringFormat sf = new StringFormat
 {
 Alignment = StringAlignment.Center,
 LineAlignment = StringAlignment.Center
 };

 string metricText = "99+";
 g.DrawString(metricText, new Font("Segoe UI",7f, FontStyle.Regular), textBrush, badgeRect, sf);
 }
 }
 }
 }
 finally
 {
 g.SmoothingMode = oldSmoothing;
 g.TextRenderingHint = oldTextRendering;
 }
 }

 private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
 {
 var path = new GraphicsPath();
 int diameter = radius *2;

 if (rect.Width < diameter || rect.Height < diameter || rect.Width <=0 || rect.Height <=0)
 {
 if (rect.Width >0 && rect.Height >0)
 path.AddRectangle(rect);
 return path;
 }

 path.AddArc(rect.X, rect.Y, diameter, diameter,180,90);
 path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter,270,90);
 path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter,0,90);
 path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter,90,90);
 path.CloseFigure();

 return path;
 }

 public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
 {
 if (string.IsNullOrEmpty(text) || textRect.Width <=0 || textRect.Height <=0) return;

 Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

 var renderFont = isSelected ? _boldFont ?? _regularFont : _regularFont;

 Rectangle adjustedTextRect = new Rectangle(
 textRect.X,
 textRect.Y,
 Math.Max(0, textRect.Width - MetricBadgeWidth),
 textRect.Height);

 TextRenderer.DrawText(g, text, renderFont, adjustedTextRect, textColor,
 TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

 PaintMetricBadge(g, textRect, isSelected);
 }

 private void PaintMetricBadge(Graphics g, Rectangle textRect, bool isSelected)
 {
 Rectangle badgeRect = new Rectangle(
 textRect.Right - MetricBadgeWidth,
 textRect.Y + textRect.Height /4,
 MetricBadgeWidth -4,
 textRect.Height /2);

 if (badgeRect.Width >0 && badgeRect.Height >0)
 {
 Color badgeColor = isSelected ? _theme.AccentColor : Color.FromArgb(100, _theme.TreeForeColor);

 using (var path = CreateRoundedRectangle(badgeRect,3))
 {
 var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, badgeColor));
 g.FillPath(brush, path);
 }

 using (var textBrush = PaintersFactory.GetSolidBrush(badgeColor))
 {
 StringFormat sf = new StringFormat
 {
 Alignment = StringAlignment.Center,
 LineAlignment = StringAlignment.Center
 };

 using (var metricFont = new Font("Segoe UI",7f, FontStyle.Regular))
 {
 g.DrawString("99+", metricFont, textBrush, badgeRect, sf);
 }
 }
 }
 }

 public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
 {
 if (g == null || owner == null || bounds.Width <=0 || bounds.Height <=0) return;

 var brush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
 g.FillRectangle(brush, bounds);

 g.SmoothingMode = SmoothingMode.AntiAlias;
 g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

 base.Paint(g, owner, bounds);
 }

 public override int GetPreferredRowHeight(SimpleItem item, Font font)
 {
 return Math.Max(32, base.GetPreferredRowHeight(item, font));
 }
 }
}
