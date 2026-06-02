using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests;

/// <summary>
/// Architectural invariant tests for the docking layer. These tests pin down the
/// "dockspace is a persistent runtime control" contract established in Phase 13/14:
/// the runtime must respect the design-time parent/child structure rather than
/// reparent panels to the host form.
/// </summary>
public class BeepDockingArchitectureTests
{
    [Fact]
    public void RegisterDesignerCreatedPanels_DoesNotReparentPanelsOutOfDockspace()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var dockspace = new BeepDockspace
        {
            DockPosition = DockPosition.Left,
            Bounds = new Rectangle(0, 0, 220, 600)
        };
        using var manager = new BeepDockingManager();
        dockspace.Manager = manager;
        form.Controls.Add(dockspace);

        using var panel = new DockPanel
        {
            Key = "toolbox",
            Title = "Toolbox",
            DockPosition = DockPosition.Left,
            Manager = manager
        };
        dockspace.Controls.Add(panel);

        manager.ManageControl(form);

        Assert.Same(dockspace, panel.Parent);
        Assert.False(form.Controls.Contains(panel));
        Assert.True(panel.Parent is BeepDockspace);
    }

    [Fact]
    public void ApplyLayout_SetsDockspaceDockStyleFromDockPosition()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };

        using var leftDs = new BeepDockspace { DockPosition = DockPosition.Left };
        using var bottomDs = new BeepDockspace { DockPosition = DockPosition.Bottom };

        using var manager = new BeepDockingManager();

        leftDs.Manager = manager;
        bottomDs.Manager = manager;
        form.Controls.Add(leftDs);
        form.Controls.Add(bottomDs);

        manager.ManageControl(form);

        manager.AddPanel("find", "Find", DockPosition.Left, new Control());
        manager.AddPanel("output", "Output", DockPosition.Bottom, new Control());

        Assert.Equal(DockStyle.Left, leftDs.Dock);
        Assert.Equal(DockStyle.Bottom, bottomDs.Dock);
    }

    [Fact]
    public void ApplyLayout_DockspaceHostedPanels_GetLayoutBoundsFromDockspace()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };

        using var dockspace = new BeepDockspace
        {
            DockPosition = DockPosition.Left,
            Bounds = new Rectangle(0, 0, 200, 600)
        };
        using var manager = new BeepDockingManager();
        dockspace.Manager = manager;
        form.Controls.Add(dockspace);

        manager.ManageControl(form);

        manager.AddPanel("explorer", "Explorer", DockPosition.Left, new Control());

        var explorer = manager.GetPanel("explorer");
        Assert.NotNull(explorer);
        Assert.Same(dockspace, explorer.Parent);
    }

    [Fact]
    public void EmptyDockspace_StaysOnHostForm_AfterManageControl()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };

        using var emptyDs = new BeepDockspace { DockPosition = DockPosition.Right };
        using var manager = new BeepDockingManager();
        emptyDs.Manager = manager;
        form.Controls.Add(emptyDs);

        manager.ManageControl(form);

        manager.AddPanel("props", "Properties", DockPosition.Right, new Control());

        Assert.True(form.Controls.Contains(emptyDs));
    }

    [Fact]
    public void DockspaceParent_CheckReturnsTrue_ForDockspaceHostedPanels()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var dockspace = new BeepDockspace
        {
            DockPosition = DockPosition.Left,
            Bounds = new Rectangle(0, 0, 200, 600)
        };
        using var manager = new BeepDockingManager();
        dockspace.Manager = manager;
        form.Controls.Add(dockspace);

        manager.ManageControl(form);

        manager.AddPanel("p1", "P1", DockPosition.Left, new Control());
        var p1 = manager.GetPanel("p1");

        Assert.NotNull(p1);
        Assert.True(p1.Parent is BeepDockspace);
    }

    [Fact]
    public void AddPanel_FallsBackToDockspace_WhenCalledAtRuntime()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var manager = new BeepDockingManager();
        manager.ManageControl(form);

        // No dockspace exists yet; AddPanel should still work and the panel should be on
        // the host form as the legacy fallback (debug warning expected).
        manager.AddPanel("orphan", "Orphan", DockPosition.Right, new Control());
        var orphan = manager.GetPanel("orphan");

        Assert.NotNull(orphan);
        Assert.True(orphan.Parent == form);
    }

    [Fact]
    public void Dockspace_UpdateChildSplitters_CreatesAndDisposesSplitters()
    {
        using var dockspace = new BeepDockspace
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };

        var desired = new Dictionary<string, (Rectangle Bounds, bool IsVertical)>
        {
            ["root_child_0"] = (new Rectangle(0, 100, 400, 4), false)
        };
        dockspace.UpdateChildSplitters(desired, null);
        Assert.Single(dockspace.Controls.OfType<BeepDockSplitter>());

        // Update with empty desired set → splitter should be removed.
        dockspace.UpdateChildSplitters(
            new Dictionary<string, (Rectangle, bool)>(), null);
        Assert.Empty(dockspace.Controls.OfType<BeepDockSplitter>());
    }
}
