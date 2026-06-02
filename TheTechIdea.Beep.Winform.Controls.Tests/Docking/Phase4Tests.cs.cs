using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Xunit;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests.Phase4
{
    /// <summary>
    /// Unit tests for Phase 4 content hosting subsystem.
    /// Tests reparenting, lifecycle, and state management.
    /// </summary>
    public class ContentHostingTests
    {
        [Fact]
        public void HostContent_ValidInput_SucceedsAndTracksControl()
        {
            // Arrange
            var hosting = new ContentHosting();
            var testControl = new Label { Text = "Test Content" };
            var windowHandle = new IntPtr(12345);
            var panelKey = "test-panel";

            // Act
            var result = hosting.HostContent(panelKey, testControl, windowHandle);

            // Assert
            Assert.True(result);
            var hosted = hosting.GetHostedContent(panelKey);
            Assert.NotNull(hosted);
            Assert.Equal(panelKey, hosted.PanelKey);
            Assert.Same(testControl, hosted.ContentControl);
            Assert.Equal(windowHandle, hosted.ChildWindowHandle);
        }

        [Fact]
        public void HostContent_NullControl_ThrowsArgumentNullException()
        {
            // Arrange
            var hosting = new ContentHosting();
            var windowHandle = new IntPtr(12345);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                hosting.HostContent("test-panel", null, windowHandle)
            );
        }

        [Fact]
        public void HostContent_ZeroWindowHandle_ThrowsArgumentException()
        {
            // Arrange
            var hosting = new ContentHosting();
            var testControl = new Label();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                hosting.HostContent("test-panel", testControl, IntPtr.Zero)
            );
        }

        [Fact]
        public void UnhostContent_ValidPanel_SucceedsAndRemovesTracking()
        {
            // Arrange
            var hosting = new ContentHosting();
            var testControl = new Label();
            var windowHandle = new IntPtr(12345);
            var panelKey = "test-panel";

            hosting.HostContent(panelKey, testControl, windowHandle);

            // Act
            var result = hosting.UnhostContent(panelKey);

            // Assert
            Assert.True(result);
            Assert.Null(hosting.GetHostedContent(panelKey));
        }

        [Fact]
        public void SetActiveContent_ValidPanel_ActivatesIt()
        {
            // Arrange
            var hosting = new ContentHosting();
            var control1 = new Label();
            var control2 = new Label();
            hosting.HostContent("panel1", control1, new IntPtr(111));
            hosting.HostContent("panel2", control2, new IntPtr(222));

            // Act
            hosting.SetActiveContent("panel2");

            // Assert
            Assert.False(hosting.GetHostedContent("panel1").IsActive);
            Assert.True(hosting.GetHostedContent("panel2").IsActive);
        }

        [Fact]
        public void HideContent_ValidPanel_HidesIt()
        {
            // Arrange
            var hosting = new ContentHosting();
            var control = new Label { Visible = true };
            hosting.HostContent("test-panel", control, new IntPtr(111));

            // Act
            var result = hosting.HideContent("test-panel");

            // Assert
            Assert.True(result);
            Assert.False(hosting.GetHostedContent("test-panel").IsVisible);
        }

        [Fact]
        public void GetDiagnostics_MultipleHostedControls_ReturnsAccurateStats()
        {
            // Arrange
            var hosting = new ContentHosting();
            hosting.HostContent("panel1", new Label(), new IntPtr(111));
            hosting.HostContent("panel2", new Label(), new IntPtr(222));
            hosting.HideContent("panel2");
            hosting.SetActiveContent("panel1");

            // Act
            var diags = hosting.GetDiagnostics();

            // Assert
            Assert.Equal(2, diags.TotalHostedControls);
            Assert.Equal(1, diags.VisibleControls);
            Assert.Equal(1, diags.HiddenControls);
            Assert.Equal("panel1", diags.ActiveContent);
        }

        [Fact]
        public void Dispose_SucceedsAndClearsTracking()
        {
            // Arrange
            var hosting = new ContentHosting();
            hosting.HostContent("panel1", new Label(), new IntPtr(111));

            // Act
            hosting.Dispose();

            // Assert
            Assert.Empty(hosting.GetAllHostedContent());
        }
    }

    /// <summary>
    /// Unit tests for Phase 4 event interception subsystem.
    /// Tests message filter installation and hook management.
    /// </summary>
    public class EventInterceptorTests
    {
        [Fact]
        public void Install_SucceedsAndSetsFlag()
        {
            // Arrange
            var interceptor = new EventInterceptor();

            // Act
            interceptor.Install();

            // Assert
            var diags = interceptor.GetDiagnostics();
            Assert.True(diags.IsInstalled);

            // Cleanup
            interceptor.Dispose();
        }

        [Fact]
        public void HookWindow_ValidWindow_RegistersHook()
        {
            // Arrange
            var interceptor = new EventInterceptor();
            interceptor.Install();
            var windowHandle = new IntPtr(12345);
            var messageHandled = false;

            void TestHandler(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
            {
                messageHandled = true;
            }

            // Act
            interceptor.HookWindow(windowHandle, 0x0201, TestHandler);  // WM_LBUTTONDOWN

            // Assert
            var diags = interceptor.GetDiagnostics();
            Assert.Equal(1, diags.HookedWindows);
            Assert.Equal(1, diags.TotalMessageHandlers);

            // Cleanup
            interceptor.Dispose();
        }

        [Fact]
        public void UnhookWindow_ValidWindow_RemovesAllHooks()
        {
            // Arrange
            var interceptor = new EventInterceptor();
            interceptor.Install();
            var windowHandle = new IntPtr(12345);

            void TestHandler(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam) { }

            interceptor.HookWindow(windowHandle, 0x0201, TestHandler);

            // Act
            interceptor.UnhookWindow(windowHandle);

            // Assert
            var diags = interceptor.GetDiagnostics();
            Assert.Equal(0, diags.HookedWindows);

            // Cleanup
            interceptor.Dispose();
        }

        [Fact]
        public void GetDiagnostics_MultipleHooks_ReturnsAccurateStats()
        {
            // Arrange
            var interceptor = new EventInterceptor();
            interceptor.Install();

            void TestHandler(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam) { }

            interceptor.HookWindow(new IntPtr(111), 0x0201, TestHandler);
            interceptor.HookWindow(new IntPtr(111), 0x0200, TestHandler);  // WM_MOUSEMOVE
            interceptor.HookWindow(new IntPtr(222), 0x0201, TestHandler);

            // Act
            var diags = interceptor.GetDiagnostics();

            // Assert
            Assert.True(diags.IsInstalled);
            Assert.Equal(2, diags.HookedWindows);
            Assert.Equal(3, diags.TotalMessageHandlers);

            // Cleanup
            interceptor.Dispose();
        }

        [Fact]
        public void Dispose_UninstallsAndClearsHooks()
        {
            // Arrange
            var interceptor = new EventInterceptor();
            interceptor.Install();

            void TestHandler(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam) { }

            interceptor.HookWindow(new IntPtr(111), 0x0201, TestHandler);

            // Act
            interceptor.Dispose();

            // Assert
            // After dispose, hooks should be cleared (verified by internal state)
            // No exceptions should be thrown
        }
    }

    /// <summary>
    /// Unit tests for Phase 4 tab interaction subsystem.
    /// Tests tab selection, double-click detection, and state management.
    /// </summary>
    public class TabInteractionHandlerTests
    {
        [Fact]
        public void RegisterTab_NewTab_SucceedsAndTracksState()
        {
            // Arrange
            var handler = new TabInteractionHandler(null, new DockLayoutTree());
            var panelKey = "panel1";

            // Act
            handler.RegisterTab(panelKey, "Tab Label");

            // Assert
            var tabState = handler.GetTabState(panelKey);
            Assert.NotNull(tabState);
            Assert.Equal(panelKey, tabState.PanelKey);
            Assert.Equal("Tab Label", tabState.TabLabel);
            Assert.False(tabState.IsSelected);

            // Cleanup
            handler.Dispose();
        }

        [Fact]
        public void UnregisterTab_ExistingTab_RemovesTracking()
        {
            // Arrange
            var handler = new TabInteractionHandler(null, new DockLayoutTree());
            handler.RegisterTab("panel1", "Tab 1");

            // Act
            handler.UnregisterTab("panel1");

            // Assert
            Assert.Null(handler.GetTabState("panel1"));

            // Cleanup
            handler.Dispose();
        }

        [Fact]
        public void UpdateTabLabel_ExistingTab_UpdatesLabel()
        {
            // Arrange
            var handler = new TabInteractionHandler(null, new DockLayoutTree());
            handler.RegisterTab("panel1", "Old Label");

            // Act
            var result = handler.UpdateTabLabel("panel1", "New Label");

            // Assert
            Assert.True(result);
            Assert.Equal("New Label", handler.GetTabState("panel1").TabLabel);

            // Cleanup
            handler.Dispose();
        }

        [Fact]
        public void GetDiagnostics_MultipleTabs_ReturnsAccurateStats()
        {
            // Arrange
            var handler = new TabInteractionHandler(null, new DockLayoutTree());
            handler.RegisterTab("panel1", "Tab 1");
            handler.RegisterTab("panel2", "Tab 2");
            handler.RegisterTab("panel3", "Tab 3");

            // Act
            var diags = handler.GetDiagnostics();

            // Assert
            Assert.Equal(3, diags.TotalTabs);
            Assert.Null(diags.ActiveTab);  // No active tab yet
            Assert.Equal(3, diags.TabStats.Count);

            // Cleanup
            handler.Dispose();
        }

        [Fact]
        public void Dispose_ClearsAllTabs()
        {
            // Arrange
            var handler = new TabInteractionHandler(null, new DockLayoutTree());
            handler.RegisterTab("panel1", "Tab 1");
            handler.RegisterTab("panel2", "Tab 2");

            // Act
            handler.Dispose();

            // Assert
            var allTabs = handler.GetAllTabStates();
            Assert.Empty(allTabs);
        }
    }

    /// <summary>
    /// Unit tests for Phase 4 painter integration subsystem.
    /// Tests render context management and tab bounds caching.
    /// </summary>
    public class PainterIntegrationTests
    {
        [Fact]
        public void RegisterPanelForRendering_ValidPanel_CreatesRenderContext()
        {
            // Arrange
            var painter = new MockDockingPainter();
            var chrome = new WindowChrome(painter);
            var integration = new PainterIntegration(painter, chrome);
            var windowHandle = new IntPtr(111);
            var group = new DockGroup();
            var bounds = new Rectangle(0, 0, 400, 300);

            // Act
            integration.RegisterPanelForRendering("panel1", windowHandle, group, bounds);

            // Assert
            var context = integration.GetRenderContext(windowHandle);
            Assert.NotNull(context);
            Assert.Equal("panel1", context.PanelKey);
            Assert.Equal(bounds, context.TotalBounds);

            // Cleanup
            integration.Dispose();
        }

        [Fact]
        public void InvalidatePanel_ExistingPanel_SetsDirtyFlag()
        {
            // Arrange
            var painter = new MockDockingPainter();
            var chrome = new WindowChrome(painter);
            var integration = new PainterIntegration(painter, chrome);
            var windowHandle = new IntPtr(111);
            var group = new DockGroup();

            integration.RegisterPanelForRendering("panel1", windowHandle, group, new Rectangle(0, 0, 400, 300));

            // Act
            integration.InvalidatePanel(windowHandle);

            // Assert
            var context = integration.GetRenderContext(windowHandle);
            Assert.True(context.IsDirty);

            // Cleanup
            integration.Dispose();
        }

        [Fact]
        public void UpdatePanelBounds_ExistingPanel_UpdatesBoundsAndInvalidates()
        {
            // Arrange
            var painter = new MockDockingPainter();
            var chrome = new WindowChrome(painter);
            var integration = new PainterIntegration(painter, chrome);
            var windowHandle = new IntPtr(111);
            var group = new DockGroup();
            var oldBounds = new Rectangle(0, 0, 400, 300);
            var newBounds = new Rectangle(0, 0, 500, 350);

            integration.RegisterPanelForRendering("panel1", windowHandle, group, oldBounds);

            // Act
            integration.UpdatePanelBounds(windowHandle, newBounds);

            // Assert
            var context = integration.GetRenderContext(windowHandle);
            Assert.Equal(newBounds, context.TotalBounds);
            Assert.True(context.IsDirty);

            // Cleanup
            integration.Dispose();
        }

        [Fact]
        public void GetDiagnostics_MultiplePanels_ReturnsAccurateStats()
        {
            // Arrange
            var painter = new MockDockingPainter();
            var chrome = new WindowChrome(painter);
            var integration = new PainterIntegration(painter, chrome);
            var group = new DockGroup();

            integration.RegisterPanelForRendering("panel1", new IntPtr(111), group, new Rectangle(0, 0, 200, 200));
            integration.RegisterPanelForRendering("panel2", new IntPtr(222), group, new Rectangle(200, 0, 200, 200));

            // Act
            var diags = integration.GetDiagnostics();

            // Assert
            Assert.True(diags.IsRenderingEnabled);
            Assert.Equal(2, diags.PanelsRegistered);
            Assert.Equal(2, diags.PanelStats.Count);

            // Cleanup
            integration.Dispose();
        }

        [Fact]
        public void SetRenderingEnabled_DisabledFlag_DisablesRendering()
        {
            // Arrange
            var painter = new MockDockingPainter();
            var chrome = new WindowChrome(painter);
            var integration = new PainterIntegration(painter, chrome);

            // Act
            integration.SetRenderingEnabled(false);

            // Assert
            var diags = integration.GetDiagnostics();
            Assert.False(diags.IsRenderingEnabled);

            // Cleanup
            integration.Dispose();
        }

        [Fact]
        public void Dispose_SucceedsAndClearsContexts()
        {
            // Arrange
            var painter = new MockDockingPainter();
            var chrome = new WindowChrome(painter);
            var integration = new PainterIntegration(painter, chrome);
            var group = new DockGroup();

            integration.RegisterPanelForRendering("panel1", new IntPtr(111), group, new Rectangle(0, 0, 200, 200));

            // Act
            integration.Dispose();

            // Assert
            var diags = integration.GetDiagnostics();
            Assert.Equal(0, diags.PanelsRegistered);
        }
    }

    /// <summary>
    /// Mock painter for testing.
    /// </summary>
    public class MockDockingPainter : IDockingPainter
    {
        public void DrawTabStrip(System.Drawing.Graphics graphics, Rectangle bounds, DockGroup group) { }
        public void DrawChrome(System.Drawing.Graphics graphics, Rectangle bounds) { }
        public void DrawSplitter(System.Drawing.Graphics graphics, Rectangle bounds, System.Windows.Forms.Orientation orientation) { }
        public void Dispose() { }
    }
}
