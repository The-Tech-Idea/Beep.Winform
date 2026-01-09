using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Switchs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Core fields and initialization
    /// </summary>
    public partial class BeepSwitch
    {
        #region Fields

        // State
        private bool _checked = false;
        private SwitchOrientation _orientation = SwitchOrientation.Horizontal;
        
        // Labels
        private string _onLabel = "On";
        private string _offLabel = "Off";
        
        // Images (legacy)
        private string? _onImagePath;
        private string? _offImagePath;
        private BeepImage _onBeepImage;
        private BeepImage _offBeepImage;
        
        // Icon library integration (NEW)
        private string _onIconName = string.Empty;
        private string _offIconName = string.Empty;
        
        // Layout metrics
        private SwitchMetrics _metrics = new SwitchMetrics();
        
        // Painter system
        private ISwitchPainter _painter;
        
        // Animation
        private Timer _animTimer;
        private float _animProgress = 0f;  // 0.0 = Off, 1.0 = On
        private bool _animating = false;
        
        // Drag support
        private bool _dragging = false;
        private int _dragStartX = 0;
        private bool _dragToToggleEnabled = true;

        #endregion

        #region Constructor

        public BeepSwitch()
        {
            // Set default size
            this.Width = 120;
            this.Height = 50;
            
            // Enable double buffering for smooth rendering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint | 
                         ControlStyles.OptimizedDoubleBuffer | 
                         ControlStyles.ResizeRedraw, true);

            // Initialize BeepImage instances (legacy support)
            _onBeepImage = new BeepImage
            {
                IsSpinning = false,
                ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio
            };
            _offBeepImage = new BeepImage
            {
                IsSpinning = false,
                ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio
            };
            
            // Initialize painter based on ControlStyle
            InitializePainter();
            
            // Set default control style if not inherited
            if (ControlStyle == Common.BeepControlStyle.None)
            {
                ControlStyle = Common.BeepControlStyle.iOS15;  // Default to iOS style
            }
        }

        private void InitializePainter()
        {
            _painter = SwitchPainterFactory.CreatePainter(ControlStyle, this);
        }

        #endregion
        
        // NOTE: Disposal is in original BeepSwitch.cs
    }
}

