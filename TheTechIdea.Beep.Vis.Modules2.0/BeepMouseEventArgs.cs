using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepMouseEventArgs : EventArgs
    {
        private readonly BeepMouseButtons button;

        private readonly int clicks;

        private readonly int x;

        private readonly int y;

        private readonly int delta;
        public bool Handled { get; set; }
        private readonly object data;
        private readonly string eventname;
        private readonly object sender;
        //
        // Summary:
        //     Gets which mouse button was pressed.
        //
        // Returns:
        //     One of the System.Windows.Forms.MouseButtons values.
        public BeepMouseButtons Button => button;

        //
        // Summary:
        //     Gets the number of times the mouse button was pressed and released.
        //
        // Returns:
        //     An System.Int32 that contains the number of times the mouse button was pressed
        //     and released.
        public int Clicks => clicks;

        //
        // Summary:
        //     Gets the x-coordinate of the mouse during the generating mouse event.
        //
        // Returns:
        //     The x-coordinate of the mouse, in pixels.
        public int X => x;

        //
        // Summary:
        //     Gets the y-coordinate of the mouse during the generating mouse event.
        //
        // Returns:
        //     The y-coordinate of the mouse, in pixels.
        public int Y => y;

        //
        // Summary:
        //     Gets a signed count of the number of detents the mouse wheel has rotated, multiplied
        //     by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.
        //
        // Returns:
        //     A signed count of the number of detents the mouse wheel has rotated, multiplied
        //     by the WHEEL_DELTA constant.
        public int Delta => delta;

        //
        // Summary:
        //     Gets the location of the mouse during the generating mouse event.
        //
        // Returns:
        //     A System.Drawing.Point that contains the x- and y- mouse coordinates, in pixels,
        //     relative to the upper-left corner of the form.
        public System.Drawing.Point Location => new System.Drawing.Point(x, y);

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.MouseEventArgs class.
        //
        // Parameters:
        //   button:
        //     One of the System.Windows.Forms.MouseButtons values that indicate which mouse
        //     button was pressed.
        //
        //   clicks:
        //     The number of times a mouse button was pressed.
        //
        //   x:
        //     The x-coordinate of a mouse click, in pixels.
        //
        //   y:
        //     The y-coordinate of a mouse click, in pixels.
        //
        //   delta:
        //     A signed count of the number of detents the wheel has rotated.
        public BeepMouseEventArgs(BeepMouseButtons button, int clicks, int x, int y, int delta)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
        }
        public BeepMouseEventArgs(BeepMouseButtons button, int clicks, int x, int y, int delta, object data)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
            this.data = data;
        }
        public BeepMouseEventArgs(BeepMouseButtons button, int clicks, int x, int y, int delta, bool handled)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
            this.Handled = handled;
        }
        public BeepMouseEventArgs(string evname,object senderobj)
        {
            this.eventname = evname;
            this.sender = senderobj;
        }
    }
}
