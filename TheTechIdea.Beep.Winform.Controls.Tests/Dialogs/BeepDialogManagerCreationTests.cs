using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests.Dialogs;

public class BeepDialogManagerCreationTests
{
    [Fact]
    public void CreateDialog_RendersEveryConfiguredLegacyButton()
    {
        RunSta(() =>
        {
            var manager = new BeepDialogManager();
            var config = new DialogConfig
            {
                Title = "Confirm",
                Message = "Continue?",
                Buttons = new[] { BeepDialogButtons.Yes, BeepDialogButtons.No, BeepDialogButtons.Cancel }
            };

            using var dialog = manager.CreateDialog(config);

            var buttonTexts = Descendants(dialog)
                .OfType<BeepButton>()
                .Select(button => button.Text)
                .ToArray();

            Assert.Contains("Yes", buttonTexts);
            Assert.Contains("No", buttonTexts);
            Assert.Contains("Cancel", buttonTexts);
        });
    }

    [Fact]
    public void CreateDialog_HostsCustomControlWithoutLosingTheControl()
    {
        RunSta(() =>
        {
            var manager = new BeepDialogManager();
            using var content = new Panel { Name = "customContent" };
            var config = DialogConfig.CreateWithUserControl("Custom", content, BeepDialogButtons.Cancel, BeepDialogButtons.Ok);

            using var dialog = manager.CreateDialog(config);

            Assert.Same(content, dialog.CustomContent);
            Assert.Same(dialog, content.FindForm());
            Assert.Equal(DockStyle.Fill, content.Dock);
        });
    }

    [Fact]
    public void CreateDialog_ExpandsCompositeLegacyButtons()
    {
        RunSta(() =>
        {
            var manager = new BeepDialogManager();
            var config = DialogConfig.CreateUnsavedChanges("Invoice");

            using var dialog = manager.CreateDialog(config);

            var buttonTexts = Descendants(dialog)
                .OfType<BeepButton>()
                .Select(button => button.Text)
                .ToArray();

            Assert.Contains("Save", buttonTexts);
            Assert.Contains("Don't Save", buttonTexts);
            Assert.Contains("Cancel", buttonTexts);
        });
    }

    private static void RunSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure != null)
        {
            throw failure;
        }
    }

    private static IEnumerable<Control> Descendants(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;

            foreach (var nested in Descendants(child))
            {
                yield return nested;
            }
        }
    }
}
