using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Examples
{
    /// <summary>
    /// Example demonstrating Phase 3 runtime window positioning.
    /// Shows how to use BeepDockingManager, MdiPanelPositioner, WindowChrome, 
    /// and PanelWindowManager to create a functional docking layout.
    /// </summary>
    public class Phase3RuntimeExample
    {
        private BeepDockingManager _dockingManager;
        private Form _hostForm;
        private PositioningUtilities.DragState _currentDragState;
        private bool _isHoveringOverSplitter = false;

        /// <summary>
        /// Initializes the example with a host form.
        /// </summary>
        public void Initialize(Form hostForm)
        {
            _hostForm = hostForm ?? throw new ArgumentNullException(nameof(hostForm));
            _currentDragState = new PositioningUtilities.DragState();

            // Create the docking manager
            _dockingManager = new BeepDockingManager(hostForm);

            // Create the native MDI client window
            _dockingManager.CreateMdiClient();

            // Wire up event handlers
            SubscribeToDockingEvents();
            SubscribeToHostFormEvents();
        }

        /// <summary>
        /// Adds example panels to the docking layout.
        /// </summary>
        public void AddExamplePanels()
        {
            // Create some example panels
            var leftPanel = new TextBox
            {
                Name = "LeftPanelContent",
                Multiline = true,
                Text = "Left Panel Content\n\nThis panel is docked on the left.",
                Dock = DockStyle.Fill
            };

            var rightPanel = new ListBox
            {
                Name = "RightPanelContent",
                Dock = DockStyle.Fill
            };
            rightPanel.Items.AddRange(new[] { "Item 1", "Item 2", "Item 3", "Item 4" });

            var topPanel = new Label
            {
                Name = "TopPanelContent",
                Text = "Top Panel - Properties",
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            var centerPanel = new RichTextBox
            {
                Name = "CenterPanelContent",
                Text = "Center Panel - Main Content Area",
                Dock = DockStyle.Fill
            };

            // Add panels to the docking manager
            _dockingManager.AddPanel("LeftPanel", "Explorer", DockPosition.Left, leftPanel);
            _dockingManager.AddPanel("RightPanel", "Results", DockPosition.Right, rightPanel);
            _dockingManager.AddPanel("TopPanel", "Properties", DockPosition.Top, topPanel);
            _dockingManager.AddPanel("CenterPanel", "Editor", DockPosition.Fill, centerPanel);

            Debug.WriteLine("[Phase3Example] Added 4 example panels");
        }

        /// <summary>
        /// Subscribes to docking manager events.
        /// </summary>
        private void SubscribeToDockingEvents()
        {
            if (_dockingManager?.PanelWindowManager != null)
            {
                _dockingManager.PanelWindowManager.PanelCreated += (s, args) =>
                {
                    Debug.WriteLine($"[Phase3Example] Panel created: {args.PanelKey}");
                };

                _dockingManager.PanelWindowManager.PanelActivated += (s, args) =>
                {
                    Debug.WriteLine($"[Phase3Example] Panel activated: {args.PanelKey}");
                };

                _dockingManager.PanelWindowManager.PanelHidden += (s, args) =>
                {
                    Debug.WriteLine($"[Phase3Example] Panel hidden: {args.PanelKey}");
                };
            }
        }

        /// <summary>
        /// Subscribes to host form events for layout management.
        /// </summary>
        private void SubscribeToHostFormEvents()
        {
            if (_hostForm == null)
                return;

            _hostForm.Resize += (s, e) =>
            {
                // Recalculate layout when the form is resized
                _dockingManager?.ResizeMdiClient();
            };

            _hostForm.MouseMove += (s, e) =>
            {
                HandleMouseMove(e.Location);
            };

            _hostForm.MouseDown += (s, e) =>
            {
                HandleMouseDown(e.Location, e.Button);
            };

            _hostForm.MouseUp += (s, e) =>
            {
                HandleMouseUp(e.Location);
            };

            _hostForm.Load += (s, e) =>
            {
                // Initial layout calculation
                _dockingManager?.ResizeMdiClient();
            };
        }

        /// <summary>
        /// Handles mouse movement for splitter interaction.
        /// </summary>
        private void HandleMouseMove(Point location)
        {
            if (_currentDragState.IsDragging)
            {
                // Update drag state
                _currentDragState.CurrentPosition = location;

                // Calculate new sizes
                PositioningUtilities.CalculateDragResult(
                    _currentDragState,
                    _hostForm.ClientSize
                );

                Debug.WriteLine(
                    $"[Phase3Example] Dragging splitter: " +
                    $"Left/Top={_currentDragState.NewLeftOrTopSize}, " +
                    $"Right/Bottom={_currentDragState.NewRightOrBottomSize}"
                );
            }
            else
            {
                // Check if hovering over splitter for cursor feedback
                // This would integrate with the layout controller's hit-testing
                _isHoveringOverSplitter = false;
                PositioningUtilities.UpdateCursorForSplitter(
                    Orientation.Vertical,
                    _isHoveringOverSplitter
                );
            }
        }

        /// <summary>
        /// Handles mouse down for starting splitter drag.
        /// </summary>
        private void HandleMouseDown(Point location, MouseButtons button)
        {
            if (button != MouseButtons.Left)
                return;

            // In a full implementation, we would hit-test splitters here
            // For this example, we demonstrate the drag state API
            _currentDragState.IsDragging = true;
            _currentDragState.StartPosition = location;
            _currentDragState.CurrentPosition = location;

            Debug.WriteLine($"[Phase3Example] Splitter drag started at {location}");
        }

        /// <summary>
        /// Handles mouse up for ending splitter drag.
        /// </summary>
        private void HandleMouseUp(Point location)
        {
            if (!_currentDragState.IsDragging)
                return;

            _currentDragState.IsDragging = false;

            Debug.WriteLine(
                $"[Phase3Example] Splitter drag ended. " +
                $"New left/top size: {_currentDragState.NewLeftOrTopSize}, " +
                $"new right/bottom size: {_currentDragState.NewRightOrBottomSize}"
            );

            // Apply the new layout if needed
            _dockingManager?.RecalculateLayout();

            _currentDragState.Reset();
        }

        /// <summary>
        /// Activates a specific panel by key.
        /// </summary>
        public void ActivatePanel(string panelKey)
        {
            _dockingManager?.PanelWindowManager?.ActivatePanel(panelKey);
            Debug.WriteLine($"[Phase3Example] Activated panel: {panelKey}");
        }

        /// <summary>
        /// Hides a specific panel by key.
        /// </summary>
        public void HidePanel(string panelKey)
        {
            _dockingManager?.PanelWindowManager?.HidePanel(panelKey);
            Debug.WriteLine($"[Phase3Example] Hid panel: {panelKey}");
        }

        /// <summary>
        /// Shows a previously hidden panel by key.
        /// </summary>
        public void ShowPanel(string panelKey)
        {
            _dockingManager?.PanelWindowManager?.ShowPanel(panelKey);
            Debug.WriteLine($"[Phase3Example] Showed panel: {panelKey}");
        }

        /// <summary>
        /// Removes a panel and destroys its window.
        /// </summary>
        public void RemovePanel(string panelKey)
        {
            _dockingManager?.RemovePanel(panelKey);
            Debug.WriteLine($"[Phase3Example] Removed panel: {panelKey}");
        }

        /// <summary>
        /// Prints diagnostic information about the current state.
        /// </summary>
        public void PrintDiagnostics()
        {
            if (_dockingManager == null)
                return;

            Debug.WriteLine("\n=== Docking Manager State ===");
            Debug.WriteLine(_dockingManager.GetDiagnostics());

            if (_dockingManager.Positioner != null)
            {
                var positionerDiags = _dockingManager.Positioner.GetDiagnostics();
                Debug.WriteLine($"\n=== Positioner State ===");
                Debug.WriteLine($"Total Managed Panels: {positionerDiags.TotalManagedPanels}");
                Debug.WriteLine($"Visible Panels: {positionerDiags.VisiblePanels}");
                Debug.WriteLine($"Hidden Panels: {positionerDiags.HiddenPanels}");
                Debug.WriteLine($"Active Panel: {positionerDiags.ActivePanel ?? "(none)"}");
            }

            if (_dockingManager.Chrome != null)
            {
                var chromeDiags = _dockingManager.Chrome.GetDiagnostics();
                Debug.WriteLine($"\n=== Chrome State ===");
                Debug.WriteLine($"Tab Strip Height: {chromeDiags.TabStripHeight}");
                Debug.WriteLine($"Chrome Height: {chromeDiags.ChromeHeight}");
                Debug.WriteLine($"Cached Tabs: {chromeDiags.CachedTabs}");
            }
        }

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        public void Dispose()
        {
            _dockingManager?.Dispose();
            _dockingManager = null;
        }
    }

    /// <summary>
    /// Example Form demonstrating Phase 3 runtime positioning.
    /// </summary>
    public partial class Phase3DemoForm : Form
    {
        private Phase3RuntimeExample _example;

        public Phase3DemoForm()
        {
            InitializeComponent();
            Text = "Beep Docking Engine - Phase 3 Runtime Positioning Example";
            Size = new Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Initialize the example
            _example = new Phase3RuntimeExample();
            _example.Initialize(this);
            _example.AddExamplePanels();

            // Print initial diagnostics
            _example.PrintDiagnostics();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _example?.Dispose();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ResumeLayout(false);
        }
    }
}
