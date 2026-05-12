using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            // Design-time placeholder rendering
            if (IsDesignModeSafe)
            {
                PaintDesignTimePlaceholder(g);
                return;
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual content rectangle
            Rectangle contentRect = bounds.IsEmpty ? GetContentRectForDrawing() : bounds;

            if (_usePainterSystem && _stylePainter != null)
            {
                // Use new painter system
                DrawWithPainter(g, contentRect);
            }
            else
            {
                // Use legacy renderer
                DrawWithLegacyRenderer(g, contentRect);
            }
        }

        // Helper method like BeepComboBox uses
        private Rectangle GetContentRectForDrawing()
        {
            if (PainterKind == BaseControlPainterKind.Material)
            {
                var r = GetContentRect();
                if (r.Width > 0 && r.Height > 0) return r;
            }
            return DrawingRect;
        }
    }
}