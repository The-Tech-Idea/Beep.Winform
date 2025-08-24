using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
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
        Ribbon,        // Office-style with ribbon area
        Mobile,        // Mobile-inspired touch layout
        Console,       // Terminal/IDE style
        Floating       // Floating panel style
    }



   
}
