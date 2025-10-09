﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ContextMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Modern BeepContextMenu implementation using painter methodology
    /// A context menu form with advanced styling and integrated helper architecture
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Context Menu")]
    [Category("Beep Controls")]
    [Description("A modern context menu with advanced styling, painter methodology, and integrated features.")]
    public partial class BeepContextMenu : Form
    {
        // All implementation is in partial classes:
        // - BeepContextMenu.Core.cs: Core fields and initialization
        // - BeepContextMenu.Properties.cs: All properties
        // - BeepContextMenu.Events.cs: Event handlers
        // - BeepContextMenu.Methods.cs: Public methods
        // - BeepContextMenu.Drawing.cs: Paint override and painting logic
    }
}
