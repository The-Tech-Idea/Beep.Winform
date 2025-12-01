using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Models
{
    /// <summary>
    /// Contains layout metrics and calculated positions for login controls
    /// </summary>
    public class LoginLayoutMetrics
    {
        /// <summary>
        /// The bounds of the container (login panel)
        /// </summary>
        public Rectangle ContainerBounds { get; set; }

        /// <summary>
        /// Available width inside the container (accounting for padding)
        /// </summary>
        public int ContainerWidth { get; set; }

        /// <summary>
        /// Available height inside the container (accounting for padding)
        /// </summary>
        public int ContainerHeight { get; set; }

        /// <summary>
        /// Standard margin between elements
        /// </summary>
        public int Margin { get; set; } = 10;

        /// <summary>
        /// Spacing between related elements
        /// </summary>
        public int Spacing { get; set; } = 15;

        /// <summary>
        /// Dictionary of control positions by control name
        /// </summary>
        public Dictionary<string, Rectangle> ControlPositions { get; set; } = new Dictionary<string, Rectangle>();

        /// <summary>
        /// Dictionary of control sizes by control name
        /// </summary>
        public Dictionary<string, Size> ControlSizes { get; set; } = new Dictionary<string, Size>();

        /// <summary>
        /// Current Y position for vertical layout
        /// </summary>
        public int CurrentY { get; set; }

        /// <summary>
        /// Current X position for horizontal layout
        /// </summary>
        public int CurrentX { get; set; }

        /// <summary>
        /// Padding of the container
        /// </summary>
        public Padding ContainerPadding { get; set; } = new Padding(10);

        /// <summary>
        /// Gets the position for a control by name
        /// </summary>
        public Point GetControlPosition(string controlName)
        {
            return ControlPositions.TryGetValue(controlName, out var rect) ? rect.Location : Point.Empty;
        }

        /// <summary>
        /// Gets the size for a control by name
        /// </summary>
        public Size GetControlSize(string controlName)
        {
            return ControlSizes.TryGetValue(controlName, out var size) ? size : Size.Empty;
        }

        /// <summary>
        /// Sets the position and size for a control
        /// </summary>
        public void SetControlBounds(string controlName, Rectangle bounds)
        {
            ControlPositions[controlName] = bounds;
            ControlSizes[controlName] = bounds.Size;
        }

        /// <summary>
        /// Gets the bounds (position and size) for a control by name
        /// </summary>
        public Rectangle GetControlBounds(string controlName)
        {
            return ControlPositions.TryGetValue(controlName, out var rect) ? rect : Rectangle.Empty;
        }

        /// <summary>
        /// Checks if a control has been positioned (exists in the metrics)
        /// </summary>
        public bool HasControl(string controlName)
        {
            return ControlPositions.ContainsKey(controlName);
        }

        /// <summary>
        /// Advances the current Y position by the specified amount
        /// </summary>
        public void AdvanceY(int amount)
        {
            CurrentY += amount;
        }

        /// <summary>
        /// Advances the current X position by the specified amount
        /// </summary>
        public void AdvanceX(int amount)
        {
            CurrentX += amount;
        }

        /// <summary>
        /// Resets the current Y position to the initial margin
        /// </summary>
        public void ResetY()
        {
            CurrentY = ContainerPadding.Top + Margin;
        }

        /// <summary>
        /// Resets the current X position to the initial margin
        /// </summary>
        public void ResetX()
        {
            CurrentX = ContainerPadding.Left + Margin;
        }
    }
}

