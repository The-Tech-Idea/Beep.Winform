using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    /// <summary>
    /// Layout data for one chip cell in a multi-select field.
    /// </summary>
    internal sealed class ChipLayoutItem
    {
        /// <summary>The source item. Null for the synthetic "+N" overflow chip.</summary>
        public SimpleItem Item            { get; init; }
        public Rectangle  ChipRect        { get; init; }
        /// <summary>Hit rect for the chip's close (×) button. Empty for overflow chip.</summary>
        public Rectangle  CloseButtonRect { get; init; }
        /// <summary>True when this is the "+N more" overflow summary chip.</summary>
        public bool       IsOverflow      { get; init; }
        public int        OverflowCount   { get; init; }
    }

    /// <summary>
    /// A complete layout snapshot: all sub-rectangles needed to paint one frame
    /// of <see cref="BeepComboBox"/>.
    /// Produced by <see cref="ComboBoxLayoutEngine.Compute"/>.
    /// </summary>
    internal sealed class ComboBoxLayoutSnapshot
    {
        public Rectangle DrawingRect         { get; init; }
        public Rectangle TextAreaRect        { get; init; }
        public Rectangle DropdownButtonRect  { get; init; }
        public Rectangle ImageRect           { get; init; }
        public Rectangle ClearButtonRect     { get; init; }
        public IReadOnlyList<ChipLayoutItem> Chips { get; init; }

        // Geometry tokens mirrored from the active ComboBoxVisualTokens
        public int     CornerRadius         { get; init; }
        public int     TextInset            { get; init; }
        public Padding InnerPadding         { get; init; }
        public bool    ShowButtonSeparator  { get; init; }
        public bool    UseSegmentedTrigger  { get; init; }
    }

    /// <summary>
    /// Stateless layout calculator for <see cref="BeepComboBox"/>.
    /// <para>
    /// No caching, no stored state. Both <c>DrawContent</c> (DrawingRect) and
    /// <c>Draw(graphics, rect)</c> use the same algorithm; only the source rect differs.
    /// </para>
    /// </summary>
    internal static class ComboBoxLayoutEngine
    {
        // ── Layout constants (logical px — scaled by owner helpers) ──────────
        private const int MinButtonWidthLogical = 28;
        private const int ClearButtonWidthLogical = 22;
        private const int ChipCloseSize    = 16;
        private const int ChipHPad         = 8;
        private const int ChipGap          = 4;
        private const int MaxChipRows      = 2;

        /// <summary>
        /// Computes a full <see cref="ComboBoxLayoutSnapshot"/> from the source rect
        /// and the current normalized render state.
        /// </summary>
        /// <param name="drawingRect">Target rectangle (DrawingRect or container-supplied).</param>
        /// <param name="state">Render state snapshot for this frame.</param>
        /// <param name="owner">Owner control — used ONLY for DPI scale helpers.</param>
        public static ComboBoxLayoutSnapshot Compute(
            Rectangle       drawingRect,
            ComboBoxRenderState state,
            BeepComboBox    owner)
        {
            if (owner == null || drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return BuildEmpty(drawingRect, state);

            var tokens   = state.VisualTokens;
            int usableH  = drawingRect.Height;
            Padding effectivePadding = owner.InnerPadding != Padding.Empty
                ? owner.InnerPadding
                : (tokens?.InnerPadding ?? new Padding(8, 4, 8, 4));

            // ── 1. Dropdown button ──────────────────────────────────────────
            int minBtnW  = owner.ScaleLogicalX(MinButtonWidthLogical);
            int preferredButton = owner.DropdownButtonWidth > 0
                ? owner.ScaleLogicalX(owner.DropdownButtonWidth)
                : (tokens?.ButtonWidth ?? MinButtonWidthLogical);
            int buttonW  = Math.Max(minBtnW, preferredButton);
            buttonW      = Math.Min(buttonW, drawingRect.Width / 3);
            buttonW      = Math.Max(minBtnW, buttonW);

            var buttonRect = new Rectangle(
                drawingRect.Right - buttonW,
                drawingRect.Y,
                buttonW,
                usableH);

            // ── 2. Leading image / icon ────────────────────────────────────
            var imageRect     = Rectangle.Empty;
            int imageConsumed = 0;
            if (state.HasLeadingImage)
            {
                int inset    = Math.Max(4, Math.Min(8, usableH / 6));
                int iconSize = Math.Max(8, usableH - inset * 2);
                int iconGap  = Math.Max(4, iconSize / 4);
                imageRect = new Rectangle(
                    drawingRect.X + inset,
                    drawingRect.Y + (usableH - iconSize) / 2,
                    iconSize, iconSize);
                imageConsumed = iconSize + iconGap + inset;
            }

            // ── 3. Clear button carve-out ──────────────────────────────────
            var clearRect     = Rectangle.Empty;
            int clearConsumed = 0;
            if (state.ShowClearButton)
            {
                int cbw  = Math.Max(16, Math.Min(owner.ScaleLogicalX(ClearButtonWidthLogical), drawingRect.Width / 4));
                clearRect = new Rectangle(
                    buttonRect.Left - cbw,
                    drawingRect.Y, cbw, usableH);
                clearConsumed = cbw;
            }

            // ── 4. Text area ───────────────────────────────────────────────
            int leftInset  = imageConsumed + owner.ScaleLogicalX(Math.Max(0, effectivePadding.Left));
            int rightLimit = buttonRect.Left - clearConsumed;
            rightLimit -= owner.ScaleLogicalX(Math.Max(0, effectivePadding.Right));
            int textX      = drawingRect.X + leftInset;
            int textW      = Math.Max(1, rightLimit - textX);
            var textRect   = new Rectangle(textX, drawingRect.Y, textW, usableH);

            // ── 5. Chip layout (multi-select only) ─────────────────────────
            var chips = (state.IsMultiSelect && state.SelectedChips?.Count > 0)
                ? ComputeChips(state, textRect, owner)
                : (IReadOnlyList<ChipLayoutItem>)Array.Empty<ChipLayoutItem>();

            return new ComboBoxLayoutSnapshot
            {
                DrawingRect        = drawingRect,
                TextAreaRect       = textRect,
                DropdownButtonRect = buttonRect,
                ImageRect          = imageRect,
                ClearButtonRect    = clearRect,
                Chips              = chips,
                CornerRadius       = tokens?.CornerRadius      ?? 6,
                TextInset          = tokens?.TextInset         ?? 8,
                InnerPadding       = effectivePadding,
                ShowButtonSeparator = tokens?.ShowButtonSeparator ?? false,
                UseSegmentedTrigger = tokens?.UseSegmentedTrigger ?? false,
            };
        }

        // ────────────────────────────────────────────────────────────────────
        // Chip layout
        // ────────────────────────────────────────────────────────────────────

        private static IReadOnlyList<ChipLayoutItem> ComputeChips(
            ComboBoxRenderState state,
            Rectangle           textAreaRect,
            BeepComboBox        owner)
        {
            if (textAreaRect.Width <= 0 || textAreaRect.Height <= 0)
                return Array.Empty<ChipLayoutItem>();

            var font = state.ThemeTokens?.LabelFont ?? SystemFonts.DefaultFont;
            bool singleLineCollapse = false;

            return ComboBoxChipLayoutEngine.Compute(
                textAreaRect,
                state.SelectedChips,
                font,
                owner,
                singleLineCollapse,
                maxRows: MaxChipRows,
                chipPaddingXLogical: ChipHPad,
                chipPaddingYLogical: 2,
                gapLogical: ChipGap,
                closeSizeLogical: ChipCloseSize);
        }

        // ────────────────────────────────────────────────────────────────────
        private static ComboBoxLayoutSnapshot BuildEmpty(Rectangle drawingRect, ComboBoxRenderState state)
            => new ComboBoxLayoutSnapshot
            {
                DrawingRect        = drawingRect,
                TextAreaRect       = Rectangle.Empty,
                DropdownButtonRect = Rectangle.Empty,
                ImageRect          = Rectangle.Empty,
                ClearButtonRect    = Rectangle.Empty,
                Chips              = Array.Empty<ChipLayoutItem>(),
                CornerRadius       = state?.VisualTokens?.CornerRadius  ?? 6,
                TextInset          = state?.VisualTokens?.TextInset     ?? 8,
                InnerPadding       = state?.VisualTokens?.InnerPadding  ?? new Padding(8, 4, 8, 4),
            };
    }
}
