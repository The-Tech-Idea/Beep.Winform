using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests;

public class BeepDockingManagerDesignTimeTests
{
    [Fact]
    public void ManageControl_RegistersDesignerCreatedPanelsAlreadyOnHost()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var manager = new BeepDockingManager();
        var panel = new DockPanel
        {
            Key = "solutionExplorer",
            Title = "Solution Explorer",
            DockPosition = DockPosition.Left,
            PreferredWidth = 220,
            Manager = manager
        };

        form.Controls.Add(panel);
        manager.ManageControl(form);

        Assert.Equal(1, manager.PanelCount);
        Assert.Same(panel, manager.GetPanel("solutionExplorer"));
        Assert.Contains(panel, manager.GetPanelsAtPosition(DockPosition.Left));
        Assert.True(panel.LayoutBounds.Width >= 219 && panel.LayoutBounds.Width <= 221,
            $"Expected width ~220, got {panel.LayoutBounds.Width}");
        Assert.True(panel.LayoutBounds.Height == 600, $"Expected height 600, got {panel.LayoutBounds.Height}");
    }

    [Fact]
    public void DockPanel_ManagerAssignmentAfterHostRegistration_RegistersPanel()
    {
        using var form = new Form { ClientSize = new Size(900, 600) };
        using var manager = new BeepDockingManager();
        var panel = new DockPanel
        {
            Key = "properties",
            Title = "Properties",
            DockPosition = DockPosition.Right,
            PreferredWidth = 260,
            Manager = manager
        };

        form.Controls.Add(panel);
        manager.ManageControl(form);

        Assert.Equal(1, manager.PanelCount);
        Assert.Same(panel, manager.GetPanel("properties"));
        Assert.True(manager.GetPanelsAtPosition(DockPosition.Right).Any(p => ReferenceEquals(p, panel)));
        Assert.True(panel.LayoutBounds.Width >= 258 && panel.LayoutBounds.Width <= 262,
            $"Expected width ~260, got {panel.LayoutBounds.Width}");
        Assert.True(panel.LayoutBounds.Height == 600, $"Expected height 600, got {panel.LayoutBounds.Height}");
    }
}
