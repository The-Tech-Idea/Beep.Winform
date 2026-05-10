using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Shared layout helpers for painter-owned caption geometry.
    /// Keeps reusable layout logic separate from FormPainterRenderHelper, which is reserved for drawing.
    /// </summary>
    internal static class FormPainterLayoutHelper
    {
        internal static bool TryBuildStandardRightAlignedCaptionLayout(
            BeepiFormPro owner,
            int captionHeight,
            int buttonWidth,
            int searchBoxWidth,
            int searchBoxPadding,
            bool includeCustomAction,
            out PainterLayoutInfo layout,
            out int titleRightBoundary)
        {
            layout = new PainterLayoutInfo();

            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                titleRightBoundary = owner.ClientSize.Width;
                return false;
            }

            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);

            int buttonY = 0;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            var buttonSize = new Size(buttonWidth, captionHeight);

            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.Close, layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.Maximize, layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;

                layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.Minimize, layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.Style, layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.Theme, layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (includeCustomAction && owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.CustomAction, layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowProfileButton)
            {
                layout.ProfileButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea(FormHitAreaNames.Profile, layout.ProfileButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(
                    buttonX - searchBoxWidth - searchBoxPadding,
                    buttonY + searchBoxPadding / 2,
                    searchBoxWidth,
                    captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea(FormHitAreaNames.Search, layout.SearchBoxRect, HitAreaType.TextBox);
                buttonX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }

            titleRightBoundary = buttonX;
            return true;
        }

        internal static bool TryBuildTrafficLightCaptionLayout(
            BeepiFormPro owner,
            int captionHeight,
            int leftButtonSize,
            int leftButtonSpacing,
            int leftStartX,
            int rightButtonWidth,
            int rightInset,
            int searchBoxWidth,
            int searchBoxPadding,
            bool includeCustomAction,
            out PainterLayoutInfo layout,
            out int leftBoundary,
            out int rightBoundary)
        {
            layout = new PainterLayoutInfo();

            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                leftBoundary = 0;
                rightBoundary = owner.ClientSize.Width;
                return false;
            }

            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            owner._hits.Register(FormHitAreaNames.Caption, layout.CaptionRect, HitAreaType.Drag);

            int leftButtonY = (captionHeight - leftButtonSize) / 2;
            int leftX = leftStartX;

            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(leftX, leftButtonY, leftButtonSize, leftButtonSize);
                owner._hits.RegisterHitArea(FormHitAreaNames.Close, layout.CloseButtonRect, HitAreaType.Button);
                leftX += leftButtonSize + leftButtonSpacing;
            }

            if (owner.ShowMinMaxButtons)
            {
                layout.MinimizeButtonRect = new Rectangle(leftX, leftButtonY, leftButtonSize, leftButtonSize);
                owner._hits.RegisterHitArea(FormHitAreaNames.Minimize, layout.MinimizeButtonRect, HitAreaType.Button);
                leftX += leftButtonSize + leftButtonSpacing;

                layout.MaximizeButtonRect = new Rectangle(leftX, leftButtonY, leftButtonSize, leftButtonSize);
                owner._hits.RegisterHitArea(FormHitAreaNames.Maximize, layout.MaximizeButtonRect, HitAreaType.Button);
                leftX += leftButtonSize + leftButtonSpacing;
            }

            int rightX = owner.ClientSize.Width - rightButtonWidth - rightInset;

            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea(FormHitAreaNames.Style, layout.StyleButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }

            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea(FormHitAreaNames.Theme, layout.ThemeButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }

            if (includeCustomAction && owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea(FormHitAreaNames.CustomAction, layout.CustomActionButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }

            if (owner.ShowProfileButton)
            {
                layout.ProfileButtonRect = new Rectangle(rightX, 0, rightButtonWidth, captionHeight);
                owner._hits.RegisterHitArea(FormHitAreaNames.Profile, layout.ProfileButtonRect, HitAreaType.Button);
                rightX -= rightButtonWidth;
            }

            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(
                    rightX - searchBoxWidth - searchBoxPadding,
                    searchBoxPadding / 2,
                    searchBoxWidth,
                    captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea(FormHitAreaNames.Search, layout.SearchBoxRect, HitAreaType.TextBox);
                rightX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }

            leftBoundary = leftX;
            rightBoundary = rightX;
            return true;
        }
    }
}