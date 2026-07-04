using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    internal sealed class DialogBackdropForm : Form
    {
        private readonly Random _rng = new Random(37);
        public DialogBackdropStyle BackdropStyle { get; set; } = DialogBackdropStyle.DimOnly;
        public float TargetOpacity { get; set; } = 0.5f;

        public DialogBackdropForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Phase 10 — DimOnly and BlurIfSupported now route through the
            // central frosted-glass painter. DimWithNoise retains its special-
            // purpose noise texture (permitted as the rule exception).
            if (BackdropStyle == DialogBackdropStyle.DimWithNoise)
            {
                // Solid dim base
                int alpha = (int)(Math.Max(0f, Math.Min(1f, TargetOpacity)) * 255);
                using var dim = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                g.FillRectangle(dim, ClientRectangle);
                PaintNoise(g);
            }
            else
            {
                // Frosted overlay via StyledImagePainter. DimOnly uses the
                // default tint; BlurIfSupported applies a Gaussian blur
                // radius automatically from the acrylic glass pipeline
                // (PaintFrostedOverlay already handles multi-pass blur).
                int blur = BackdropStyle == DialogBackdropStyle.BlurIfSupported ? 16 : 8;
                int alpha = (int)(Math.Max(0f, Math.Min(1f, TargetOpacity)) * 64);  // 0..64 alpha range
                var tint = Color.FromArgb(alpha, 0, 0, 0);
                StyledImagePainter.PaintFrostedOverlay(g, ClientRectangle, blur, tint);
            }
        }

        private void PaintNoise(Graphics g)
        {
            using var pen = new Pen(Color.FromArgb(18, 255, 255, 255), 1f);
            int points = Math.Max(120, (ClientSize.Width * ClientSize.Height) / 9500);
            for (int i = 0; i < points; i++)
            {
                int x = _rng.Next(Math.Max(1, ClientSize.Width));
                int y = _rng.Next(Math.Max(1, ClientSize.Height));
                g.DrawLine(pen, x, y, x + 1, y);
            }
        }
    }
}
