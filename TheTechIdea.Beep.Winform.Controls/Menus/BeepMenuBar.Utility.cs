// BeepMenuBar.Utility.cs
// Phase 02 — Partial-Class Split.
//
// Hosts utility helpers that do not belong to layout, drawing, input,
// theming, or lifecycle: sample data loading and the global-function
// dispatcher used when a menu item carries a MethodName.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        /// <summary>
        /// Populates the menu bar with five sample items. Intended for
        /// designer previews and the Phase 09 demo form.
        /// </summary>
        public void LoadSampleMenuItems()
        {
            var sampleItems = new BindingList<SimpleItem>
            {
                new SimpleItem { Text = "File",  Description = "File operations",  ImagePath = "file"  },
                new SimpleItem { Text = "Edit",  Description = "Edit operations",  ImagePath = "edit"  },
                new SimpleItem { Text = "View",  Description = "View options",     ImagePath = "view"  },
                new SimpleItem { Text = "Tools", Description = "Tool options",     ImagePath = "tools" },
                new SimpleItem { Text = "Help",  Description = "Help and support", ImagePath = "help"  }
            };

            MenuItems = sampleItems;
        }

        /// <summary>
        /// Delegate invoked to run a method bound to a <see cref="SimpleItem.MethodName"/>.
        /// The host wires this to resolve and invoke the method; the menubar only
        /// dispatches through it.
        /// </summary>
        public delegate void RunFunctionFromExtensions(SimpleItem item, string MethodName);

        /// <summary>
        /// Host-provided dispatcher for menu items that carry a
        /// <see cref="SimpleItem.MethodName"/>. When null, a method-bound item reports
        /// <see cref="Errors.Failed"/> rather than fabricating success.
        /// </summary>
        public RunFunctionFromExtensions RunFunctionFromExtensionsHandler { get; set; }

        /// <summary>
        /// Invokes a global function bound to a menu item via SimpleItem.MethodName.
        /// </summary>
        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string MethodName)
        {
            var errorsInfo = new ErrorsInfo();
            try
            {
                if (RunFunctionFromExtensionsHandler == null)
                {
                    errorsInfo.Flag    = Errors.Failed;
                    errorsInfo.Message = $"No {nameof(RunFunctionFromExtensionsHandler)} is wired to invoke '{MethodName}'.";
                    return errorsInfo;
                }

                RunFunctionFromExtensionsHandler(item, MethodName);
                errorsInfo.Flag = Errors.Ok;
            }
            catch (Exception ex)
            {
                errorsInfo.Flag    = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex      = ex;
                BeepLog.Failure(this, $"RunMethodFromGlobalFunctions('{MethodName}')", ex);
            }
            return errorsInfo;
        }
    }
}
