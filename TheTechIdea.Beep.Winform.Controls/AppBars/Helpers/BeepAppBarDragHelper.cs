using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Handles form dragging functionality for BeepAppBar
    /// </summary>
    internal class BeepAppBarDragHelper
    {
        private readonly IBeepAppBarHost _host;
        private readonly BeepAppBarStateStore _stateStore;
        private readonly BeepAppBarLayoutHelper _layoutHelper;

        // P/Invoke for dragging
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        // Dragging configuration
        private bool _enableFormDragging = true;
        private List<string> _draggableAreas = new List<string> { "Title", "Logo" };

        public BeepAppBarDragHelper(
            IBeepAppBarHost host,
            BeepAppBarStateStore stateStore,
            BeepAppBarLayoutHelper layoutHelper)
        {
            _host = host;
            _stateStore = stateStore;
            _layoutHelper = layoutHelper;
        }

        #region "Public Properties"

        /// <summary>
        /// Enables or disables form dragging
        /// </summary>
        public bool EnableFormDragging 
        { 
            get => _enableFormDragging; 
            set => _enableFormDragging = value; 
        }

        /// <summary>
        /// Areas where dragging is allowed
        /// </summary>
        public List<string> DraggableAreas 
        { 
            get => _draggableAreas; 
            set => _draggableAreas = value ?? new List<string>(); 
        }

        #endregion

        #region "Drag Detection"

        /// <summary>
        /// Checks if the current mouse position is in a draggable area
        /// </summary>
        public bool IsInDraggableArea(System.Drawing.Point mousePoint)
        {
            if (!_enableFormDragging)
                return false;

            // If no specific draggable areas are defined, entire AppBar is draggable
            if (_draggableAreas == null || _draggableAreas.Count == 0)
                return true;

            // Check each defined draggable area
            foreach (string area in _draggableAreas)
            {
                if (CheckAreaContainsPoint(area.ToLower(), mousePoint))
                    return true;
            }

            // Check if mouse is in empty space (not over any interactive elements)
            if (_draggableAreas.Contains("empty", StringComparer.OrdinalIgnoreCase))
            {
                return !_layoutHelper.IsInteractiveArea(mousePoint);
            }

            return false;
        }

        private bool CheckAreaContainsPoint(string area, System.Drawing.Point mousePoint)
        {
            switch (area)
            {
                case "logo":
                    return _host.ShowLogo && !string.IsNullOrEmpty(_host.LogoImage) && 
                           _layoutHelper.GetLogoRect().Contains(mousePoint);

                case "title":
                    return _host.ShowTitle && _layoutHelper.GetTitleRect().Contains(mousePoint);

                case "appbar":
                case "all":
                    return true;

                default:
                    return false;
            }
        }

        #endregion

        #region "Drag Operations"

        /// <summary>
        /// Starts the drag operation using Windows API
        /// </summary>
        public void StartDrag(System.Drawing.Point mousePoint)
        {
            Form parentForm = _host.AsControl.FindForm();
            if (parentForm == null)
                return;

            ReleaseCapture();
            SendMessage(parentForm.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        /// <summary>
        /// Handles mouse down for potential dragging
        /// </summary>
        public bool HandleMouseDown(MouseEventArgs e)
        {
            if (_host.DesignMode || e.Button != MouseButtons.Left)
                return false;

            // Check if this is a left mouse button click in a draggable area
            if (IsInDraggableArea(e.Location))
            {
                // Only start dragging if we're not clicking on interactive elements
                if (!_layoutHelper.IsInteractiveArea(e.Location))
                {
                    StartDrag(e.Location);
                    return true; // Indicates that drag was started
                }
            }

            return false; // No drag started
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Sets which areas of the AppBar allow dragging
        /// </summary>
        public void SetDraggableAreas(params string[] areas)
        {
            _draggableAreas = new List<string>(areas);
        }

        /// <summary>
        /// Sets form dragging enabled/disabled
        /// </summary>
        public void SetFormDraggingEnabled(bool enabled)
        {
            _enableFormDragging = enabled;
            if (!enabled && _stateStore.IsDragging)
            {
                _stateStore.IsDragging = false;
            }
        }

        /// <summary>
        /// Checks if the AppBar is currently being dragged
        /// </summary>
        public bool IsDragging => _stateStore.IsDragging;

        #endregion
    }
}