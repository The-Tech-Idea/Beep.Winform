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
 /// Ant Design tree painter.
 /// Features: Clean lines, checkbox support, folder icons, proper spacing.
 /// Uses theme colors for consistent appearance across light/dark themes.
 /// </summary>
 public class AntDesignTreePainter : BaseTreePainter
 {
 private const int ItemPadding =4;

 private Font _regularFont;

 public override void Initialize(BeepTree owner, IBeepTheme theme)
 {
 base.Initialize(owner, theme);
 _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
 }

 /// <summary>
 /// Ant Design-specific node painting with clean, flat design.
 /// Features: Flat rectangular backgrounds, caret toggles, clean checkboxes with1px borders, minimal styling.
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

 // Enable anti-aliasing for clean Ant Design appearance
 var oldSmoothing = g.SmoothingMode;
 var oldTextRendering = g.TextRenderingHint;
 g.SmoothingMode = SmoothingMode.AntiAlias;
 g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

 try
 {
 // STEP1: Draw flat rectangular background (no rounded corners)
 if (isSelected)
 {
 var bgBrush = PaintersFactory.GetSolidBrush(GetSelectedBackColor());
 g.FillRectangle(bgBrush, nodeBounds);

 var accentPen = PaintersFactory.GetPen(_theme.AccentColor,2f);
 g.DrawLine(accentPen, nodeBounds.Left, nodeBounds.Top, nodeBounds.Left, nodeBounds.Bottom);
 }
 else if (isHovered)
 {
 var hoverBrush = PaintersFactory.GetSolidBrush(GetHoverBackColor());
 g.FillRectangle(hoverBrush, nodeBounds);
 }

  // STEP2: Draw Ant Design caret toggle
  bool hasChildren = node.Item.Children != null && node.Item.Children.Count >0;
  if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
  {
  // CRITICAL: Transform to viewport coordinates
  var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
  Color caretColor = _theme.TreeForeColor;

  DrawChevron(g, toggleRect, caretColor, 1.5f, node.Item.IsExpanded);
  }

 // STEP3: Draw Ant Design checkbox (clean rectangular with thin border)
 if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
 {
 // CRITICAL: Transform to viewport coordinates
 var checkboxRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
 Color borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
 Color fillColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

 Rectangle checkRect = new Rectangle(
 checkboxRect.X +2,
 checkboxRect.Y +2,
 Math.Max(0, checkboxRect.Width -4),
 Math.Max(0, checkboxRect.Height -4));

 var borderPen = PaintersFactory.GetPen(borderColor,1f);
 g.DrawRectangle(borderPen, checkRect);

 if (node.Item.IsChecked)
 {
 var fillBrush = PaintersFactory.GetSolidBrush(fillColor);
 Rectangle fillRect = new Rectangle(
 checkboxRect.X +3,
 checkboxRect.Y +3,
 Math.Max(0, checkboxRect.Width -5),
 Math.Max(0, checkboxRect.Height -5));

  g.FillRectangle(fillBrush, fillRect);

  using (var checkPen = (Pen)PaintersFactory.GetPen(Color.White, 2f).Clone())
  {
  checkPen.StartCap = LineCap.Round;
  checkPen.EndCap = LineCap.Round;

  int centerX = checkboxRect.Left + checkboxRect.Width /2;
  int centerY = checkboxRect.Top + checkboxRect.Height /2;

  g.DrawLine(checkPen, centerX -4, centerY, centerX -1, centerY +3);
  g.DrawLine(checkPen, centerX -1, centerY +3, centerX +4, centerY -3);
  }
  }
  }

  // STEP4: Draw Ant Design folder icon
 if (node.IconRectContent != Rectangle.Empty)
 {
 var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
 if (!string.IsNullOrEmpty(node.Item.ImagePath))
 PaintIcon(g, iconRect, node.Item.ImagePath);
 }

 // STEP5: Draw text with Ant Design typography
 if (node.TextRectContent != Rectangle.Empty)
 {
 // CRITICAL: Transform to viewport coordinates
 var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
 Color textColor = isSelected ? GetSelectedForeColor() : _theme.TreeForeColor;

 TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _regularFont, textRect, textColor,
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
 if (nodeBounds.Width <=0 || nodeBounds.Height <=0) return;

 if (isSelected)
 {
 var brush = PaintersFactory.GetSolidBrush(GetSelectedBackColor());
 g.FillRectangle(brush, nodeBounds);
 }
 else if (isHovered)
 {
 var hoverBrush = PaintersFactory.GetSolidBrush(GetHoverBackColor());
 g.FillRectangle(hoverBrush, nodeBounds);
 }
 }

  public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
  {
  if (!hasChildren || toggleRect.Width <=0 || toggleRect.Height <=0) return;

  Color caretColor = _theme.TreeForeColor;

  DrawChevron(g, toggleRect, caretColor, 1.5f, isExpanded);
  }

 public override void PaintCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isIndeterminate, bool isHovered)
 {
 if (checkboxRect.Width <=0 || checkboxRect.Height <=0) return;

 Color borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
 Color fillColor = isChecked || isIndeterminate ? _theme.AccentColor : _theme.TreeBackColor;

 var pen = PaintersFactory.GetPen(borderColor,1f);
 Rectangle checkRect = new Rectangle(
 checkboxRect.X +2,
 checkboxRect.Y +2,
 Math.Max(0, checkboxRect.Width -4),
 Math.Max(0, checkboxRect.Height -4));

 g.DrawRectangle(pen, checkRect);

 if (isChecked || isIndeterminate)
 {
 var brush = PaintersFactory.GetSolidBrush(fillColor);
 Rectangle fillRect = new Rectangle(
 checkboxRect.X +3,
 checkboxRect.Y +3,
 Math.Max(0, checkboxRect.Width -5),
 Math.Max(0, checkboxRect.Height -5));

  g.FillRectangle(brush, fillRect);

  if (isChecked)
  {
    using (var checkPen = (Pen)PaintersFactory.GetPen(Color.White, 2f).Clone())
    {
    checkPen.StartCap = LineCap.Round;
    checkPen.EndCap = LineCap.Round;

    int centerX = checkboxRect.Left + checkboxRect.Width /2;
    int centerY = checkboxRect.Top + checkboxRect.Height /2;

    g.DrawLine(checkPen, centerX -4, centerY, centerX -1, centerY +3);
    g.DrawLine(checkPen, centerX -1, centerY +3, centerX +4, centerY -3);
    }
  }
  else if (isIndeterminate)
  {
    // Draw indeterminate dash
    var dashPen = PaintersFactory.GetPen(Color.White, 2f);
    int centerY = checkboxRect.Top + checkboxRect.Height / 2;
    int dashPadding = checkboxRect.Width / 4;
    g.DrawLine(dashPen, checkboxRect.X + dashPadding, centerY, checkboxRect.Right - dashPadding, centerY);
  }
  }
  }

  public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
 {
 if (iconRect.Width <=0 || iconRect.Height <=0) return;

 if (!string.IsNullOrEmpty(imagePath))
 {
 try
 {
 Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.AntDesign);
 return;
 }
 catch { }
 }

 PaintDefaultAntIcon(g, iconRect);
 }

 private void PaintDefaultAntIcon(Graphics g, Rectangle iconRect)
 {
 Color iconColor = _theme.AccentColor;

 int padding = iconRect.Width /5;
 Rectangle innerRect = new Rectangle(
 iconRect.X + padding,
 iconRect.Y + padding,
 Math.Max(0, iconRect.Width - padding *2),
 Math.Max(0, iconRect.Height - padding *2));

 using (var path = new GraphicsPath())
 {
 int tabWidth = innerRect.Width /3;
 int tabHeight = innerRect.Height /4;

 path.AddLine(innerRect.Left, innerRect.Top + tabHeight, innerRect.Left + tabWidth, innerRect.Top + tabHeight);
 path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight, innerRect.Left + tabWidth +2, innerRect.Top);
 path.AddLine(innerRect.Left + tabWidth +2, innerRect.Top, innerRect.Right, innerRect.Top);
 path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
 path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
 path.CloseFigure();

 var fillBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, iconColor));
 g.FillPath(fillBrush, path);

 var pen = PaintersFactory.GetPen(iconColor,1f);
 g.DrawPath(pen, path);
 }
 }

 public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
 {
 if (string.IsNullOrEmpty(text) || textRect.Width <=0 || textRect.Height <=0) return;

 Color textColor = isSelected ? GetSelectedForeColor() : _theme.TreeForeColor;

 TextRenderer.DrawText(g, text, _regularFont, textRect, textColor,
 TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
 return Math.Max(28, base.GetPreferredRowHeight(item, font));
 }
 }
}
