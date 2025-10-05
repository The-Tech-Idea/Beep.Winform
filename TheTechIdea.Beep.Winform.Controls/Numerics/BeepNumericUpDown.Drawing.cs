using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Partial class for BeepNumericUpDown drawing/painting logic
    /// </summary>
    public partial class BeepNumericUpDown
    {
        #region Drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_currentPainter == null)
            {
                InitializePainter();
            }

            if (_currentPainter != null)
            {
                var context = new NumericUpDownPainterContext(this);
                _currentPainter.Paint(e.Graphics, context, ClientRectangle);
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            CalculateButtonAreas();
            RefreshHitAreas();
        }

        private void CalculateButtonAreas()
        {
            if (!_showSpinButtons) return;

            int buttonWidth = GetButtonWidthForSize(_buttonSize);
            int buttonHeight = Height;

            // Down button (left side)
            _downButtonRect = new Rectangle(0, 0, buttonWidth, buttonHeight);

            // Up button (right side)
            _upButtonRect = new Rectangle(Width - buttonWidth, 0, buttonWidth, buttonHeight);
        }

        private int GetButtonWidthForSize(NumericSpinButtonSize size)
        {
            return size switch
            {
                NumericSpinButtonSize.Small => System.Math.Min(20, Width / 6),
                NumericSpinButtonSize.Standard => System.Math.Min(24, Width / 5),
                NumericSpinButtonSize.Large => System.Math.Min(28, Width / 4),
                NumericSpinButtonSize.ExtraLarge => System.Math.Min(32, Width / 3),
                _ => System.Math.Min(24, Width / 5)
            };
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Legacy support for BaseControl Draw method
            if (_currentPainter != null)
            {
                var context = new NumericUpDownPainterContext(this);
                _currentPainter.Paint(graphics, context, rectangle);
            }
        }
        #endregion
    }
}
