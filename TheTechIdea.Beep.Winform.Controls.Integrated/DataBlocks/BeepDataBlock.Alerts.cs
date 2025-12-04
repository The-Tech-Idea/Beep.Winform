using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Alert System
    /// Provides Oracle Forms ALERT equivalent functionality
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Alert Methods
        
        /// <summary>
        /// Show a custom alert dialog
        /// Oracle Forms equivalent: SHOW_ALERT
        /// </summary>
        public DialogResult ShowAlert(string title, string message, AlertStyle style = AlertStyle.Information, AlertButtons buttons = AlertButtons.Ok)
        {
            // Convert to MessageBox equivalents
            MessageBoxIcon icon = style switch
            {
                AlertStyle.Stop => MessageBoxIcon.Stop,
                AlertStyle.Caution => MessageBoxIcon.Warning,
                AlertStyle.Information => MessageBoxIcon.Information,
                AlertStyle.Question => MessageBoxIcon.Question,
                _ => MessageBoxIcon.None
            };
            
            MessageBoxButtons mboxButtons = buttons switch
            {
                AlertButtons.Ok => MessageBoxButtons.OK,
                AlertButtons.OkCancel => MessageBoxButtons.OKCancel,
                AlertButtons.YesNo => MessageBoxButtons.YesNo,
                AlertButtons.YesNoCancel => MessageBoxButtons.YesNoCancel,
                AlertButtons.RetryCancel => MessageBoxButtons.RetryCancel,
                _ => MessageBoxButtons.OK
            };
            
            // Fire ON-MESSAGE trigger before showing
            var alertContext = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.OnMessage,
                Parameters = new Dictionary<string, object>
                {
                    ["Title"] = title,
                    ["Message"] = message,
                    ["Style"] = style,
                    ["Buttons"] = buttons
                }
            };
            
            _ = ExecuteTriggers(TriggerType.OnMessage, alertContext);
            
            // Show alert
            return MessageBox.Show(message, title, mboxButtons, icon);
        }
        
        /// <summary>
        /// Show confirmation dialog (Yes/No)
        /// </summary>
        public bool ConfirmAction(string message, string title = "Confirm")
        {
            return ShowAlert(title, message, AlertStyle.Question, AlertButtons.YesNo) == DialogResult.Yes;
        }
        
        /// <summary>
        /// Show OK/Cancel confirmation
        /// </summary>
        public bool ConfirmOkCancel(string message, string title = "Confirm")
        {
            return ShowAlert(title, message, AlertStyle.Question, AlertButtons.OkCancel) == DialogResult.OK;
        }
        
        /// <summary>
        /// Show information alert
        /// </summary>
        public void ShowInfo(string message, string title = "Information")
        {
            ShowAlert(title, message, AlertStyle.Information, AlertButtons.Ok);
        }
        
        /// <summary>
        /// Show warning alert
        /// </summary>
        public void ShowWarning(string message, string title = "Warning")
        {
            ShowAlert(title, message, AlertStyle.Caution, AlertButtons.Ok);
        }
        
        /// <summary>
        /// Show error alert
        /// </summary>
        public void ShowError(string message, string title = "Error")
        {
            ShowAlert(title, message, AlertStyle.Stop, AlertButtons.Ok);
        }
        
        #endregion
    }
    
    #region Supporting Enums
    
    /// <summary>
    /// Alert style (icon type)
    /// Oracle Forms equivalent: Alert_Style property
    /// </summary>
    public enum AlertStyle
    {
        Stop,
        Caution,
        Information,
        Question,
        None
    }
    
    /// <summary>
    /// Alert buttons
    /// Oracle Forms equivalent: Alert_Button property
    /// </summary>
    public enum AlertButtons
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
        RetryCancel
    }
    
    #endregion
}
