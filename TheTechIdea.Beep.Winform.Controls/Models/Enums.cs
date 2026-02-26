using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public enum ArrowDirection { Up, Down }
    /// <summary>
    /// Material elevation levels (0-5)
    /// </summary>
    public enum MaterialElevation
    {
        Level0 = 0,  // No shadow
        Level1 = 1,  // 1dp elevation
        Level2 = 2,  // 2dp elevation
        Level3 = 3,  // 4dp elevation
        Level4 = 4,  // 8dp elevation
        Level5 = 5   // 16dp elevation
    }

    /// <summary>
    /// Control state enum for consistent state handling
    /// </summary>
    public enum ControlState
    {
        Normal,
        Hovered,
        Pressed,
        Selected,
        Disabled,
        Focused
    }

    /// <summary>
    /// Enum for external drawing layers
    /// </summary>
    public enum DrawingLayer
    {
        BeforeContent,
        AfterContent,
        AfterAll
    }
    
    public enum FormUIStyle
    {
        Mac,           // macOS traffic lights on left
        Modern,        // Standard Windows layout
        Classic,       // Windows XP compact
        Minimal,       // Ultra-thin, no icon
        Material,      // Tall Google Material Design
        Fluent,        // Microsoft Fluent Design
        Ribbon,        // Office-Style with ribbon area
        Mobile,        // Mobile-inspired touch layout
        Console,       // Terminal/IDE Style
        Floating       // Floating panel Style
    }

    /// <summary>
    /// Visual presets for BeepComboBox styling, mapped to Material variants + custom tweaks.
    /// </summary>
    public enum BeepComboBoxVariant
    {
        Outlined = 0,       // Rectangular outline
        Underline = 1,      // Line-only bottom (Material Standard)
        RoundedOutlined = 2,// Pill outline
        Filled = 3,         // Filled rectangle
        RoundedFilled = 4,  // Filled pill
        Subtle = 5          // Low-contrast filled (disabled-like)
    }
}
