# PowerShell script to fix all NavBar painters text rendering
# Replaces g.DrawString() with DrawNavItemText() helper method calls

$paintersDir = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\NavBars\Painters"

$fixPatterns = @(
    # Pattern: Multi-line g.DrawString with Font, Brush, StringFormat
    @{
        Name = "Fluent2-Horizontal"
        File = "Fluent2NavBarPainter.cs"
        Old = @'
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        Color textColor = context.UseThemeColors && context.Theme != null
                            ? context.Theme.ForeColor
                            : Color.FromArgb(50, 49, 48);
                        
                        using (var font = new Font("Segoe UI", 10f, FontStyle.Regular))
                        using (var brush = new SolidBrush(textColor))
                        using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            var textRect = new Rectangle(itemRect.X + padding, itemRect.Y + iconSize + 8, itemRect.Width - padding * 2, itemRect.Height - iconSize - 12);
                            g.DrawString(item.Text, font, brush, textRect, sf);
                        }
                    }
'@
        New = @'
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(itemRect.X + padding, itemRect.Y + iconSize + 8, itemRect.Width - padding * 2, itemRect.Height - iconSize - 12);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Segoe UI", 9f);
                    }
'@
    }
)

Write-Host "This script would fix NavBar painters, but it's complex." -ForegroundColor Yellow
Write-Host "Please use multi_replace_string_in_file tool instead for accuracy." -ForegroundColor Yellow
