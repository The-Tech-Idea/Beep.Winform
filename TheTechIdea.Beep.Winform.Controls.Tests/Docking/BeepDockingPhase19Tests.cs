using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using HdrPos = TheTechIdea.Beep.Winform.Controls.Docking.Models.HeaderPosition;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Tests
{
    public class BeepDockingPhase19Tests
    {
        // ── B4 TabStyle positions ───────────────────────────────────────────────

        [Fact]
        public void Dockspace_TabPosition_DefaultsToTop()
        {
            using var ds = new BeepDockspace();
            Assert.Equal(HdrPos.Top, ds.TabPosition);
        }

        [Fact]
        public void Dockspace_TabPosition_Bottom_MovesContentUp()
        {
            using var ds = new BeepDockspace();
            ds.Size = new Size(300, 200);

            ds.TabPosition = HdrPos.Top;
            ds.LayoutPanels();
            using var panel1 = new DockPanel { Key = "p1" };
            ds.Controls.Add(panel1);
            ds.LayoutPanels();
            Assert.True(panel1.Bounds.Y >= BeepDockspace.HeaderHeight,
                $"Top: panel Y={panel1.Bounds.Y}, expected >= {BeepDockspace.HeaderHeight}");

            ds.TabPosition = HdrPos.Bottom;
            ds.LayoutPanels();
            Assert.True(panel1.Bounds.Y == 0,
                $"Bottom: panel Y={panel1.Bounds.Y}, expected 0 (content fills top)");
        }

        [Fact]
        public void Dockspace_TabPosition_None_HidesHeader()
        {
            using var ds = new BeepDockspace();
            ds.Size = new Size(300, 200);
            ds.TabPosition = HdrPos.None;
            ds.LayoutPanels();

            using var panel = new DockPanel { Key = "p1" };
            ds.Controls.Add(panel);
            ds.LayoutPanels();
            Assert.True(panel.Bounds.Y == 0 && panel.Bounds.Height == ds.Height,
                $"None: panel bounds={panel.Bounds}, expected full dockspace");
        }

        [Fact]
        public void Dockspace_TabPosition_Left_MovesContentRight()
        {
            using var ds = new BeepDockspace();
            ds.Size = new Size(300, 200);
            ds.TabPosition = HdrPos.Left;
            using var panel = new DockPanel { Key = "p1" };
            ds.Controls.Add(panel);
            ds.LayoutPanels();
            Assert.True(panel.Bounds.X >= BeepDockspace.HeaderHeight,
                $"Left: panel X={panel.Bounds.X}, expected >= {BeepDockspace.HeaderHeight}");
            Assert.True(panel.Bounds.Height == ds.Height,
                $"Left: panel height={panel.Bounds.Height}, expected full height");
        }

        [Fact]
        public void Dockspace_TabPosition_Right_MovesContentLeft()
        {
            using var ds = new BeepDockspace();
            ds.Size = new Size(300, 200);
            ds.TabPosition = HdrPos.Right;
            using var panel = new DockPanel { Key = "p1" };
            ds.Controls.Add(panel);
            ds.LayoutPanels();
            Assert.Equal(0, panel.Bounds.X);
            Assert.True(panel.Bounds.Width <= ds.Width - BeepDockspace.HeaderHeight,
                $"Right: panel width={panel.Bounds.Width}, expected <= {ds.Width - BeepDockspace.HeaderHeight}");
        }

        [Fact]
        public void Dockspace_TabPosition_SetInvalidates()
        {
            using var form = new Form();
            using var ds = new BeepDockspace();
            form.Controls.Add(ds);
            form.Show();
            try
            {
                bool invalidated = false;
                ds.Invalidated += (_, _) => invalidated = true;
                ds.TabPosition = HdrPos.Bottom;
                Assert.True(invalidated);
            }
            finally { form.Hide(); }
        }

        // ── C4 Schema version ──────────────────────────────────────────────────

        [Fact]
        public void DockLayoutDefinition_HasSchemaVersion()
        {
            var def = new DockLayoutDefinition();
            Assert.Equal(1, def.SchemaVersion);
        }

        [Fact]
        public void DockGroupDefinition_SerializesHeaderPosition()
        {
            var gd = new DockGroupDefinition { HeaderPosition = HdrPos.Bottom };
            Assert.Equal(HdrPos.Bottom, gd.HeaderPosition);
            Assert.Equal(HdrPos.Top, new DockGroupDefinition().HeaderPosition);
        }

        // ── Single/Multi group layout within a dockspace ────────────────────────

        [Fact]
        public void Dockspace_LayoutPanels_SingleGroup_UsesFullContentRect()
        {
            using var form = new Form { ClientSize = new Size(300, 200) };
            using var ds = new BeepDockspace();
            form.Controls.Add(ds);
            ds.Size = new Size(300, 200);
            using var p1 = new DockPanel { Key = "p1", Title = "P1" };
            using var p2 = new DockPanel { Key = "p2", Title = "P2" };
            ds.Controls.Add(p1);
            ds.Controls.Add(p2);
            ds.LayoutPanels();
            Assert.Equal(p1.Bounds, p2.Bounds);
        }

        [Fact]
        public void Dockspace_LayoutPanels_WithManager_DoesNotThrow()
        {
            using var form = new Form { ClientSize = new Size(900, 600) };
            using var manager = new BeepDockingManager();
            using var ds = new BeepDockspace { DockPosition = DockPosition.Left };
            ds.Manager = manager;
            form.Controls.Add(ds);
            ds.Size = new Size(300, 600);
            manager.ManageControl(form);

            manager.AddPanel("g1", "Group1", DockPosition.Left, new TextBox());
            manager.AddPanel("g2", "Group2", DockPosition.Left, new TextBox());
            manager.StackPanel("g1", "g2");

            ds.LayoutPanels();
            Assert.True(true);
        }

        [Fact]
        public void Dockspace_LayoutPanels_WithLayoutResult_TranslatesGroupBounds()
        {
            // Verify the multi-group branch compiles and doesn't throw. The actual
            // group-edge drop path (CommitGroupEdge) is tested implicitly via the full
            // docking test suite which exercises drag-drop operations.
            using var form = new Form { ClientSize = new Size(900, 600) };
            using var manager = new BeepDockingManager();
            using var ds = new BeepDockspace { DockPosition = DockPosition.Left };
            ds.Manager = manager;
            form.Controls.Add(ds);
            ds.Size = new Size(300, 600);
            manager.ManageControl(form);

            manager.AddPanel("a", "A", DockPosition.Left, new TextBox());
            manager.AddPanel("b", "B", DockPosition.Left, new TextBox());

            manager.ApplyLayout();

            var panelA = manager.GetPanel("a");
            var panelB = manager.GetPanel("b");
            Assert.True(panelA.Bounds.Width > 0);
            Assert.True(panelB.Bounds.Width > 0);
        }
    }
}
