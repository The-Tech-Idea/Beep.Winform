using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Painters
{
    /// <summary>
    /// Base painter for wizard forms
    /// Provides common painting functionality and theme integration
    /// </summary>
    public abstract class WizardPainterBase
    {
        protected BaseWizardForm? Owner { get; private set; }
        protected IBeepTheme? Theme { get; private set; }
        protected WizardInstance? Instance { get; private set; }

        /// <summary>
        /// Initialize the painter with owner form and theme
        /// </summary>
        public virtual void Initialize(BaseWizardForm owner, IBeepTheme theme, WizardInstance instance)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Theme = theme;
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// Paint the wizard header
        /// </summary>
        public abstract void PaintHeader(Graphics g, Rectangle bounds);

        /// <summary>
        /// Paint the wizard content area
        /// </summary>
        public abstract void PaintContent(Graphics g, Rectangle bounds);

        /// <summary>
        /// Paint the wizard footer/button area
        /// </summary>
        public abstract void PaintFooter(Graphics g, Rectangle bounds);

        /// <summary>
        /// Paint progress indicator
        /// </summary>
        public virtual void PaintProgress(Graphics g, Rectangle bounds, int currentStep, int totalSteps)
        {
            // Default implementation - can be overridden
        }

        /// <summary>
        /// Paint step indicators
        /// </summary>
        public virtual void PaintStepIndicators(Graphics g, Rectangle bounds)
        {
            // Default implementation - can be overridden
        }

        #region Helper Methods

        protected GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            var arcRect = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arcRect, 180, 90);
            arcRect.X = bounds.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = bounds.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = bounds.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected void DrawSoftShadow(Graphics g, Rectangle bounds, int blurRadius, int layers = 3, int offset = 2)
        {
            // Simple shadow implementation
            for (int i = layers; i > 0; i--)
            {
                var shadowRect = new Rectangle(
                    bounds.X + offset + i,
                    bounds.Y + offset + i,
                    bounds.Width,
                    bounds.Height);

                using (var shadowBrush = new SolidBrush(Color.FromArgb(10 / i, Color.Black)))
                {
                    using (var shadowPath = CreateRoundedPath(shadowRect, 8))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }
        }

        protected Color GetAccentColor()
        {
            return Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
        }

        protected Color GetBackColor()
        {
            return Theme?.BackColor ?? Color.White;
        }

        protected Color GetForeColor()
        {
            return Theme?.ForeColor ?? Color.Black;
        }

        protected Color GetBorderColor()
        {
            return Theme?.BorderColor ?? Color.LightGray;
        }

        #endregion
    }
}
