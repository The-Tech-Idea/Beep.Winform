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