using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Quick Actions - Grid of action buttons for common operations
    /// </summary>
    internal sealed class QuickActionsPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public QuickActionsPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 8, ctx.DrawingRect.Y + 8,
                ctx.DrawingRect.Width - 16, ctx.DrawingRect.Height - 16);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.FromArgb(250, 250, 250));
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);

            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(20, Color.Gray), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var actions = ctx.CustomData.ContainsKey("Actions") ?
                (List<NavigationItem>)ctx.CustomData["Actions"] : CreateSampleQuickActions();

            if (!actions.Any()) return;

            DrawQuickActionGrid(g, ctx, actions);
        }

        private List<NavigationItem> CreateSampleQuickActions()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "New", IsActive = false },
                new NavigationItem { Text = "Save", IsActive = false },
                new NavigationItem { Text = "Print", IsActive = false },
                new NavigationItem { Text = "Share", IsActive = false }
            };
        }

        private void DrawQuickActionGrid(Graphics g, WidgetContext ctx, List<NavigationItem> actions)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            int cols = Math.Min(actions.Count, 4); // Max 4 columns
            int rows = (int)Math.Ceiling((double)actions.Count / cols);
            
            int buttonSize = Math.Min((ctx.ContentRect.Width - (cols - 1) * 8) / cols, 
                                    (ctx.ContentRect.Height - (rows - 1) * 8) / rows);
            
            using var actionFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                int col = i % cols;
                int row = i / cols;
                
                int x = ctx.ContentRect.X + col * (buttonSize + 8);
                int y = ctx.ContentRect.Y + row * (buttonSize + 8);
                var actionRect = new Rectangle(x, y, buttonSize, buttonSize);
                
                // Action button background
                using var actionBrush = new SolidBrush(Color.FromArgb(10, primaryColor));
                using var actionPath = CreateRoundedPath(actionRect, 8);
                g.FillPath(actionBrush, actionPath);
                
                // Hover effect border
                using var hoverPen = new Pen(Color.FromArgb(30, primaryColor), 1);
                g.DrawPath(hoverPen, actionPath);
                
                // Action icon
                var iconSize = buttonSize / 2;
                var iconRect = new Rectangle(x + (buttonSize - iconSize) / 2, y + 8, iconSize, iconSize);
                string iconName = GetQuickActionIcon(action.Text);
                _imagePainter.DrawSvg(g, iconName, iconRect, primaryColor, 0.9f);
                
                // Action label
                using var textBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var textRect = new Rectangle(x, y + iconRect.Bottom + 4, buttonSize, 20);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(action.Text, actionFont, textBrush, textRect, format);
            }
        }

        private string GetQuickActionIcon(string actionText)
        {
            var text = actionText?.ToLower() ?? "";
            if (text.Contains("new") || text.Contains("add")) return "plus";
            if (text.Contains("save")) return "save";
            if (text.Contains("print")) return "printer";
            if (text.Contains("share")) return "share-2";
            if (text.Contains("edit")) return "edit";
            if (text.Contains("delete")) return "trash-2";
            if (text.Contains("copy")) return "copy";
            if (text.Contains("search")) return "search";
            return "zap";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}