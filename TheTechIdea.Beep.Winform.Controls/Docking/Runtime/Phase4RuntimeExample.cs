using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Examples
{
    /// <summary>
    /// Demonstrates Phase 4 content hosting, event routing, and interaction capabilities.
    /// Shows how content controls are hosted in MDI child windows and how user interactions
    /// (tabs, splitters, mouse events) are routed through the Phase 4 subsystems.
    /// </summary>
    public class Phase4RuntimeExample : IDisposable
    {
        private BeepDockingManager _dockingManager;
        private Form _hostForm;
        private bool _disposed = false;

        /// <summary>
        /// Initializes the Phase 4 example.
        /// </summary>
        public void Initialize(Form hostForm)
        {
            if (hostForm == null)
                throw new ArgumentNullException(nameof(hostForm));

            _hostForm = hostForm;

            // Create the docking manager for the host form
            _dockingManager = new BeepDockingManager(_hostForm);

            // Create the MDI client window (required before adding panels)
            _dockingManager.CreateMdiClient();

            // Wire up event handlers for content hosting
            _dockingManager.ContentHosting.GetAllHostedContent();  // Verify it's initialized

            // Wire up event handlers for tab interactions
            _dockingManager.TabHandler.TabClicked += OnTabClicked;
            _dockingManager.TabHandler.TabClosed += OnTabClosed;
            _dockingManager.TabHandler.TabDoubleClicked += OnTabDoubleClicked;

            // Wire up splitter drag events
            _dockingManager.DragHandler.DragStarted += OnDragStarted;
            _dockingManager.DragHandler.DragUpdated += OnDragUpdated;
            _dockingManager.DragHandler.DragCompleted += OnDragCompleted;

            Console.WriteLine("[Phase4Example] Initialized docking manager");
        }

        /// <summary>
        /// Adds example panels with content.
        /// </summary>
        public void AddExamplePanels()
        {
            // Add left panel with tree view
            var leftTree = CreateTreeViewPanel();
            _dockingManager.AddPanel(
                "LeftPanel",
                "Project Explorer",
                DockPosition.Left,
                leftTree
            );

            // Add right panel with properties grid
            var rightProps = CreatePropertiesPanel();
            _dockingManager.AddPanel(
                "RightPanel",
                "Properties",
                DockPosition.Right,
                rightProps
            );

            // Add top panel with toolbar
            var topBar = CreateToolbarPanel();
            _dockingManager.AddPanel(
                "TopPanel",
                "Toolbar",
                DockPosition.Top,
                topBar
            );

            // Add center panel with document area
            var centerDoc = CreateDocumentPanel();
            _dockingManager.AddPanel(
                "CenterPanel",
                "Document",
                DockPosition.Fill,
                centerDoc
            );

            Console.WriteLine("[Phase4Example] Added example panels");
        }

        /// <summary>
        /// Demonstrates activating a panel programmatically.
        /// </summary>
        public void ActivatePanel(string panelKey)
        {
            _dockingManager.ActivatePanel(panelKey);
            Console.WriteLine($"[Phase4Example] Activated panel '{panelKey}'");
        }

        /// <summary>
        /// Demonstrates hiding a panel.
        /// </summary>
        public void HidePanel(string panelKey)
        {
            var panel = _dockingManager.GetPanel(panelKey);
            if (panel != null)
            {
                var hosted = _dockingManager.ContentHosting.GetHostedContent(panelKey);
                if (hosted != null)
                {
                    _dockingManager.ContentHosting.HideContent(panelKey);
                    Console.WriteLine($"[Phase4Example] Hidden panel '{panelKey}'");
                }
            }
        }

        /// <summary>
        /// Demonstrates showing a hidden panel.
        /// </summary>
        public void ShowPanel(string panelKey)
        {
            var hosted = _dockingManager.ContentHosting.GetHostedContent(panelKey);
            if (hosted != null)
            {
                _dockingManager.ContentHosting.ShowContent(panelKey);
                Console.WriteLine($"[Phase4Example] Showed panel '{panelKey}'");
            }
        }

        /// <summary>
        /// Demonstrates removing a panel.
        /// </summary>
        public void RemovePanel(string panelKey)
        {
            _dockingManager.RemovePanel(panelKey);
            Console.WriteLine($"[Phase4Example] Removed panel '{panelKey}'");
        }

        /// <summary>
        /// Prints comprehensive diagnostics for all Phase 4 subsystems.
        /// </summary>
        public void PrintDiagnostics()
        {
            Console.WriteLine("\n=== Phase 4 Runtime Diagnostics ===\n");

            // Content Hosting Diagnostics
            var contentDiags = _dockingManager.ContentHosting.GetDiagnostics();
            Console.WriteLine("Content Hosting:");
            Console.WriteLine($"  Total Hosted: {contentDiags.TotalHostedControls}");
            Console.WriteLine($"  Visible: {contentDiags.VisibleControls}");
            Console.WriteLine($"  Hidden: {contentDiags.HiddenControls}");
            Console.WriteLine($"  Active: {contentDiags.ActiveContent ?? "(none)"}");
            foreach (var desc in contentDiags.HostedDescriptors)
            {
                Console.WriteLine($"    - {desc.PanelKey} ({desc.ControlType}) " +
                    $"[Visible: {desc.IsVisible}, Active: {desc.IsActive}]");
            }

            // Event Interceptor Diagnostics
            var eventDiags = _dockingManager.EventInterceptor.GetDiagnostics();
            Console.WriteLine("\nEvent Interceptor:");
            Console.WriteLine($"  Installed: {eventDiags.IsInstalled}");
            Console.WriteLine($"  Hooked Windows: {eventDiags.HookedWindows}");
            Console.WriteLine($"  Total Handlers: {eventDiags.TotalMessageHandlers}");

            // Tab Handler Diagnostics
            var tabDiags = _dockingManager.TabHandler.GetDiagnostics();
            Console.WriteLine("\nTab Interaction Handler:");
            Console.WriteLine($"  Total Tabs: {tabDiags.TotalTabs}");
            Console.WriteLine($"  Active Tab: {tabDiags.ActiveTab ?? "(none)"}");
            foreach (var stat in tabDiags.TabStats)
            {
                Console.WriteLine($"    - {stat.PanelKey}: '{stat.TabLabel}' " +
                    $"[Selected: {stat.IsSelected}, Clicks: {stat.ClickCount}]");
            }

            // Painter Integration Diagnostics
            var painterDiags = _dockingManager.PainterIntegration.GetDiagnostics();
            Console.WriteLine("\nPainter Integration:");
            Console.WriteLine($"  Rendering Enabled: {painterDiags.IsRenderingEnabled}");
            Console.WriteLine($"  Panels Registered: {painterDiags.PanelsRegistered}");
            Console.WriteLine($"  Cached Tab Bounds: {painterDiags.CachedTabBounds}");
            foreach (var stat in painterDiags.PanelStats)
            {
                Console.WriteLine($"    - {stat.PanelKey} at 0x{stat.WindowHandle:X8} " +
                    $"[Paints: {stat.PaintCount}, Dirty: {stat.IsDirty}]");
            }

            // Splitter Drag Handler Diagnostics
            var dragDiags = _dockingManager.DragHandler.GetDiagnostics();
            Console.WriteLine("\nSplitter Drag Handler:");
            Console.WriteLine($"  Is Dragging: {dragDiags.IsDragging}");
            if (dragDiags.IsDragging)
            {
                Console.WriteLine($"  Drag Start: {dragDiags.DragStartPoint}");
                Console.WriteLine($"  Drag Current: {dragDiags.DragCurrentPoint}");
                Console.WriteLine($"  Drag Delta: {dragDiags.DragDelta}");
            }

            Console.WriteLine("\n=== End Diagnostics ===\n");
        }

        // ===== Event Handlers =====

        private void OnTabClicked(object sender, TabClickedEventArgs e)
        {
            Console.WriteLine($"[Phase4Example] Tab clicked: '{e.TabLabel}' " +
                $"(was already active: {e.WasAlreadyActive})");
        }

        private void OnTabClosed(object sender, TabClosedEventArgs e)
        {
            Console.WriteLine($"[Phase4Example] Tab close requested: '{e.TabLabel}'");
            // Could prompt user for confirmation or perform cleanup here
        }

        private void OnTabDoubleClicked(object sender, TabDoubleClickedEventArgs e)
        {
            Console.WriteLine($"[Phase4Example] Tab double-clicked: '{e.TabLabel}'");
            // Could toggle floating window or maximize panel here
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            Console.WriteLine($"[Phase4Example] Splitter drag started at {e.StartPoint} " +
                $"({e.SplitterOrientation})");
        }

        private void OnDragUpdated(object sender, DragUpdatedEventArgs e)
        {
            Console.WriteLine($"[Phase4Example] Splitter drag updated: " +
                $"delta={e.DragDelta}, " +
                $"left/top={e.NewLeftOrTopSize}, " +
                $"right/bottom={e.NewRightOrBottomSize}");
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Console.WriteLine($"[Phase4Example] Splitter drag completed at {e.EndPoint}, " +
                $"new split ratio: {e.NewSplitRatio}%");
        }

        // ===== Helper Methods =====

        private Control CreateTreeViewPanel()
        {
            var treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                Name = "ProjectTree"
            };

            treeView.Nodes.Add("Project 1");
            treeView.Nodes[0].Nodes.Add("File 1");
            treeView.Nodes[0].Nodes.Add("File 2");
            treeView.Nodes.Add("Project 2");

            return treeView;
        }

        private Control CreatePropertiesPanel()
        {
            var propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                Name = "PropertiesGrid"
            };

            // Would normally bind to selected object
            return propertyGrid;
        }

        private Control CreateToolbarPanel()
        {
            var toolStrip = new ToolStrip
            {
                Name = "MainToolbar",
                AutoSize = true
            };

            toolStrip.Items.Add("New");
            toolStrip.Items.Add("Open");
            toolStrip.Items.Add("Save");
            toolStrip.Items.Add("-");
            toolStrip.Items.Add("Cut");
            toolStrip.Items.Add("Copy");
            toolStrip.Items.Add("Paste");

            return toolStrip;
        }

        private Control CreateDocumentPanel()
        {
            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Name = "DocumentArea",
                Text = "Document content goes here..."
            };

            return richTextBox;
        }

        /// <summary>
        /// Disposes the example and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _dockingManager?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Usage example showing how to use Phase 4 runtime capabilities in an application.
    /// </summary>
    public static class Phase4ExampleUsage
    {
        /// <summary>
        /// Example: Create a docked application with interactive panels.
        /// </summary>
        public static void RunExample()
        {
            // Create host form
            var mainForm = new Form
            {
                Text = "Phase 4 Runtime Example",
                Size = new Size(1000, 700),
                StartPosition = FormStartPosition.CenterScreen
            };

            // Create and initialize example
            var example = new Phase4RuntimeExample();
            example.Initialize(mainForm);

            // Add example panels
            example.AddExamplePanels();

            // Print diagnostics after adding panels
            example.PrintDiagnostics();

            // Show form
            mainForm.Show();

            // Simulate some user interactions
            mainForm.FormClosed += (s, e) =>
            {
                example.Dispose();
            };

            // Example interaction sequence (would normally be driven by user actions):
            /*
            // Activate a panel
            example.ActivatePanel("LeftPanel");

            // Hide a panel
            example.HidePanel("TopPanel");

            // Show it again
            example.ShowPanel("TopPanel");

            // Print diagnostics
            example.PrintDiagnostics();

            // Remove a panel
            example.RemovePanel("RightPanel");
            */
        }
    }
}
