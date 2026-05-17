// BeepDocumentManagerDesigner.WizardPrefs.cs
// Phase 07 polish — wizard "Don't show again" persistence + reset verb.
//
// Persistence model:
//   • HKCU\Software\TheTechIdea\Beep\Design\DocumentSetupWizard\AutoOpen (DWORD)
//       1 (default) = show the setup wizard automatically on every fresh drop
//       0           = user opted out via the wizard's "Don't show again" checkbox
//
// Why HKCU and not the project file:
//   This is a *developer preference* about the design experience itself, not a
//   property of any one form. Anchoring it to HKCU mirrors the way DevExpress,
//   Telerik and Microsoft's own tooling persist first-run setup choices for
//   their toolbox components.
//
// The runtime project never touches this key — only the design server.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed partial class BeepDocumentManagerDesigner
    {
        private const string WizardPrefsKeyPath =
            @"Software\TheTechIdea\Beep\Design\DocumentSetupWizard";
        private const string WizardAutoOpenValueName = "AutoOpen";

        /// <summary>
        /// True when the setup wizard should auto-open on a fresh component
        /// drop. Defaults to true if the user has never opted out.
        /// </summary>
        internal static bool ShouldAutoOpenWizard()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(WizardPrefsKeyPath);
                if (key == null) return true;
                var raw = key.GetValue(WizardAutoOpenValueName);
                if (raw is int i) return i != 0;
                if (raw is string s && int.TryParse(s, out var v)) return v != 0;
                return true;
            }
            catch
            {
                return true; // design-time safety: any failure → show the wizard
            }
        }

        /// <summary>
        /// Persist the user's "Don't show this wizard automatically next time"
        /// choice from the wizard footer checkbox.
        /// </summary>
        internal static void SetAutoOpenWizard(bool autoOpen)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(WizardPrefsKeyPath);
                key?.SetValue(WizardAutoOpenValueName, autoOpen ? 1 : 0, RegistryValueKind.DWord);
            }
            catch
            {
                // Non-fatal: HKCU writes can be denied by group policy on some
                // locked-down dev boxes — fall through silently.
            }
        }

        /// <summary>Verb handler: restore the default "show wizard on every drop" behaviour.</summary>
        internal void OnResetWizardPreference(object? sender, EventArgs e)
        {
            SetAutoOpenWizard(true);
            MessageBox.Show(
                "The Beep Document Area setup wizard will now open again the next time you " +
                "drop a BeepDocumentManager or BeepDocumentHost onto a form.",
                "Setup Wizard Preference Reset",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
