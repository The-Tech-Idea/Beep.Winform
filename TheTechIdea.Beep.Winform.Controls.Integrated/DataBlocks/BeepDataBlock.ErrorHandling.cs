using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Centralized Error Handling
    /// Provides consistent, testable, flexible error handling
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        private List<ErrorInfo> _errorLog = new List<ErrorInfo>();
        private const int MaxErrorLogSize = 100;
        private bool _suppressErrorDialogs = false;
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Fired when an error occurs
        /// Subscribe to this for custom error handling (logging, UI, etc.)
        /// </summary>
        public event EventHandler<DataBlockErrorEventArgs> OnError;
        
        /// <summary>
        /// Fired when a warning occurs
        /// </summary>
        public event EventHandler<DataBlockErrorEventArgs> OnWarning;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Whether to suppress error dialogs (for testing or custom error UI)
        /// </summary>
        public bool SuppressErrorDialogs
        {
            get => _suppressErrorDialogs;
            set => _suppressErrorDialogs = value;
        }
        
        /// <summary>
        /// Get error log
        /// </summary>
        public IReadOnlyList<ErrorInfo> ErrorLog => _errorLog.AsReadOnly();
        
        #endregion
        
        #region Error Handling Methods
        
        /// <summary>
        /// Handle an error (centralized error handling)
        /// </summary>
        protected void HandleError(Exception ex, string context, ErrorSeverity severity = ErrorSeverity.Error)
        {
            var errorInfo = new ErrorInfo
            {
                Exception = ex,
                Context = context,
                Severity = severity,
                Timestamp = DateTime.Now,
                BlockName = this.Name
            };
            
            // Add to error log
            AddToErrorLog(errorInfo);
            
            // Create event args
            var args = new DataBlockErrorEventArgs
            {
                Exception = ex,
                Context = context,
                Block = this,
                Severity = severity,
                ErrorInfo = errorInfo
            };
            
            // Fire event
            if (severity == ErrorSeverity.Warning)
            {
                OnWarning?.Invoke(this, args);
            }
            else
            {
                OnError?.Invoke(this, args);
            }
            
            // Default handling if not handled by event
            if (!args.Handled && !_suppressErrorDialogs)
            {
                var icon = severity == ErrorSeverity.Warning ? MessageBoxIcon.Warning : MessageBoxIcon.Error;
                var title = severity == ErrorSeverity.Warning ? "Warning" : "Error";
                
                MessageBox.Show(
                    $"{context}\n\n{ex.Message}",
                    title,
                    MessageBoxButtons.OK,
                    icon);
            }
            
            // Fire ON-ERROR trigger
            var triggerContext = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.OnError,
                ErrorMessage = ex.Message,
                Parameters = new Dictionary<string, object>
                {
                    ["Exception"] = ex,
                    ["Context"] = context,
                    ["Severity"] = severity
                }
            };
            
            _ = ExecuteTriggers(TriggerType.OnError, triggerContext);
        }
        
        /// <summary>
        /// Handle a warning
        /// </summary>
        protected void HandleWarning(string message, string context)
        {
            var warningEx = new InvalidOperationException(message);
            HandleError(warningEx, context, ErrorSeverity.Warning);
        }
        
        /// <summary>
        /// Clear error log
        /// </summary>
        public void ClearErrorLog()
        {
            _errorLog.Clear();
        }
        
        /// <summary>
        /// Get errors for a specific context
        /// </summary>
        public List<ErrorInfo> GetErrorsForContext(string context)
        {
            return _errorLog.Where(e => e.Context == context).ToList();
        }
        
        #endregion
        
        #region Internal Methods
        
        private void AddToErrorLog(ErrorInfo error)
        {
            _errorLog.Add(error);
            
            // Trim log if too large
            if (_errorLog.Count > MaxErrorLogSize)
            {
                _errorLog.RemoveAt(0);
            }
        }
        
        #endregion
    }
    
    #region Supporting Classes
    
    /// <summary>
    /// Error information
    /// </summary>
    public class ErrorInfo
    {
        public Exception Exception { get; set; }
        public string Context { get; set; }
        public ErrorSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; }
        public string BlockName { get; set; }
        
        public TimeSpan Age => DateTime.Now - Timestamp;
    }
    
    /// <summary>
    /// Error severity
    /// </summary>
    public enum ErrorSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }
    
    /// <summary>
    /// Error event args
    /// </summary>
    public class DataBlockErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string Context { get; set; }
        public BeepDataBlock Block { get; set; }
        public ErrorSeverity Severity { get; set; }
        public ErrorInfo ErrorInfo { get; set; }
        public bool Handled { get; set; }
    }
    
    #endregion
}

