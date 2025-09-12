using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public partial class BeepDialogManager
    {
        public async Task<bool> RunWithProgressAsync(
            string title,
            Func<IProgress<int>, CancellationToken, Task> operation,
            string cancelText = "Cancel",
            BeepDialogOptions? options = null,
            CancellationToken token = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var bar = new BeepProgressBar { Dock = DockStyle.Top, Height = 20, Style = ProgressBarStyle.Striped, Minimum = 0, Maximum = 100 };
            var pct = new BeepLabel { Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleLeft, Height = 20, Text = "0%" };
            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
            body.Controls.Add(bar);
            body.Controls.Add(pct);

            var content = new BeepDialogContent
            {
                Title = title,
                CustomBody = body,
                Buttons = new[]
                {
                    new BeepDialogButton { Text = cancelText, Result = BeepDialogResult.Cancel, IsCancel = true }
                }
            };

            var progress = new Progress<int>(v =>
            {
                v = Math.Max(0, Math.Min(100, v));
                bar.Style = ProgressBarStyle.Animated;
                bar.Value = v;
                pct.Text = $"{v}%";
            });

            // Kick off the operation
            var opTask = Task.Run(() => operation(progress, cts.Token), cts.Token)
                .ContinueWith(t =>
                {
                    // When done or canceled, close the dialog on the UI thread
                    _host.BeginInvoke(new Action(async () =>
                    {
                        await CloseTopAsync(t.IsCanceled ? BeepDialogResult.Cancel
                                                         : (t.IsFaulted ? BeepDialogResult.Cancel : BeepDialogResult.OK));
                    }));
                }, TaskScheduler.Default);

            // Show modal; it will be closed by the op continuation or by user Cancel
            var result = await ShowAsync(content, options ?? new BeepDialogOptions { Kind = BeepDialogKind.Centered }, token);

            if (result == BeepDialogResult.Cancel)
                cts.Cancel(); // ensure the task is asked to stop

            try { await opTask; } catch { /* swallow; caller gets false */ }

            return result == BeepDialogResult.OK;
        }
    }
}
