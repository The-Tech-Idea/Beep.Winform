using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Message/Status Line System
    /// Provides Oracle Forms message line equivalent
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        private Queue<BlockMessage> _messageQueue = new Queue<BlockMessage>();
        private BlockMessage _currentMessage;
        private System.Windows.Forms.Timer _messageTimer;
        private int _messageDisplayDuration = 3000;  // 3 seconds default
        private bool _autoHideMessages = true;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Duration to display messages (milliseconds)
        /// </summary>
        public int MessageDisplayDuration
        {
            get => _messageDisplayDuration;
            set => _messageDisplayDuration = Math.Max(1000, value);
        }
        
        /// <summary>
        /// Whether messages auto-hide after duration
        /// </summary>
        public bool AutoHideMessages
        {
            get => _autoHideMessages;
            set => _autoHideMessages = value;
        }
        
        /// <summary>
        /// Current message being displayed. When coordinated, delegates to FormsManager.
        /// </summary>
        public string CurrentMessage
        {
            get
            {
                if (IsCoordinated && _formsManager?.Messages != null)
                {
                    try { return _formsManager.Messages.GetCurrentMessage(this.Name); }
                    catch { }
                }
                return _currentMessage?.Text;
            }
        }
        
        /// <summary>
        /// Current message level. When coordinated, delegates to FormsManager.
        /// </summary>
        public MessageLevel CurrentMessageLevel
        {
            get
            {
                if (IsCoordinated && _formsManager?.Messages != null)
                {
                    try { return (MessageLevel)(int)_formsManager.Messages.GetCurrentMessageLevel(this.Name); }
                    catch { }
                }
                return _currentMessage?.Level ?? MessageLevel.Info;
            }
        }
        
        #endregion
        
        #region Message Methods
        
        /// <summary>
        /// Set a message to display
        /// Oracle Forms equivalent: MESSAGE built-in
        /// </summary>
        public void SetMessage(string text, MessageLevel level = MessageLevel.Info)
        {
            // Delegate to FormsManager when coordinated
            if (IsCoordinated && _formsManager?.Messages != null)
            {
                _formsManager.Messages.SetMessage(this.Name, text,
                    (Editor.Forms.Models.MessageLevel)(int)level);
                return;
            }

            var message = new BlockMessage
            {
                Text = text,
                Level = level,
                Timestamp = DateTime.Now
            };
            
            // Update SYSTEM.MESSAGE_TEXT
            if (SYSTEM != null)
            {
                SYSTEM.MESSAGE_TEXT = text;
            }
            
            // Queue or display immediately
            if (_currentMessage == null)
            {
                DisplayMessage(message);
            }
            else
            {
                _messageQueue.Enqueue(message);
            }
        }
        
        /// <summary>
        /// Clear current message
        /// </summary>
        public void ClearMessage()
        {
            if (IsCoordinated && _formsManager?.Messages != null)
                _formsManager.Messages.ClearMessage(this.Name);

            _messageTimer?.Stop();
            _currentMessage = null;
            
            if (SYSTEM != null)
            {
                SYSTEM.MESSAGE_TEXT = "";
            }
            
            // Show next queued message if any
            ShowNextMessage();
        }
        
        /// <summary>
        /// Show info message
        /// </summary>
        public void ShowInfoMessage(string text)
        {
            SetMessage(text, MessageLevel.Info);
        }
        
        /// <summary>
        /// Show success message
        /// </summary>
        public void ShowSuccessMessage(string text)
        {
            SetMessage(text, MessageLevel.Success);
        }
        
        /// <summary>
        /// Show warning message
        /// </summary>
        public void ShowWarningMessage(string text)
        {
            SetMessage(text, MessageLevel.Warning);
        }
        
        /// <summary>
        /// Show error message
        /// </summary>
        public void ShowErrorMessage(string text)
        {
            SetMessage(text, MessageLevel.Error);
        }
        
        #endregion
        
        #region Internal Message Display
        
        private void DisplayMessage(BlockMessage message)
        {
            _currentMessage = message;
            
            // Start auto-hide timer
            if (_autoHideMessages)
                StartMessageTimer();
            
            // Update status (could be status bar, label, etc.)
            Status = message.Text;
            
            // Fire message event for custom UI
            OnMessageDisplayed?.Invoke(this, new MessageEventArgs { Message = message });
        }
        
        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            ClearMessage();
        }

        /// <summary>
        /// Start (or restart) the auto-hide message timer.
        /// Called by Coordination.cs after attaching message manager events.
        /// </summary>
        public void StartMessageTimer()
        {
            if (!_autoHideMessages) return;
            if (_messageTimer == null)
            {
                _messageTimer = new System.Windows.Forms.Timer();
                _messageTimer.Tick += MessageTimer_Tick;
            }
            _messageTimer.Stop();
            _messageTimer.Interval = _messageDisplayDuration;
            _messageTimer.Start();
        }
        
        private void ShowNextMessage()
        {
            if (_messageQueue.Count > 0)
            {
                var nextMessage = _messageQueue.Dequeue();
                DisplayMessage(nextMessage);
            }
        }
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Fired when a message is displayed
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessageDisplayed;
        
        #endregion
    }
    
    #region Supporting Classes
    
    /// <summary>
    /// Message information
    /// </summary>
    public class BlockMessage
    {
        public string Text { get; set; }
        public MessageLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    /// <summary>
    /// Message level
    /// </summary>
    public enum MessageLevel
    {
        Info,
        Success,
        Warning,
        Error
    }
    
    /// <summary>
    /// Message event args
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public BlockMessage Message { get; set; }
    }
    
    #endregion
}
