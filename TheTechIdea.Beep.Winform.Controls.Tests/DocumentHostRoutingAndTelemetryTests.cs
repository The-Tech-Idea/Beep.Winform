using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class DocumentHostRoutingAndTelemetryTests
    {
        [Fact]
        public void ExecuteCommand_RoutedContext_UsesContextAwareCanExecuteAndExecute()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };
            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);

            bool executed = false;
            host.CommandRegistry.Register(new BeepCommandEntry
            {
                Id = "test.context.execute",
                Title = "Test",
                Category = "Tests",
                CanExecuteWithContext = ctx => ctx.ActiveDocumentId == "doc-1",
                ExecuteWithContext = ctx => executed = ctx.ActiveDocumentId == "doc-1"
            });

            bool result = host.CommandService.ExecuteCommand("test.context.execute");
            Assert.True(result);
            Assert.True(executed);
        }

        [Fact]
        public void CanExecuteCommand_HostScope_DocumentScopedCommandReturnsFalse()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };
            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);

            bool canExecute = host.CommandService.CanExecuteCommand(
                "document.close",
                DocumentCommandTargetScope.Host);

            Assert.False(canExecute);
        }

        [Fact]
        public void ExecuteCommand_RoutedContext_ContainsScopeMetadata()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };
            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);

            string? capturedScope = null;
            host.CommandRegistry.Register(new BeepCommandEntry
            {
                Id = "test.scope.metadata",
                Title = "Scope metadata",
                Category = "Tests",
                CanExecuteWithContext = _ => true,
                ExecuteWithContext = ctx =>
                {
                    if (ctx.Metadata.TryGetValue("scope", out var scope))
                        capturedScope = scope?.ToString();
                }
            });

            bool executed = host.CommandService.ExecuteCommand(
                "test.scope.metadata",
                DocumentCommandTargetScope.ActiveGroup);

            Assert.True(executed);
            Assert.Equal(DocumentCommandTargetScope.ActiveGroup.ToString(), capturedScope);
        }

        [Fact]
        public void ExecuteCommand_GlobalScope_ClearsActiveDocumentAndGroupInContext()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };
            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);

            DocumentCommandContext? captured = null;
            host.CommandRegistry.Register(new BeepCommandEntry
            {
                Id = "test.scope.global",
                Title = "Global scope",
                Category = "Tests",
                CanExecuteWithContext = _ => true,
                ExecuteWithContext = ctx => captured = ctx
            });

            bool executed = host.CommandService.ExecuteCommand(
                "test.scope.global",
                DocumentCommandTargetScope.Global);

            Assert.True(executed);
            Assert.NotNull(captured);
            Assert.Null(captured!.ActiveDocumentId);
            Assert.Null(captured.ActiveGroupId);
            Assert.False(captured.IsFloatingDocument);
        }

        [Fact]
        public void ExecuteCommand_WithIncomingContext_UsesProvidedDocumentTarget()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };
            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);
            host.AddDocument("doc-2", "Document 2", string.Empty, activate: false);

            bool executed = host.CommandService.ExecuteCommand(
                "document.close",
                DocumentCommandTargetScope.ActiveDocument,
                new DocumentCommandContext { ActiveDocumentId = "doc-2" });

            Assert.True(executed);
            Assert.Null(host.GetPanel("doc-2"));
            Assert.NotNull(host.GetPanel("doc-1"));
        }

        [Fact]
        public void ExecuteCommand_UnknownId_ReturnsFalse()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };

            bool executed = host.CommandService.ExecuteCommand("unknown.command.id");
            bool canExecute = host.CommandService.CanExecuteCommand("unknown.command.id");

            Assert.False(executed);
            Assert.False(canExecute);
        }

        [Fact]
        public void ExecuteCommand_Success_IncrementsUsageCount()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };

            host.CommandRegistry.Register(new BeepCommandEntry
            {
                Id = "test.usage.success",
                Title = "Usage success",
                Category = "Tests",
                CanExecuteWithContext = _ => true,
                ExecuteWithContext = _ => { }
            });

            bool executed = host.CommandService.ExecuteCommand("test.usage.success");
            var cmd = host.CommandRegistry.FindById("test.usage.success");

            Assert.True(executed);
            Assert.NotNull(cmd);
            Assert.Equal(1, cmd!.UsageCount);
            Assert.NotNull(cmd.LastUsed);
        }

        [Fact]
        public void ExecuteCommand_CannotExecute_DoesNotIncrementUsageCount()
        {
            var host = new BeepDocumentHost
            {
                EnableRoutedCommands = true
            };

            host.CommandRegistry.Register(new BeepCommandEntry
            {
                Id = "test.usage.blocked",
                Title = "Usage blocked",
                Category = "Tests",
                CanExecuteWithContext = _ => false,
                ExecuteWithContext = _ => { }
            });

            bool executed = host.CommandService.ExecuteCommand("test.usage.blocked");
            var cmd = host.CommandRegistry.FindById("test.usage.blocked");

            Assert.False(executed);
            Assert.NotNull(cmd);
            Assert.Equal(0, cmd!.UsageCount);
            Assert.Null(cmd.LastUsed);
        }

        [Fact]
        public void SplitDocumentHorizontal_TransactionalDocking_LeavesLayoutUnsuspended()
        {
            var host = new BeepDocumentHost
            {
                EnableTransactionalDocking = true
            };

            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);
            host.AddDocument("doc-2", "Document 2", string.Empty, activate: false);

            bool splitOk = host.SplitDocumentHorizontal("doc-1");
            Assert.True(splitOk);

            var method = typeof(BeepDocumentHost).GetMethod(
                "GetLayoutSuspended",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);

            bool isSuspended = (bool)(method!.Invoke(host, null) ?? true);
            Assert.False(isSuspended);
        }

        [Fact]
        public void TryRestoreLayout_WithTelemetryEnabled_EmitsRestoreFailureEvent()
        {
            var host = new BeepDocumentHost
            {
                EnableHostTelemetry = true
            };

            DocumentHostTelemetryEvent? emitted = null;
            host.Profiler.TelemetryEmitted += (_, evt) => emitted = evt;

            var report = host.TryRestoreLayout("{bad json", out _);

            Assert.False(report.IsSuccess);
            Assert.NotNull(emitted);
            Assert.Equal(DocumentHostOperationType.RestoreLayout, emitted!.OperationType);
            Assert.False(emitted.Success);
            Assert.Equal("layout.restore", emitted.EventName);
        }

        [Fact]
        public void AutoHideAndRestoreDocument_KeepsLayoutTreeValid()
        {
            var host = new BeepDocumentHost
            {
                EnableTransactionalDocking = true
            };

            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);
            host.AddDocument("doc-2", "Document 2", string.Empty, activate: false);

            host.AutoHideDocument("doc-1", AutoHideSide.Left);
            host.RestoreAutoHideDocument("doc-1");

            var report = LayoutTreeValidator.Validate(host.LayoutRoot);
            Assert.True(report.IsValid);
            Assert.NotNull(host.GetPanel("doc-1"));
        }

        [Fact]
        public void BatchMoveDocument_UsesCoordinatedMutation_AndKeepsLayoutTreeValid()
        {
            var host = new BeepDocumentHost
            {
                EnableTransactionalDocking = true
            };

            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);
            host.AddDocument("doc-2", "Document 2", string.Empty, activate: false);
            Assert.True(host.SplitDocumentHorizontal("doc-2"));

            var method = typeof(BeepDocumentHost).GetMethod(
                "GetActiveGroupId",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);

            string? targetGroupId = method!.Invoke(host, null) as string;
            Assert.False(string.IsNullOrWhiteSpace(targetGroupId));

            bool moved = host.BatchMoveDocument("doc-1", targetGroupId!);
            Assert.True(moved);

            var report = LayoutTreeValidator.Validate(host.LayoutRoot);
            Assert.True(report.IsValid);
        }

        [Fact]
        public void MergeAllGroups_UsesSingleCoordinatedMutation_AndLeavesValidTree()
        {
            var host = new BeepDocumentHost
            {
                EnableTransactionalDocking = true
            };

            host.AddDocument("doc-1", "Document 1", string.Empty, activate: true);
            host.AddDocument("doc-2", "Document 2", string.Empty, activate: false);
            host.AddDocument("doc-3", "Document 3", string.Empty, activate: false);

            Assert.True(host.SplitDocumentHorizontal("doc-2"));
            Assert.True(host.SplitDocumentVertical("doc-3"));

            host.MergeAllGroups();

            var report = LayoutTreeValidator.Validate(host.LayoutRoot);
            Assert.True(report.IsValid);

            bool closeDoc2 = host.CloseDocument("doc-2");
            bool closeDoc3 = host.CloseDocument("doc-3");
            Assert.True(closeDoc2);
            Assert.True(closeDoc3);
            Assert.NotNull(host.GetPanel("doc-1"));
        }

        [Fact]
        public void TreeMutationCoordinator_UnknownOperationName_ThrowsArgumentException()
        {
            var host = new BeepDocumentHost();
            var coordinator = new DocumentHostTreeMutationCoordinator(host);
            var ex = Assert.Throws<ArgumentException>(() =>
                coordinator.Execute("drop-dock-center-typo", () => { }));
            Assert.Contains("Unknown document-host operation name", ex.Message);
        }

    }
}
