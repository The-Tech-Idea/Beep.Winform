using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption
{
    /// <summary>
    /// Strategy interface for painting and interacting with caption system buttons.
    /// Implementations decide layout (left/right clusters), visuals, and hit tests.
    /// </summary>
    internal interface ICaptionRenderer
    {
        void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider);
        void UpdateTheme(IBeepTheme theme);
        void SetShowSystemButtons(bool show);

        /// <summary>
        /// Computes the horizontal insets that must be reserved for caption chrome so the title can be drawn.
        /// </summary>
        void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset);

        /// <summary>
        /// Paints the caption system buttons and returns the area that should be invalidated.
        /// </summary>
        void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea);

        /// <summary>
        /// Updates hover state on mouse move. Returns true if state changed and outputs area to invalidate.
        /// </summary>
        bool OnMouseMove(Point location, out Rectangle invalidatedArea);

        /// <summary>
        /// Handles mouse leave, clearing hover state and returning area to invalidate if any.
        /// </summary>
        void OnMouseLeave(out Rectangle invalidatedArea);

        /// <summary>
        /// Performs hit test and returns an action to execute on click (e.g., Close/Minimize/ToggleMaximize). Returns true if a button was hit.
        /// </summary>
        bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea);

        /// <summary>
        /// Whether the given client point is over any system button.
        /// </summary>
        bool HitTest(Point location);
    }
}
