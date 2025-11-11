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
    /// Process Flow - Visual representation of workflow steps with progress indicators
    /// </summary>
    internal sealed class ProcessFlowPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ProcessFlowPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 12, ctx.DrawingRect.Y + 12,
                ctx.DrawingRect.Width - 24, ctx.DrawingRect.Height - 24);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var processes = ctx.ProcessFlowItems?.OfType<NavigationItem>().ToList() ?? CreateSampleProcessFlow();
            int activeProcess = ctx.ActiveProcessIndex;

            if (!processes.Any()) return;

            DrawFlowDiagram(g, ctx, processes, activeProcess);
        }

        private List<NavigationItem> CreateSampleProcessFlow()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Input", IsActive = false },
                new NavigationItem { Text = "Process", IsActive = true },
                new NavigationItem { Text = "Validate", IsActive = false },
                new NavigationItem { Text = "Output", IsActive = false }
            };
        }

        private void DrawFlowDiagram(Graphics g, WidgetContext ctx, List<NavigationItem> processes, int activeIndex)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            var successColor = Color.FromArgb(76, 175, 80);
            var pendingColor = Color.FromArgb(189, 189, 189);

            int nodeWidth = 80;
            int nodeHeight = 50;
            int spacing = (ctx.ContentRect.Width - nodeWidth * processes.Count) / Math.Max(processes.Count - 1, 1);

            using var processFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);

            for (int i = 0; i < processes.Count; i++)
            {
                var process = processes[i];
                bool isCompleted = i < activeIndex;
                bool isActive = i == activeIndex;
                bool isPending = i > activeIndex;

                int x = ctx.ContentRect.X + i * (nodeWidth + spacing);
                int y = ctx.ContentRect.Y + (ctx.ContentRect.Height - nodeHeight) / 2;
                var nodeRect = new Rectangle(x, y, nodeWidth, nodeHeight);

                // Node styling
                Color nodeColor = isCompleted ? successColor : isActive ? primaryColor : pendingColor;
                using var nodeBrush = new SolidBrush(Color.FromArgb(20, nodeColor));
                using var nodePen = new Pen(nodeColor, isActive ? 3 : 2);
                using var nodePath = CreateRoundedPath(nodeRect, 8);

                // Node shadow for active states
                if (isActive || isCompleted)
                {
                    var shadowRect = new Rectangle(nodeRect.X + 2, nodeRect.Y + 2, nodeRect.Width, nodeRect.Height);
                    using var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
                    using var shadowPath = CreateRoundedPath(shadowRect, 8);
                    g.FillPath(shadowBrush, shadowPath);
                }

                g.FillPath(nodeBrush, nodePath);
                g.DrawPath(nodePen, nodePath);

                // Process icon
                var iconRect = new Rectangle(x + (nodeWidth - 20) / 2, y + 8, 20, 20);
                string iconName = GetProcessIcon(process.Text, i);
                _imagePainter.DrawSvg(g, iconName, iconRect, nodeColor, 0.9f);

                // Process label
                using var textBrush = new SolidBrush(nodeColor);
                var textRect = new Rectangle(x, y + 28, nodeWidth, 20);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(process.Text, processFont, textBrush, textRect, format);

                // Flow arrow (except for last process)
                if (i < processes.Count - 1)
                {
                    var arrowColor = i < activeIndex ? successColor : pendingColor;
                    using var arrowPen = new Pen(arrowColor, 2);
                    
                    int arrowStartX = nodeRect.Right + 4;
                    int arrowEndX = nodeRect.Right + spacing - 4;
                    int arrowY = y + nodeHeight / 2;
                    
                    // Arrow line
                    g.DrawLine(arrowPen, arrowStartX, arrowY, arrowEndX, arrowY);
                    
                    // Arrow head
                    var arrowHead = new Point[] {
                        new Point(arrowEndX - 6, arrowY - 4),
                        new Point(arrowEndX, arrowY),
                        new Point(arrowEndX - 6, arrowY + 4)
                    };
                    using var arrowBrush = new SolidBrush(arrowColor);
                    g.FillPolygon(arrowBrush, arrowHead);
                }
            }
        }

        private string GetProcessIcon(string processText, int index)
        {
            var text = processText?.ToLower() ?? "";
            if (text.Contains("input")) return "upload";
            if (text.Contains("process")) return "cpu";
            if (text.Contains("validate")) return "shield-check";
            if (text.Contains("output")) return "download";
            if (text.Contains("decision")) return "help-circle";
            return "circle";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

}