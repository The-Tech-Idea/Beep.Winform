// BeepMenuBar.Layout.cs
// Phase 02 — Partial-Class Split.
//
// Owns hit-area registration, per-item rectangle computation, font-height
// probes, vertical-padding-by-style policy, and GetPreferredSize. All
// chrome sizing flows through the BeepStyling helpers (border, padding,
// shadow) so the menubar respects the same style-driven box model as
// every other Beep control.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // ─────────────────────────────────────────────────────────────────
        // Hit-area registration
        // ─────────────────────────────────────────────────────────────────

        private void RefreshHitAreas()
        {
            if (items == null || items.Count == 0)
            {
                ClearHitList();
                return;
            }

            RefreshLegacyHitAreas();
        }

        private void RefreshLegacyHitAreas()
        {
            UpdateDrawingRect();
            ClearHitList();

            var menuRects = CalculateMenuItemRects();

            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];
                int itemIndex = i; // capture for closure

                AddHitArea(
                    $"MenuItem_{itemIndex}",
                    rect,
                    null,
                    () => HandleMenuItemClick(item, itemIndex));
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Per-item rectangle computation
        // ─────────────────────────────────────────────────────────────────

        // Visibility relaxed in Phase 07 so the Accessibility partial can
        // resolve per-item screen bounds for AccessibleObject.Bounds.
        internal List<Rectangle> CalculateMenuItemRects()
        {
            var rects = new List<Rectangle>();
            if (items == null || items.Count == 0) return rects;

            int gapBetweenButtons = ScaleUi(5);
            int startX            = ScaleUi(5);

            // Style-driven chrome sizes (border + padding + shadow blur).
            int styleBorderWidth = (int)BeepStyling.GetBorderThickness(ControlStyle);
            int stylePadding     = BeepStyling.GetPadding(ControlStyle);
            int styleShadowDepth = StyleShadows.HasShadow(ControlStyle)
                ? Math.Max(2, StyleShadows.GetShadowBlur(ControlStyle) / 2)
                : 0;

            // IMPORTANT: shadow contributes to width too — it extends past the border.
            int totalChromeWidth  = (styleBorderWidth * 2) + (stylePadding * 2) + (styleShadowDepth * 2);
            int totalChromeHeight = (styleBorderWidth * 2) + (stylePadding * 2) + styleShadowDepth;

            // Content height derives from the actual font metric (resilient to GDI/DPI quirks).
            int fontHeight     = GetFontHeightSafe(_textFont, this);
            int contentPadding = GetVerticalPaddingForStyle(ControlStyle);
            int contentHeight  = fontHeight + contentPadding;
            int outerItemHeight = contentHeight + totalChromeHeight;

            // Symmetric vertical breathing room (6 px top + 6 px bottom, DPI-scaled).
            int verticalBuffer = ScaleUi(6);
            int buttonTop = (Height - outerItemHeight) / 2;
            if (buttonTop < 0) buttonTop = verticalBuffer;
            else buttonTop = Math.Max(buttonTop, verticalBuffer);

            int currentX = startX;
            foreach (var item in items)
            {
                if (item == null) continue;

                int outerItemWidth = CalculateMenuItemWidth(item, totalChromeWidth);

                var rect = new Rectangle(currentX, buttonTop, outerItemWidth, outerItemHeight);
                rects.Add(rect);
                currentX += outerItemWidth + gapBetweenButtons;
            }

            return rects;
        }

        private static int GetFontHeightSafe(Font font, Control context)
        {
            try
            {
                if (font == null)
                    return context?.Font?.Height ?? SystemFonts.DefaultFont.Height;

                var sz = TextRenderer.MeasureText(
                    "Ag", font,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

                return Math.Max(1, sz.Height);
            }
            catch
            {
                try { return context?.Font?.Height ?? SystemFonts.DefaultFont.Height; }
                catch { return 12; }
            }
        }

        private int CalculateMenuItemWidth(SimpleItem item, int totalChromeWidth)
        {
            if (item == null) return 80;

            int textWidth = 0;
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextRenderer.MeasureText(item.Text, _textFont);
                textWidth = textSize.Width;
            }

            int imageWidth          = !string.IsNullOrEmpty(item.ImagePath) ? ScaledImageSize + ScaleUi(8) : 0;
            int internalContentPad  = ScaleUi(16); // 8 px on each side
            int contentWidth        = textWidth + imageWidth + internalContentPad;
            int totalWidth          = contentWidth + totalChromeWidth;

            return Math.Max(totalWidth, ScaleUi(60)); // DPI-scaled floor.
        }

        // ─────────────────────────────────────────────────────────────────
        // Style-specific vertical padding policy
        //
        // Some styles ship larger borders/shadows and need extra padding
        // for the text to feel optically centred.
        // ─────────────────────────────────────────────────────────────────
        private int GetVerticalPaddingForStyle(BeepControlStyle style)
        {
            switch (style)
            {
                case BeepControlStyle.Fluent:
                case BeepControlStyle.Fluent2:    return ScaleUi(10);
                case BeepControlStyle.Gnome:      return ScaleUi(12);
                case BeepControlStyle.Neumorphism: return ScaleUi(14);
                case BeepControlStyle.iOS15:      return ScaleUi(12);
                case BeepControlStyle.Kde:        return ScaleUi(10);
                case BeepControlStyle.Tokyo:      return ScaleUi(12);
                default:                          return ScaleUi(8);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // GetPreferredSize
        // ─────────────────────────────────────────────────────────────────

        public override Size GetPreferredSize(Size proposedSize)
        {
            int preferredWidth = proposedSize.Width <= 0 ? Width : Math.Max(Width, proposedSize.Width);

            // Respect developer-set height — see Properties.cs `Height` setter.
            if (_heightManuallySet)
                return new Size(preferredWidth, Height);

            int fontHeight     = GetFontHeightSafe(_textFont, this);
            int contentPadding = GetVerticalPaddingForStyle(ControlStyle);

            int styleBorderWidth = (int)BeepStyling.GetBorderThickness(ControlStyle);
            int stylePadding     = BeepStyling.GetPadding(ControlStyle);
            int styleShadowDepth = StyleShadows.HasShadow(ControlStyle)
                ? Math.Max(2, StyleShadows.GetShadowBlur(ControlStyle) / 2)
                : 0;

            int totalChromeHeight = (styleBorderWidth * 2) + (stylePadding * 2) + styleShadowDepth;
            int outerItemHeight   = fontHeight + contentPadding + totalChromeHeight;

            int verticalBuffer  = ScaleUi(12);
            int preferredHeight = Math.Max(ScaledMenuItemHeight + verticalBuffer, outerItemHeight + verticalBuffer);

            return new Size(preferredWidth, preferredHeight);
        }

        // ─────────────────────────────────────────────────────────────────
        // Font-driven menu-item-height calculation
        //
        // Called once during construction. After that the height is
        // locked via _menuItemHeightLocked so a FormStyle swap cannot
        // resize the bar mid-flight.
        // ─────────────────────────────────────────────────────────────────
        private void UpdateMenuItemHeightForFont()
        {
            if (_textFont == null) return;
            if (_menuItemHeightLocked) return;

            int fontHeight = GetFontHeightSafe(_textFont, this);
            int minHeight  = fontHeight + ScaleUi(8); // 4 px top + 4 px bottom.

            if (_menuItemHeight < minHeight)
            {
                _menuItemHeight = minHeight;
            }
        }
    }
}
