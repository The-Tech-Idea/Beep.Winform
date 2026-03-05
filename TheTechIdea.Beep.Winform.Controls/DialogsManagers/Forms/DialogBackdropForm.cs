using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;

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
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var dim = new SolidBrush(Color.FromArgb((int)(Math.Max(0f, Math.Min(1f, TargetOpacity)) * 255), 0, 0, 0));
            e.Graphics.FillRectangle(dim, ClientRectangle);

            if (BackdropStyle == DialogBackdropStyle.DimWithNoise)
            {
                PaintNoise(e.Graphics);
            }
            else if (BackdropStyle == DialogBackdropStyle.BlurIfSupported)
            {
                PaintBlurSimulation(e.Graphics);
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

        /// <summary>
        /// Delegates to the <see cref="BeepControlStyle.GlassAcrylic"/> background painter —
        /// the same frosted-glass effect used by all Beep controls with that style.
        /// No custom GDI gradient needed here.
        /// </summary>
        private void PaintBlurSimulation(Graphics g)
        {
            var painter = BackgroundPainterFactory.CreatePainter(BeepControlStyle.GlassAcrylic);
            if (painter == null) return;

            using var path = new GraphicsPath();
            path.AddRectangle(ClientRectangle);
            painter.Paint(g, path, BeepControlStyle.GlassAcrylic,
                theme: null, useThemeColors: false, state: ControlState.Normal);
        }
    }
}
