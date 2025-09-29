using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Adapter painter that renders using BaseControlMaterialHelper (Material Design skin).
    /// </summary>
    internal sealed class MaterialBaseControlPainter : IBaseControlPainter
    {
        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null) return;
            owner._materialHelper ??= new BaseControlMaterialHelper(owner);
            owner._materialHelper.UpdateLayout();
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;
            owner._materialHelper ??= new BaseControlMaterialHelper(owner);

            // Apply elevation prefs
            owner._materialHelper.SetElevation(owner.MaterialElevationLevel);
            owner._materialHelper.SetElevationEnabled(owner.MaterialUseElevation);

            // Draw MD field
            owner._materialHelper.DrawAll(g);

            // Draw label and supporting text same as BaseControl.DrawMaterialContent
            if (!string.IsNullOrEmpty(owner.LabelText))
            {
                using var labelFont = new Font(owner.Font, FontStyle.Regular);
                var labelColor = owner.HasError ? owner.ErrorColor : (owner._currentTheme?.SecondaryTextColor ?? owner.ForeColor);
                var fieldRect = owner._materialHelper.GetFieldRect();
                var labelRect = new Rectangle(fieldRect.Left + 8, Math.Max(2, fieldRect.Top - (int)(labelFont.Height * 0.8)), fieldRect.Width - 16, labelFont.Height);
                if (owner.MaterialVariant !=MaterialTextFieldVariant.Outlined)
                {
                    labelRect = new Rectangle(fieldRect.Left + 8, fieldRect.Top - labelFont.Height - 2, fieldRect.Width - 16, labelFont.Height);
                }
                TextRenderer.DrawText(g, owner.LabelText, labelFont, labelRect, labelColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            string supporting = string.Empty;
            var supportingColor = owner._currentTheme?.SecondaryTextColor ?? owner.ForeColor;
            if (owner.HasError && !string.IsNullOrEmpty(owner.ErrorText))
            {
                supporting = owner.ErrorText;
                supportingColor = owner.ErrorColor;
            }
            else if (!string.IsNullOrEmpty(owner.HelperText))
            {
                supporting = owner.HelperText;
            }

            if (!string.IsNullOrEmpty(supporting))
            {
                using var supportFont = new Font(owner.Font.FontFamily, Math.Max(8f, owner.Font.Size - 1f), FontStyle.Regular);
                var rect = owner._materialHelper.GetSupportingTextRect(supportFont.Height);
                if (rect.Width > 10 && rect.Height > 0)
                {
                    TextRenderer.DrawText(g, supporting, supportFont, rect, supportingColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner?._materialHelper == null) return;
            var lead = owner._materialHelper.GetLeadingIconRect();
            var trail = owner._materialHelper.GetTrailingIconRect();
            if (!lead.IsEmpty && owner.LeadingIconClickable) register?.Invoke("MaterialLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register?.Invoke("MaterialTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }
    }
}
