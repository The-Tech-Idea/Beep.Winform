using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class DocumentManagerDesignTimeRegressionTests
    {
        [Fact]
        public void TabbedView_AddDocumentDescriptor_PreservesDescriptorIdentityAndState()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };

            var descriptor = new DocumentDescriptor
            {
                Id = "doc-from-descriptor",
                Title = "Descriptor Document",
                CanClose = false,
                IsModified = true
            };

            var panel = view.AddDocument(descriptor);

            Assert.Same(panel, host.GetPanel("doc-from-descriptor"));
            Assert.Equal("Descriptor Document", panel!.DocumentTitle);
            Assert.False(panel.CanClose);
            Assert.True(panel.IsModified);
        }

        [Fact]
        public void ManagerDesignTimeDocuments_MutationsApplyImmediatelyToTabbedHost()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };
            using var manager = new BeepDocumentManager();
            manager.Site = new DesignModeSite();
            manager.View = view;

            var descriptor = new DocumentDescriptor
            {
                Id = "manager-doc",
                Title = "Manager Document"
            };

            manager.DesignTimeDocuments.Add(descriptor);

            Assert.NotNull(host.GetPanel("manager-doc"));

            descriptor.Title = "Renamed Manager Document";

            Assert.Equal("Renamed Manager Document", host.GetPanel("manager-doc")!.DocumentTitle);

            manager.DesignTimeDocuments.Remove(descriptor);

            Assert.Null(host.GetPanel("manager-doc"));
            Assert.Equal(0, host.DocumentCount);
        }

        [Fact]
        public void ManagerDesignTimeDocuments_ApplyWhenViewIsAssignedAfterDocuments()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };
            using var manager = new BeepDocumentManager();
            manager.Site = new DesignModeSite();

            manager.DesignTimeDocuments.Add(new DocumentDescriptor
            {
                Id = "deferred-doc",
                Title = "Deferred Document",
                IsPinned = true
            });

            manager.View = view;

            var panel = host.GetPanel("deferred-doc");
            Assert.NotNull(panel);
            Assert.Equal("Deferred Document", panel!.DocumentTitle);
        }

        [Fact]
        public void ManagerDesignTimeDocuments_BatchedDesignerUpdateAppliesOnceOnEnd()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };
            using var manager = new BeepDocumentManager();
            manager.Site = new DesignModeSite();
            manager.View = view;

            manager.BeginDesignTimeDocumentUpdate();
            manager.DesignTimeDocuments.Add(new DocumentDescriptor
            {
                Id = "batched-doc",
                Title = "Batched Document"
            });

            Assert.Null(host.GetPanel("batched-doc"));

            manager.EndDesignTimeDocumentUpdate(applyChanges: true);

            Assert.NotNull(host.GetPanel("batched-doc"));
            Assert.Equal(1, host.DocumentCount);
        }

        [Fact]
        public void ManagerRefreshDesignTimeDocuments_DoesNotRemoveRuntimeDocumentsOutsideDesigner()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };
            using var manager = new BeepDocumentManager { View = view };

            host.AddDocument("runtime-doc", "Runtime Document", iconPath: null, activate: false);
            manager.DesignTimeDocuments.Add(new DocumentDescriptor
            {
                Id = "seed-doc",
                Title = "Seed Document"
            });

            manager.RefreshDesignTimeDocuments();

            Assert.NotNull(host.GetPanel("runtime-doc"));
            Assert.NotNull(host.GetPanel("seed-doc"));
        }

        [Fact]
        public void ManagerAddDocument_RaisesDocumentAddedThroughTabbedView()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };
            using var manager = new BeepDocumentManager { View = view };

            DocumentAddedEventArgs? raised = null;
            manager.DocumentAdded += (_, e) => raised = e;

            var panel = manager.AddDocument("Runtime Document", iconPath: "runtime.svg", activate: false);

            Assert.NotNull(panel);
            Assert.NotNull(raised);
            Assert.Same(panel, raised!.Panel);
            Assert.Equal("Runtime Document", raised.Descriptor.Title);
            Assert.Equal("runtime.svg", raised.Descriptor.IconPath);
        }

        [Fact]
        public void ManagerTabbedView_HostAssignedAfterView_ReplaysSettingsAndDesignTimeDocuments()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView();
            using var manager = new BeepDocumentManager
            {
                AutoSaveLayout = true,
                SessionFile = "%LocalAppData%\\Beep\\late-host.json",
                ThemeName = "LateHostTheme",
                DefaultPolicy = new BeepDocumentPolicy
                {
                    AllowFloat = false,
                    AllowPin = false,
                    AllowSplit = false,
                    MaxSplitDepth = 2
                }
            };
            manager.Site = new DesignModeSite();
            manager.DesignTimeDocuments.Add(new DocumentDescriptor
            {
                Id = "late-host-doc",
                Title = "Late Host Document"
            });

            manager.View = view;

            Assert.Null(host.GetPanel("late-host-doc"));

            view.Host = host;

            Assert.NotNull(host.GetPanel("late-host-doc"));
            Assert.Equal("Late Host Document", host.GetPanel("late-host-doc")!.DocumentTitle);
            Assert.True(host.AutoSaveLayout);
            Assert.EndsWith("Beep\\late-host.json", host.SessionFile);
            Assert.Equal("LateHostTheme", host.ThemeName);
            Assert.False(host.AllowFloat);
            Assert.False(host.AllowPin);
            Assert.False(host.AllowSplit);
            Assert.Equal(2, host.MaxSplitDepth);
        }

        [Fact]
        public void WindowMenuText_UpdatesExistingLiveWindowMenu()
        {
            using var host = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = host };
            using var menu = new MenuStrip();
            using var manager = new BeepDocumentManager { View = view, WindowMenuOwner = menu };

            Assert.Contains(menu.Items.Cast<ToolStripItem>(), item => item.Text == "&Window");

            manager.WindowMenuText = "&Documents";

            Assert.DoesNotContain(menu.Items.Cast<ToolStripItem>(), item => item.Text == "&Window");
            Assert.Contains(menu.Items.Cast<ToolStripItem>(), item => item.Text == "&Documents");
        }

        [Fact]
        public void ManagerTabbedView_HostReassigned_MovesDesignTimeDocumentsAndDetachesOldHost()
        {
            using var firstHost = new BeepDocumentHost();
            using var secondHost = new BeepDocumentHost();
            using var view = new BeepTabbedView { Host = firstHost };
            using var manager = new BeepDocumentManager();
            manager.Site = new DesignModeSite();
            manager.DesignTimeDocuments.Add(new DocumentDescriptor
            {
                Id = "reassigned-doc",
                Title = "Reassigned Document"
            });
            manager.View = view;

            Assert.NotNull(firstHost.GetPanel("reassigned-doc"));

            view.Host = secondHost;

            Assert.Null(firstHost.GetPanel("reassigned-doc"));
            Assert.NotNull(secondHost.GetPanel("reassigned-doc"));

            int addedEvents = 0;
            manager.DocumentAdded += (_, _) => addedEvents++;

            firstHost.OpenDocument(DocumentDescriptor.Create("old-host-runtime", "Old Host Runtime"));
            secondHost.OpenDocument(DocumentDescriptor.Create("new-host-runtime", "New Host Runtime"));

            Assert.Equal(1, addedEvents);
        }

        private sealed class DesignModeSite : ISite
        {
            public IComponent? Component => null;
            public IContainer? Container => null;
            public bool DesignMode => true;
            public string? Name { get; set; }

            public object? GetService(Type serviceType) => null;
        }
    }
}
