using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Switchs.Helpers;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    // NOTE: SwitchOrientation enum moved to Switchs/Models/SwitchOrientation.cs

    [ToolboxItem(true)]
    [DisplayName("Beep Switch")]
    [Category("Beep Controls")]
    [Description("A cylindrical toggle switch control with customizable labels, images, and orientation.")]
    public partial class BeepSwitch : BaseControl
    {
        // NOTE: Fields moved to BeepSwitch.Core.cs
        // NOTE: Events and Properties moved to BeepSwitch.Properties.cs

        // NOTE: Constructor moved to BeepSwitch.Core.cs

        // NOTE: Painting moved to BeepSwitch.Drawing.cs
        // NOTE: Legacy drawing methods below are kept for reference but not used
        
        /// <summary>
        /// LEGACY: Draws the switch in horizontal orientation (replaced by painter system).
        /// Off label appears on the left and On label on the right.
        /// </summary>
        private void DrawHorizontalSwitch_Legacy(Graphics g, Rectangle rectangle)
        {
            int padding = 8; // General padding

            // Measure label sizes.
            Size offLabelSize = TextRenderer.MeasureText(OffLabel, this.Font);
            Size onLabelSize = TextRenderer.MeasureText(OnLabel, this.Font);

            // Determine the track rectangle, reserving space for the labels.
            int trackX = offLabelSize.Width + padding * 2;
            int trackY = padding;
            int trackWidth = DrawingRect.Width - offLabelSize.Width - onLabelSize.Width - padding * 4;
            int trackHeight = DrawingRect.Height - padding * 2;
            Rectangle trackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);

            // Create a capsule (pill-shaped) path for the track.
            using (GraphicsPath trackPath = GetCapsulePath_Legacy(trackRect, vertical: false))
            {
                // Draw the background image (clipped inside the capsule) if available.
                if (Checked && _onBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _onBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else if (!Checked && _offBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _offBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else
                {
                    // Otherwise, fill with a gradient based on the theme.
                    Color startColor, endColor;
                    if (Checked)
                    {
                        startColor = _currentTheme?.CheckBoxBackColor ?? Color.Empty;
                        endColor = _currentTheme?.CheckBoxForeColor ?? Color.Empty;
                    }
                    else
                    {
                        startColor = _currentTheme?.GradientStartColor ?? Color.White;
                        endColor = _currentTheme?.GradientEndColor ?? Color.LightGray;
                    }
                    using (LinearGradientBrush brush = new LinearGradientBrush(trackRect, startColor, endColor, LinearGradientMode.Horizontal))
                    {
                        g.FillPath(brush, trackPath);
                    }
                }
                // Draw the border of the track.
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawPath(pen, trackPath);
                }
            }

            // Draw the slider (the circular knob).
            int sliderDiameter = trackHeight - 4;
            int sliderY = trackRect.Y + 2;
            int sliderX = Checked ? (trackRect.Right - sliderDiameter - 2) : (trackRect.X + 2);
            Rectangle sliderRect = new Rectangle(sliderX, sliderY, sliderDiameter, sliderDiameter);
            using (GraphicsPath sliderPath = new GraphicsPath())
            {
                sliderPath.AddEllipse(sliderRect);
                using (PathGradientBrush pgb = new PathGradientBrush(sliderPath))
                {
                    pgb.CenterColor = Color.White;
                    pgb.SurroundColors = new Color[] { Color.LightGray };
                    g.FillEllipse(pgb, sliderRect);
                }
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawEllipse(pen, sliderRect);
                }
            }

            // Draw the labels.
            Rectangle offLabelRect = new Rectangle(padding, trackRect.Y, offLabelSize.Width, trackRect.Height);
            TextRenderer.DrawText(g, OffLabel, this.Font, offLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            Rectangle onLabelRect = new Rectangle(trackRect.Right + padding, trackRect.Y, onLabelSize.Width, trackRect.Height);
            TextRenderer.DrawText(g, OnLabel, this.Font, onLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// LEGACY: Draws the switch in vertical orientation (replaced by painter system).
        /// On label appears at the top and Off label at the bottom.
        /// </summary>
        private void DrawVerticalSwitch_Legacy(Graphics g, Rectangle rectangle)
        {
            int padding = 8; // General padding

            // Measure label sizes.
            Size onLabelSize = TextRenderer.MeasureText(OnLabel, this.Font);
            Size offLabelSize = TextRenderer.MeasureText(OffLabel, this.Font);

            // Determine the track rectangle, leaving room for the labels.
            int trackY = onLabelSize.Height + padding * 2;
            int trackX = padding;
            int trackHeight = DrawingRect.Height - onLabelSize.Height - offLabelSize.Height - padding * 4;
            int trackWidth = DrawingRect.Width - padding * 2;
            Rectangle trackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);

            // Create a capsule path for the vertical track.
            using (GraphicsPath trackPath = GetCapsulePath_Legacy(trackRect, vertical: true))
            {
                if (Checked && _onBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _onBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else if (!Checked && _offBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _offBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else
                {
                    Color startColor, endColor;
                    if (Checked)
                    {
                        startColor = _currentTheme?.CheckBoxBackColor ?? Color.Empty;
                        endColor = _currentTheme?.CheckBoxForeColor ?? Color.Empty;
                    }
                    else
                    {
                        startColor = _currentTheme?.GradientStartColor ?? Color.White;
                        endColor = _currentTheme?.GradientEndColor ?? Color.LightGray;
                    }
                    using (LinearGradientBrush brush = new LinearGradientBrush(trackRect, startColor, endColor, LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, trackPath);
                    }
                }
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawPath(pen, trackPath);
                }
            }

            // Draw the slider (toggle knob).
            int sliderDiameter = trackWidth - 4;
            int sliderX = trackRect.X + 2;
            int sliderY = Checked ? (trackRect.Y + 2) : (trackRect.Bottom - sliderDiameter - 2);
            Rectangle sliderRect = new Rectangle(sliderX, sliderY, sliderDiameter, sliderDiameter);
            using (GraphicsPath sliderPath = new GraphicsPath())
            {
                sliderPath.AddEllipse(sliderRect);
                using (PathGradientBrush pgb = new PathGradientBrush(sliderPath))
                {
                    pgb.CenterColor = Color.White;
                    pgb.SurroundColors = new Color[] { Color.LightGray };
                    g.FillEllipse(pgb, sliderRect);
                }
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawEllipse(pen, sliderRect);
                }
            }

            // Draw the labels.
            Rectangle onLabelRect = new Rectangle(0, padding, this.Width, onLabelSize.Height);
            TextRenderer.DrawText(g, OnLabel, this.Font, onLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            Rectangle offLabelRect = new Rectangle(0, trackRect.Bottom + padding, this.Width, offLabelSize.Height);
            TextRenderer.DrawText(g, OffLabel, this.Font, offLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// LEGACY: Creates a capsule-shaped GraphicsPath (replaced by CreateTrackPath in Drawing.cs).
        /// </summary>
        /// <param name="rect">The rectangle for the capsule.</param>
        /// <param name="vertical">If true, creates a vertical capsule; otherwise horizontal.</param>
        private GraphicsPath GetCapsulePath_Legacy(Rectangle rect, bool vertical)
        {
            GraphicsPath path = new GraphicsPath();
            if (vertical)
            {
                int radius = rect.Width / 2;
                // Top arc.
                path.AddArc(rect.X, rect.Y, rect.Width, 2 * radius, 180, 180);
                // Bottom arc.
                path.AddArc(rect.X, rect.Bottom - 2 * radius, rect.Width, 2 * radius, 0, 180);
            }
            else
            {
                int radius = rect.Height / 2;
                // Left arc.
                path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
                // Right arc.
                path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
            }
            path.CloseFigure();
            return path;
        }

        // NOTE: Mouse Interaction moved to BeepSwitch.Interaction.cs

        // NOTE: Event Raisers moved to BeepSwitch.Properties.cs
        // NOTE: Data Binding Methods moved to BeepSwitch.DataBinding.cs

        // NOTE: Theme Support moved to BeepSwitch.Theme.cs

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _onBeepImage?.Dispose();
                _offBeepImage?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
