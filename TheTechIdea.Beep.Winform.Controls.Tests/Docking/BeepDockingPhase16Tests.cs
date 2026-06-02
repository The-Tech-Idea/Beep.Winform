using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests;

/// <summary>
/// Behavioral tests for the Phase 16 UX additions:
/// <list type="bullet">
///   <item><c>ActiveAutoHideContent</c> on the manager (auto-hide slide-in focus)</item>
///   <item><c>ShowSnapGuides</c> on the manager + the <see cref="DockingGuideOverlay"/> snap line</item>
/// </list>
/// </summary>
public class BeepDockingPhase16Tests
{
    [Fact]
    public void ActiveAutoHideContent_DefaultsToTrue_CanBeTurnedOff()
    {
        using var manager = new BeepDockingManager();
        Assert.True(manager.ActiveAutoHideContent);
        manager.ActiveAutoHideContent = false;
        Assert.False(manager.ActiveAutoHideContent);
    }

    [Fact]
    public void AutoHideSlidePanel_HostedPanel_IsSetAfterShow()
    {
        using var slide = new AutoHideSlidePanel(DockPosition.Left);
        using var panel = new DockPanel
        {
            Key = "tools",
            Title = "Tools",
            PreferredWidth = 200,
            DockPosition = DockPosition.Left
        };

        slide.Show(panel);

        Assert.Same(panel, slide.HostedPanel);

        // Subscribe/unsubscribe the SlideShown event to confirm the event surface is wired.
        EventHandler<DockPanel> handler = (_, p) => { _ = p; };
        slide.SlideShown += handler;
        slide.SlideShown -= handler;
    }

    [Fact]
    public void ShowSnapGuides_DefaultsToTrue_CanBeTurnedOff()
    {
        using var manager = new BeepDockingManager();
        Assert.True(manager.ShowSnapGuides);
        manager.ShowSnapGuides = false;
        Assert.False(manager.ShowSnapGuides);
    }

    [Fact]
    public void DockingGuideOverlay_ShowSnapGuide_AcceptsRectAndOrientation()
    {
        using var overlay = new DockingGuideOverlay();
        // Pass-through: should not throw with any non-empty rect + orientation.
        overlay.ShowSnapGuide(new Rectangle(50, 60, 200, 100), DockPosition.Left);
        overlay.ShowSnapGuide(new Rectangle(10, 10, 100, 4), DockPosition.Top);
        overlay.ShowSnapGuide(Rectangle.Empty, DockPosition.Fill);
        overlay.ClearSnapGuide();
    }
}
