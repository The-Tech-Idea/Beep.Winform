using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Border
{
    internal interface IBorderRenderer
    {
        void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider);
        void UpdateTheme(IBeepTheme theme);
        /// <summary>
        /// Paints the caption system buttons and returns the area that should be invalidated.
        /// </summary>
        void Paint(Graphics g, GraphicsPath BorderPath, float scale, IBeepTheme theme, FormWindowState windowState, out GraphicsPath invalidatedArea);
        /// <summary>
        /// Updates hover state on mouse move. Returns true if state changed and outputs area to invalidate.
        /// </summary>
        bool OnMouseMove(Point location, out GraphicsPath invalidatedArea);

        /// <summary>
        /// Handles mouse leave, clearing hover state and returning area to invalidate if any.
        /// </summary>
        void OnMouseLeave(out GraphicsPath invalidatedArea);

        /// <summary>
        /// Performs hit test and returns an action to execute on click (e.g., Close/Minimize/ToggleMaximize). Returns true if a button was hit.
        /// </summary>
        bool OnMouseDown(Point location, Form form, out GraphicsPath invalidatedArea);

        /// <summary>
        /// Whether the given client point is over any system button.
        /// </summary>
        bool HitTest(Point location);
    }
}
