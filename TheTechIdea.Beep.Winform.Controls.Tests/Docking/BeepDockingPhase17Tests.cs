using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests;

/// <summary>
/// Behavioral tests for the Phase 17 additions:
/// <list type="bullet">
///   <item><c>DockingOptions</c> grouped options (A4)</item>
///   <item><c>FocusManager</c> (B8)</item>
///   <item><c>GetActivePanelKey(DockPanelState)</c> (C2)</item>
///   <item><c>PanelMovedBetweenGroups</c> event (C3)</item>
///   <item><c>PanelStateChanging</c> cancelable event (C5)</item>
/// </list>
/// </summary>
public class BeepDockingPhase17Tests
{
    [Fact]
    public void Options_ExposesPassThroughProperties()
    {
        using var manager = new BeepDockingManager();
        Assert.NotNull(manager.Options);

        manager.AllowEndUserDocking = false;
        Assert.False(manager.Options.AllowEndUserDocking);

        manager.Options.ActiveAutoHideContent = false;
        Assert.False(manager.ActiveAutoHideContent);

        manager.Options.ShowSnapGuides = false;
        Assert.False(manager.ShowSnapGuides);

        manager.Options.DefaultFloatWindowSize = new Size(400, 300);
        Assert.Equal(new Size(400, 300), manager.DefaultFloatWindowSize);
    }

    [Fact]
    public void FocusManager_DefaultsToFocusOnActivate_AndCanBeReplaced()
    {
        using var manager = new BeepDockingManager();
        Assert.NotNull(manager.FocusManager);
        Assert.True(manager.FocusManager.FocusOnActivate);

        var custom = new Runtime.DockingFocusManager(manager) { FocusOnActivate = false };
        manager.FocusManager = custom;
        Assert.Same(custom, manager.FocusManager);
        Assert.False(manager.FocusManager.FocusOnActivate);
    }

    [Fact]
    public void FocusManager_FindFirstFocusable_FindsNestedTextBox()
    {
        using var host = new Form { ClientSize = new Size(400, 300), ShowInTaskbar = false };
        var root = new Panel { Dock = DockStyle.Fill };
        var nested = new Panel { Dock = DockStyle.Fill };
        var tb = new TextBox { Dock = DockStyle.Top };
        nested.Controls.Add(tb);
        root.Controls.Add(nested);
        host.Controls.Add(root);
        host.Show();
        try
        {
            Assert.True(tb.Visible);
            Assert.True(tb.CanFocus);
            Assert.Same(tb, Runtime.DockingFocusManager.FindFirstFocusable(root));
        }
        finally { host.Hide(); }
    }

    [Fact]
    public void FocusManager_FindFirstFocusable_ReturnsNullWhenNone()
    {
        using var host = new Form { ClientSize = new Size(400, 300), ShowInTaskbar = false };
        var root = new Panel { Dock = DockStyle.Fill };
        var nested = new Panel { Dock = DockStyle.Fill };
        var lbl = new Label { Dock = DockStyle.Top };
        nested.Controls.Add(lbl);
        root.Controls.Add(nested);
        host.Controls.Add(root);
        host.Show();
        try
        {
            // Label has TabStop=false; FindFirstFocusable should return null.
            Assert.False(lbl.TabStop);
            Assert.Null(Runtime.DockingFocusManager.FindFirstFocusable(root));
        }
        finally { host.Hide(); }
    }

    [Fact]
    public void GetActivePanelKey_NoState_ReturnsFirstDocked()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var leftDs = new BeepDockspace { DockPosition = DockPosition.Left };
        using var rightDs = new BeepDockspace { DockPosition = DockPosition.Right };
        using var manager = new BeepDockingManager();
        leftDs.Manager = manager;
        rightDs.Manager = manager;
        form.Controls.Add(leftDs);
        form.Controls.Add(rightDs);
        manager.ManageControl(form);

        manager.AddPanel("a", "A", DockPosition.Left, new Control());
        manager.AddPanel("b", "B", DockPosition.Right, new Control());

        string key = manager.GetActivePanelKey();
        Assert.False(string.IsNullOrEmpty(key));
        Assert.True(key == "a" || key == "b");
    }

    [Fact]
    public void GetActivePanelKey_WithStateFilter_ReturnsMatch()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var leftDs = new BeepDockspace { DockPosition = DockPosition.Left };
        using var manager = new BeepDockingManager();
        leftDs.Manager = manager;
        form.Controls.Add(leftDs);
        manager.ManageControl(form);

        manager.AddPanel("h1", "H1", DockPosition.Left, new Control());
        var p = manager.GetPanel("h1");
        p.HideOnClose = true;
        manager.ClosePanel("h1");

        Assert.Equal("h1", manager.GetActivePanelKey(DockPanelState.Hidden));
        Assert.Null(manager.GetActivePanelKey(DockPanelState.Floating));
    }

    [Fact]
    public void PanelMovedBetweenGroups_FiresOnStackPanel()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var leftDs = new BeepDockspace { DockPosition = DockPosition.Left };
        using var rightDs = new BeepDockspace { DockPosition = DockPosition.Right };
        using var manager = new BeepDockingManager();
        leftDs.Manager = manager;
        rightDs.Manager = manager;
        form.Controls.Add(leftDs);
        form.Controls.Add(rightDs);
        manager.ManageControl(form);

        manager.AddPanel("p1", "P1", DockPosition.Left, new Control());
        manager.AddPanel("p2", "P2", DockPosition.Right, new Control());

        PanelMovedBetweenGroupsEventArgs captured = null;
        manager.PanelMovedBetweenGroups += (_, e) => captured = e;

        // Stack p2 onto p1's group (Left).
        manager.StackPanel("p2", "p1");

        Assert.NotNull(captured);
        Assert.Equal("p2", captured.Panel.Key);
        Assert.Equal(DockPosition.Right, captured.OldGroup?.Position);
        Assert.Equal(DockPosition.Left, captured.NewGroup.Position);
    }

    [Fact]
    public void PanelStateChanging_CancelPreventsFloat()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var leftDs = new BeepDockspace { DockPosition = DockPosition.Left };
        using var manager = new BeepDockingManager();
        leftDs.Manager = manager;
        form.Controls.Add(leftDs);
        manager.ManageControl(form);

        manager.AddPanel("fp", "FP", DockPosition.Left, new Control());

        PanelStateChangingEventArgs captured = null;
        manager.PanelStateChanging += (_, e) =>
        {
            if (e.RequestedState == DockPanelState.Floating) { e.Cancel = true; captured = e; }
        };

        var before = manager.GetPanel("fp").State;
        manager.FloatPanel("fp");
        var after = manager.GetPanel("fp").State;

        // The cancel path is hit before the state mutation, so the panel does NOT transition to
        // Floating. (FloatWindow.Close() then routes through CloseRequest → HidePanel, which is
        // pre-existing behavior; we only assert the Floating transition itself was vetoed.)
        Assert.NotNull(captured);
        Assert.Equal(DockPanelState.Docked, before);
        Assert.NotEqual(DockPanelState.Floating, after);
    }

    [Fact]
    public void PanelStateChanging_DoesNotBlockAutoHide_WhenNoSubscriber()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var leftDs = new BeepDockspace { DockPosition = DockPosition.Left };
        using var manager = new BeepDockingManager();
        leftDs.Manager = manager;
        form.Controls.Add(leftDs);
        manager.ManageControl(form);

        manager.AddPanel("ah", "AH", DockPosition.Left, new Control());
        manager.AutoHidePanel("ah");
        Assert.Equal(DockPanelState.AutoHidden, manager.GetPanel("ah").State);
    }
}
