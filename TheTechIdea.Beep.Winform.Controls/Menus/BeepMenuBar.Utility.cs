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
        /// Invokes a global function bound to a menu item via SimpleItem.MethodName.
        /// </summary>
        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string MethodName)
        {
            ErrorsInfo errorsInfo = new ErrorsInfo();
            try
            {
                OnMenuItemSelected(item);
                //SimpleItemFactory.RunFunctionWithTreeHandler(item, MethodName);
            }
            catch (Exception ex)
            {
                errorsInfo.Flag    = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex      = ex;
            }
            return errorsInfo;
        }
    }
}
