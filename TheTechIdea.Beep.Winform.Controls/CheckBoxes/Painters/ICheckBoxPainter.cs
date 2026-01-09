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
        public Font TextFont { get; set; }
        public string Text { get; set; }
        public bool HideText { get; set; }
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Right;
    }

    /// <summary>
    /// Interface for checkbox painters
    /// </summary>
    public interface ICheckBoxPainter
    {
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
