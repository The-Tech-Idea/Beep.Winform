using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepMouseEventArgs : EventArgs
    {
        public enum BeepMouseButtons
        {
            Left,
            Right,
            Middle,
            XButton1,
            XButton2
        }
        public string EventName 
        {
            get { return eventname; }
            set { eventname = value; }

        }
        private  BeepMouseButtons button;

        private  int clicks;

        private  int x;

        private  int y;

        private  int delta;
        public bool Handled { get; set; }
        private  object data;
        private string eventname;
        private  object sender;
        public object Data
        {
            get { return data; }
            set { data = value; }
        }
        public object Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        //
        // Summary:
        //     Gets which mouse button was pressed.
        //
        // Returns:
        //     One of the System.Windows.Forms.MouseButtons values.
        public BeepMouseButtons Button
        {
            get { return button; }
            set { button = value; }
        }

        //
        // Summary:
        //     Gets the number of times the mouse button was pressed and released.
        //
        // Returns:
        //     An System.Int32 that contains the number of times the mouse button was pressed
        //     and released.
        public int Clicks 
        {
            get { return clicks; }
            set { clicks = value; }

        }

        //
        // Summary:
        //     Gets the x-coordinate of the mouse during the generating mouse event.
        //
        // Returns:
        //     The x-coordinate of the mouse, in pixels.
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        //
        // Summary:
        //     Gets the y-coordinate of the mouse during the generating mouse event.
        //
        // Returns:
        //     The y-coordinate of the mouse, in pixels.
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        //
        // Summary:
        //     Gets a signed count of the number of detents the mouse wheel has rotated, multiplied
        //     by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.
        //
        // Returns:
        //     A signed count of the number of detents the mouse wheel has rotated, multiplied
        //     by the WHEEL_DELTA constant.
        public int Delta
        {
            get { return delta; }
            set { delta = value; }
        }

        //
        // Summary:
        //     Gets the location of the mouse during the generating mouse event.
        //
        // Returns:
        //     A System.Drawing.Point that contains the x- and y- mouse coordinates, in pixels,
        //     relative to the upper-left corner of the form.
        private Point _location;
        public System.Drawing.Point Location
        {
            get { return _location; }
            set { _location = value; }
            
       }

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
        public BeepMouseEventArgs(string evname, BeepMouseButtons mouseButtons, int clicks, int x, int y, int delta)
        {
            this.button = mouseButtons;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
            this.eventname = evname;
        }
        public BeepMouseEventArgs(string evname, BeepMouseButtons mouseButtons, int clicks, int x, int y, int delta, object data)
        {
            this.button = mouseButtons;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
            this.data = data;
            this.eventname = evname;
        }
       
        public BeepMouseEventArgs(string evname,object senderobj)
        {
            this.eventname = evname;
            this.sender = senderobj;
        }

        public BeepMouseEventArgs()
        {
        }
    }
}
