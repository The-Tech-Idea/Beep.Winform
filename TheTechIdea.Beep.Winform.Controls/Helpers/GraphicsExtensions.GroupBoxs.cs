using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{

    public static partial class GraphicsExtensions
    {
     

        #region Core Titled Border Path Generation (Extended)

        /// <summary>
        /// Creates only the shadow and border paths for a titled panel.
        /// Returns (ShadowPath, BorderPath). Caller owns both and must Dispose().
        /// </summary>
        public static (GraphicsPath ShadowPath, GraphicsPath BorderPath) CreateTitledShadowAndBorderPaths(
            RectangleF bounds,
            RectangleF headerTextRect,
            BorderTextPosition position = BorderTextPosition.Top,
            TitledBorderStyle style = TitledBorderStyle.ClassicGap,
            float cornerRadius = 10f,
            float gapPadding = 6f,
            float shadowOffsetX = 4f,
            float shadowOffsetY = 4f,
            float shadowBlur = 8f)
        {
            var result = CreateTitledBorderPaths(
                bounds, headerTextRect, position, style,
                cornerRadius, gapPadding, headerPadding: 6f, edgeInset: 12f,
                headerOverlap: 0f, headerAreaHeight: 44f);

            // Clean up unused paths from the main function
            result.HeaderPath?.Dispose();
            result.DividerPath?.Dispose();

            // Generate the shadow path based on the main border path
            GraphicsPath shadowPath = null;
            if (shadowOffsetX != 0 || shadowOffsetY != 0 || shadowBlur > 0)
            {
                shadowPath = CreateShadowPath(result.BorderPath, shadowOffsetX, shadowOffsetY, shadowBlur);
            }

            return (shadowPath, result.BorderPath);
        }

        /// <summary>
        /// Creates the main titled border path and associated elements (text location, header box, header path, divider path).
        /// Caller owns the returned GraphicsPath instances (BorderPath, HeaderPath, DividerPath) and must Dispose them.
        /// </summary>
        public static (PointF TextLocation, RectangleF HeaderBounds, GraphicsPath BorderPath, GraphicsPath HeaderPath, GraphicsPath DividerPath)
        CreateTitledBorderPaths(
            RectangleF bounds,
            RectangleF headerTextRect,
            BorderTextPosition position = BorderTextPosition.Top,
            TitledBorderStyle style = TitledBorderStyle.ClassicGap,
            float cornerRadius = 10f,
            float gapPadding = 6f,
            float headerPadding = 6f,
            float edgeInset = 12f,
            float headerOverlap = 0f,
            float headerAreaHeight = 44f)
        {
            // Compute the header "box" (text size + padding) positioned on the requested side.
            RectangleF headerBox = ComputeHeaderBox(bounds, headerTextRect, position, headerPadding, edgeInset, headerOverlap, headerAreaHeight, style);
            // Recommended location for drawing the text (inside the header box).
            PointF textLocation = new PointF(headerBox.Left + headerPadding, headerBox.Top + headerPadding);

            GraphicsPath headerPath = null;
            GraphicsPath dividerPath = null;

            // Build border path according to style.
            GraphicsPath borderPath;
            switch (style)
            {
                case TitledBorderStyle.HeaderArea:
                    // Full border; header is inside border at the top, with a divider line.
                    borderPath = CreateRoundedRectanglePath(bounds, new CornerRadius((int)cornerRadius));
                    dividerPath = CreateHeaderDividerPath(bounds, position, headerAreaHeight, cornerRadius);
                    break;
                case TitledBorderStyle.TopNotch:
                    // Notch is only meaningful for Top; fallback to ClassicGap otherwise.
                    if (position == BorderTextPosition.Top)
                    {
                        // Notch uses the header box span on the top edge.
                        var notchSpan = new RectangleF(headerBox.Left, bounds.Top, headerBox.Width, 1);
                        borderPath = CreateTopNotchedRoundedRectanglePath(bounds, notchSpan,
                            notchDepth: Math.Min(18f, Math.Max(8f, headerBox.Height)),
                            cornerRadius: cornerRadius,
                            notchCornerRadius: Math.Min(8f, cornerRadius));
                    }
                    else
                    {
                        borderPath = CreateRoundedBorderWithGap(bounds, cornerRadius, position,
                            GapFromHeaderBox(bounds, headerBox, position, gapPadding));
                    }
                    break;
                case TitledBorderStyle.CutCornerGap:
                    // Cut corners + top gap; for other sides fallback to ClassicGap rounded.
                    if (position == BorderTextPosition.Top)
                    {
                        borderPath = CreateCutCornerBorderWithTopGap(bounds, cut: Math.Min(12f, cornerRadius + 2),
                            headerBox: headerBox, gapPadding: gapPadding);
                    }
                    else
                    {
                        borderPath = CreateRoundedBorderWithGap(bounds, cornerRadius, position,
                            GapFromHeaderBox(bounds, headerBox, position, gapPadding));
                    }
                    break;
                case TitledBorderStyle.FilledTab:
                case TitledBorderStyle.PillBadge:
                case TitledBorderStyle.ClassicGap:
                    borderPath = CreateRoundedBorderWithGap(bounds, cornerRadius, position,
                        GapFromHeaderBox(bounds, headerBox, position, gapPadding));
                    break;
                // --- NEW STYLES ---
                case TitledBorderStyle.CardHeader:
                    borderPath = CreateRoundedRectanglePath(bounds, new CornerRadius((int)cornerRadius));
                    // Optionally calculate headerBox differently for this style if needed
                    // For now, assume headerPath is returned for potential separate drawing
                    float tabRadius = Math.Min(headerBox.Height / 2f, cornerRadius);
                    headerPath = CreateRoundedRectanglePath(headerBox, new CornerRadius((int)tabRadius));
                    // No divider needed for CardHeader in this example
                    // dividerPath remains null
                    break;

                case TitledBorderStyle.ProtrudingTab:
                    // Example: A tab protrudes from the top-left, rest is rounded.
                    borderPath = CreateProtrudingTabBorderPath(bounds, cornerRadius, position, headerBox, gapPadding);
                    // The headerPath could represent the tab itself
                    float tabRad = Math.Min(headerBox.Height / 2f, cornerRadius);
                    headerPath = CreateRoundedRectanglePath(headerBox, new CornerRadius((int)tabRad));
                    break;

                case TitledBorderStyle.AngledHeader:
                    // Example: The top portion of the panel is an angled parallelogram for the header.
                    borderPath = CreateAngledHeaderBorderPath(bounds, headerBox, cornerRadius, headerAreaHeight);
                    // Header path is the angled top part
                    headerPath = CreateAngledHeaderPath(bounds, headerBox, headerAreaHeight);
                    break;

                case TitledBorderStyle.DocumentStyle:
                    // Example: Standard rounded rect with a small triangular cut-out indicating a "folded" corner.
                    borderPath = CreateDocumentStyleBorderPath(bounds, cornerRadius, position);
                    // Potentially calculate headerBox differently for this style if needed
                    break;

                case TitledBorderStyle.ChatBubbleStyle:
                    // Example: Uses CreateSpeechBubble logic.
                    borderPath = CreateChatBubbleBorderPath(bounds, cornerRadius, position, headerBox);
                    // Header path might just be text location hint here, or the whole bubble
                    break;

                case TitledBorderStyle.SegmentedHeader:
                    // Example: Create multiple small "tab" shapes along the top edge, highlighting the active one (this).
                    borderPath = CreateSegmentedHeaderBorderPath(bounds, headerBox, cornerRadius);
                    // HeaderPath represents the group of segments
                    headerPath = CreateSegmentedHeaderPath(bounds, headerBox, cornerRadius);
                    break;

                case TitledBorderStyle.CutoutTitleCorner:
                    // Example: A notch or cutout (maybe rounded or angular) in a corner where the title goes.
                    borderPath = CreateCutoutCornerBorderPath(bounds, position, headerBox, cornerRadius);
                    // Header path is the cutout area itself, potentially
                    // headerPath = CalculateCutoutPath(...);
                    break;

                // --- END NEW STYLES ---
                default:
                    borderPath = CreateRoundedBorderWithGap(bounds, cornerRadius, position,
                        GapFromHeaderBox(bounds, headerBox, position, gapPadding));
                    break;
            }

            // Optional header "tab" / "badge" background.
            if (style == TitledBorderStyle.FilledTab)
            {
                float tabRadius = Math.Min(headerBox.Height / 2f, cornerRadius);
                headerPath = CreateRoundedRectanglePath(headerBox, new CornerRadius((int)tabRadius));
            }
            else if (style == TitledBorderStyle.PillBadge)
            {
                headerPath = CreateCapsulePath(headerBox);
            }

            return (textLocation, headerBox, borderPath, headerPath, dividerPath);
        }

        #endregion

        #region Helper Functions for Path Generation

        private static RectangleF ComputeHeaderBox(
            RectangleF bounds,
            RectangleF headerTextRect,
            BorderTextPosition position,
            float headerPadding,
            float edgeInset,
            float headerOverlap,
            float headerAreaHeight,
            TitledBorderStyle style)
        {
            float textW = Math.Max(0f, headerTextRect.Width);
            float textH = Math.Max(0f, headerTextRect.Height);
            float boxW = textW + headerPadding * 2f;
            float boxH = textH + headerPadding * 2f;
            bool hasOffset =
                (position == BorderTextPosition.Top || position == BorderTextPosition.Bottom)
                    ? (Math.Abs(headerTextRect.X) > 0.01f)
                    : (Math.Abs(headerTextRect.Y) > 0.01f);
            float x = 0f, y = 0f;
            if (style == TitledBorderStyle.HeaderArea)
            {
                // Header box is inside the border "header area".
                switch (position)
                {
                    case BorderTextPosition.Bottom:
                        x = hasOffset ? bounds.Left + edgeInset + headerTextRect.X : bounds.Left + (bounds.Width - boxW) / 2f;
                        y = bounds.Bottom - headerAreaHeight + (headerAreaHeight - boxH) / 2f;
                        break;
                    case BorderTextPosition.Left:
                        x = bounds.Left + (headerAreaHeight - boxW) / 2f;
                        y = hasOffset ? bounds.Top + edgeInset + headerTextRect.Y : bounds.Top + (bounds.Height - boxH) / 2f;
                        break;
                    case BorderTextPosition.Right:
                        x = bounds.Right - headerAreaHeight + (headerAreaHeight - boxW) / 2f;
                        y = hasOffset ? bounds.Top + edgeInset + headerTextRect.Y : bounds.Top + (bounds.Height - boxH) / 2f;
                        break;
                    case BorderTextPosition.Top:
                    default:
                        x = hasOffset ? bounds.Left + edgeInset + headerTextRect.X : bounds.Left + (bounds.Width - boxW) / 2f;
                        y = bounds.Top + (headerAreaHeight - boxH) / 2f;
                        break;
                }
                return new RectangleF(x, y, boxW, boxH);
            }

            // Default: header box sits centered on the border line (GroupBox-like).
            switch (position)
            {
                case BorderTextPosition.Bottom:
                    x = hasOffset ? bounds.Left + edgeInset + headerTextRect.X : bounds.Left + (bounds.Width - boxW) / 2f;
                    y = (bounds.Bottom - boxH / 2f) - headerOverlap;
                    break;
                case BorderTextPosition.Left:
                    x = (bounds.Left - boxW / 2f) + headerOverlap;
                    y = hasOffset ? bounds.Top + edgeInset + headerTextRect.Y : bounds.Top + (bounds.Height - boxH) / 2f;
                    break;
                case BorderTextPosition.Right:
                    x = (bounds.Right - boxW / 2f) - headerOverlap;
                    y = hasOffset ? bounds.Top + edgeInset + headerTextRect.Y : bounds.Top + (bounds.Height - boxH) / 2f;
                    break;
                case BorderTextPosition.Top:
                default:
                    x = hasOffset ? bounds.Left + edgeInset + headerTextRect.X : bounds.Left + (bounds.Width - boxW) / 2f;
                    y = (bounds.Top - boxH / 2f) + headerOverlap;
                    break;
            }
            return new RectangleF(x, y, boxW, boxH);
        }

        private struct GapSpan
        {
            public float Start;
            public float End;
        }

        private static GapSpan GapFromHeaderBox(RectangleF bounds, RectangleF headerBox, BorderTextPosition position, float gapPadding)
        {
            switch (position)
            {
                case BorderTextPosition.Bottom:
                case BorderTextPosition.Top:
                    return new GapSpan { Start = headerBox.Left - gapPadding, End = headerBox.Right + gapPadding };
                case BorderTextPosition.Left:
                case BorderTextPosition.Right:
                    return new GapSpan { Start = headerBox.Top - gapPadding, End = headerBox.Bottom + gapPadding };
                default:
                    return new GapSpan { Start = headerBox.Left - gapPadding, End = headerBox.Right + gapPadding };
            }
        }

        private static GraphicsPath CreateRoundedBorderWithGap(RectangleF rect, float radius, BorderTextPosition position, GapSpan gap)
        {
            float r = Math.Max(0f, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2f));
            float d = r * 2f;

            // Clamp gap to the side span that excludes the corner arcs.
            if (position == BorderTextPosition.Top || position == BorderTextPosition.Bottom)
            {
                float min = rect.Left + r;
                float max = rect.Right - r;
                float s = Math.Max(min, Math.Min(gap.Start, max));
                float e = Math.Max(min, Math.Min(gap.End, max));
                if (e < s) (s, e) = (e, s);
                gap = new GapSpan { Start = s, End = e };
            }
            else
            {
                float min = rect.Top + r;
                float max = rect.Bottom - r;
                float s = Math.Max(min, Math.Min(gap.Start, max));
                float e = Math.Max(min, Math.Min(gap.End, max));
                if (e < s) (s, e) = (e, s);
                gap = new GapSpan { Start = s, End = e };
            }

            var path = new GraphicsPath();

            // No rounding? Use a simple rectangle with a missing segment.
            if (r <= 0.01f)
            {
                return CreateRectangleBorderWithGap(rect, position, gap);
            }

            // Build a single open figure that traces the border but skips the gap segment.
            switch (position)
            {
                case BorderTextPosition.Top:
                    // Start at (gap.End, top) and go clockwise, ending at (gap.Start, top).
                    path.StartFigure();
                    if (gap.End < rect.Right - r) path.AddLine(gap.End, rect.Top, rect.Right - r, rect.Top);
                    path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                    path.AddLine(rect.Right, rect.Top + r, rect.Right, rect.Bottom - r);
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                    path.AddLine(rect.Right - r, rect.Bottom, rect.Left + r, rect.Bottom);
                    path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                    path.AddLine(rect.Left, rect.Bottom - r, rect.Left, rect.Top + r);
                    path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                    if (rect.Left + r < gap.Start) path.AddLine(rect.Left + r, rect.Top, gap.Start, rect.Top);
                    break;
                case BorderTextPosition.Bottom:
                    // Start at (gap.End, bottom) and go clockwise, ending at (gap.Start, bottom).
                    path.StartFigure();
                    if (gap.End < rect.Right - r) path.AddLine(gap.End, rect.Bottom, rect.Right - r, rect.Bottom);
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 90, -90);     // to right side
                    path.AddLine(rect.Right, rect.Bottom - r, rect.Right, rect.Top + r);
                    path.AddArc(rect.Right - d, rect.Top, d, d, 0, -90);            // to top
                    path.AddLine(rect.Right - r, rect.Top, rect.Left + r, rect.Top);
                    path.AddArc(rect.Left, rect.Top, d, d, 270, -90);               // to left side
                    path.AddLine(rect.Left, rect.Top + r, rect.Left, rect.Bottom - r);
                    path.AddArc(rect.Left, rect.Bottom - d, d, d, 180, -90);        // to bottom
                    if (rect.Left + r < gap.Start) path.AddLine(rect.Left + r, rect.Bottom, gap.Start, rect.Bottom);
                    break;
                case BorderTextPosition.Left:
                    // Start at (left, gap.End) and go clockwise, ending at (left, gap.Start).
                    path.StartFigure();
                    if (gap.End < rect.Bottom - r) path.AddLine(rect.Left, gap.End, rect.Left, rect.Bottom - r);
                    path.AddArc(rect.Left, rect.Bottom - d, d, d, 180, 90);
                    path.AddLine(rect.Left + r, rect.Bottom, rect.Right - r, rect.Bottom);
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 90, 90);
                    path.AddLine(rect.Right, rect.Bottom - r, rect.Right, rect.Top + r);
                    path.AddArc(rect.Right - d, rect.Top, d, d, 0, 90);
                    path.AddLine(rect.Right - r, rect.Top, rect.Left + r, rect.Top);
                    path.AddArc(rect.Left, rect.Top, d, d, 270, 90);
                    if (rect.Top + r < gap.Start) path.AddLine(rect.Left, rect.Top + r, rect.Left, gap.Start);
                    break;
                case BorderTextPosition.Right:
                    // Start at (right, gap.End) and go clockwise, ending at (right, gap.Start).
                    path.StartFigure();
                    if (gap.End < rect.Bottom - r) path.AddLine(rect.Right, gap.End, rect.Right, rect.Bottom - r);
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                    path.AddLine(rect.Right - r, rect.Bottom, rect.Left + r, rect.Bottom);
                    path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                    path.AddLine(rect.Left, rect.Bottom - r, rect.Left, rect.Top + r);
                    path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                    path.AddLine(rect.Left + r, rect.Top, rect.Right - r, rect.Top);
                    path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                    if (rect.Top + r < gap.Start) path.AddLine(rect.Right, rect.Top + r, rect.Right, gap.Start);
                    break;
            }
            return path;
        }

        private static GraphicsPath CreateRectangleBorderWithGap(RectangleF rect, BorderTextPosition position, GapSpan gap)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            switch (position)
            {
                case BorderTextPosition.Top:
                    path.AddLine(gap.End, rect.Top, rect.Right, rect.Top);
                    path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                    path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
                    path.AddLine(rect.Left, rect.Top, gap.Start, rect.Top);
                    break;
                case BorderTextPosition.Bottom:
                    path.AddLine(gap.End, rect.Bottom, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
                    path.AddLine(rect.Right, rect.Top, rect.Left, rect.Top);
                    path.AddLine(rect.Left, rect.Top, rect.Left, rect.Bottom);
                    path.AddLine(rect.Left, rect.Bottom, gap.Start, rect.Bottom);
                    break;
                case BorderTextPosition.Left:
                    path.AddLine(rect.Left, gap.End, rect.Left, rect.Bottom);
                    path.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Top);
                    path.AddLine(rect.Right, rect.Top, rect.Left, rect.Top);
                    path.AddLine(rect.Left, rect.Top, rect.Left, gap.Start);
                    break;
                case BorderTextPosition.Right:
                    path.AddLine(rect.Right, gap.End, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                    path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
                    path.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
                    path.AddLine(rect.Right, rect.Top, rect.Right, gap.Start);
                    break;
            }
            return path;
        }

        private static GraphicsPath CreateHeaderDividerPath(RectangleF bounds, BorderTextPosition position, float headerAreaHeight, float cornerRadius)
        {
            var p = new GraphicsPath();
            float r = Math.Max(0f, Math.Min(cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2f));
            float y;
            switch (position)
            {
                case BorderTextPosition.Bottom:
                    y = bounds.Bottom - headerAreaHeight;
                    p.AddLine(bounds.Left + r, y, bounds.Right - r, y);
                    break;
                case BorderTextPosition.Left:
                    float xL = bounds.Left + headerAreaHeight;
                    p.AddLine(xL, bounds.Top + r, xL, bounds.Bottom - r);
                    break;
                case BorderTextPosition.Right:
                    float xR = bounds.Right - headerAreaHeight;
                    p.AddLine(xR, bounds.Top + r, xR, bounds.Bottom - r);
                    break;
                case BorderTextPosition.Top:
                default:
                    y = bounds.Top + headerAreaHeight;
                    p.AddLine(bounds.Left + r, y, bounds.Right - r, y);
                    break;
            }
            return p;
        }

        private static GraphicsPath CreateCutCornerBorderWithTopGap(RectangleF rect, float cut, RectangleF headerBox, float gapPadding)
        {
            float c = Math.Max(0f, Math.Min(cut, Math.Min(rect.Width, rect.Height) / 2f));
            float gapStart = headerBox.Left - gapPadding;
            float gapEnd = headerBox.Right + gapPadding;

            // Clamp gap to top edge span excluding chamfers.
            float min = rect.Left + c;
            float max = rect.Right - c;
            gapStart = Math.Max(min, Math.Min(gapStart, max));
            gapEnd = Math.Max(min, Math.Min(gapEnd, max));
            if (gapEnd < gapStart) (gapStart, gapEnd) = (gapEnd, gapStart);

            var p = new GraphicsPath();
            p.StartFigure();
            // Start just after the gap and trace clockwise around, ending just before the gap.
            if (gapEnd < rect.Right - c) p.AddLine(gapEnd, rect.Top, rect.Right - c, rect.Top);
            p.AddLine(rect.Right - c, rect.Top, rect.Right, rect.Top + c);
            p.AddLine(rect.Right, rect.Top + c, rect.Right, rect.Bottom - c);
            p.AddLine(rect.Right, rect.Bottom - c, rect.Right - c, rect.Bottom);
            p.AddLine(rect.Right - c, rect.Bottom, rect.Left + c, rect.Bottom);
            p.AddLine(rect.Left + c, rect.Bottom, rect.Left, rect.Bottom - c);
            p.AddLine(rect.Left, rect.Bottom - c, rect.Left, rect.Top + c);
            p.AddLine(rect.Left, rect.Top + c, rect.Left + c, rect.Top);
            if (rect.Left + c < gapStart) p.AddLine(rect.Left + c, rect.Top, gapStart, rect.Top);
            return p;
        }

        /// <summary>
        /// Creates a rounded rectangle with an inward top notch (useful for a "header slot" look).
        /// The notch span is defined by <paramref name="notchSpan"/> on the top edge of <paramref name="rect"/>.
        /// Caller owns the returned GraphicsPath and must Dispose it.
        /// </summary>
        public static GraphicsPath CreateTopNotchedRoundedRectanglePath(
            RectangleF rect,
            RectangleF notchSpan,
            float notchDepth = 16f,
            float cornerRadius = 12f,
            float notchCornerRadius = 6f)
        {
            float r = Math.Max(0f, Math.Min(cornerRadius, Math.Min(rect.Width, rect.Height) / 2f));
            float d = r * 2f;
            float nr = Math.Max(0f, Math.Min(notchCornerRadius, Math.Min(notchSpan.Width, notchDepth) / 2f));
            float nd = nr * 2f;

            // Clamp notch span to top edge excluding corner arcs.
            float minX = rect.Left + r;
            float maxX = rect.Right - r;
            float notchLeft = Math.Max(minX, Math.Min(notchSpan.Left, maxX));
            float notchRight = Math.Max(minX, Math.Min(notchSpan.Right, maxX));
            if (notchRight < notchLeft) (notchLeft, notchRight) = (notchRight, notchLeft);

            float depth = Math.Max(0f, Math.Min(notchDepth, rect.Height - r - 1f));

            var p = new GraphicsPath();
            p.StartFigure();
            // Start at top just after notch
            p.AddLine(notchRight, rect.Top, rect.Right - r, rect.Top);
            p.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            p.AddLine(rect.Right, rect.Top + r, rect.Right, rect.Bottom - r);
            p.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            p.AddLine(rect.Right - r, rect.Bottom, rect.Left + r, rect.Bottom);
            p.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            p.AddLine(rect.Left, rect.Bottom - r, rect.Left, rect.Top + r);
            p.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            // Top edge to notch start
            p.AddLine(rect.Left + r, rect.Top, notchLeft, rect.Top);
            // Notch going inward and back out (rounded inner corners)
            // Left inner corner
            if (nr > 0.01f)
            {
                p.AddArc(notchLeft, rect.Top, nd, nd, 180, 90);
                p.AddLine(notchLeft + nr, rect.Top + nr, notchLeft + nr, rect.Top + depth - nr);
                p.AddArc(notchLeft, rect.Top + depth - nd, nd, nd, 270, 90);
            }
            else
            {
                p.AddLine(notchLeft, rect.Top, notchLeft, rect.Top + depth);
            }
            // Bottom of notch
            p.AddLine(notchLeft + nr, rect.Top + depth, notchRight - nr, rect.Top + depth);
            // Right inner corner
            if (nr > 0.01f)
            {
                p.AddArc(notchRight - nd, rect.Top + depth - nd, nd, nd, 0, 90);
                p.AddLine(notchRight, rect.Top + depth - nr, notchRight, rect.Top + nr);
                p.AddArc(notchRight - nd, rect.Top, nd, nd, 90, 90);
            }
            else
            {
                p.AddLine(notchRight, rect.Top + depth, notchRight, rect.Top);
            }
            return p;
        }

        #endregion

        #region New Style Helper Functions

        private static GraphicsPath CreateProtrudingTabBorderPath(
            RectangleF bounds, float cornerRadius, BorderTextPosition position, RectangleF headerBox, float gapPadding)
        {
            // This is complex. Imagine the main body is a rounded rect,
            // but the area where the 'tab' was is now a bump out.
            // A simpler way might be to union the main rounded rect with the tab rect,
            // then subtract the overlap area.

            // For illustration, let's modify the top edge to include a tab-like shape
            // Only makes sense for Top position in this example.
            if (position != BorderTextPosition.Top) return CreateRoundedRectanglePath(bounds, new CornerRadius((int)cornerRadius));

            var path = new GraphicsPath();
            float r = Math.Max(0f, Math.Min(cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2f));
            float d = r * 2f;

            float tabLeft = headerBox.Left;
            float tabRight = headerBox.Right;
            float tabHeight = headerBox.Height + 2; // Make it slightly taller than the text box

            // Start at top-left, arc to corner
            path.StartFigure();
            if (bounds.Left + r < tabLeft) path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            else path.AddLine(bounds.Left, bounds.Top, tabLeft, bounds.Top);

            // Go up and across the tab
            path.AddLine(tabLeft, bounds.Top, tabLeft, bounds.Top - tabHeight);
            path.AddLine(tabLeft, bounds.Top - tabHeight, tabRight, bounds.Top - tabHeight);
            path.AddLine(tabRight, bounds.Top - tabHeight, tabRight, bounds.Top);

            // Continue around the rest of the rectangle
            if (tabRight < bounds.Right - r) path.AddLine(tabRight, bounds.Top, bounds.Right - r, bounds.Top);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddLine(bounds.Right, bounds.Top + r, bounds.Right, bounds.Bottom - r);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddLine(bounds.Right - r, bounds.Bottom, bounds.Left + r, bounds.Bottom);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.AddLine(bounds.Left, bounds.Bottom - r, bounds.Left, bounds.Top + r);

            // Connect back to the start if not already done by the first arc
            if (bounds.Left + r >= tabLeft) path.CloseFigure(); // If tab starts at corner

            return path;
        }

        private static GraphicsPath CreateAngledHeaderBorderPath(
            RectangleF bounds, RectangleF headerBox, float cornerRadius, float headerAreaHeight)
        {
            var path = new GraphicsPath();
            float r = Math.Max(0f, Math.Min(cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2f));
            float d = r * 2f;

            // Define the 4 points of the main body (excluding the angled header)
            // Bottom points
            PointF bl = new PointF(bounds.Left, bounds.Bottom - r);
            PointF br = new PointF(bounds.Right, bounds.Bottom - r);
            // Top points (where angled header starts)
            PointF tl = new PointF(bounds.Left + r, bounds.Top + headerAreaHeight);
            PointF tr = new PointF(bounds.Right - r, bounds.Top + headerAreaHeight);

            // Define the 4 points of the angled header section
            // Assume a simple skew for the top edge
            float angleSkew = headerAreaHeight * 0.3f; // How much the top skews inwards
            PointF atl = new PointF(bounds.Left + angleSkew, bounds.Top + r); // Angled Top Left
            PointF atr = new PointF(bounds.Right - angleSkew, bounds.Top + r); // Angled Top Right

            path.StartFigure();

            // Add the angled top edge (from ATL to ATR)
            path.AddLine(atl.X, atl.Y, atr.X, atr.Y);

            // Add right-side arc (from ATR down to TR then to BR)
            path.AddLine(atr.X, atr.Y, tr.X, tr.Y); // From angled corner to main body corner
            path.AddArc(bounds.Right - d, bounds.Top + headerAreaHeight, d, d, 270, 90); // Arc down right side
            path.AddLine(bounds.Right, bounds.Top + headerAreaHeight + r, bounds.Right, bounds.Bottom - r);

            // Add bottom edge and left-side arc
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddLine(bounds.Right - r, bounds.Bottom, bounds.Left + r, bounds.Bottom);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.AddLine(bounds.Left, bounds.Bottom - r, bounds.Left, bounds.Top + headerAreaHeight + r);

            // Add left-side arc (from BL up to TL then to ATL)
            path.AddArc(bounds.Left, bounds.Top + headerAreaHeight, d, d, 180, 90); // Arc up left side
            path.AddLine(bounds.Left + r, bounds.Top + headerAreaHeight, atl.X, atl.Y); // To angled corner

            path.CloseFigure();
            return path;
        }

        private static GraphicsPath CreateAngledHeaderPath(
            RectangleF bounds, RectangleF headerBox, float headerAreaHeight)
        {
            var path = new GraphicsPath();
            float angleSkew = headerAreaHeight * 0.3f;
            PointF atl = new PointF(bounds.Left + angleSkew, bounds.Top);
            PointF atr = new PointF(bounds.Right - angleSkew, bounds.Top);
            PointF abr = new PointF(bounds.Right - angleSkew, bounds.Top + headerAreaHeight);
            PointF abl = new PointF(bounds.Left + angleSkew, bounds.Top + headerAreaHeight);

            path.AddPolygon(new PointF[] { atl, atr, abr, abl });
            return path;
        }

        private static GraphicsPath CreateDocumentStyleBorderPath(
            RectangleF bounds, float cornerRadius, BorderTextPosition position)
        {
            // A common document style is a rounded rect with a small triangle cut from a corner (often top-right)
            // Let's implement the cut-corner idea manually.
            float cutSize = Math.Min(15, Math.Min(bounds.Width, bounds.Height) / 4); // Size of the cut
            float r = Math.Max(0f, Math.Min(cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2f));
            float d = r * 2f;

            var path = new GraphicsPath();
            path.StartFigure();

            // Start at top-left
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);

            // Line to top-right cut start
            path.AddLine(bounds.X + r, bounds.Y, bounds.Right - cutSize, bounds.Y);

            // Cut (diagonal) or fold
            // Fold is usually: from (Right-Cut, Top) to (Right, Top+Cut)
            path.AddLine(bounds.Right - cutSize, bounds.Y, bounds.Right, bounds.Y + cutSize);

            // Right side down to bottom-right
            path.AddLine(bounds.Right, bounds.Y + cutSize, bounds.Right, bounds.Bottom - r);

            // Bottom-right corner
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);

            // Bottom line
            path.AddLine(bounds.Right - r, bounds.Bottom, bounds.X + r, bounds.Bottom);

            // Bottom-left corner
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);

            // Left line
            path.AddLine(bounds.X, bounds.Bottom - r, bounds.X, bounds.Y + r);

            path.CloseFigure();
            return path;
        }

        private static GraphicsPath CreateChatBubbleBorderPath(
            RectangleF bounds, float cornerRadius, BorderTextPosition position, RectangleF headerBox)
        {
            // Position the tail based on the 'position' enum or a fixed rule
            // For now, let's fix it to BottomLeft as an example
            return CreateSpeechBubble(bounds.X, bounds.Y, bounds.Width, bounds.Height, cornerRadius, tailSize: 10, SpeechBubbleTailPosition.BottomLeft);
        }

        private static GraphicsPath CreateSegmentedHeaderBorderPath(
            RectangleF bounds, RectangleF headerBox, float cornerRadius)
        {
            return CreateRoundedRectanglePath(bounds, new CornerRadius((int)cornerRadius));
        }

        private static GraphicsPath CreateSegmentedHeaderPath(
            RectangleF bounds, RectangleF headerBox, float cornerRadius)
        {
            var path = new GraphicsPath();
            // Define segments. For example, 3 segments spanning the headerBox width.
            int numSegments = 3;
            float totalWidth = headerBox.Width;
            float segWidth = (totalWidth - (numSegments - 1) * 2) / numSegments; // 2px gap between
            float segHeight = headerBox.Height;
            float segRadius = Math.Min(segHeight / 2, 5f); // Capsule shape for each segment

            for (int i = 0; i < numSegments; i++)
            {
                float x = headerBox.Left + i * (segWidth + 2);
                var segRect = new RectangleF(x, headerBox.Top, segWidth, segHeight);
                var segPath = CreateRoundedRectanglePath(segRect, new CornerRadius((int)segRadius));
                path.AddPath(segPath, connect: false); // Don't connect figures, add separately
                segPath.Dispose();
            }
            return path;
        }

        private static GraphicsPath CreateCutoutCornerBorderPath(
            RectangleF bounds, BorderTextPosition position, RectangleF headerBox, float cornerRadius)
        {
            // Decide which corner to cut based on position or default to TopRight
            float cutSize = Math.Min(15, Math.Min(bounds.Width, bounds.Height) / 6);
            float r = Math.Max(0f, Math.Min(cornerRadius, Math.Min(bounds.Width, bounds.Height) / 2f));
            float d = r * 2f;

            var path = new GraphicsPath();
            path.StartFigure();

            // We mimic a rounded rectangle but skip/cut one corner.
            // Simplified implementation: always cutout TopRight for now, or respect position
            // But implementing for all 4 positions manually is verbose. 
            // Let's assume TopRight cut for Top/Right/Default, TopLeft for Left, etc. if needed.
            // For brevity, let's implement the "TopRight" cutout logic which seems to be the intent of the default.

            bool cutTopRight = true; // Hardcoded for this fix to match default behavior in original code

            // Top-left
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);

            if (cutTopRight)
            {
                // Line to cutout start
                path.AddLine(bounds.X + r, bounds.Y, bounds.Right - cutSize, bounds.Y);
                // The cutout - inverted arc (scoop)
                path.AddArc(bounds.Right - cutSize, bounds.Y, cutSize, cutSize, 270, -90); 
            }
            else
            {
                path.AddLine(bounds.X + r, bounds.Y, bounds.Right - r, bounds.Y);
                path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            }

            // Right side
            path.AddLine(bounds.Right, bounds.Y + (cutTopRight ? cutSize : r), bounds.Right, bounds.Bottom - r);
            
            // Bottom-right
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);

            // Bottom
            path.AddLine(bounds.Right - r, bounds.Bottom, bounds.X + r, bounds.Bottom);

            // Bottom-left
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);

            // Left
            path.AddLine(bounds.X, bounds.Bottom - r, bounds.X, bounds.Y + r);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Core Path Manipulation Utilities (Required for Shadow Path)

        /// <summary>
        /// Creates a shadow path from any GraphicsPath by offsetting and optionally blurring.
        /// </summary>




        #endregion



        public static GraphicsPath CreateCapsulePath(RectangleF rect)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            float r = Math.Min(rect.Width, rect.Height) / 2f;
            float d = r * 2f;
            path.AddArc(rect.Left, rect.Top, d, d, 90, 180);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 180);
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath CreateCutCornerRectanglePath(RectangleF rect, float cut)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            float c = Math.Max(0f, Math.Min(cut, Math.Min(rect.Width, rect.Height) / 2f));
            path.StartFigure();
            path.AddLine(rect.Left + c, rect.Top, rect.Right - c, rect.Top);
            path.AddLine(rect.Right - c, rect.Top, rect.Right, rect.Top + c);
            path.AddLine(rect.Right, rect.Top + c, rect.Right, rect.Bottom - c);
            path.AddLine(rect.Right, rect.Bottom - c, rect.Right - c, rect.Bottom);
            path.AddLine(rect.Right - c, rect.Bottom, rect.Left + c, rect.Bottom);
            path.AddLine(rect.Left + c, rect.Bottom, rect.Left, rect.Bottom - c);
            path.AddLine(rect.Left, rect.Bottom - c, rect.Left, rect.Top + c);
            path.AddLine(rect.Left, rect.Top + c, rect.Left + c, rect.Top);
            path.CloseFigure();
            return path;
        }




    }
  /// <summary>
        /// Where the title/header text is placed relative to the border.
        /// </summary>
        public enum BorderTextPosition
        {
            Top,
            Bottom,
            Left,
            Right
        }

        /// <summary>
        /// Border styles intended for modern UI/UX panels with a title/header.
        /// Includes new styles like CardHeader, ProtrudingTab, etc.
        /// </summary>
        public enum TitledBorderStyle
        {
            /// <summary>Classic GroupBox-like border with a "gap" behind the title text.</summary>
            ClassicGap,
            /// <summary>Classic gap border plus a rounded "tab" background behind the title.</summary>
            FilledTab,
            /// <summary>Rounded panel with a header area inside the border and a divider line.</summary>
            HeaderArea,
            /// <summary>Rounded panel with an inward notch under the title (top only).</summary>
            TopNotch,
            /// <summary>Pill / capsule header badge; border uses a classic gap.</summary>
            PillBadge,
            /// <summary>Cut-corner border (chamfer) with a classic top gap.</summary>
            CutCornerGap,

            // --- NEW STYLES ---
            /// <summary>Card-style panel with a distinct header section and body, often with a subtle separator.</summary>
            CardHeader,
            /// <summary>Panel with a tab-like element protruding from one side (e.g., left, right, top).</summary>
            ProtrudingTab,
            /// <summary>Panel with a slanted or angled header area.</summary>
            AngledHeader,
            /// <summary>Panel shaped like a document or paper sheet with a fold-over corner.</summary>
            DocumentStyle,
            /// <summary>Panel with a floating or elevated header block above the main body.</summary>
            FloatingHeader,
            /// <summary>Panel shaped like a chat message bubble with a tail.</summary>
            ChatBubbleStyle,
            /// <summary>Panel with a segmented header (like multiple small tabs) even for a single title.</summary>
            SegmentedHeader,
            /// <summary>Panel with a cut-out corner specifically for the title text.</summary>
            CutoutTitleCorner
        }
}
