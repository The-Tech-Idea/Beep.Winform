using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests;

public class BeepFormsKeyboardShortcutProviderTests
{
    [Fact]
    public void Register_NewShortcut_IsInShortcuts()
    {
        var provider = new BeepFormsKeyboardShortcutProvider();
        var shortcut = new BeepFormsKeyboardShortcut
        {
            KeyData = Keys.Control | Keys.S,
            DisplayName = "Save",
            RequiresActiveBlock = true,
            ExecuteAsync = _ => Task.FromResult(true)
        };
        provider.Register(shortcut);
        Assert.Contains(shortcut, provider.Shortcuts);
    }

    [Fact]
    public void Register_OverwriteExisting_ReplacesPrevious()
    {
        var provider = new BeepFormsKeyboardShortcutProvider();
        var first = new BeepFormsKeyboardShortcut { KeyData = Keys.Control | Keys.S, DisplayName = "First" };
        var second = new BeepFormsKeyboardShortcut { KeyData = Keys.Control | Keys.S, DisplayName = "Second" };
        provider.Register(first);
        provider.Register(second);
        Assert.Single(provider.Shortcuts);
        Assert.Equal("Second", provider.Shortcuts[0].DisplayName);
    }

    [Fact]
    public void Unregister_RemovesShortcut()
    {
        var provider = new BeepFormsKeyboardShortcutProvider();
        var shortcut = new BeepFormsKeyboardShortcut { KeyData = Keys.F8, DisplayName = "Execute Query" };
        provider.Register(shortcut);
        provider.Unregister(Keys.F8);
        Assert.Empty(provider.Shortcuts);
    }

    [Fact]
    public void ProcessCmdKey_NoForm_DoesNotCrash()
    {
        var provider = new BeepFormsKeyboardShortcutProvider();
        var shortcut = new BeepFormsKeyboardShortcut { KeyData = Keys.F8, ExecuteAsync = _ => Task.FromResult(true) };
        provider.Register(shortcut);
        var result = provider.ProcessCmdKey(default, Keys.F8);
        Assert.False(result);
    }

    [Fact]
    public void ProcessCmdKey_UnregisteredKey_ReturnsFalse()
    {
        var provider = new BeepFormsKeyboardShortcutProvider();
        provider.Register(new BeepFormsKeyboardShortcut { KeyData = Keys.F8, ExecuteAsync = _ => Task.FromResult(true) });
        var result = provider.ProcessCmdKey(default, Keys.F9);
        Assert.False(result);
    }
}

public class BeepFormsMessageServiceTests
{
    [Fact]
    public void Publish_SetsCurrentMessageAndSeverity()
    {
        var service = new BeepFormsMessageService();
        var viewState = new BeepFormsViewState();
        service.Publish(viewState, "Record saved", BeepFormsMessageSeverity.Success);

        Assert.Equal("Record saved", service.CurrentMessage);
        Assert.Equal(BeepFormsMessageSeverity.Success, service.CurrentSeverity);
        Assert.Equal("Record saved", viewState.CurrentMessage);
        Assert.Equal(BeepFormsMessageSeverity.Success, viewState.MessageSeverity);
    }

    [Fact]
    public void Clear_ResetsMessageToEmpty()
    {
        var service = new BeepFormsMessageService();
        var viewState = new BeepFormsViewState();
        service.Publish(viewState, "Error occurred", BeepFormsMessageSeverity.Error);
        service.Clear(viewState);

        Assert.Equal(string.Empty, service.CurrentMessage);
        Assert.Equal(BeepFormsMessageSeverity.None, service.CurrentSeverity);
        Assert.Equal(string.Empty, viewState.CurrentMessage);
    }

    [Fact]
    public void Publish_NullMessage_SetsEmptyString()
    {
        var service = new BeepFormsMessageService();
        var viewState = new BeepFormsViewState();
        service.Publish(viewState, null!);

        Assert.Equal(string.Empty, service.CurrentMessage);
    }

    [Fact]
    public void Publish_OverwritesPreviousMessage()
    {
        var service = new BeepFormsMessageService();
        var viewState = new BeepFormsViewState();
        service.Publish(viewState, "First", BeepFormsMessageSeverity.Info);
        service.Publish(viewState, "Second", BeepFormsMessageSeverity.Warning);

        Assert.Equal("Second", service.CurrentMessage);
        Assert.Equal(BeepFormsMessageSeverity.Warning, service.CurrentSeverity);
    }
}

public class BeepFormsCommandRouterTests
{
    [Fact]
    public async Task SwitchToBlock_NoManager_ReturnsFalse()
    {
        var router = new BeepFormsCommandRouter();
        var result = await router.SwitchToBlockAsync("BLOCK1");
        Assert.False(result);
    }

    [Fact]
    public async Task EnterQuery_NoManager_ReturnsFalse()
    {
        var router = new BeepFormsCommandRouter();
        var result = await router.EnterQueryAsync("BLOCK1");
        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteQuery_NoManager_ReturnsFalse()
    {
        var router = new BeepFormsCommandRouter();
        var result = await router.ExecuteQueryAsync("BLOCK1");
        Assert.False(result);
    }

    [Fact]
    public async Task CommitForm_NoManager_ReturnsError()
    {
        var router = new BeepFormsCommandRouter();
        var result = await router.CommitFormAsync();
        Assert.Equal(Errors.Failed, result.Flag);
    }

    [Fact]
    public async Task RollbackForm_NoManager_ReturnsError()
    {
        var router = new BeepFormsCommandRouter();
        var result = await router.RollbackFormAsync();
        Assert.Equal(Errors.Failed, result.Flag);
    }

    [Fact]
    public async Task ClearBlock_NoManager_ReturnsFalse()
    {
        var router = new BeepFormsCommandRouter();
        var result = await router.ClearBlockAsync("BLOCK1");
        Assert.False(result);
    }
}

public class BeepFormsManagerAdapterTests
{
    [Fact]
    public void Attach_NullManager_SetsNull()
    {
        var adapter = new BeepFormsManagerAdapter();
        adapter.Attach(null);
        var viewState = new BeepFormsViewState();
        adapter.Sync(viewState);
        Assert.False(viewState.IsDirty);
    }

    [Fact]
    public void Sync_NoActiveBlock_DoesNotThrow()
    {
        var adapter = new BeepFormsManagerAdapter();
        var viewState = new BeepFormsViewState();
        adapter.Sync(viewState);
        Assert.Equal(string.Empty, viewState.RecordPositionText);
    }

    [Fact]
    public void SyncBlock_NullBlockView_DoesNotThrow()
    {
        var adapter = new BeepFormsManagerAdapter();
        adapter.SyncBlock(null!);
    }
}

public class BeepFormsViewStateTests
{
    [Fact]
    public void NewViewState_HasEmptyDefaults()
    {
        var state = new BeepFormsViewState();
        Assert.False(state.IsDirty);
        Assert.False(state.IsQueryMode);
        Assert.Equal(string.Empty, state.CurrentMessage);
        Assert.Equal(BootstrapState.Idle, state.BootstrapState);
        Assert.Empty(state.WorkflowHistory);
    }

    [Fact]
    public void WorkflowHistory_StoresEntriesInOrder()
    {
        var state = new BeepFormsViewState();
        state.WorkflowHistoryItems.Insert(0, new BeepFormsWorkflowEntry { Text = "Latest" });
        state.WorkflowHistoryItems.Insert(0, new BeepFormsWorkflowEntry { Text = "Older" });
        Assert.Equal(2, state.WorkflowHistoryItems.Count);
        Assert.Equal("Older", state.WorkflowHistoryItems[0].Text);
        Assert.Equal("Latest", state.WorkflowHistoryItems[1].Text);
    }

    [Fact]
    public void BootstrapState_DefaultsToIdle()
    {
        var state = new BeepFormsViewState();
        Assert.Equal(BootstrapState.Idle, state.BootstrapState);
    }
}

public class BeepFormsUnitOfWorkEventArgsTests
{
    [Fact]
    public void PreCreate_IsPreEvent_ReturnsTrue()
    {
        var args = new BeepFormsUnitOfWorkEventArgs
        {
            BlockName = "EMP",
            EventKind = BeepFormsUnitOfWorkEventKind.PreCreate
        };
        Assert.True(args.IsPreEvent);
        Assert.False(args.IsPostEvent);
    }

    [Fact]
    public void PostQuery_IsPostEvent_ReturnsTrue()
    {
        var args = new BeepFormsUnitOfWorkEventArgs
        {
            BlockName = "EMP",
            EventKind = BeepFormsUnitOfWorkEventKind.PostQuery
        };
        Assert.False(args.IsPreEvent);
        Assert.True(args.IsPostEvent);
    }

    [Fact]
    public void CurrentChanged_IsNeitherPreNorPost()
    {
        var args = new BeepFormsUnitOfWorkEventArgs
        {
            EventKind = BeepFormsUnitOfWorkEventKind.CurrentChanged
        };
        Assert.False(args.IsPreEvent);
        Assert.False(args.IsPostEvent);
    }

    [Fact]
    public void ItemChanged_WithPropertyName_ShowsFieldNameInActivity()
    {
        var args = new BeepFormsUnitOfWorkEventArgs
        {
            BlockName = "EMP",
            EventKind = BeepFormsUnitOfWorkEventKind.ItemChanged,
            PropertyName = "EmployeeName"
        };
        Assert.Contains("EmployeeName", args.ActivityText);
    }

    [Fact]
    public void ItemChanged_NoPropertyName_ShowsGenericText()
    {
        var args = new BeepFormsUnitOfWorkEventArgs
        {
            EventKind = BeepFormsUnitOfWorkEventKind.ItemChanged
        };
        Assert.Equal("Field changed", args.ActivityText);
    }
}

public class BeepFormsBootstrapEventArgsTests
{
    [Fact]
    public void Constructor_SetsStateAndMessage()
    {
        var args = new BeepFormsBootstrapEventArgs(BootstrapState.Succeeded);
        Assert.Equal(BootstrapState.Succeeded, args.State);
        Assert.Null(args.ErrorMessage);
    }

    [Fact]
    public void Constructor_WithError_SetsErrorMessage()
    {
        var args = new BeepFormsBootstrapEventArgs(BootstrapState.Failed, "Connection refused");
        Assert.Equal(BootstrapState.Failed, args.State);
        Assert.Equal("Connection refused", args.ErrorMessage);
    }
}

public class BeepFormsKeyboardShortcutTests
{
    [Fact]
    public void Shortcut_HasReasonableDefaults()
    {
        var s = new BeepFormsKeyboardShortcut { KeyData = Keys.Control | Keys.S };
        Assert.True(s.RequiresActiveBlock);
        Assert.True(s.RequiresManager);
        Assert.Equal("General", s.Category);
    }
}
