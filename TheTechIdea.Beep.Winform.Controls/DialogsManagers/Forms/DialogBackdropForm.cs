using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

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

        private void PaintBlurSimulation(Graphics g)
        {
            var center = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            using var brush = new PathGradientBrush(new[]
            {
                new Point(0,0),
                new Point(ClientRectangle.Width,0),
                new Point(ClientRectangle.Width,ClientRectangle.Height),
                new Point(0,ClientRectangle.Height)
            })
            {
                CenterPoint = center,
                CenterColor = Color.FromArgb(24, 255, 255, 255),
                SurroundColors = new[]
                {
                    Color.FromArgb(6, 0, 0, 0),
                    Color.FromArgb(6, 0, 0, 0),
                    Color.FromArgb(6, 0, 0, 0),
                    Color.FromArgb(6, 0, 0, 0)
                }
            };
            g.FillRectangle(brush, ClientRectangle);
        }
    }
}
