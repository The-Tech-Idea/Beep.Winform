using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docks.Painters;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks
{
    /// <summary>
    /// Popup form for displaying BeepDock in a floating window
    /// Inherits from BeepiFormPro for consistent theming and FormStyle support
    /// </summary>
    public class BeepDockPopup : BeepiFormPro
    {
        private readonly List<SimpleItem> _items;
        private readonly DockConfig _config;
        private IDockPainter _painter;
        private readonly List<DockItemState> _itemStates;
        private int _hoveredIndex = -1;

        /// <summary>
        /// Event fired when a dock item is clicked
        /// </summary>
        public event EventHandler<DockItemEventArgs> ItemClicked;

        /// <summary>
        /// Event fired when a dock item is hovered
        /// </summary>
        public event EventHandler<DockItemEventArgs> ItemHovered;

        /// <summary>
        /// Gets or sets the dock configuration
        /// </summary>
        public DockConfig Config
        {
            get => _config;
        }

        /// <summary>
        /// Gets or sets the dock style
        /// </summary>
        public DockStyle DockStyle
        {
            get => _config.Style;
            set
            {
                if (_config.Style != value)
                {
                    _config.Style = value;
                    _painter = DockPainterFactory.GetPainter(value);
                    UpdateSize();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the collection of dock items
        /// </summary>
        public List<SimpleItem> Items => _items;

        public BeepDockPopup()
        {
            _items = new List<SimpleItem>();
            _config = new DockConfig
            {
                Style = DockStyle.AppleDock,
                Position = DockPosition.Bottom,
                ShowBackground = true,
                BackgroundOpacity = 0.85f,
                ShowShadow = true
            };
            _itemStates = new List<DockItemState>();
            _painter = DockPainterFactory.GetPainter(_config.Style);

            InitializeForm();
        }

        public BeepDockPopup(DockConfig config) : this()
        {
            if (config != null)
            {
                _config = config;
                _painter = DockPainterFactory.GetPainter(config.Style);
            }
        }

        private void InitializeForm()
        {
            // Form properties for transparency and floating behavior
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false;
            StartPosition = FormStartPosition.Manual;

            // Enable transparency
          //  IsTransparent = true;
            BackColor = Color.Transparent;

            // Set double buffering for smooth rendering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            // Event handlers
            MouseMove += OnDockMouseMove;
            MouseLeave += OnDockMouseLeave;
            MouseClick += OnDockMouseClick;
            Paint += OnDockPaint;
        }

        /// <summary>
        /// Adds an item to the dock
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item == null)
                return;

            _items.Add(item);

            var state = new DockItemState
            {
                Item = item,
                Index = _items.Count - 1,
                CurrentScale = 1.0f,
                TargetScale = 1.0f,
                CurrentOpacity = 1.0f
            };
            _itemStates.Add(state);

            UpdateSize();
            Invalidate();
        }

        /// <summary>
        /// Removes an item from the dock
        /// </summary>
        public void RemoveItem(SimpleItem item)
        {
            int index = _items.IndexOf(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                _itemStates.RemoveAt(index);

                // Update indices
                for (int i = index; i < _itemStates.Count; i++)
                {
                    _itemStates[i].Index = i;
                }

                UpdateSize();
                Invalidate();
            }
        }

        /// <summary>
        /// Clears all items from the dock
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
            _itemStates.Clear();
            UpdateSize();
            Invalidate();
        }

        /// <summary>
        /// Updates the form size based on items and configuration
        /// </summary>
        private void UpdateSize()
        {
            if (_items.Count == 0)
            {
                Size = new Size(100, _config.DockHeight);
                return;
            }

            // Calculate size based on orientation
            int totalSize = 0;
            int thickness = _config.DockHeight;

            if (_config.Orientation == DockOrientation.Horizontal)
            {
                totalSize = (_config.ItemSize * _items.Count) +
                           (_config.Spacing * Math.Max(0, _items.Count - 1)) +
                           (_config.Padding * 2);
                Size = new Size(totalSize, thickness);
            }
            else
            {
                totalSize = (_config.ItemSize * _items.Count) +
                           (_config.Spacing * Math.Max(0, _items.Count - 1)) +
                           (_config.Padding * 2);
                Size = new Size(thickness, totalSize);
            }
        }

        /// <summary>
        /// Positions the dock at the specified screen position
        /// </summary>
        public void ShowAtPosition(Point screenPosition)
        {
            Location = screenPosition;
            Show();
        }

        /// <summary>
        /// Shows the dock at the bottom center of the primary screen
        /// </summary>
        public void ShowAtScreenBottom()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            int x = screen.Left + (screen.Width - Width) / 2;
            int y = screen.Bottom - Height - 10;
            ShowAtPosition(new Point(x, y));
        }

        /// <summary>
        /// Shows the dock at the specified dock position
        /// </summary>
        public void ShowAtDockPosition(DockPosition position)
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            Point location;

            switch (position)
            {
                case DockPosition.Top:
                    location = new Point(screen.Left + (screen.Width - Width) / 2, screen.Top + 10);
                    break;

                case DockPosition.Bottom:
                    location = new Point(screen.Left + (screen.Width - Width) / 2, screen.Bottom - Height - 10);
                    break;

                case DockPosition.Left:
                    location = new Point(screen.Left + 10, screen.Top + (screen.Height - Height) / 2);
                    break;

                case DockPosition.Right:
                    location = new Point(screen.Right - Width - 10, screen.Top + (screen.Height - Height) / 2);
                    break;

                case DockPosition.Center:
                    location = new Point(screen.Left + (screen.Width - Width) / 2, screen.Top + (screen.Height - Height) / 2);
                    break;

                default:
                    location = new Point(screen.Left + (screen.Width - Width) / 2, screen.Bottom - Height - 10);
                    break;
            }

            ShowAtPosition(location);
        }

        private void OnDockPaint(object sender, PaintEventArgs e)
        {
            if (_painter == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Paint dock background
            _painter.PaintDockBackground(g, ClientRectangle, _config, CurrentTheme);

            // Paint each item
            foreach (var state in _itemStates)
            {
                _painter.PaintDockItem(g, state, _config, CurrentTheme);
                _painter.PaintIndicator(g, state, _config, CurrentTheme);
            }
        }

        private void OnDockMouseMove(object sender, MouseEventArgs e)
        {
            // Hit test to find hovered item
            int newHoveredIndex = -1;

            for (int i = 0; i < _itemStates.Count; i++)
            {
                if (_itemStates[i].Bounds.Contains(e.Location))
                {
                    newHoveredIndex = i;
                    break;
                }
            }

            if (newHoveredIndex != _hoveredIndex)
            {
                // Update hover states
                if (_hoveredIndex >= 0 && _hoveredIndex < _itemStates.Count)
                {
                    _itemStates[_hoveredIndex].IsHovered = false;
                    _itemStates[_hoveredIndex].TargetScale = 1.0f;
                }

                _hoveredIndex = newHoveredIndex;

                if (_hoveredIndex >= 0)
                {
                    _itemStates[_hoveredIndex].IsHovered = true;
                    _itemStates[_hoveredIndex].TargetScale = _config.MaxScale;

                    // Fire hover event
                    ItemHovered?.Invoke(this, new DockItemEventArgs(_itemStates[_hoveredIndex].Item, _hoveredIndex));
                }

                UpdateItemBounds();
                Invalidate();
            }
        }

        private void OnDockMouseLeave(object sender, EventArgs e)
        {
            if (_hoveredIndex >= 0 && _hoveredIndex < _itemStates.Count)
            {
                _itemStates[_hoveredIndex].IsHovered = false;
                _itemStates[_hoveredIndex].TargetScale = 1.0f;
            }

            _hoveredIndex = -1;
            UpdateItemBounds();
            Invalidate();
        }

        private void OnDockMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            for (int i = 0; i < _itemStates.Count; i++)
            {
                if (_itemStates[i].Bounds.Contains(e.Location))
                {
                    ItemClicked?.Invoke(this, new DockItemEventArgs(_itemStates[i].Item, i));
                    break;
                }
            }
        }

        private void UpdateItemBounds()
        {
            if (_items.Count == 0)
                return;

            var bounds = Helpers.DockLayoutHelper.CalculateItemBounds(
                ClientRectangle,
                _items,
                _config,
                _hoveredIndex,
                1.0f
            );

            for (int i = 0; i < bounds.Length && i < _itemStates.Count; i++)
            {
                _itemStates[i].Bounds = bounds[i];
                _itemStates[i].HitBounds = Helpers.DockHitTestHelper.CalculateHitBounds(bounds[i], 4);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            
            if (Visible)
            {
                UpdateItemBounds();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                MouseMove -= OnDockMouseMove;
                MouseLeave -= OnDockMouseLeave;
                MouseClick -= OnDockMouseClick;
                Paint -= OnDockPaint;
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Event arguments for dock item events
    /// </summary>
    public class DockItemEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int Index { get; }

        public DockItemEventArgs(SimpleItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}
