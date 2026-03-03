using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IDialogManager
    {
        Task MsgBoxAsync(string title, string promptText, CancellationToken cancellationToken = default);
        Task<DialogReturn> ShowAlertAsync(string title, string message, string icon, CancellationToken cancellationToken = default);
        Task ShowMessegeAsync(string title, string message, string icon, CancellationToken cancellationToken = default);
        Task ShowExceptionAsync(string title, System.Exception ex, CancellationToken cancellationToken = default);

        Task<DialogReturn> ConfirmAsync(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, CancellationToken cancellationToken = default);
        Task<DialogReturn> ConfirmAsync(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton, CancellationToken cancellationToken = default);

        Task<DialogReturn> InputBoxAsync(string title, string promptText, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputLargeBoxAsync(string title, string promptText, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputBoxYesNoAsync(string title, string promptText, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputComboBoxAsync(string title, string promptText, List<SimpleItem> itvalues, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputComboBoxAsync(string title, string promptText, List<string> values, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputListBoxAsync(string title, string promptText, List<SimpleItem> itvalues, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputRadioGroupBoxAsync(string title, string promptText, List<SimpleItem> itvalues, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputCheckListAsync(string title, string promptText, List<SimpleItem> items, CancellationToken cancellationToken = default);
        Task<DialogReturn> MultiSelectAsync(string title, string promptText, List<SimpleItem> items, CancellationToken cancellationToken = default);
        Task<DialogReturn> DialogComboAsync(string text, List<SimpleItem> comboSource, string displayMember, string valueMember, CancellationToken cancellationToken = default);

        Task<DialogReturn> InputPasswordAsync(string title, string promptText, bool masked = true, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputIntAsync(string title, string promptText, int? min = null, int? max = null, int? @default = null, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputDoubleAsync(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputDateTimeAsync(string title, string promptText, System.DateTime? min = null, System.DateTime? max = null, System.DateTime? @default = null, CancellationToken cancellationToken = default);
        Task<DialogReturn> InputTimeSpanAsync(string title, string promptText, System.TimeSpan? min = null, System.TimeSpan? max = null, System.TimeSpan? @default = null, CancellationToken cancellationToken = default);

        Task<DialogReturn> SelectColorAsync(string? title = null, string? initialColor = null, CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectFontAsync(string? title = null, string? initialFont = null, CancellationToken cancellationToken = default);

        Task<DialogReturn> LoadFileDialogAsync(string exts, string dir, string filter, CancellationToken cancellationToken = default);
        Task<DialogReturn> LoadFileDialogAsync(string exts, string dir, string filter, string initialFileName, CancellationToken cancellationToken = default);
        Task<List<string>> LoadFilesDialogAsync(string exts, string dir, string filter, CancellationToken cancellationToken = default);
        Task<DialogReturn> SaveFileDialogAsync(string exts, string dir, string filter, CancellationToken cancellationToken = default);
        Task<DialogReturn> SaveFileDialogAsync(string exts, string dir, string filter, string defaultFileName, CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectFolderDialogAsync(CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectFolderDialogAsync(string title, string initialDir, bool allowCreate, CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectFileAsync(List<SimpleItem> files, string filter, CancellationToken cancellationToken = default);
        Task<DialogReturn> ConfirmOverwriteAsync(string filePath, CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectSpecialDirectoriesComboBoxAsync(CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectSpecialDirectoriesListBoxAsync(CancellationToken cancellationToken = default);
        Task<DialogReturn> SelectSpecialDirectoriesRadioGroupBoxAsync(CancellationToken cancellationToken = default);

        Task<IProgressHandle> ShowProgressAsync(string title, string? message = null, CancellationToken cancellationToken = default);
        Task ShowToastAsync(string message, int durationMs = 3000, string? icon = null, CancellationToken cancellationToken = default);
    }

    public interface IProgressHandle : IAsyncDisposable
    {
        void Update(int percent, string? status = null);
        void Complete(string? finalMessage = null);
    }

    public class DialogReturn
    {
        public BeepDialogResult Result { get; set; }
        public string Value { get; set; } = string.Empty;
        public object? Tag { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Submit { get; set; } = false;
        public List<SimpleItem> Items { get; set; } = new List<SimpleItem>();
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public BeepDialogButtons UserAction { get; set; } = BeepDialogButtons.Ok;
        public DialogReturn()
        {
            Result = BeepDialogResult.None;
            Value = string.Empty;
            Tag = null;
        }

    }
}