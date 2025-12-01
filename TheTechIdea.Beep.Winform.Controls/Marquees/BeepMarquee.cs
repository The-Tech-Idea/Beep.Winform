
using System.ComponentModel;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;


namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// This Control will show a web-like marquee of items (IBeepUIComponent).
    /// The effect is similar to an HTML <marquee> with continuous scrolling and seamless wrap-around.
    /// </summary>
    /// 
    [ToolboxItem(true)]
    [Category("UI")]
    [Description("A control that displays a continuous marquee of items.")]
    [DisplayName("Beep Marquee")]
    public class BeepMarquee : BaseControl
    {
        private Dictionary<string, IBeepUIComponent> _marqueeComponents
            = new Dictionary<string, IBeepUIComponent>();

        // The timer that drives animation.
        private Timer _timer;

        // The horizontal position from which we start drawing the first component.
        // As we scroll, we decrement (or increment) this offset.
        private float _scrollOffset = 0f;

        // Pixels to move per timer tick. 
        // Increase for faster scrolling, decrease for slower.
        private float _scrollSpeed = 2f;

        // If true, scroll from right to left. If false, left to right.
        public bool ScrollLeft { get; set; } = true;

        // Spacing (in pixels) between consecutive components.
        public int ComponentSpacing { get; set; } = 20;

        // The refresh rate of the marquee in milliseconds.
        public int ScrollInterval
        {
            get => _timer?.Interval ?? 30;
            set
            {
                if (_timer != null)
                    _timer.Interval = value;
            }
        }

        public float ScrollSpeed
        {
            get => _scrollSpeed;
            set => _scrollSpeed = Math.Max(0, value);
        }

        public BeepMarquee():base()
        {
            // Create and configure the timer
            _timer = new Timer();
            _timer.Interval = 30; // Default to ~33 FPS
            _timer.Tick += (sender, e) => OnTimerTick();

            // Only start the timer if not in design mode
            if (!DesignMode)
            {
                _timer.Start();
            }

       
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode)
            {
                _timer.Start();
            }
        }
        /// <summary>
        /// Adds a new component to the marquee. 
        /// If the key already exists, it is replaced.
        /// </summary>
        public void AddMarqueeComponent(string key, IBeepUIComponent component)
        {
            _marqueeComponents[key] = component;
            Invalidate();
        }

        /// <summary>
        /// Optional method to remove an existing marquee component by key.
        /// </summary>
        public void RemoveMarqueeComponent(string key)
        {
            if (_marqueeComponents.ContainsKey(key))
            {
                _marqueeComponents.Remove(key);
                Invalidate();
            }
        }

        /// <summary>
        /// Timer handler that updates the scroll position and redraws.
        /// </summary>
        private void OnTimerTick()
        { // Skip animation logic if in design mode
            if (DesignMode) return;
            // Move the offset left or right
            _scrollOffset += ScrollLeft ? -_scrollSpeed : _scrollSpeed;

            // Because this is a continuous marquee, we do a wrap-around check.
            // We'll compute how wide one "full loop" is:
            float totalWidth = GetTotalComponentsWidth();

            // For an infinitely repeating effect, we can let offset go negative 
            // (or positive) beyond the total width, then "wrap" it back so that 
            // the user never sees a break in the scrolling.

            // If scrolling left and offset is less than -totalWidth, reset offset
            // to 0 to start the cycle again. (Equivalent to pushing the first item 
            // back to the right side).
            if (ScrollLeft && _scrollOffset < -totalWidth)
            {
                _scrollOffset = 0;
            }
            // If scrolling right and offset is greater than totalWidth, wrap to 0.
            else if (!ScrollLeft && _scrollOffset > totalWidth)
            {
                _scrollOffset = 0;
            }

            Invalidate();
        }

        /// <summary>
        /// Draw all components in a row, offset by _scrollOffset. 
        /// Once we reach the end, we keep drawing them again to achieve the wrap-around.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Clear background
            e.Graphics.Clear(this.BackColor);

            // We treat the list of components like one continuous band. 
            // We may need to draw them multiple times to fill the space when the offset wraps around.

            float totalWidth = GetTotalComponentsWidth();
            float startX = _scrollOffset;

            // We'll keep drawing these "bands" of components until we fill the visible area.
            // The visible area might be bigger or smaller than totalWidth. 
            // So we handle that in a loop.
            // 
            // For a typical marquee, you only need to draw at most 2 times to 
            // handle the wrap (once for the main band, once for the repeated band).

            // We do a while loop to handle the scenario if the control width 
            // is significantly larger than totalWidth, but typically 2 passes are enough.

            float drawX = startX;
            while (drawX < this.Width)
            {
                DrawComponents(e.Graphics, drawX);
                drawX += totalWidth;
            }

            // Also handle if user resizes smaller or if offset is negative, 
            // we might need to draw one pass to the left:
            drawX = startX - totalWidth;
            while (drawX + totalWidth > 0)
            {
                DrawComponents(e.Graphics, drawX);
                drawX -= totalWidth;
            }
        }

        /// <summary>
        /// Draws all components in a row starting from a given X coordinate.
        /// </summary>
        private void DrawComponents(Graphics graphics, float startX)
        {
            float currentX = startX;

            int centerY = this.Height / 2;

            foreach (var kvp in _marqueeComponents)
            {
                var comp = kvp.Value;
                if (comp == null)
                    continue;

                int compWidth = comp.Width;
                int compHeight = comp.Height;

                // Calculate top Y so the component is centered vertically
                int topY = centerY - (compHeight / 2);

                // The rectangle where the component will be drawn
                Rectangle rect = new Rectangle((int)currentX, topY, compWidth, compHeight);

                // Let the component draw itself
                comp.Draw(graphics, rect);

                // Move currentX by the component's width and spacing
                currentX += compWidth + ComponentSpacing;
            }
        }

        /// <summary>
        /// Computes the total width of all components + spacing (one "band").
        /// </summary>
        private float GetTotalComponentsWidth()
        {
            float totalWidth = 0;
            foreach (var kvp in _marqueeComponents)
            {
                if (kvp.Value != null)
                {
                    totalWidth += kvp.Value.Width + ComponentSpacing;
                }
            }
            // Remove the last extra spacing if desired:
            // totalWidth -= ComponentSpacing;
            return totalWidth <= 0 ? 1 : totalWidth; // avoid zero to prevent division issues
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // You can reset or re-calc offset here if needed, but typically not necessary.
        }
    }
}
