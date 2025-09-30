using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Adapter painter that renders using the legacy ControlPaintHelper (classic mode).
    /// Adds icon-aware text drawing so Leading/Trailing icons don’t overlap text.
    /// </summary>
    internal sealed class ClassicBaseControlPainter : IBaseControlPainter
    {
        public void UpdateLayout(Base.BaseControl owner)
        {
            owner?._paint?.UpdateRects();
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;
            owner._paint?.EnsureUpdated();
            owner._paint?.Draw(g);

            // Integrate icons + adjust content rect if any icon paths are set
            var drawingRect = owner._paint?.DrawingRect ?? new Rectangle(0, 0, owner.Width, owner.Height);
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;

            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(drawingRect);
                icons.Draw(g);
            }

            // Main text is drawn centrally by BaseControl.DrawContent to avoid duplicates.
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;
            var drawingRect = owner._paint?.DrawingRect ?? new Rectangle(0, 0, owner.Width, owner.Height);
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (!(hasLeading || hasTrailing)) return;

            var icons = new BaseControlIconsHelper(owner);
            icons.UpdateLayout(drawingRect);
            var lead = icons.LeadingRect;
            var trail = icons.TrailingRect;
            if (!lead.IsEmpty && owner.LeadingIconClickable) register("ClassicLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register("ClassicTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }
    }
}
