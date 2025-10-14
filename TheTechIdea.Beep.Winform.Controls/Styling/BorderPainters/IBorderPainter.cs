
namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    public interface IBorderPainter
    {
         void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal);
    }
}