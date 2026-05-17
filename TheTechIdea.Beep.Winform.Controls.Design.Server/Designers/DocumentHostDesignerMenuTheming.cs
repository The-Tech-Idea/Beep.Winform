// DocumentHostDesignerMenuTheming.cs
// Phase 03 — split out from BeepDocumentHostDesigner.cs (workspace rule:
// one file, one class).
//
// The design-time ContextMenuStrip rendered by BeepDocumentHostDesigner
// honours the host's current BeepStyling theme (light / dark / brand
// colours) so it doesn't visually clash with the Visual Studio shell. To
// keep the renderer pluggable and unit-testable it lives in two tiny
// classes here rather than inside the 2400-line designer file.
//
//   DocumentHostDesignerMenuRenderer  — overrides only OnRenderItemText so
//   disabled items dim correctly against the themed background.
//
//   DocumentHostDesignerColorTable   — the actual ProfessionalColorTable
//   that paints menu chrome from (background, foreground) inputs.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class DocumentHostDesignerMenuRenderer : ToolStripProfessionalRenderer
    {
        public DocumentHostDesignerMenuRenderer(Color backColor, Color foreColor)
            : base(new DocumentHostDesignerColorTable(backColor, foreColor))
        {
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled
                ? e.Item.ForeColor
                : Color.FromArgb(140, e.Item.ForeColor);
            base.OnRenderItemText(e);
        }
    }

    internal sealed class DocumentHostDesignerColorTable : ProfessionalColorTable
    {
        private readonly Color _background;
        private readonly Color _foreground;
        private readonly Color _highlight;

        public DocumentHostDesignerColorTable(Color background, Color foreground)
        {
            _background = background;
            _foreground = foreground;
            _highlight = Blend(background, SystemColors.Highlight, 0.22f);
        }

        public override Color MenuBorder => Blend(_background, _foreground, 0.15f);
        public override Color MenuItemBorder => Color.Transparent;
        public override Color MenuItemSelected => _highlight;
        public override Color MenuItemSelectedGradientBegin => _highlight;
        public override Color MenuItemSelectedGradientEnd => _highlight;
        public override Color ToolStripDropDownBackground => _background;
        public override Color MenuStripGradientBegin => _background;
        public override Color MenuStripGradientEnd => _background;
        public override Color SeparatorDark => Blend(_background, _foreground, 0.18f);
        public override Color SeparatorLight => _background;
        public override Color ImageMarginGradientBegin => _background;
        public override Color ImageMarginGradientMiddle => _background;
        public override Color ImageMarginGradientEnd => _background;

        private static Color Blend(Color a, Color b, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            return Color.FromArgb(
                255,
                (int)Math.Round(a.R + ((b.R - a.R) * amount)),
                (int)Math.Round(a.G + ((b.G - a.G) * amount)),
                (int)Math.Round(a.B + ((b.B - a.B) * amount)));
        }
    }
}
