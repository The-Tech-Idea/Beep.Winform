using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Base designer for all widget controls
    /// Provides common design-time functionality for widgets
    /// </summary>
    public abstract class BaseWidgetDesigner : BaseBeepControlDesigner
    {
        /// <summary>
        /// Gets the widget control being designed
        /// </summary>
        protected BaseControl? Widget => Component as BaseControl;
    }
}
