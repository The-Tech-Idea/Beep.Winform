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
        /// Current message being displayed
        /// </summary>
        public string CurrentMessage => _currentMessage?.Text;
        
        /// <summary>
        /// Current message level
        /// </summary>
        public MessageLevel CurrentMessageLevel => _currentMessage?.Level ?? MessageLevel.Info;
        
        #endregion
        
        #region Message Methods
        
        /// <summary>
        /// Set a message to display
        /// Oracle Forms equivalent: MESSAGE built-in
        /// </summary>
        public void SetMessage(string text, MessageLevel level = MessageLevel.Info)
        {
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
            
            // Create timer for auto-hide
            if (_autoHideMessages)
            {
                if (_messageTimer == null)
                {
                    _messageTimer = new System.Windows.Forms.Timer();
                    _messageTimer.Tick += MessageTimer_Tick;
                }
                
                _messageTimer.Stop();
                _messageTimer.Interval = _messageDisplayDuration;
                _messageTimer.Start();
            }
            
            // Update status (could be status bar, label, etc.)
            Status = message.Text;
            
            // Fire message event for custom UI
            OnMessageDisplayed?.Invoke(this, new MessageEventArgs { Message = message });
        }
        
        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            ClearMessage();
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
