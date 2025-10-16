
namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    public interface IBorderPainter
    {
         /// <summary>
         /// Paint border and return the inner path (content area after border)
         /// </summary>
         GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal);
    }
}