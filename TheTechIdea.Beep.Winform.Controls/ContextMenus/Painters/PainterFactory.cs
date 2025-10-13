using System;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    public static class PainterFactory
    {
        public static IContextMenuPainter CreateContextMenuPainter(FormStyle style)
        {
            return style switch
            {
                // Core Styles - IMPLEMENTED
                FormStyle.Modern => new ModernContextMenuPainter(),
                FormStyle.Minimal => new MinimalContextMenuPainter(),
                FormStyle.Material => new MaterialContextMenuPainter(),
                FormStyle.Fluent => new FluentContextMenuPainter(),
                
                // Platform-Inspired - IMPLEMENTED
                FormStyle.MacOS => new MacOSContextMenuPainter(),
                FormStyle.iOS => new iOSContextMenuPainter(),
                FormStyle.GNOME => new MaterialContextMenuPainter(), // TODO: Create GNOMEContextMenuPainter
                FormStyle.KDE => new MaterialContextMenuPainter(), // TODO: Create KDEContextMenuPainter
                FormStyle.Ubuntu => new MaterialContextMenuPainter(), // TODO: Create UbuntuContextMenuPainter
                
                // Designer Styles - IMPLEMENTED
                FormStyle.Glass => new GlassContextMenuPainter(),
                FormStyle.Cartoon => new CartoonContextMenuPainter(),
                FormStyle.ChatBubble => new MaterialContextMenuPainter(), // TODO: Create ChatBubbleContextMenuPainter
                FormStyle.Metro => new StandardContextMenuPainter(), // TODO: Create MetroContextMenuPainter
                FormStyle.Metro2 => new StandardContextMenuPainter(), // TODO: Create Metro2ContextMenuPainter
                FormStyle.NeoMorphism => new MaterialContextMenuPainter(), // TODO: Create NeoMorphismContextMenuPainter
                FormStyle.Glassmorphism => new GlassContextMenuPainter(), // Similar to Glass
                FormStyle.Brutalist => new MinimalContextMenuPainter(), // TODO: Create BrutalistContextMenuPainter
                
                // Theme-Based Styles - TODO
                FormStyle.Dracula => new MaterialContextMenuPainter(), // TODO: Create DraculaContextMenuPainter
                FormStyle.Solarized => new MaterialContextMenuPainter(), // TODO: Create SolarizedContextMenuPainter
                FormStyle.OneDark => new MaterialContextMenuPainter(), // TODO: Create OneDarkContextMenuPainter
                FormStyle.GruvBox => new MaterialContextMenuPainter(), // TODO: Create GruvBoxContextMenuPainter
                FormStyle.Nord => new MaterialContextMenuPainter(), // TODO: Create NordContextMenuPainter
                FormStyle.Tokyo => new MaterialContextMenuPainter(), // TODO: Create TokyoContextMenuPainter
                
                // Effect Styles - TODO
                FormStyle.Retro => new StandardContextMenuPainter(), // TODO: Create RetroContextMenuPainter
                FormStyle.Cyberpunk => new MaterialContextMenuPainter(), // TODO: Create CyberpunkContextMenuPainter
                FormStyle.Neon => new MaterialContextMenuPainter(), // TODO: Create NeonContextMenuPainter
                FormStyle.Holographic => new MaterialContextMenuPainter(), // TODO: Create HolographicContextMenuPainter
                
                // Specialized - TODO
                FormStyle.Nordic => new MinimalContextMenuPainter(), // TODO: Create NordicContextMenuPainter
                FormStyle.ArcLinux => new MaterialContextMenuPainter(), // TODO: Create ArcLinuxContextMenuPainter
                FormStyle.Paper => new FlatContextMenuPainter(), // TODO: Rename to PaperContextMenuPainter
                FormStyle.Custom => new StandardContextMenuPainter(), // User customizable
                
                // Default fallback
                _ => new StandardContextMenuPainter()
            };
        }

        public static bool HasDedicatedPainter(FormStyle style)
        {
            return style switch
            {
                FormStyle.Modern => true,
                FormStyle.Minimal => true,
                FormStyle.Material => true,
                FormStyle.Fluent => true,
                FormStyle.MacOS => true,
                FormStyle.iOS => true,
                FormStyle.Glass => true,
                FormStyle.Cartoon => true,
                FormStyle.Glassmorphism => true, // Uses Glass
                _ => false
            };
        }

        public static string GetPainterInfo(FormStyle style)
        {
            if (HasDedicatedPainter(style))
            {
                return $"Dedicated {style} painter";
            }
            else
            {
                var fallback = CreateContextMenuPainter(style);
                return $"Using {fallback.GetType().Name} as temporary fallback";
            }
        }
    }
}
