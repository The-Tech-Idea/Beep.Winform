using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Configuration for tooltip appearance and behavior
    /// Inspired by DevExpress, Material-UI, Ant Design tooltip configurations
    /// Integrated with BeepControlStyle for consistent design system
    /// </summary>
    public class ToolTipConfig
    {
        #region Identification

        /// <summary>
        /// Unique key for this tooltip instance
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Custom tag for storing additional data
        /// </summary>
        public object Tag { get; set; }

        #endregion

        #region Content

        /// <summary>
        /// Primary text content
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Optional title/header
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Rich HTML content (for advanced scenarios)
        /// </summary>
        public string Html { get; set; }

        #endregion

        #region Positioning

        /// <summary>
        /// Screen position where tooltip should appear.
        /// <para>
        /// Prefer <see cref="AnchorRect"/> when the tooltip belongs to a control: a point cannot
        /// express alignment. The <c>*Start</c> / <c>*End</c> placements are defined against the
        /// anchor's edges, so a point-sized anchor collapses them onto the centred placement.
        /// </para>
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Screen rectangle of the element the tooltip describes.
        /// <para>
        /// This is what the placement engine positions against — the gap, the flip decision, the
        /// shift, and the arrow's target all derive from it. When left empty the engine falls back
        /// to a degenerate rectangle at <see cref="Position"/>, which preserves the older
        /// cursor-anchored behaviour but cannot express <c>*Start</c> / <c>*End</c> alignment.
        /// </para>
        /// <para>
        /// For a control this is <c>control.RectangleToScreen(control.ClientRectangle)</c>.
        /// </para>
        /// </summary>
        public Rectangle AnchorRect { get; set; } = Rectangle.Empty;

        /// <summary>
        /// The anchor as a rectangle, always. Returns <see cref="AnchorRect"/> when set, otherwise
        /// a 1x1 rectangle at <see cref="Position"/> so callers have exactly one shape to reason
        /// about.
        /// </summary>
        public Rectangle ResolveAnchorRect()
            => AnchorRect.IsEmpty ? new Rectangle(Position, new Size(1, 1)) : AnchorRect;

        /// <summary>
        /// Distance in pixels kept between the tooltip and the edge of the working area.
        /// Was a hard-coded, non-DPI-scaled constant inside the positioning helper.
        /// </summary>
        public int ViewportPadding { get; set; } = 8;

        /// <summary>
        /// The control this tooltip describes, when there is one. Supplies the anchor rectangle and
        /// lets the tooltip follow the control as it moves (see <see cref="AutoUpdate"/>).
        /// Not serialised — this is a live reference held only while the tooltip is open.
        /// </summary>
        public Control AnchorControl { get; set; }

        /// <summary>
        /// Keep the tooltip attached to <see cref="AnchorControl"/> while it is visible: reposition
        /// on scroll, move, resize and monitor change, and hide once the anchor is gone or clipped.
        /// <para>
        /// Defaults to true. Turn it off for a tooltip placed deliberately at a fixed point that
        /// should not chase anything.
        /// </para>
        /// </summary>
        public bool AutoUpdate { get; set; } = true;

        /// <summary>
        /// Preferred placement relative to target
        /// </summary>
        public ToolTipPlacement Placement { get; set; } = ToolTipPlacement.Auto;

        /// <summary>
        /// Offset distance from target in pixels
        /// </summary>
        public int Offset { get; set; } = 8;

        /// <summary>
        /// Whether tooltip follows cursor movement
        /// </summary>
        public bool FollowCursor { get; set; } = false;

        #endregion

        #region Timing

        /// <summary>
        /// Display duration in milliseconds (0 = indefinite)
        /// </summary>
        public int Duration { get; set; } = 3000;

        /// <summary>
        /// Delay before showing in milliseconds
        /// </summary>
        public int? ShowDelay { get; set; }

        /// <summary>
        /// Delay before hiding in milliseconds
        /// </summary>
        public int? HideDelay { get; set; }

        /// <summary>
        /// Name of the delay group this tooltip belongs to.
        /// <para>
        /// Once any tooltip in a group has been shown, others in the same group appear immediately
        /// for a short window after the last one closes — so sweeping a toolbar costs one
        /// <see cref="ShowDelay"/> rather than one per button. Leave null to have the group derived
        /// from the anchor's parent, which makes a toolbar, ribbon or grid header behave correctly
        /// without the caller configuring anything.
        /// </para>
        /// </summary>
        public string DelayGroup { get; set; }

        #endregion

        #region Appearance - Theme and Style

        /// <summary>
        /// Tooltip type for semantic styling (Success, Warning, Error, Info, etc.)
        /// </summary>
        public ToolTipType Type { get; set; } = ToolTipType.Default;

        /// <summary>
        /// BeepControlStyle for visual design (Material3, iOS15, Fluent2, etc.)
        /// </summary>
        public BeepControlStyle Style { get; set; } = BeepControlStyle.Material3;


        #endregion

        #region Appearance - Colors

        /// <summary>
        /// Custom background color (overrides theme)
        /// </summary>
        public Color? BackColor { get; set; }

        /// <summary>
        /// Custom foreground/text color (overrides theme)
        /// </summary>
        public Color? ForeColor { get; set; }

        /// <summary>
        /// Custom border color (overrides theme)
        /// </summary>
        public Color? BorderColor { get; set; }

        #endregion

        #region Appearance - Typography and Layout

        /// <summary>
        /// Custom font (overrides theme)
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Maximum size constraint
        /// </summary>
        public Size? MaxSize { get; set; }

        /// <summary>
        /// Horizontal text alignment for body text (Near = left, Center, Far = right)
        /// </summary>
        public StringAlignment TextHAlign { get; set; } = StringAlignment.Near;

        /// <summary>
        /// Vertical text alignment for body text (Near = top, Center, Far = bottom)
        /// </summary>
        public StringAlignment TextVAlign { get; set; } = StringAlignment.Near;

        #endregion

        #region Appearance - Visual Elements

        /// <summary>
        /// Show arrow/pointer towards target
        /// </summary>
        public bool ShowArrow { get; set; } = true;

        /// <summary>
        /// Show drop shadow.
        /// </summary>
        public bool ShowShadow { get; set; } = true;

        /// <summary>
        /// Duplicate of <see cref="ShowShadow"/>, kept so existing callers keep compiling.
        /// <para>
        /// Two independent booleans meant the same thing, both defaulted to <c>true</c>, and every
        /// painter had to test <c>ShowShadow || EnableShadow</c> — so setting either one to false
        /// did nothing unless you knew to set both. This now forwards to <see cref="ShowShadow"/>,
        /// which is the single source of truth.
        /// </para>
        /// </summary>
        [Obsolete("Use ShowShadow. This forwards to it and will be removed.")]
        public bool EnableShadow
        {
            get => ShowShadow;
            set => ShowShadow = value;
        }

        /// <summary>
        /// Show close button
        /// </summary>
        public bool Closable { get; set; } = false;

        #endregion

        #region Icons and Images

        /// <summary>
        /// Icon image object
        /// </summary>
        public Image Icon { get; set; }

        /// <summary>
        /// Path to icon file
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Path to SVG or image file
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Maximum size for images
        /// </summary>
        public Size MaxImageSize { get; set; } = new Size(24, 24);

        /// <summary>
        /// Apply theme colors to SVG images
        /// </summary>
        public bool ApplyThemeOnImage { get; set; } = true;

        #endregion

        #region Animation

        /// <summary>
        /// Animation type (None, Fade, Scale, Slide, Bounce)
        /// </summary>
        public ToolTipAnimation Animation { get; set; } = ToolTipAnimation.Fade;

        /// <summary>
        /// Animation duration in milliseconds
        /// </summary>
        public int AnimationDuration { get; set; } = 200;

        #endregion

        #region Step Style Properties

        /// <summary>
        /// Current step number (for Step Style)
        /// </summary>
        public int CurrentStep { get; set; } = 1;

        /// <summary>
        /// Total number of steps (for Step Style)
        /// </summary>
        public int TotalSteps { get; set; } = 1;

        /// <summary>
        /// Duplicate of <see cref="Title"/>, kept so existing callers keep compiling.
        /// <para>
        /// Documented as "step-specific title", but nothing ever set it and nothing ever read it:
        /// <c>TourToolTipPainter</c> renders <see cref="Title"/> as the step heading. Two properties
        /// for one heading meant a caller could set this one and see nothing appear.
        /// </para>
        /// </summary>
        [Obsolete("Use Title. This forwards to it and will be removed.")]
        public string StepTitle
        {
            get => Title;
            set => Title = value;
        }

        /// <summary>
        /// Show navigation buttons (for Step Style)
        /// </summary>
        public bool ShowNavigationButtons { get; set; } = true;

        #endregion

        #region Events

        /// <summary>Called when tooltip is closed</summary>
        public Action<string> OnClose { get; set; }

        /// <summary>Called when tooltip is shown</summary>
        public Action<string> OnShow { get; set; }

        /// <summary>Back-compat forwarder to <see cref="Style"/>. Use Style instead.</summary>
        public BeepControlStyle ControlStyle
        {
            get => Style;
            internal set => Style = value;
        }

        // C7: Promote button click callbacks to ToolTipConfig so that any
        // tooltip (not just popovers) can wire primary/secondary button
        // actions. PopoverConfig inherits these and keeps its existing
        // Action<string> OnPrimaryClick/OnSecondaryClick as a back-compat
        // shim, but the tour now uses the parameterless ones here.
        /// <summary>
        /// Called when the primary (confirm / next) action button is clicked.
        /// Parameterless so tour/painter code can wire lambdas without a key.
        /// </summary>
        public Action OnPrimaryClick { get; set; }

        /// <summary>
        /// Called when the secondary (cancel / skip) action button is clicked.
        /// Parameterless so tour/painter code can wire lambdas without a key.
        /// </summary>
        public Action OnSecondaryClick { get; set; }

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 1 — Arrow quality
        // ──────────────────────────────────────────────────────────────────────
        #region Arrow

        /// <summary>
        /// Pixel offset of arrow tip from the center of the tooltip edge.
        /// Positive moves toward End alignment; negative toward Start.
        /// </summary>
        public int ArrowOffset { get; set; } = 0;

        /// <summary>
        /// Visual style of the arrow caret (Sharp, Rounded, Hidden).
        /// </summary>
        public ToolTipArrowStyle ArrowStyle { get; set; } = ToolTipArrowStyle.Sharp;

        /// <summary>
        /// Pixel size of the arrow (base width / height).  DPI-scaled at paint time.
        /// </summary>
        public int ArrowSize { get; set; } = 8;

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 2 — Rich content
        // ──────────────────────────────────────────────────────────────────────
        #region Rich Content

        /// <summary>
        /// Ordered list of content blocks rendered inside the tooltip.
        /// When populated, overrides <see cref="Text"/> / <see cref="Title"/> / <see cref="Html"/>.
        /// </summary>
        public List<ToolTipContentItem> ContentItems { get; set; }

        /// <summary>
        /// When true, <see cref="Text"/> is parsed for lightweight markup:
        /// **bold**, *italic*, `code`, [link label] syntax.
        /// </summary>
        public bool UseMarkup { get; set; } = false;

        /// <summary>
        /// Card silhouette. Defaults to <see cref="ToolTipShape.Rounded"/>, which keeps the radius
        /// the active BeepControlStyle already specifies.
        /// </summary>
        public ToolTipShape Shape { get; set; } = ToolTipShape.Rounded;

        private ToolTipLayoutVariant _layoutVariant = ToolTipLayoutVariant.Simple;
        private bool _layoutVariantSetExplicitly;

        /// <summary>Layout variant: Simple / Rich / Card / Preview / Tour / Shortcut.</summary>
        /// <remarks>
        /// Defaults to <see cref="ToolTipLayoutVariant.Simple"/>, which draws body text only.
        /// <para>
        /// If a <see cref="Title"/> is set and the caller never chose a variant, this reports
        /// <see cref="ToolTipLayoutVariant.Rich"/> so the title is actually drawn. Simple
        /// deliberately suppresses the title, so leaving the default in place silently discarded
        /// every title a caller supplied - the tooltip rendered body-only with no indication that
        /// half the content had been dropped.
        /// </para>
        /// <para>
        /// Setting this property explicitly always wins, including an explicit
        /// <c>Simple</c> with a title present: "Simple means simple" still holds when it is asked
        /// for by name.
        /// </para>
        /// </remarks>
        public ToolTipLayoutVariant LayoutVariant
        {
            get => !_layoutVariantSetExplicitly
                   && _layoutVariant == ToolTipLayoutVariant.Simple
                   && !string.IsNullOrWhiteSpace(Title)
                       ? ToolTipLayoutVariant.Rich
                       : _layoutVariant;
            set
            {
                _layoutVariant = value;
                _layoutVariantSetExplicitly = true;
            }
        }

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 3 — Popover / trigger
        // ──────────────────────────────────────────────────────────────────────
        #region Trigger and Popover

        /// <summary>What user action shows this tooltip/popover.</summary>
        public ToolTipTriggerMode TriggerMode { get; set; } = ToolTipTriggerMode.Hover;

        /// <summary>
        /// When true, tooltip stays open while the mouse is over it (WCAG 1.4.13).
        /// </summary>
        public bool PersistOnHover { get; set; } = true;

        /// <summary>
        /// When true, tooltip is also shown when the target control receives keyboard focus.
        /// </summary>
        public bool KeyboardTriggerable { get; set; } = true;

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 4 — Preview
        // ──────────────────────────────────────────────────────────────────────
        #region Preview Content

        /// <summary>File-system path to an image shown at the top of a Preview tooltip.</summary>
        public string PreviewImagePath { get; set; }

        /// <summary>Pixel size of the preview image area (default 280×160).</summary>
        public Size PreviewImageSize { get; set; } = new Size(280, 160);

        /// <summary>Sub-title shown beneath the preview image.</summary>
        public string PreviewSubtitle { get; set; }

        /// <summary>Footer text shown at the bottom of the preview area.</summary>
        public string PreviewFooterText { get; set; }

        /// <summary>
        /// Optional async delegate for lazy-loading the preview image.
        /// If supplied, a skeleton placeholder is shown until the task completes.
        /// </summary>
        public Func<System.Threading.Tasks.Task<Image>> LoadPreviewAsync { get; set; }

        /// <summary>
        /// The preview image once resolved — from <see cref="LoadPreviewAsync"/> or by loading
        /// <see cref="PreviewImagePath"/> exactly once.
        /// <para>
        /// Painters must read this rather than touching the file system. The preview painter used
        /// to call <c>Image.FromFile</c> on <em>every paint</em> and dispose the result, so a
        /// visible preview tooltip re-read its image from disk on every repaint.
        /// </para>
        /// <para>
        /// Owned by <c>ToolTipInstance</c>, which disposes it with the tooltip.
        /// </para>
        /// </summary>
        internal Image ResolvedPreviewImage { get; set; }

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 5 — Keyboard shortcut badges
        // ──────────────────────────────────────────────────────────────────────
        #region Shortcut Badges

        /// <summary>
        /// Keyboard shortcuts displayed as key-cap badges in the tooltip footer.
        /// E.g. Ctrl+S, Alt+F4.
        /// </summary>
        public List<ShortcutKeyItem> Shortcuts { get; set; }

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 7 — Animation easing
        // ──────────────────────────────────────────────────────────────────────
        #region Animation Easing

        /// <summary>Easing function applied to all animations for this tooltip.</summary>
        public EasingFunction AnimationEasing { get; set; } = EasingFunction.EaseOut;

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 9 — Sticky / pinned
        // ──────────────────────────────────────────────────────────────────────
        #region Pinnable

        /// <summary>
        /// When true, a pin icon appears allowing the user to keep the tooltip open.
        /// </summary>
        public bool Pinnable { get; set; } = false;

        /// <summary>Current pin state (managed by ToolTipManager).</summary>
        public bool IsPinned { get; set; } = false;

        #endregion

        // ──────────────────────────────────────────────────────────────────────
        // SPRINT 11 — Accessibility
        // ──────────────────────────────────────────────────────────────────────
        #region Accessibility

        /// <summary>
        /// Minimum WCAG contrast ratio enforced between ForeColor and BackColor.
        /// Default 4.5 (WCAG AA for normal text).
        /// </summary>
        public double MinContrastRatio { get; set; } = 4.5;

        #endregion
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Supporting models declared in this file for convenience
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// A single keyboard-shortcut badge item displayed in tooltip footer.
    /// </summary>
    public class ShortcutKeyItem
    {
        /// <summary>Modifier keys (Ctrl, Shift, Alt, etc.).</summary>
        public Keys Modifiers { get; set; } = Keys.None;

        /// <summary>Primary key (letter, function key, etc.).</summary>
        public Keys Key { get; set; } = Keys.None;

        /// <summary>
        /// Optional override display text (e.g. "Del" instead of "Delete").
        /// When null the Keys.ToString() value is used.
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>Returns a human-readable string like "Ctrl + S".</summary>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DisplayText)) return DisplayText;

            var parts = new List<string>();
            if ((Modifiers & Keys.Control) == Keys.Control) parts.Add("Ctrl");
            if ((Modifiers & Keys.Alt)     == Keys.Alt)     parts.Add("Alt");
            if ((Modifiers & Keys.Shift)   == Keys.Shift)   parts.Add("Shift");
            if (Key != Keys.None) parts.Add(Key.ToString());
            return string.Join(" + ", parts);
        }
    }

    /// <summary>
    /// A single block of rich content rendered inside the tooltip.
    /// </summary>
    public class ToolTipContentItem
    {
        /// <summary>Which section this item belongs to.</summary>
        public ToolTipSection Section { get; set; } = ToolTipSection.Body;

        /// <summary>Text to display (supports markup when UseMarkup=true).</summary>
        public string Text { get; set; }

        /// <summary>Path to an icon/image displayed to the left of the text.</summary>
        public string IconPath { get; set; }

        /// <summary>Render text in bold font weight.</summary>
        public bool IsBold { get; set; }

        /// <summary>Render text in italic.</summary>
        public bool IsItalic { get; set; }

        /// <summary>Render text in monospace with a tinted code background.</summary>
        public bool IsCode { get; set; }

        /// <summary>When true, text is rendered as a clickable hyperlink.</summary>
        public bool IsLink { get; set; }

        /// <summary>Target URL or command raised via LinkClicked event.</summary>
        public string LinkTarget { get; set; }

        /// <summary>Optional font override for this item.</summary>
        public Font Font { get; set; }

        /// <summary>Optional foreground color override for this item.</summary>
        public Color? ForeColor { get; set; }
    }
}
