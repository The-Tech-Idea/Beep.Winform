using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using Size = System.Drawing.Size;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
      
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var availableRect = DrawingRect;

         //   _painter?.RenderGroupBackground(g, availableRect, _renderOptions);
            if (UseThemeColors && _currentTheme != null)
            {
                _painter?.RenderGroupBackground(g, availableRect, _renderOptions);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }
            if (!string.IsNullOrEmpty(_titleText))
            {
                var titleRect = new Rectangle(availableRect.X, availableRect.Y, availableRect.Width, _titleHeight);
                var titleFlags = GetTextFormatFlags(_titleAlignment);
                TextRenderer.DrawText(g, _titleText, _titleFont, titleRect, _titleColor, titleFlags);
            }

            _closeRects.Clear();
            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                var state = new ChipVisualState { IsSelected = chip.IsSelected, IsHovered = chip.IsHovered, Variant = chip.Variant, Size = chip.Size, Color = chip.Color, IsFocused = (_focusedIndex == i && Focused) };
                Rectangle closeRect = Rectangle.Empty;
                if (_painter != null)
                {
                    _painter.RenderChip(g, chip.Item, chip.Bounds, state, _renderOptions, out closeRect);
                }
                if (!closeRect.IsEmpty) _closeRects[i] = closeRect;

                // Focus ring
                if (state.IsFocused)
                {
                    using var pen = new Pen(_currentTheme.FocusIndicatorColor, 2f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                    var r = Rectangle.Inflate(chip.Bounds, 3, 3);
                    g.DrawRectangle(pen, r);
                }

                // Badge/counter (right small pill)
                if (_renderOptions.GetBadgeCount != null)
                {
                    var cnt = _renderOptions.GetBadgeCount(chip.Item);
                    if (cnt.HasValue)
                    {
                        string s = cnt.Value.ToString();
                        var sz = TextRenderer.MeasureText(g, s, Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
                        var br = new Rectangle(chip.Bounds.Right - (sz.Width + 12), chip.Bounds.Top - 8, sz.Width + 10, 16);
                        using var bg = new SolidBrush(_currentTheme.NavigationSelectedBackColor);
                        using var fg = new SolidBrush(_currentTheme.NavigationSelectedForeColor);
                        g.FillRectangle(bg, br);
                        TextRenderer.DrawText(g, s, Font, br, _currentTheme.NavigationSelectedForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
            }
        }

        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            var flags = TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping;
            return alignment switch
            {
                ContentAlignment.TopLeft => flags | TextFormatFlags.Left | TextFormatFlags.Top,
                ContentAlignment.TopCenter => flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.Top,
                ContentAlignment.TopRight => flags | TextFormatFlags.Right | TextFormatFlags.Top,
                ContentAlignment.MiddleLeft => flags | TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                ContentAlignment.MiddleCenter => flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ContentAlignment.MiddleRight => flags | TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ContentAlignment.BottomLeft => flags | TextFormatFlags.Left | TextFormatFlags.Bottom,
                ContentAlignment.BottomCenter => flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom,
                _ => flags | TextFormatFlags.Right | TextFormatFlags.Bottom,
            };
        }
    }
}
