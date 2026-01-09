using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Core fields, properties, and initialization for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Helper Instance"
        
        /// <summary>
        /// The main helper that coordinates all functionality
        /// </summary>
        private BeepSimpleTextBoxHelper _helper;
        
        /// <summary>
        /// Gets the helper instance for internal use
        /// </summary>
        internal BeepSimpleTextBoxHelper Helper => _helper;
        
        #endregion
        
        #region "Core Fields"
        
        private BeepImage _beepImage;
        private string _placeholderText = "";
        private Rectangle _textRect;
        private List<string> _lines = new List<string>();
        private bool _isInitializing = true;
        
        // Performance optimizations
        private Timer _delayedUpdateTimer;
        private bool _needsTextUpdate = false;
        private bool _needsLayoutUpdate = false;
        
        // Animation support
        private Timer _animationTimer;
        private float _focusAnimationProgress = 0f;
        private bool _isFocusAnimating = false;
        
        // Smart features
        private DateTime _lastKeyPressTime = DateTime.MinValue;
        private bool _isTyping = false;
        private Timer _typingTimer;

        // Cached metrics to avoid CreateGraphics during resize/measure
        private int _cachedTextHeightPx = -1; // height of "Aj" in current font
        private int _cachedMinHeightPx = -1;  // text height + padding + borders
        
        #endregion
        
        #region "Events"
        
        public new event EventHandler TextChanged;
        public event EventHandler SearchTriggered;
        public event EventHandler<EventArgs> TypingStarted;
        public event EventHandler<EventArgs> TypingStopped;
        
        #endregion
        
        #region "Constructor"
        
        public BeepTextBox() : base()
        {
            _isInitializing = true;
            
            InitializeComponents();
            SetControlStyles();
            InitializeProperties();
            InitializeTimers();
            
            _helper = new BeepSimpleTextBoxHelper(this);
            
            // compute cached sizes early using TextRenderer which is safe without Graphics
            RecomputeMinHeight();

            UpdateDrawingRect();
            UpdateLines();
            
            _isInitializing = false;
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializeComponents()
        {
            _beepImage = new BeepImage()
            {
                Size = _maxImageSize,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ClipShape = ImageClipShape.None,
                Visible = false,
                IsFrameless = true,
                ImageEmbededin = ImageEmbededin.TextBox,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            
            if (_currentTheme != null)
            {
                _beepImage.Theme = _currentTheme.ToString();
                _beepImage.BackColor = _currentTheme.TextBoxBackColor;
            }
        }
        
        private void SetControlStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.ContainerControl, false);
        }
        
        private void InitializeProperties()
        {
            _text = string.Empty;
            BoundProperty = "Text";
            ShowAllBorders = true;
            TabStop = true;
        }
        
        private void InitializeTimers()
        {
            // Delayed update timer for performance
            _delayedUpdateTimer = new Timer()
            {
                Interval = 50 // 50ms delay for batching updates
            };
            _delayedUpdateTimer.Tick += DelayedUpdateTimer_Tick;
            
            // Animation timer
            _animationTimer = new Timer()
            {
                Interval = 16 // ~60 FPS
            };
            _animationTimer.Tick += AnimationTimer_Tick;
            
            // Typing timer
            _typingTimer = new Timer()
            {
                Interval = 1000 // 1 second
            };
            _typingTimer.Tick += TypingTimer_Tick;
            
            // Incremental search timer
            _incrementalSearchTimer = new Timer()
            {
                Interval = 300 // 300ms delay for incremental search
            };
            _incrementalSearchTimer.Tick += IncrementalSearchTimer_Tick;
        }
        
        protected override Size DefaultSize => new Size(200, 34);
        
        #endregion
        
        #region "Cached metrics helpers"
        private void RecomputeMinHeight()
        {
            try
            {
                // Use TextUtils for cached measurement (no Graphics needed, but TextUtils can work without Graphics)
                var fontToUse = _textFont ?? Font ?? SystemFonts.MessageBoxFont;
                // TextUtils requires Graphics, so use TextRenderer here for initialization (before Graphics is available)
                var sz = TextRenderer.MeasureText("Aj", fontToUse, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                _cachedTextHeightPx = Math.Max(1, sz.Height);
                _cachedMinHeightPx = _cachedTextHeightPx + Padding.Vertical + (_borderWidth * 2);

                if (!_multiline && _cachedMinHeightPx > 0)
                {
                    if (MinimumSize.Height < _cachedMinHeightPx)
                    {
                        MinimumSize = new Size(MinimumSize.Width, _cachedMinHeightPx);
                    }
                }
            }
            catch
            {
                // Fallback to a reasonable default if measure fails
                _cachedTextHeightPx = Math.Max(12, (int)(_textFont?.Size ?? 10));
                _cachedMinHeightPx = _cachedTextHeightPx + Padding.Vertical + (_borderWidth * 2);
            }
        }
        #endregion
        
        #region "Cleanup"
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _delayedUpdateTimer?.Stop();
                _delayedUpdateTimer?.Dispose();
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _typingTimer?.Stop();
                _typingTimer?.Dispose();
                _incrementalSearchTimer?.Stop();
                _incrementalSearchTimer?.Dispose();
                
                _helper?.Dispose();
                _textFont?.Dispose();
                _lineNumberFont?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        #endregion
    }
}
