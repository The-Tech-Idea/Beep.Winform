using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Static manager for creating and managing wizard instances
    /// </summary>
    public static class WizardManager
    {
        private static readonly Dictionary<string, WizardInstance> _activeWizards = new Dictionary<string, WizardInstance>();

        /// <summary>
        /// Default wizard style
        /// </summary>
        public static WizardStyle DefaultStyle { get; set; } = WizardStyle.HorizontalStepper;

        /// <summary>
        /// Enable animations globally
        /// </summary>
        public static bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Create a new wizard instance
        /// </summary>
        public static WizardInstance CreateWizard(WizardConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(config.Key))
                config.Key = Guid.NewGuid().ToString();

            // Remove existing wizard with same key
            if (_activeWizards.TryGetValue(config.Key, out var existing))
            {
                existing.Close();
                _activeWizards.Remove(config.Key);
            }

            var instance = new WizardInstance(config);
            _activeWizards[config.Key] = instance;

            return instance;
        }

        /// <summary>
        /// Show wizard as modal dialog
        /// </summary>
        public static DialogResult ShowWizard(WizardConfig config)
        {
            var instance = CreateWizard(config);
            return instance.ShowDialog();
        }

        /// <summary>
        /// Show wizard as modal dialog with owner
        /// </summary>
        public static DialogResult ShowWizard(WizardConfig config, IWin32Window owner)
        {
            var instance = CreateWizard(config);
            return instance.ShowDialog(owner);
        }

        /// <summary>
        /// Get active wizard by key
        /// </summary>
        public static WizardInstance GetWizard(string key)
        {
            _activeWizards.TryGetValue(key, out var wizard);
            return wizard;
        }

        /// <summary>
        /// Close wizard by key
        /// </summary>
        public static void CloseWizard(string key)
        {
            if (_activeWizards.TryGetValue(key, out var wizard))
            {
                wizard.Close();
                _activeWizards.Remove(key);
            }
        }

        /// <summary>
        /// Close all active wizards
        /// </summary>
        public static void CloseAllWizards()
        {
            foreach (var wizard in _activeWizards.Values.ToList())
            {
                wizard.Close();
            }
            _activeWizards.Clear();
        }

        /// <summary>
        /// Unregister wizard (called internally when wizard closes)
        /// </summary>
        internal static void UnregisterWizard(string key)
        {
            _activeWizards.Remove(key);
        }
    }
}
