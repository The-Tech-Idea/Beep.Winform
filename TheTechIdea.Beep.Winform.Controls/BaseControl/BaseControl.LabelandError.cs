using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    //  Layout Management and Calculation  extension for BaseControl (partial)
    public partial class BaseControl
    {
        private void DrawLabelAndHelperNonMaterial(Graphics g, Base.BaseControl owner)
        {
            if (LabelTextOn )
            {
                // Draw label text centered vertically on the top border
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    var labelHeight = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    // Center the label vertically on the top border
                    int borderThickness = owner.BorderThickness;
                    int labelY = Math.Max(0, _borderRect.Top - (labelHeight / 2) - (borderThickness / 2));
                    var labelRect = new Rectangle(_borderRect.Left + 6, labelY, Math.Max(10, _borderRect.Width - 12), labelHeight);
                    Color labelColor = string.IsNullOrEmpty(owner.ErrorText) ? (owner.ForeColor) : owner.ErrorColor;
                    TextRenderer.DrawText(g, owner.LabelText, lf, labelRect, labelColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
           
            
            // Draw helper or error text centered vertically on the bottom border
           if(_showHelperText || HasError)
            {
                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    var supportHeight = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    // Center the support text vertically on the bottom border
                    int borderThickness = owner.BorderThickness;
                    int supportY = _borderRect.Bottom - (supportHeight / 2) - (borderThickness / 2);
                    var supportRect = new Rectangle(_borderRect.Left + 6, supportY, Math.Max(10, _borderRect.Width - 12), supportHeight);
                    Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : (owner.ForeColor);
                    TextRenderer.DrawText(g, supporting, sf, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            
        }

    }
}
