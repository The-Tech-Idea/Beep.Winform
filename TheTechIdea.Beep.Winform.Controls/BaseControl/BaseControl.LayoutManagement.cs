using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Base
{
    //  Layout Management and Calculation  extension for BaseControl (partial)
    public partial class BaseControl
    {
        public void UpdateLayout()
        {
          
            // Compute border thickness to consider
            int border = 0;
            if ( ShowAllBorders ||  BorderThickness > 0 && ( ShowTopBorder ||  ShowBottomBorder ||  ShowLeftBorder ||  ShowRightBorder))
            {
                border =  BorderThickness;
            }
            else

            {
            }

             // Base paddings + optional offsets
            var padding =  Padding;
            int leftPad = padding.Left +  LeftoffsetForDrawingRect;
            int topPad = padding.Top +  TopoffsetForDrawingRect;
            int rightPad = padding.Right +  RightoffsetForDrawingRect;
            int bottomPad = padding.Bottom +  BottomoffsetForDrawingRect;

            // Shadow offset
            int shadow =  ShowShadow ?  ShadowOffset : 0;

            // Calculate inner drawing rect
            int calculatedWidth =  Width - (shadow * 2 + border * 2 + leftPad + rightPad);
            int calculatedHeight =  Height - (shadow * 2 + border * 2 + topPad + bottomPad);

            var inner = new Rectangle(
                shadow + border + leftPad,
                shadow + border + topPad,
                Math.Max(0, calculatedWidth),
                Math.Max(0, calculatedHeight)
            );

            // Reserve space for label and helper when not material
            // Since they're centered on the borders, we only need to reserve half the text height + border thickness
            try
            {
                int reserveTop = 0;
                int reserveBottom = 0;
                if(LabelTextOn )
                {
                    if (!string.IsNullOrEmpty( LabelText))
                    {
                        float labelSize = Math.Max(8f,  Font.Size - 1f);
                        using var lf = new Font( Font.FontFamily, labelSize, FontStyle.Regular);
                        int h = TextRenderer.MeasureText( "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                        // Reserve space for half the text height (since it's centered on the border)
                        reserveTop = (h / 2) + (border / 2);
                    }
                }
                if(HasError || _showHelperText)
                {
                    string supporting = !string.IsNullOrEmpty( ErrorText) ? ErrorText : HelperText;
                    if (!string.IsNullOrEmpty(supporting))
                    {
                        float supSize = Math.Max(8f,  Font.Size - 1f);
                        using var sf = new Font( Font.FontFamily, supSize, FontStyle.Regular);
                        int h = TextRenderer.MeasureText( "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                        // Reserve space for half the text height (since it's centered on the border)
                        reserveBottom = (h / 2) + (border / 2);
                    }
                }

            
                if (reserveTop > 0 || reserveBottom > 0)
                {
                    inner = new Rectangle(
                        inner.X,
                        inner.Y + reserveTop,
                        inner.Width,
                        Math.Max(0, inner.Height - reserveTop - reserveBottom)
                    );
                }
            }
            catch { /* best-effort */ }
            var borderRect = new Rectangle(
                  shadow,
                  shadow,
                  Math.Max(0,  Width - (shadow) * 2),
                  Math.Max(0,  Height - (shadow) * 2)
              );
            // Border rectangle like BeepControl
            if (border > 0)
            {
                int halfPen = (int)Math.Ceiling( BorderThickness / 2f);
                 borderRect = new Rectangle(
                    shadow + halfPen,
                    shadow + halfPen,
                    Math.Max(0,  Width - (shadow + halfPen) * 2),
                    Math.Max(0,  Height - (shadow + halfPen) * 2)
                );
            }


                // Compute icon-adjusted content
                Rectangle contentRect = inner;
            bool hasLeading = !string.IsNullOrEmpty( LeadingIconPath) || !string.IsNullOrEmpty( LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty( TrailingIconPath) || !string.IsNullOrEmpty( TrailingImagePath) ||  ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var iconsHelper = new BaseControlIconsHelper(this);
                iconsHelper.UpdateLayout(inner);
                contentRect = iconsHelper.AdjustedContentRect;
            }

            _drawingRect = inner;    // inner rectangle equivalent for derived controls to use
            _borderRect = borderRect;
            _contentRect = contentRect;
             if (!(hasLeading || hasTrailing)) return;

            var icons = new BaseControlIconsHelper(this);
            icons.UpdateLayout(_drawingRect);
            var lead = icons.LeadingRect;
            var trail = icons.TrailingRect;
           // if (!lead.IsEmpty &&  LeadingIconClickable) register("ClassicLeadingIcon", lead,  TriggerLeadingIconClick);
          //  if (!trail.IsEmpty &&  TrailingIconClickable) register("ClassicTrailingIcon", trail,  TriggerTrailingIconClick);
        }
    }
}