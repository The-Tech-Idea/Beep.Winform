using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class DocumentHostDesignerAndRestoreRegressionTests
    {
        [Fact]
        public void BeepDocumentHost_AdvertisesDesignServerDesigner()
        {
            var designers = TypeDescriptor.GetAttributes(typeof(BeepDocumentHost))
                .OfType<DesignerAttribute>();

            Assert.Contains(designers, attribute =>
                attribute.DesignerTypeName.Contains("BeepDocumentHostDesigner", StringComparison.Ordinal)
                && attribute.DesignerTypeName.Contains("TheTechIdea.Beep.Winform.Controls.Design.Server", StringComparison.Ordinal));
        }

        [Fact]
        public void ApplyDesignTimeDocuments_UpdatesExistingPanelsFromDescriptors()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            var descriptor = new DocumentDescriptor { Id = "doc1", Title = "Document 1" };
            host.DesignTimeDocuments.Add(descriptor);
            host.ApplyDesignTimeDocuments();

            descriptor.Title = "Renamed Document";
            descriptor.CanClose = false;
            host.ApplyDesignTimeDocuments();

            var panel = host.GetPanel("doc1");
            Assert.NotNull(panel);
            Assert.Equal("Renamed Document", panel!.DocumentTitle);
            Assert.False(panel.CanClose);
        }

        [Fact]
        public void ApplyDesignTimeDocuments_RemovesPanelsMissingFromDesignTimeCollection()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            var keep = new DocumentDescriptor { Id = "doc1", Title = "Keep" };
            var remove = new DocumentDescriptor { Id = "doc2", Title = "Remove" };
            host.DesignTimeDocuments.Add(keep);
            host.DesignTimeDocuments.Add(remove);
            host.ApplyDesignTimeDocuments();

            host.DesignTimeDocuments.Remove(remove);
            host.ApplyDesignTimeDocuments();

            Assert.NotNull(host.GetPanel("doc1"));
            Assert.Null(host.GetPanel("doc2"));
            Assert.Equal(1, host.DocumentCount);
        }

        [Fact]
        public void DesignTimeDocuments_MutationsApplyImmediatelyInDesignMode()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            var descriptor = new DocumentDescriptor { Id = "doc1", Title = "Document 1" };
            host.DesignTimeDocuments.Add(descriptor);

            Assert.NotNull(host.GetPanel("doc1"));

            descriptor.Title = "Renamed Document";

            Assert.Equal("Renamed Document", host.GetPanel("doc1")!.DocumentTitle);
        }

        [Fact]
        public void DesignTimeDocuments_ReusesRegisteredDesignPanel()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            var panel = new BeepDocumentPanel("doc1", "Designer Panel");
            host.RegisterDocumentPanel(panel, activate: false);

            host.DesignTimeDocuments.Add(new DocumentDescriptor
            {
                Id = "doc1",
                Title = "Descriptor Title",
                CanClose = false
            });

            Assert.Same(panel, host.GetPanel("doc1"));
            Assert.Equal(1, host.DocumentCount);
            Assert.Equal("Descriptor Title", panel.DocumentTitle);
            Assert.False(panel.CanClose);
        }

        [Fact]
        public void DesignTimeDocuments_InitialContentChangesApplyImmediatelyInDesignMode()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            var descriptor = new DocumentDescriptor
            {
                Id = "doc1",
                Title = "Document 1",
                InitialContent = DocumentInitialContent.Empty
            };
            host.DesignTimeDocuments.Add(descriptor);

            Assert.Empty(host.GetPanel("doc1")!.Controls);

            descriptor.InitialContent = DocumentInitialContent.Label;

            var panel = host.GetPanel("doc1");
            Assert.NotNull(panel);
            Assert.Single(panel!.Controls);
            Assert.IsType<Label>(panel.Controls[0]);
        }

        [Fact]
        public void DesignerMode_RemovingAuthoredChild_DoesNotMarkHostAsDetaching()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            using var child = new Button { Name = "designerChild" };
            host.Controls.Add(child);
            host.Controls.Remove(child);

            Assert.False(GetPrivateBool(host, "_isDesignerDetaching"));
            Assert.False(GetPrivateBool(host, "_isDisposingHost"));
        }

        [Fact]
        public void DesignerMode_ReparentingHost_ClearsDetachFlagsWhenAttachedAgain()
        {
            using var host = new BeepDocumentHost();
            host.Site = new DesignModeSite();

            using var firstParent = new Panel();
            using var secondParent = new Panel();

            firstParent.Controls.Add(host);
            firstParent.Controls.Remove(host);

            Assert.True(GetPrivateBool(host, "_isDesignerDetaching"));
            Assert.True(GetPrivateBool(host, "_isDisposingHost"));

            secondParent.Controls.Add(host);

            Assert.False(GetPrivateBool(host, "_isDesignerDetaching"));
            Assert.False(GetPrivateBool(host, "_isDisposingHost"));
        }

        [Fact]
        public void TryRestoreLayout_RestoresMultiGroupTopologyFromSavedLayout()
        {
            using var source = new BeepDocumentHost();
            source.AddDocument("doc-1", "Document 1", string.Empty, activate: true);
            source.AddDocument("doc-2", "Document 2", string.Empty, activate: false);

            Assert.True(source.SplitDocumentHorizontal("doc-2"));
            string layoutJson = source.SaveLayout();

            using var target = new BeepDocumentHost();
            int factoryCalls = 0;
            target.RestoreDocumentFactory = e =>
            {
                factoryCalls++;
                return DocumentDescriptor.Create(e.DocumentId, e.Title, e.IconPath);
            };

            var report = target.TryRestoreLayout(layoutJson, out _);

            Assert.True(report.IsSuccess);
            Assert.True(factoryCalls >= 2);
            Assert.True(target.Groups.Count >= 2);
            Assert.NotNull(target.GetPanel("doc-1"));
            Assert.NotNull(target.GetPanel("doc-2"));
            Assert.NotEqual(GetDocumentGroupId(target, "doc-1"), GetDocumentGroupId(target, "doc-2"));
            Assert.True(LayoutTreeValidator.Validate(target.LayoutRoot).IsValid);
        }

        [Fact]
        public void TryRestoreLayout_RestoresFloatingDocumentState()
        {
            const string layoutJson = """
            {
              "schemaVersion": 2,
              "activeDocumentId": "doc-1",
              "layoutTree": {
                "type": "tabGroup",
                "nodeId": "group-1",
                "groupId": "group-1",
                "selectedDocumentId": "doc-1",
                "documents": [
                  {
                    "id": "doc-1",
                    "title": "Document 1",
                    "iconPath": "",
                    "isPinned": false,
                    "isModified": false,
                    "customData": {
                      "kind": "editor"
                    }
                  }
                ]
              },
              "floatingWindows": [
                {
                  "documentId": "doc-1",
                  "bounds": {
                    "x": 40,
                    "y": 50,
                    "w": 420,
                    "h": 320
                  },
                  "windowState": "Normal"
                }
              ],
              "autoHideEntries": [],
              "mruSnapshot": ["doc-1"]
            }
            """;

            using var host = new BeepDocumentHost();
            host.RestoreDocumentFactory = e => DocumentDescriptor.Create(e.DocumentId, e.Title, e.IconPath);

            var report = host.TryRestoreLayout(layoutJson, out _);

            Assert.True(report.IsSuccess);
            Assert.Contains("doc-1", report.Restored);
            Assert.Equal(DocumentDockState.Floating, host.GetDocumentDockState("doc-1"));
            Assert.True(host.DockBackDocument("doc-1"));
            Assert.Equal(DocumentDockState.Docked, host.GetDocumentDockState("doc-1"));
        }

        private static bool GetPrivateBool(object instance, string fieldName)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            return (bool)(field!.GetValue(instance) ?? false);
        }

        private static string GetDocumentGroupId(BeepDocumentHost host, string documentId)
        {
            var field = typeof(BeepDocumentHost).GetField("_docGroupMap", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);

            var map = field!.GetValue(host) as System.Collections.Generic.Dictionary<string, string>;
            Assert.NotNull(map);
            Assert.True(map!.TryGetValue(documentId, out var groupId));
            return groupId!;
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
