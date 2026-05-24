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
        Assert.Equal(new Rectangle(0, 0, 220, 600), panel.LayoutBounds);
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
            PreferredWidth = 260
        };

        form.Controls.Add(panel);
        manager.ManageControl(form);

        panel.Manager = manager;

        Assert.Equal(1, manager.PanelCount);
        Assert.Same(panel, manager.GetPanel("properties"));
        Assert.True(manager.GetPanelsAtPosition(DockPosition.Right).Any(p => ReferenceEquals(p, panel)));
        Assert.Equal(new Rectangle(640, 0, 260, 600), panel.LayoutBounds);
    }
}
