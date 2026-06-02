using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests;

/// <summary>
/// Behavioral tests for the Phase 15 UX feature-parity additions:
/// <list type="bullet">
///   <item>HideOnClose (panel-level)</item>
///   <item>ShowHint (dockspace-level tooltip gating)</item>
///   <item>AllowEndUserDocking (manager-level drag gate)</item>
///   <item>DockPadding (dockspace-level inner inset)</item>
///   <item>DefaultFloatWindowSize (manager-level fallback size)</item>
/// </list>
/// </summary>
public class BeepDockingPhase15Tests
{
    [Fact]
    public void ClosePanel_WithHideOnTrue_HidesPanelWithoutRemovingIt()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var manager = new BeepDockingManager();
        manager.ManageControl(form);

        manager.AddPanel("p1", "P1", DockPosition.Left, new Control());
        var p1 = manager.GetPanel("p1");
        p1.HideOnClose = true;

        manager.ClosePanel("p1");

        Assert.Equal(DockPanelState.Hidden, p1.State);
        Assert.False(p1.Visible);
        Assert.False(manager.IsPanelClosed("p1"));
        Assert.True(manager.GetAllPanels().Any(p => p.Key == "p1"));
    }

    [Fact]
    public void ShowHint_DefaultsToTrue_CanBeTurnedOff()
    {
        using var ds = new BeepDockspace();
        Assert.True(ds.ShowHint);
        ds.ShowHint = false;
        Assert.False(ds.ShowHint);
    }

    [Fact]
    public void AllowEndUserDocking_DefaultsToTrue_CanBeTurnedOff()
    {
        using var form = new Form();
        using var manager = new BeepDockingManager();
        manager.ManageControl(form);

        Assert.True(manager.AllowEndUserDocking);
        manager.AllowEndUserDocking = false;
        Assert.False(manager.AllowEndUserDocking);
    }

    [Fact]
    public void DockPadding_InsetsPanelBoundsInLayoutPanels()
    {
        using var ds = new BeepDockspace
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using var panel = new DockPanel { Key = "x", Title = "X" };
        ds.Controls.Add(panel);
        ds.DockPadding = new Padding(10, 5, 20, 15);
        ds.LayoutPanels();

        // 400 wide, 300 tall dockspace; top header takes HeaderHeight (≤24 by default).
        // After padding (H=30, V=20), the panel bounds should be inset accordingly.
        Assert.True(panel.Bounds.Width <= 370,
            $"Panel width {panel.Bounds.Width} should be ≤ 400 - 30 = 370");
        Assert.True(panel.Bounds.Height <= 280,
            $"Panel height {panel.Bounds.Height} should be ≤ 300 - 20 = 280");
        Assert.True(panel.Bounds.X >= 10);
        Assert.True(panel.Bounds.Y >= 5);
    }

    [Fact]
    public void DefaultFloatWindowSize_DefaultsToEmpty()
    {
        using var manager = new BeepDockingManager();
        Assert.Equal(Size.Empty, manager.DefaultFloatWindowSize);
        manager.DefaultFloatWindowSize = new Size(480, 320);
        Assert.Equal(new Size(480, 320), manager.DefaultFloatWindowSize);
    }
}
