using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Factory for creating border painter instances based on BeepControlStyle
    /// </summary>
    public static class BorderPainterFactory
    {
        /// <summary>
        /// Creates a border painter instance for the specified Style
        /// </summary>
        /// <param name="style">The control Style to create a painter for</param>
        /// <returns>An IBorderPainter implementation, or null for None Style</returns>
        public static IBorderPainter CreatePainter(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.None => null,
                BeepControlStyle.Material3 => new Material3BorderPainterWrapper(),
                BeepControlStyle.iOS15 => new iOS15BorderPainterWrapper(),
                BeepControlStyle.AntDesign => new AntDesignBorderPainterWrapper(),
                BeepControlStyle.Fluent2 => new Fluent2BorderPainterWrapper(),
                BeepControlStyle.MaterialYou => new MaterialYouBorderPainterWrapper(),
                BeepControlStyle.Windows11Mica => new Windows11MicaBorderPainterWrapper(),
                BeepControlStyle.MacOSBigSur => new MacOSBigSurBorderPainterWrapper(),
                BeepControlStyle.ChakraUI => new ChakraUIBorderPainterWrapper(),
                BeepControlStyle.TailwindCard => new TailwindCardBorderPainterWrapper(),
                BeepControlStyle.NotionMinimal => new NotionMinimalBorderPainterWrapper(),
                BeepControlStyle.Minimal => new MinimalBorderPainterWrapper(),
                BeepControlStyle.VercelClean => new VercelCleanBorderPainterWrapper(),
                BeepControlStyle.StripeDashboard => new StripeDashboardBorderPainterWrapper(),
                BeepControlStyle.DarkGlow => new DarkGlowBorderPainterWrapper(),
                BeepControlStyle.DiscordStyle => new DiscordStyleBorderPainterWrapper(),
                BeepControlStyle.GradientModern => new GradientModernBorderPainterWrapper(),
                BeepControlStyle.GlassAcrylic => new GlassAcrylicBorderPainterWrapper(),
                BeepControlStyle.Neumorphism => new NeumorphismBorderPainterWrapper(),
                BeepControlStyle.Bootstrap => new BootstrapBorderPainterWrapper(),
                BeepControlStyle.FigmaCard => new FigmaCardBorderPainterWrapper(),
                BeepControlStyle.PillRail => new PillRailBorderPainterWrapper(),
                BeepControlStyle.Apple => new AppleBorderPainterWrapper(),
                BeepControlStyle.Fluent => new FluentBorderPainterWrapper(),
                BeepControlStyle.Material => new MaterialBorderPainterWrapper(),
                BeepControlStyle.WebFramework => new WebFrameworkBorderPainterWrapper(),
                BeepControlStyle.Effect => new EffectBorderPainterWrapper(),
                BeepControlStyle.Metro => new MetroBorderPainterWrapper(),
                BeepControlStyle.Office => new OfficeBorderPainterWrapper(),
                BeepControlStyle.NeoBrutalist => new NeoBrutalistBorderPainterWrapper(),
                BeepControlStyle.HighContrast => new HighContrastBorderPainterWrapper(),
                BeepControlStyle.Gnome => new GnomeBorderPainterWrapper(),
                BeepControlStyle.Kde => new KdeBorderPainterWrapper(),
                BeepControlStyle.Cinnamon => new CinnamonBorderPainterWrapper(),
                BeepControlStyle.Elementary => new ElementaryBorderPainterWrapper(),
                BeepControlStyle.Gaming => new GamingBorderPainterWrapper(),
                BeepControlStyle.Neon => new NeonBorderPainterWrapper(),
                BeepControlStyle.ArcLinux => new ArcLinuxBorderPainterWrapper(),
                BeepControlStyle.Brutalist => new BrutalistBorderPainterWrapper(),
                BeepControlStyle.Cartoon => new CartoonBorderPainterWrapper(),
                BeepControlStyle.ChatBubble => new ChatBubbleBorderPainterWrapper(),
                BeepControlStyle.Cyberpunk => new CyberpunkBorderPainterWrapper(),
                BeepControlStyle.Dracula => new DraculaBorderPainterWrapper(),
                BeepControlStyle.Glassmorphism => new GlassmorphismBorderPainterWrapper(),
                BeepControlStyle.Holographic => new HolographicBorderPainterWrapper(),
                BeepControlStyle.GruvBox => new GruvBoxBorderPainterWrapper(),
                BeepControlStyle.Metro2 => new Metro2BorderPainterWrapper(),
                BeepControlStyle.Modern => new ModernBorderPainterWrapper(),
                BeepControlStyle.Nord => new NordBorderPainterWrapper(),
                BeepControlStyle.Nordic => new NordicBorderPainterWrapper(),
                BeepControlStyle.OneDark => new OneDarkBorderPainterWrapper(),
                BeepControlStyle.Paper => new PaperBorderPainterWrapper(),
          
                BeepControlStyle.Solarized => new SolarizedBorderPainterWrapper(),
                BeepControlStyle.Terminal => new TerminalBorderPainterWrapper(),
                BeepControlStyle.Tokyo => new TokyoBorderPainterWrapper(),
                BeepControlStyle.Ubuntu => new UbuntuBorderPainterWrapper(),
                BeepControlStyle.Shadcn => new ShadcnBorderPainterWrapper(),
                BeepControlStyle.RadixUI => new RadixUIBorderPainterWrapper(),
                BeepControlStyle.NextJS => new NextJSBorderPainterWrapper(),
                BeepControlStyle.Linear => new LinearBorderPainterWrapper(),
                _ => null
            };
        }

        /// <summary>
        /// Gets the control state from hover/focus/pressed flags
        /// </summary>
        public static ControlState GetControlState(bool isHovered, bool isFocused, bool isPressed)
        {
            if (isPressed) return ControlState.Pressed;
            if (isFocused) return ControlState.Focused;
            if (isHovered) return ControlState.Hovered;
            return ControlState.Normal;
        }
    }

    #region Wrapper Base Class
    /// <summary>
    /// Base wrapper class that implements IBorderPainter by delegating to static painter methods
    /// </summary>
    public abstract class BorderPainterWrapperBase : IBorderPainter
    {
        protected readonly BeepControlStyle Style;
        protected readonly bool UseThemeColors;

        protected BorderPainterWrapperBase(BeepControlStyle style, bool useThemeColors = true)
        {
            Style = style;
            UseThemeColors = useThemeColors;
        }

        /// <summary>
        /// Delegates to the static Paint method of the concrete painter
        /// </summary>
        protected abstract GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state);

        public GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // If the configured border width for this style is ZERO or negative, do not attempt to paint.
            // This prevents border painters from drawing fallback lines when the style explicitly specifies no border.
            float configuredWidth = StyleBorders.GetBorderWidth(Style);
            if (configuredWidth <= 0f)
            {
                // No border required - return the original path unchanged.
                return path;
            }

            return PaintStatic(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }
    #endregion

    #region Concrete Wrapper Implementations
    public class Material3BorderPainterWrapper : BorderPainterWrapperBase
    {
        public Material3BorderPainterWrapper() : base(BeepControlStyle.Material3) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return Material3BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class iOS15BorderPainterWrapper : BorderPainterWrapperBase
    {
        public iOS15BorderPainterWrapper() : base(BeepControlStyle.iOS15) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return iOS15BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class AntDesignBorderPainterWrapper : BorderPainterWrapperBase
    {
        public AntDesignBorderPainterWrapper() : base(BeepControlStyle.AntDesign) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return AntDesignBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class Fluent2BorderPainterWrapper : BorderPainterWrapperBase
    {
        public Fluent2BorderPainterWrapper() : base(BeepControlStyle.Fluent2) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return Fluent2BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MaterialYouBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MaterialYouBorderPainterWrapper() : base(BeepControlStyle.MaterialYou) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return MaterialYouBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class Windows11MicaBorderPainterWrapper : BorderPainterWrapperBase
    {
        public Windows11MicaBorderPainterWrapper() : base(BeepControlStyle.Windows11Mica) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return Windows11MicaBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MacOSBigSurBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MacOSBigSurBorderPainterWrapper() : base(BeepControlStyle.MacOSBigSur) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return MacOSBigSurBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ChakraUIBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ChakraUIBorderPainterWrapper() : base(BeepControlStyle.ChakraUI) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return ChakraUIBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class TailwindCardBorderPainterWrapper : BorderPainterWrapperBase
    {
        public TailwindCardBorderPainterWrapper() : base(BeepControlStyle.TailwindCard) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return TailwindCardBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NotionMinimalBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NotionMinimalBorderPainterWrapper() : base(BeepControlStyle.NotionMinimal) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NotionMinimalBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MinimalBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MinimalBorderPainterWrapper() : base(BeepControlStyle.Minimal) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return MinimalBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class VercelCleanBorderPainterWrapper : BorderPainterWrapperBase
    {
        public VercelCleanBorderPainterWrapper() : base(BeepControlStyle.VercelClean) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return VercelCleanBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class StripeDashboardBorderPainterWrapper : BorderPainterWrapperBase
    {
        public StripeDashboardBorderPainterWrapper() : base(BeepControlStyle.StripeDashboard) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return StripeDashboardBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class DarkGlowBorderPainterWrapper : BorderPainterWrapperBase
    {
        public DarkGlowBorderPainterWrapper() : base(BeepControlStyle.DarkGlow) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return DarkGlowBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class DiscordStyleBorderPainterWrapper : BorderPainterWrapperBase
    {
        public DiscordStyleBorderPainterWrapper() : base(BeepControlStyle.DiscordStyle) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return DiscordStyleBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GradientModernBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GradientModernBorderPainterWrapper() : base(BeepControlStyle.GradientModern) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return GradientModernBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GlassAcrylicBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GlassAcrylicBorderPainterWrapper() : base(BeepControlStyle.GlassAcrylic) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return GlassAcrylicBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NeumorphismBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NeumorphismBorderPainterWrapper() : base(BeepControlStyle.Neumorphism) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NeumorphismBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class BootstrapBorderPainterWrapper : BorderPainterWrapperBase
    {
        public BootstrapBorderPainterWrapper() : base(BeepControlStyle.Bootstrap) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return BootstrapBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class FigmaCardBorderPainterWrapper : BorderPainterWrapperBase
    {
        public FigmaCardBorderPainterWrapper() : base(BeepControlStyle.FigmaCard) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return FigmaCardBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class PillRailBorderPainterWrapper : BorderPainterWrapperBase
    {
        public PillRailBorderPainterWrapper() : base(BeepControlStyle.PillRail) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return PillRailBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class AppleBorderPainterWrapper : BorderPainterWrapperBase
    {
        public AppleBorderPainterWrapper() : base(BeepControlStyle.Apple) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return AppleBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class FluentBorderPainterWrapper : BorderPainterWrapperBase
    {
        public FluentBorderPainterWrapper() : base(BeepControlStyle.Fluent) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return FluentBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MaterialBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MaterialBorderPainterWrapper() : base(BeepControlStyle.Material) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return MaterialBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class WebFrameworkBorderPainterWrapper : BorderPainterWrapperBase
    {
        public WebFrameworkBorderPainterWrapper() : base(BeepControlStyle.WebFramework) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return WebFrameworkBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class EffectBorderPainterWrapper : BorderPainterWrapperBase
    {
        public EffectBorderPainterWrapper() : base(BeepControlStyle.Effect) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return EffectBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MetroBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MetroBorderPainterWrapper() : base(BeepControlStyle.Metro) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return MetroBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class OfficeBorderPainterWrapper : BorderPainterWrapperBase
    {
        public OfficeBorderPainterWrapper() : base(BeepControlStyle.Office) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return OfficeBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NeoBrutalistBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NeoBrutalistBorderPainterWrapper() : base(BeepControlStyle.NeoBrutalist) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NeoBrutalistBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class HighContrastBorderPainterWrapper : BorderPainterWrapperBase
    {
        public HighContrastBorderPainterWrapper() : base(BeepControlStyle.HighContrast) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return HighContrastBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GnomeBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GnomeBorderPainterWrapper() : base(BeepControlStyle.Gnome) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return GnomeBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class KdeBorderPainterWrapper : BorderPainterWrapperBase
    {
        public KdeBorderPainterWrapper() : base(BeepControlStyle.Kde) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return KdeBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class CinnamonBorderPainterWrapper : BorderPainterWrapperBase
    {
        public CinnamonBorderPainterWrapper() : base(BeepControlStyle.Cinnamon) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return CinnamonBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ElementaryBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ElementaryBorderPainterWrapper() : base(BeepControlStyle.Elementary) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return ElementaryBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GamingBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GamingBorderPainterWrapper() : base(BeepControlStyle.Gaming) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return GamingBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NeonBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NeonBorderPainterWrapper() : base(BeepControlStyle.Neon) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NeonBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ArcLinuxBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ArcLinuxBorderPainterWrapper() : base(BeepControlStyle.ArcLinux) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return ArcLinuxBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class BrutalistBorderPainterWrapper : BorderPainterWrapperBase
    {
        public BrutalistBorderPainterWrapper() : base(BeepControlStyle.Brutalist) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return BrutalistBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class CartoonBorderPainterWrapper : BorderPainterWrapperBase
    {
        public CartoonBorderPainterWrapper() : base(BeepControlStyle.Cartoon) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return CartoonBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ChatBubbleBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ChatBubbleBorderPainterWrapper() : base(BeepControlStyle.ChatBubble) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return ChatBubbleBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class CyberpunkBorderPainterWrapper : BorderPainterWrapperBase
    {
        public CyberpunkBorderPainterWrapper() : base(BeepControlStyle.Cyberpunk) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return CyberpunkBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class DraculaBorderPainterWrapper : BorderPainterWrapperBase
    {
        public DraculaBorderPainterWrapper() : base(BeepControlStyle.Dracula) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return DraculaBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GlassmorphismBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GlassmorphismBorderPainterWrapper() : base(BeepControlStyle.Glassmorphism) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return GlassmorphismBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class HolographicBorderPainterWrapper : BorderPainterWrapperBase
    {
        public HolographicBorderPainterWrapper() : base(BeepControlStyle.Holographic) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return HolographicBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GruvBoxBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GruvBoxBorderPainterWrapper() : base(BeepControlStyle.GruvBox) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return GruvBoxBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class Metro2BorderPainterWrapper : BorderPainterWrapperBase
    {
        public Metro2BorderPainterWrapper() : base(BeepControlStyle.Metro2) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return Metro2BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ModernBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ModernBorderPainterWrapper() : base(BeepControlStyle.Modern) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return ModernBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NordBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NordBorderPainterWrapper() : base(BeepControlStyle.Nord) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NordBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NordicBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NordicBorderPainterWrapper() : base(BeepControlStyle.Nordic) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NordicBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class OneDarkBorderPainterWrapper : BorderPainterWrapperBase
    {
        public OneDarkBorderPainterWrapper() : base(BeepControlStyle.OneDark) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return OneDarkBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class PaperBorderPainterWrapper : BorderPainterWrapperBase
    {
        public PaperBorderPainterWrapper() : base(BeepControlStyle.Paper) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return PaperBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class RetroBorderPainterWrapper : BorderPainterWrapperBase
    {
        public RetroBorderPainterWrapper() : base(BeepControlStyle.Retro) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return RetroBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class SolarizedBorderPainterWrapper : BorderPainterWrapperBase
    {
        public SolarizedBorderPainterWrapper() : base(BeepControlStyle.Solarized) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return SolarizedBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class TerminalBorderPainterWrapper : BorderPainterWrapperBase
    {
        public TerminalBorderPainterWrapper() : base(BeepControlStyle.Terminal) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return TerminalBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class TokyoBorderPainterWrapper : BorderPainterWrapperBase
    {
        public TokyoBorderPainterWrapper() : base(BeepControlStyle.Tokyo) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return TokyoBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class UbuntuBorderPainterWrapper : BorderPainterWrapperBase
    {
        public UbuntuBorderPainterWrapper() : base(BeepControlStyle.Ubuntu) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return UbuntuBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ShadcnBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ShadcnBorderPainterWrapper() : base(BeepControlStyle.Shadcn) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return ShadcnBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class RadixUIBorderPainterWrapper : BorderPainterWrapperBase
    {
        public RadixUIBorderPainterWrapper() : base(BeepControlStyle.RadixUI) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return RadixUIBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NextJSBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NextJSBorderPainterWrapper() : base(BeepControlStyle.NextJS) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return NextJSBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class LinearBorderPainterWrapper : BorderPainterWrapperBase
    {
        public LinearBorderPainterWrapper() : base(BeepControlStyle.Linear) { }
        protected override GraphicsPath PaintStatic(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            return LinearBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }
    #endregion
}
