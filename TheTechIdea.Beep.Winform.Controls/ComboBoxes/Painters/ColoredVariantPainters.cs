using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Blue themed dropdown painter with colored accents
    /// </summary>
    internal class BlueDropdownPainter : OutlinedComboBoxPainter
    {
        private static readonly Color _accent = Color.FromArgb(66, 133, 244);

        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            var pen = PaintersFactory.GetPen(_accent, _owner.Focused ? 2f : 1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }

        protected override Color GetArrowColor()
        {
            if (!_owner.Enabled) return Color.FromArgb(120, _accent);
            if (_owner.Focused || _owner.IsButtonHovered) return _accent;
            return Color.FromArgb(180, _accent);
        }
    }

    /// <summary>
    /// Green themed dropdown with success/positive styling
    /// </summary>
    internal class GreenDropdownPainter : OutlinedComboBoxPainter
    {
        private static readonly Color _accent = Color.FromArgb(52, 168, 83);

        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            var pen = PaintersFactory.GetPen(_accent, _owner.Focused ? 2f : 1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }

        protected override Color GetArrowColor()
        {
            if (!_owner.Enabled) return Color.FromArgb(120, _accent);
            if (_owner.Focused || _owner.IsButtonHovered) return _accent;
            return Color.FromArgb(180, _accent);
        }
    }
    
    /// <summary>
    /// Inverted color scheme dropdown (dark background)
    /// </summary>
    internal class InvertedComboBoxPainter : OutlinedComboBoxPainter
    {
    
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(100, 100, 100);
            var pen = PaintersFactory.GetPen(borderColor, 1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
        
        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;

            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;

            Color textColor = _helper.IsShowingPlaceholder()
                ? Color.FromArgb(100, 255, 255, 255)
                : (_theme?.ComboBoxForeColor != Color.Empty
                    ? _theme.ComboBoxForeColor
                    : Color.White);

            Font textFont = _owner.TextFont
                ?? BeepThemesManager.ToFont(_theme?.LabelFont)
                ?? PaintersFactory.GetFont("Segoe UI", 9f, FontStyle.Regular);

            System.Windows.Forms.TextRenderer.DrawText(g, displayText, textFont, textAreaRect, textColor,
                System.Windows.Forms.TextFormatFlags.Left |
                System.Windows.Forms.TextFormatFlags.VerticalCenter |
                System.Windows.Forms.TextFormatFlags.EndEllipsis);
        }
    }
    
    /// <summary>
    /// Error state dropdown with red/error styling.
    /// Assign <c>BeepComboBox.ComboBoxType = Error</c> when validation fails.
    /// For conditional per-state error display use <c>HasError</c> with any other type.
    /// </summary>
    internal class ErrorComboBoxPainter : OutlinedComboBoxPainter
    {
        private static readonly Color _errorRed = Color.FromArgb(220, 53, 69);

        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            var pen = PaintersFactory.GetPen(_errorRed, 2f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }

        protected override Color GetArrowColor() =>
            _owner.Enabled ? _errorRed : Color.FromArgb(120, _errorRed);
    }
}
