using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Chips.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
        private void SwitchPainter(ChipStyle kind)
        {
            _renderOptions.Style = kind; // still pass style for fine-tuning inside painters

            switch (kind)
            {
                case ChipStyle.Classic:
                case ChipStyle.Professional:
                case ChipStyle.HighContrast:
                    _painter = new OutlinedChipGroupPainter();
                    break;

                case ChipStyle.Minimalist:
                    _painter = new TextChipGroupPainter();
                    break;

                case ChipStyle.Colorful:
                case ChipStyle.Soft:
                    _painter = new SmoothChipGroupPainter();
                    break;

                case ChipStyle.Default:
                case ChipStyle.Modern:
                default:
                    _painter = new DefaultChipGroupPainter();
                    break;
            }
            _painter.Initialize(this, _currentTheme);
        }
    }
}
