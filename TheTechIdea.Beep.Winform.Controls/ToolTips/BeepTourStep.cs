using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 6 — Represents a single step in a <see cref="BeepTourManager"/> guided tour.
    /// </summary>
    public class BeepTourStep
    {
        /// <summary>The control to highlight and anchor the tooltip to.</summary>
        public Control TargetControl { get; set; }

        /// <summary>Primary heading for this step.</summary>
        public string Title { get; set; }

        /// <summary>Descriptive body text.</summary>
        public string Body { get; set; }

        /// <summary>Optional image shown above the text (file-system path).</summary>
        public string ImagePath { get; set; }

        /// <summary>Where to anchor the tooltip relative to <see cref="TargetControl"/>.</summary>
        public ToolTipPlacement Placement { get; set; } = ToolTipPlacement.Auto;

        /// <summary>Called just before this step is shown (e.g. scroll target into view).</summary>
        public Action OnEnter { get; set; }

        /// <summary>Called just after this step is dismissed.</summary>
        public Action OnLeave { get; set; }
    }
}
