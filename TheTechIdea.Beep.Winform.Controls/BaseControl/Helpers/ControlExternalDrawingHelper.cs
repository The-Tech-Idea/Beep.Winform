using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    // Move public types outside the helper class for accessibility
    public delegate void DrawExternalHandler(Graphics parentGraphics, Rectangle childBounds);

    public class ExternalDrawingFunction
    {
        public Control ChildControl { get; set; }
        public DrawExternalHandler Handler { get; set; }
        public DrawingLayer Layer { get; set; }
        public bool Redraw { get; set; } = true;

        public ExternalDrawingFunction(DrawExternalHandler handler, DrawingLayer layer)
        {
            Handler = handler;
            Layer = layer;
        }

        public bool IsValid => Handler != null;

        public void Invoke(Graphics g, Rectangle childBounds)
        {
            Handler?.Invoke(g, childBounds);
        }

        public void Clear()
        {
            Handler = null;
        }

        public void Dispose()
        {
            Handler = null;
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handler = null;
            }
        }
    }

    internal class ControlExternalDrawingHelper
    {
        private readonly BaseControl _owner;
        private readonly Dictionary<Control, ExternalDrawingFunction> _childExternalDrawers = new();

        public ControlExternalDrawingHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Properties
        public DrawingLayer ExternalDrawingLayer { get; set; } = DrawingLayer.AfterAll;
        #endregion

        #region External Drawing Management
        public void AddChildExternalDrawing(Control child, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            var drawingFunction = new ExternalDrawingFunction(handler, layer)
            {
                ChildControl = child,
                Redraw = true
            };

            if (_childExternalDrawers.ContainsKey(child))
            {
                _childExternalDrawers[child] = drawingFunction;
                _childExternalDrawers[child].Redraw = true;
            }
            else
            {
                _childExternalDrawers.Add(child, drawingFunction);
            }
        }

        public void SetChildExternalDrawingRedraw(Control child, bool redraw)
        {
            if (child != null && _childExternalDrawers.TryGetValue(child, out var function))
            {
                function.Redraw = redraw;
            }
        }

        public void ClearChildExternalDrawing(Control child)
        {
            if (child == null) return;

            if (_childExternalDrawers.TryGetValue(child, out var function))
            {
                function.Dispose();
                _childExternalDrawers.Remove(child);
                _owner.Invalidate();
            }
        }

        public void ClearAllChildExternalDrawing()
        {
            foreach (var function in _childExternalDrawers.Values)
            {
                function.Dispose();
            }

            _childExternalDrawers.Clear();
            _owner.Invalidate();
        }

        public void PerformExternalDrawing(Graphics g, DrawingLayer layer)
        {
            if (_childExternalDrawers == null) return;

            foreach (var kvp in _childExternalDrawers)
            {
                Control childControl = kvp.Key;
                ExternalDrawingFunction drawingFunction = kvp.Value;

                if (childControl.Visible &&
                    drawingFunction.Layer == layer &&
                    drawingFunction.Redraw &&
                    drawingFunction.IsValid)
                {
                    drawingFunction.Invoke(g, childControl.Bounds);
                }
            }
        }
        #endregion

        #region Badge Drawing Support
        public void DrawBadgeExternally(Graphics g, Rectangle childBounds, string badgeText, Color badgeBackColor, Color badgeForeColor, Font badgeFont, BadgeShape badgeShape = BadgeShape.Circle)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            const int badgeSize = 22;
            int x = childBounds.Right - badgeSize / 2;
            int y = childBounds.Top - badgeSize / 2;
            var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);

            DrawBadgeImplementation(g, badgeRect, badgeText, badgeBackColor, badgeForeColor, badgeFont, badgeShape);
        }

        private void DrawBadgeImplementation(Graphics g, Rectangle badgeRect, string badgeText, Color badgeBackColor, Color badgeForeColor, Font badgeFont, BadgeShape badgeShape)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Badge background
            using (var brush = new SolidBrush(badgeBackColor))
            {
                switch (badgeShape)
                {
                    case BadgeShape.Circle:
                        g.FillEllipse(brush, badgeRect);
                        break;
                    case BadgeShape.RoundedRectangle:
                        using (var path = ControlPaintHelper.GetRoundedRectPath(badgeRect, badgeRect.Height / 4))
                            g.FillPath(brush, path);
                        break;
                    case BadgeShape.Rectangle:
                        g.FillRectangle(brush, badgeRect);
                        break;
                }
            }

            // Badge text
            if (!string.IsNullOrEmpty(badgeText))
            {
                using (var textBrush = new SolidBrush(badgeForeColor))
                    using (var scaledFont = _owner.DisableDpiAndScaling ? badgeFont : GetScaledBadgeFont(g, badgeText, new Size(badgeRect.Width - 4, badgeRect.Height - 4), badgeFont))
                {
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(badgeText, scaledFont, textBrush, badgeRect, fmt);
                }
            }
        }

        private Font GetScaledBadgeFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            if (string.IsNullOrEmpty(text) || maxSize.Width <= 0 || maxSize.Height <= 0)
                return new Font(originalFont.FontFamily, 8, FontStyle.Bold);

            if (text.Length == 1)
            {
                float fontSize = Math.Max(6, Math.Min(maxSize.Height * 0.65f, 10));
                return new Font(originalFont.FontFamily, fontSize, FontStyle.Bold);
            }

            for (float size = originalFont.Size; size >= 6; size -= 0.5f)
            {
                using (var testFont = new Font(originalFont.FontFamily, size, FontStyle.Bold))
                {
                    var measuredSize = TextRenderer.MeasureText(g, text, testFont);
                    if (measuredSize.Width <= maxSize.Width && measuredSize.Height <= maxSize.Height)
                    {
                        return new Font(originalFont.FontFamily, size, FontStyle.Bold);
                    }
                }
            }

            return new Font(originalFont.FontFamily, 6, FontStyle.Bold);
        }
        #endregion

        #region Cleanup
        public void Dispose()
        {
            ClearAllChildExternalDrawing();
        }
        #endregion
    }
}