using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    /// <summary>
    /// Interface for dialog painters
    /// Provides methods for rendering dialogs with BeepStyling integration
    /// </summary>
    public interface IDialogPainter
    {
        /// <summary>
        /// Paint the complete dialog
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Dialog bounds</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint dialog background
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Background bounds</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void PaintBackground(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint dialog border
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Border bounds</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint shadow effect
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Shadow bounds</param>
        /// <param name="config">Dialog configuration</param>
        void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config);

        /// <summary>
        /// Paint dialog icon
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="iconRect">Icon rectangle</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint dialog title
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="titleRect">Title rectangle</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint dialog message/content
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="messageRect">Message rectangle</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void PaintMessage(Graphics g, Rectangle messageRect, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint dialog buttons
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="buttonBounds">Button area bounds</param>
        /// <param name="config">Dialog configuration</param>
        /// <param name="theme">Current theme</param>
        void PaintButtons(Graphics g, Rectangle buttonBounds, DialogConfig config, IBeepTheme theme);

        /// <summary>
        /// Calculate required dialog size based on content
        /// Accounts for BorderThickness and ShadowSize from BeepControlStyle
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="config">Dialog configuration</param>
        /// <returns>Calculated size</returns>
        Size CalculateSize(Graphics g, DialogConfig config);

        /// <summary>
        /// Calculate layout rectangles for all dialog components
        /// </summary>
        /// <param name="bounds">Overall dialog bounds</param>
        /// <param name="config">Dialog configuration</param>
        /// <returns>Layout information</returns>
        DialogLayout CalculateLayout(Rectangle bounds, DialogConfig config);
    }

    /// <summary>
    /// Dialog layout information
    /// Contains rectangles for all dialog components
    /// </summary>
    public class DialogLayout
    {
        /// <summary>
        /// Overall content area (excluding borders and shadow)
        /// </summary>
        public Rectangle ContentArea { get; set; }

        /// <summary>
        /// Icon rectangle
        /// </summary>
        public Rectangle IconRect { get; set; }

        /// <summary>
        /// Title rectangle
        /// </summary>
        public Rectangle TitleRect { get; set; }

        /// <summary>
        /// Message/content rectangle
        /// </summary>
        public Rectangle MessageRect { get; set; }

        /// <summary>
        /// Details rectangle (if present)
        /// </summary>
        public Rectangle DetailsRect { get; set; }

        /// <summary>
        /// Custom control rectangle (if present)
        /// </summary>
        public Rectangle CustomControlRect { get; set; }

        /// <summary>
        /// Button area rectangle
        /// </summary>
        public Rectangle ButtonAreaRect { get; set; }

        /// <summary>
        /// Individual button rectangles
        /// </summary>
        public Rectangle[] ButtonRects { get; set; }

        /// <summary>
        /// Close button rectangle (title bar)
        /// </summary>
        public Rectangle CloseButtonRect { get; set; }

        /// <summary>
        /// Shadow offset and size
        /// </summary>
        public Rectangle ShadowRect { get; set; }
    }
}
