using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// State information for a checkbox
    /// </summary>
    public struct CheckBoxItemState
    {
        public bool IsChecked;
        public bool IsIndeterminate;
        public bool IsHovered;
        /// <summary>True while the primary mouse button is held down over the control.</summary>
        public bool IsPressed;
        public bool IsFocused;
        public bool IsDisabled;
    }

    /// <summary>
    /// Rendering options for checkbox painters
    /// </summary>
    public class CheckBoxRenderOptions
    {
        public IBeepTheme Theme { get; set; }
        public bool UseThemeColors { get; set; } = true;
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;
        public CheckBoxes.Helpers.CheckBoxStyle CheckBoxStyle { get; set; } = CheckBoxes.Helpers.CheckBoxStyle.Material3;
        public int CheckBoxSize { get; set; } = 20;
        public int Spacing { get; set; } = 8;
        public int Padding { get; set; } = 4;
        public int BorderRadius { get; set; } = 4;
        public int BorderWidth { get; set; } = 2;
        public int CheckMarkThickness { get; set; } = 2;
        public float GlyphSizeRatio { get; set; } = 0.62f;
        public string CheckIconPath { get; set; }
        public string IndeterminateIconPath { get; set; }
        public Font TextFont { get; set; }
        public string Text { get; set; }
        public bool HideText { get; set; }
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Right;
        /// <summary>When true, text and glyph order follow right-to-left reading direction.
        /// Set from Control.RightToLeft by the draw path.</summary>
        public bool RightToLeft { get; set; }
    }

    /// <summary>
    /// Declares what a painter supports so the control and tooling can reason about style
    /// parity without reading the full painter implementation.
    /// Intentional divergence (e.g., Switch not supporting ThreeState) is documented here
    /// rather than discovered at runtime.
    /// </summary>
    public readonly struct CheckBoxPainterCapabilities
    {
        /// <summary>Painter renders indeterminate state distinctly from checked and unchecked.</summary>
        public readonly bool SupportsThreeState;

        /// <summary>Painter renders a distinct hover highlight on the glyph or track.</summary>
        public readonly bool SupportsHoverHighlight;

        /// <summary>Painter renders a distinct keyboard-focus ring or indicator.</summary>
        public readonly bool SupportsFocusRing;

        /// <summary>Painter applies disabled-state color dimming instead of just falling through.</summary>
        public readonly bool SupportsDisabledState;

        /// <summary>Painter renders a user-supplied check icon image when CheckIconPath is set.</summary>
        public readonly bool SupportsCustomCheckIcon;

        /// <summary>
        /// Painter family name for diagnostics and capability-matrix reporting.
        /// e.g. "Material3", "Switch", "Button".
        /// </summary>
        public readonly string Family;

        /// <summary>
        /// Short note explaining any intentional divergence from the standard checkbox contract.
        /// Empty string when the painter fully follows the standard contract.
        /// </summary>
        public readonly string IntentionalDivergenceNote;

        public CheckBoxPainterCapabilities(
            string family,
            bool supportsThreeState,
            bool supportsHoverHighlight,
            bool supportsFocusRing,
            bool supportsDisabledState,
            bool supportsCustomCheckIcon,
            string intentionalDivergenceNote = "")
        {
            Family = family;
            SupportsThreeState = supportsThreeState;
            SupportsHoverHighlight = supportsHoverHighlight;
            SupportsFocusRing = supportsFocusRing;
            SupportsDisabledState = supportsDisabledState;
            SupportsCustomCheckIcon = supportsCustomCheckIcon;
            IntentionalDivergenceNote = intentionalDivergenceNote ?? string.Empty;
        }
    }

    /// <summary>
    /// Interface for checkbox painters
    /// </summary>
    public interface ICheckBoxPainter
    {
        /// <summary>
        /// Returns the capability matrix for this painter so callers can reason about style
        /// parity and intentional divergence without reading the full implementation.
        /// </summary>
        CheckBoxPainterCapabilities GetCapabilities();

        /// <summary>
        /// Paint the checkbox box
        /// </summary>
        void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options);

        /// <summary>
        /// Paint the check mark
        /// </summary>
        void PaintCheckMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options);

        /// <summary>
        /// Paint the indeterminate mark
        /// </summary>
        void PaintIndeterminateMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options);

        /// <summary>
        /// Paint the text label
        /// </summary>
        void PaintText(Graphics g, Rectangle bounds, string text, CheckBoxRenderOptions options);
    }
}
