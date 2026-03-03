using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 3 — Extended configuration for persistent popover panels.
    /// Inherits all <see cref="ToolTipConfig"/> properties and adds
    /// action-button, confirmation and trigger options.
    /// </summary>
    public class PopoverConfig : ToolTipConfig
    {
        /// <summary>
        /// Whether clicking outside the popover dismisses it.
        /// Default true.
        /// </summary>
        public bool DismissOnClickOutside { get; set; } = true;

        /// <summary>
        /// Whether pressing Escape dismisses the popover.
        /// Default true.
        /// </summary>
        public bool DismissOnEscape { get; set; } = true;

        /// <summary>
        /// Label for the primary/confirm action button.
        /// Leave null to omit the button.
        /// </summary>
        public string PrimaryButtonText { get; set; }

        /// <summary>
        /// Label for the secondary/cancel action button.
        /// Leave null to omit the button.
        /// </summary>
        public string SecondaryButtonText { get; set; }

        /// <summary>
        /// Semantic type of the primary button — controls its color (Danger = red, Primary = brand, etc.).
        /// </summary>
        public ToolTipType PrimaryButtonType { get; set; } = ToolTipType.Primary;

        /// <summary>
        /// Raised when the primary button is clicked.
        /// The string argument is <see cref="ToolTipConfig.Key"/>.
        /// </summary>
        public Action<string> OnPrimaryClick { get; set; }

        /// <summary>
        /// Raised when the secondary button is clicked or the popover is dismissed via cancel.
        /// </summary>
        public Action<string> OnSecondaryClick { get; set; }

        /// <summary>
        /// Maximum width of the popover in pixels.
        /// Default 360.
        /// </summary>
        public int MaxPopoverWidth { get; set; } = 360;
    }
}
