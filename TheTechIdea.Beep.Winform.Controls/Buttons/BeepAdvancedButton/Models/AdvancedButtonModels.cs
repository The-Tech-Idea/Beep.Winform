using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models
{
    /// <summary>
    /// Event args for toggle state changes
    /// </summary>
    public class ToggleChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new toggle state
        /// </summary>
        public bool IsToggled { get; set; }

        /// <summary>
        /// Which area was clicked (for Split buttons: "Left", "Right", or null for regular toggle)
        /// </summary>
        public string? AreaClicked { get; set; }

        public ToggleChangedEventArgs(bool isToggled, string? areaClicked = null)
        {
            IsToggled = isToggled;
            AreaClicked = areaClicked;
        }
    }

    /// <summary>
    /// Context object containing all data needed for button painting
    /// </summary>
    public class AdvancedButtonPaintContext
    {
        public NewsBannerStyle NewsBannerStyle { get; set; }
        public ContactButtonLayout ContactLayout { get; set; }
        public Graphics Graphics { get; set; }
        public Rectangle Bounds { get; set; }
        public AdvancedButtonState State { get; set; }
        public string Text { get; set; }
        public ImagePainter ImagePainter { get; set; }
        public string IconLeft { get; set; }
        public string IconRight { get; set; }
        public bool IsToggled { get; set; }
        public bool IsLoading { get; set; }
        public BeepTheme Theme { get; set; }
        public Font Font { get; set; }
        
        // Shape property - determines how painters render the button
        public ButtonShape Shape { get; set; }
        
        // Split button area states (for Toggle with Split shape)
        public bool LeftAreaHovered { get; set; }
        public bool RightAreaHovered { get; set; }
        public bool LeftAreaPressed { get; set; }
        public bool RightAreaPressed { get; set; }
        
        // Border properties
        public int BorderRadius { get; set; }
        public int BorderWidth { get; set; }
        public Color BorderColor { get; set; }
        
        // Color properties
        public Color SolidBackground { get; set; }
        public Color SolidForeground { get; set; }
        public Color HoverBackground { get; set; }
        public Color HoverForeground { get; set; }
        public Color PressedBackground { get; set; }
        public Color DisabledBackground { get; set; }
        public Color DisabledForeground { get; set; }
        public Color GlowColor { get; set; }
        public Color RippleColor { get; set; }
        public Color LoadingIndicatorColor { get; set; }
        public Color ToggleOnColor { get; set; }
        public Color ToggleOffColor { get; set; }
        public Color ToggleBorderColor { get; set; }
        public Color TextColor { get; set; }
        public Color IconColor { get; set; }
        public Color BackgroundColor { get; set; }
    public Color SecondaryColor { get; set; }

        // Shadow properties
        public bool ShowShadow { get; set; }
        public int ShadowBlur { get; set; }
        public Color ShadowColor { get; set; }
        
        // Animation properties
        public bool RippleActive { get; set; }
        public Point RippleCenter { get; set; }
        public float RippleProgress { get; set; }
        
        // Size
        public AdvancedButtonSize ButtonSize { get; set; }
        
        // Button shape for rendering
        public ButtonShape ButtonShape { get; set; }
        public ChevronStyle ChevronStyle { get; set; }

        // Chip/Tag specific properties
        public bool ShowCloseIcon { get; set; }
        public bool IsBadge { get; set; }
        public string BadgeText { get; set; }
        public Color BadgeColor { get; set; }
        public bool IsSelected { get; set; }
        public bool IsOutlined { get; set; }
        
        // Contact button specific properties
        public Color IconBackgroundColor { get; set; }
        public bool IconCircleBackground { get; set; }
        public bool IconAngledBackground { get; set; }
        public bool IconArrowBackground { get; set; }
        
        // State flags
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public bool ShowBorder { get; set; }
        public int BorderThickness { get; set; }
        
       
    }

    /// <summary>
    /// Measurement data for button layout calculations
    /// </summary>
    public class AdvancedButtonMetrics
    {
        public int PaddingHorizontal { get; set; }
        public int PaddingVertical { get; set; }
        public int IconSize { get; set; }
        public int IconTextGap { get; set; }
        public int MinWidth { get; set; }
        public int Height { get; set; }
        public int BorderRadius { get; set; }
        
        /// <summary>
        /// Get metrics based on button size
        /// </summary>
        public static AdvancedButtonMetrics GetMetrics(AdvancedButtonSize size)
        {
            return size switch
            {
                AdvancedButtonSize.Small => new AdvancedButtonMetrics
                {
                    PaddingHorizontal = 12,
                    PaddingVertical = 6,
                    IconSize = 16,
                    IconTextGap = 8,
                    MinWidth = 64,
                    Height = 32,
                    BorderRadius = 6
                },
                AdvancedButtonSize.Medium => new AdvancedButtonMetrics
                {
                    PaddingHorizontal = 16,
                    PaddingVertical = 8,
                    IconSize = 20,
                    IconTextGap = 8,
                    MinWidth = 80,
                    Height = 40,
                    BorderRadius = 8
                },
                AdvancedButtonSize.Large => new AdvancedButtonMetrics
                {
                    PaddingHorizontal = 20,
                    PaddingVertical = 12,
                    IconSize = 24,
                    IconTextGap = 12,
                    MinWidth = 96,
                    Height = 48,
                    BorderRadius = 10
                },
                _ => GetMetrics(AdvancedButtonSize.Medium)
            };
        }
    }
}
