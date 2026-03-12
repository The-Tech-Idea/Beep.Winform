using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Abstract intermediate base for all design-system field painters.
    /// Adds variant-specific overrides for filled, segmented, borderless, and
    /// search-icon button behaviours on top of <see cref="ComboBoxFieldPainterBase"/>.
    /// </summary>
    internal abstract class DesignSystemComboBoxFieldPainterBase : ComboBoxFieldPainterBase
    {
        protected virtual int CornerRadius => 6;
        protected virtual bool IsFilled => false;
        protected virtual bool IsSegmented => false;
        protected virtual bool IsBorderless => false;
        protected virtual bool IsSearchIconButton => false;
        protected override bool ShowButtonSeparator => !IsBorderless && !IsFilled && !IsSegmented;

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;

            if (IsFilled)
            {
                Color fillBase = _theme?.ComboBoxBackColor ?? _theme?.BackColor ?? SystemColors.Window;
                Color fillTint = Color.FromArgb(_owner.Focused ? 30 : 20, fillBase.R, fillBase.G, fillBase.B);
                using var path = GetRoundedRectPath(textAreaRect, ScaleX(CornerRadius));
                g.FillPath(PaintersFactory.GetSolidBrush(fillTint), path);
            }

            base.DrawTextArea(g, textAreaRect);
        }

        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            if (IsSegmented)
            {
                Color accent = _theme?.PrimaryColor ?? _theme?.ComboBoxHoverBorderColor ?? Color.FromArgb(0, 120, 215);
                Color fill = _owner.IsButtonHovered || _owner.IsDropdownOpen
                    ? PathPainterHelpers.WithAlphaIfNotEmpty(accent, 220)
                    : PathPainterHelpers.WithAlphaIfNotEmpty(accent, 170);

                using var segPath = GetRoundedRectPath(buttonRect, ScaleX(6));
                g.FillPath(PaintersFactory.GetSolidBrush(fill), segPath);

                Color segmentedIcon = _theme?.OnPrimaryColor ?? Color.Empty;
                if (segmentedIcon == Color.Empty) segmentedIcon = Color.White;
                DrawButtonIcon(g, buttonRect, segmentedIcon, false);
                return;
            }

            if (_owner.IsButtonHovered && _owner.Enabled)
            {
                Color hoverFill = PathPainterHelpers.WithAlphaIfNotEmpty(
                    _theme?.ComboBoxHoverBackColor != Color.Empty
                        ? _theme.ComboBoxHoverBackColor
                        : (_theme?.PrimaryColor ?? Color.Empty), 48);
                if (hoverFill != Color.Empty && hoverFill.A > 0)
                {
                    using var hoverPath = GetRoundedRectPath(Rectangle.Inflate(buttonRect, -ScaleX(1), -ScaleY(1)), ScaleX(4));
                    g.FillPath(PaintersFactory.GetSolidBrush(hoverFill), hoverPath);
                }
            }

            if (ShowButtonSeparator)
            {
                Color separatorColor = PathPainterHelpers.WithAlphaIfNotEmpty(
                    _theme?.ComboBoxBorderColor != Color.Empty ? _theme.ComboBoxBorderColor : (_theme?.BorderColor ?? Color.Gray),
                    160);
                int sepTop = buttonRect.Top + ScaleY(6);
                int sepBottom = buttonRect.Bottom - ScaleY(6);
                if (sepBottom > sepTop)
                {
                    g.DrawLine(PaintersFactory.GetPen(separatorColor, 1f), buttonRect.Left, sepTop, buttonRect.Left, sepBottom);
                }
            }

            DrawButtonIcon(g, buttonRect, GetArrowColor(), IsSearchIconButton);
        }

        private void DrawButtonIcon(Graphics g, Rectangle buttonRect, Color iconColor, bool searchIcon)
        {
            int iconSize = Math.Min(ScaleX(15), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8));
            if (iconSize <= 4)
            {
                DrawDropdownArrow(g, buttonRect, iconColor, _owner.IsDropdownOpen);
                return;
            }

            var iconRect = new Rectangle(
                buttonRect.X + (buttonRect.Width - iconSize) / 2,
                buttonRect.Y + (buttonRect.Height - iconSize) / 2,
                iconSize,
                iconSize);

            if (searchIcon)
            {
                DrawSvgIcon(g, iconRect, SvgsUI.Search, iconColor, 0.9f);
            }
            else
            {
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, iconColor, 0.9f, _owner?.ChevronAngle ?? 0f);
            }
        }
    }
}
