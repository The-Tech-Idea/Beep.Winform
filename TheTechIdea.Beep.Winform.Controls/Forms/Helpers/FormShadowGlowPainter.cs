using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Renders shadow and glow effects behind/around the form shape.
    /// Pure drawing helper (stateless except for configuration properties).
    /// </summary>
    internal sealed class FormShadowGlowPainter
    {
        public int ShadowDepth { get; set; } = 6;
        public Color ShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);
        public bool EnableGlow { get; set; } = true;
        public float GlowSpread { get; set; } = 8f;
        public Color GlowColor { get; set; } = Color.FromArgb(100, 72, 170, 255);

        public void PaintShadow(Graphics g, GraphicsPath shapePath)
        {
            if (ShadowDepth <= 0 || shapePath == null) return;
            using var shadowPath = (GraphicsPath)shapePath.Clone();
            using var b = new SolidBrush(ShadowColor);
            g.TranslateTransform(ShadowDepth, ShadowDepth);
            g.FillPath(b, shadowPath);
            g.TranslateTransform(-ShadowDepth, -ShadowDepth);
        }

        public void PaintGlow(Graphics g, GraphicsPath shapePath)
        {
            if (!EnableGlow || GlowSpread <= 0f || shapePath == null) return;
            using var pen = new Pen(GlowColor, GlowSpread)
            {
                LineJoin = LineJoin.Round,
                Alignment = PenAlignment.Inset
            };
            g.DrawPath(pen, shapePath);
        }
    }
}
