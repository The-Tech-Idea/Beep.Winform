using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Factory for creating background painter instances based on BeepControlStyle
    /// </summary>
    public static class BackgroundPainterFactory
    {
        /// <summary>
        /// Creates a background painter instance for the specified Style
        /// </summary>
        /// <param name="style">The control Style to create a painter for</param>
        /// <returns>An IBackgroundPainter implementation, or null for None Style</returns>
        public static IBackgroundPainter CreatePainter(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.None => null,
                BeepControlStyle.Material3 => new Material3BackgroundPainterWrapper(),
                BeepControlStyle.iOS15 => new iOS15BackgroundPainterWrapper(),
                BeepControlStyle.AntDesign => new AntDesignBackgroundPainterWrapper(),
                BeepControlStyle.Fluent2 => new Fluent2BackgroundPainterWrapper(),
                BeepControlStyle.MaterialYou => new MaterialYouBackgroundPainterWrapper(),
                BeepControlStyle.Windows11Mica => new Windows11MicaBackgroundPainterWrapper(),
                BeepControlStyle.MacOSBigSur => new MacOSBigSurBackgroundPainterWrapper(),
                BeepControlStyle.ChakraUI => new ChakraUIBackgroundPainterWrapper(),
                BeepControlStyle.TailwindCard => new TailwindCardBackgroundPainterWrapper(),
                BeepControlStyle.NotionMinimal => new NotionMinimalBackgroundPainterWrapper(),
                BeepControlStyle.Minimal => new MinimalBackgroundPainterWrapper(),
                BeepControlStyle.VercelClean => new VercelCleanBackgroundPainterWrapper(),
                BeepControlStyle.StripeDashboard => new StripeDashboardBackgroundPainterWrapper(),
                BeepControlStyle.DarkGlow => new DarkGlowBackgroundPainterWrapper(),
                BeepControlStyle.DiscordStyle => new DiscordStyleBackgroundPainterWrapper(),
                BeepControlStyle.GradientModern => new GradientModernBackgroundPainterWrapper(),
                BeepControlStyle.GlassAcrylic => new GlassAcrylicBackgroundPainterWrapper(),
                BeepControlStyle.Neumorphism => new NeumorphismBackgroundPainterWrapper(),
                BeepControlStyle.Bootstrap => new BootstrapBackgroundPainterWrapper(),
                BeepControlStyle.FigmaCard => new FigmaCardBackgroundPainterWrapper(),
                BeepControlStyle.PillRail => new PillRailBackgroundPainterWrapper(),
                BeepControlStyle.Apple => new AppleBackgroundPainterWrapper(),
                BeepControlStyle.Fluent => new FluentBackgroundPainterWrapper(),
                BeepControlStyle.Material => new MaterialBackgroundPainterWrapper(),
                BeepControlStyle.WebFramework => new WebFrameworkBackgroundPainterWrapper(),
                BeepControlStyle.Effect => new EffectBackgroundPainterWrapper(),
                BeepControlStyle.Metro => new MetroBackgroundPainterWrapper(),
                BeepControlStyle.Office => new OfficeBackgroundPainterWrapper(),
                BeepControlStyle.NeoBrutalist => new NeoBrutalistBackgroundPainterWrapper(),
                BeepControlStyle.HighContrast => new HighContrastBackgroundPainterWrapper(),
                BeepControlStyle.Gnome => new GnomeBackgroundPainterWrapper(),
                BeepControlStyle.Kde => new KdeBackgroundPainterWrapper(),
                BeepControlStyle.Cinnamon => new CinnamonBackgroundPainterWrapper(),
                BeepControlStyle.Elementary => new ElementaryBackgroundPainterWrapper(),
                BeepControlStyle.Gaming => new GamingBackgroundPainterWrapper(),
                BeepControlStyle.Neon => new NeonBackgroundPainterWrapper(),
                BeepControlStyle.Terminal => new TerminalBackgroundPainterWrapper(),
                BeepControlStyle.ArcLinux => new ArcLinuxBackgroundPainterWrapper(),
                BeepControlStyle.Brutalist => new BrutalistBackgroundPainterWrapper(),
                BeepControlStyle.Cartoon => new CartoonBackgroundPainterWrapper(),
                BeepControlStyle.ChatBubble => new ChatBubbleBackgroundPainterWrapper(),
                BeepControlStyle.Cyberpunk => new CyberpunkBackgroundPainterWrapper(),
                BeepControlStyle.Dracula => new DraculaBackgroundPainterWrapper(),
                BeepControlStyle.Glassmorphism => new GlassmorphismBackgroundPainterWrapper(),
                BeepControlStyle.Holographic => new HolographicBackgroundPainterWrapper(),
                BeepControlStyle.GruvBox => new GruvBoxBackgroundPainterWrapper(),
                BeepControlStyle.Metro2 => new Metro2BackgroundPainterWrapper(),
                BeepControlStyle.Modern => new ModernBackgroundPainterWrapper(),
                BeepControlStyle.Nord => new NordBackgroundPainterWrapper(),
                BeepControlStyle.Nordic => new NordicBackgroundPainterWrapper(),
                BeepControlStyle.OneDark => new OneDarkBackgroundPainterWrapper(),
                BeepControlStyle.Paper => new PaperBackgroundPainterWrapper(),
                BeepControlStyle.Solarized => new SolarizedBackgroundPainterWrapper(),
                BeepControlStyle.Tokyo => new TokyoBackgroundPainterWrapper(),
                BeepControlStyle.Ubuntu => new UbuntuBackgroundPainterWrapper(),
                BeepControlStyle.Retro => new RetroBackgroundPainterWrapper(),
                BeepControlStyle.Shadcn => new ShadcnBackgroundPainterWrapper(),
                BeepControlStyle.RadixUI => new RadixUIBackgroundPainterWrapper(),
                BeepControlStyle.NextJS => new NextJSBackgroundPainterWrapper(),
                BeepControlStyle.Linear => new LinearBackgroundPainterWrapper(),
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

    #region Background Painter Interface
    /// <summary>
    /// Interface for background painter implementations
    /// </summary>
    public interface IBackgroundPainter
    {
        /// <summary>
        /// Paints background for the specified path (fills the area, does not modify path)
        /// </summary>
        void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal);
    }
    #endregion

    #region Wrapper Base Class
    /// <summary>
    /// Base wrapper class that implements IBackgroundPainter by delegating to static painter methods
    /// </summary>
    public abstract class BackgroundPainterWrapperBase : IBackgroundPainter
    {
        protected readonly BeepControlStyle Style;
        protected readonly bool UseThemeColors;

        protected BackgroundPainterWrapperBase(BeepControlStyle style, bool useThemeColors = true)
        {
            Style = style;
            UseThemeColors = useThemeColors;
        }

        /// <summary>
        /// Delegates to the static Paint method of the concrete painter
        /// </summary>
        protected abstract void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state);

        public void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            PaintStatic(g, path, style, theme, useThemeColors, state);
        }
    }
    #endregion

    #region Concrete Wrapper Implementations
    public class Material3BackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public Material3BackgroundPainterWrapper() : base(BeepControlStyle.Material3) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Material3BackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class iOS15BackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public iOS15BackgroundPainterWrapper() : base(BeepControlStyle.iOS15) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            iOS15BackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class AntDesignBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public AntDesignBackgroundPainterWrapper() : base(BeepControlStyle.AntDesign) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            AntDesignBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class Fluent2BackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public Fluent2BackgroundPainterWrapper() : base(BeepControlStyle.Fluent2) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Fluent2BackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class MaterialYouBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public MaterialYouBackgroundPainterWrapper() : base(BeepControlStyle.MaterialYou) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MaterialYouBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class Windows11MicaBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public Windows11MicaBackgroundPainterWrapper() : base(BeepControlStyle.Windows11Mica) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Windows11MicaBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class MacOSBigSurBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public MacOSBigSurBackgroundPainterWrapper() : base(BeepControlStyle.MacOSBigSur) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MacOSBigSurBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class ChakraUIBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public ChakraUIBackgroundPainterWrapper() : base(BeepControlStyle.ChakraUI) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ChakraUIBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class TailwindCardBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public TailwindCardBackgroundPainterWrapper() : base(BeepControlStyle.TailwindCard) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            TailwindCardBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NotionMinimalBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NotionMinimalBackgroundPainterWrapper() : base(BeepControlStyle.NotionMinimal) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NotionMinimalBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class MinimalBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public MinimalBackgroundPainterWrapper() : base(BeepControlStyle.Minimal) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MinimalBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class VercelCleanBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public VercelCleanBackgroundPainterWrapper() : base(BeepControlStyle.VercelClean) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            VercelCleanBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class StripeDashboardBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public StripeDashboardBackgroundPainterWrapper() : base(BeepControlStyle.StripeDashboard) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            StripeDashboardBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class DarkGlowBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public DarkGlowBackgroundPainterWrapper() : base(BeepControlStyle.DarkGlow) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            DarkGlowBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class DiscordStyleBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public DiscordStyleBackgroundPainterWrapper() : base(BeepControlStyle.DiscordStyle) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            DiscordStyleBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class GradientModernBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public GradientModernBackgroundPainterWrapper() : base(BeepControlStyle.GradientModern) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GradientModernBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class GlassAcrylicBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public GlassAcrylicBackgroundPainterWrapper() : base(BeepControlStyle.GlassAcrylic) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GlassAcrylicBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NeumorphismBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NeumorphismBackgroundPainterWrapper() : base(BeepControlStyle.Neumorphism) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NeumorphismBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class BootstrapBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public BootstrapBackgroundPainterWrapper() : base(BeepControlStyle.Bootstrap) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            BootstrapBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class FigmaCardBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public FigmaCardBackgroundPainterWrapper() : base(BeepControlStyle.FigmaCard) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            FigmaCardBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class PillRailBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public PillRailBackgroundPainterWrapper() : base(BeepControlStyle.PillRail) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            PillRailBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class AppleBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public AppleBackgroundPainterWrapper() : base(BeepControlStyle.Apple) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            AppleBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class FluentBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public FluentBackgroundPainterWrapper() : base(BeepControlStyle.Fluent) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            FluentBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class MaterialBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public MaterialBackgroundPainterWrapper() : base(BeepControlStyle.Material) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MaterialBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class WebFrameworkBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public WebFrameworkBackgroundPainterWrapper() : base(BeepControlStyle.WebFramework) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            WebFrameworkBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class EffectBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public EffectBackgroundPainterWrapper() : base(BeepControlStyle.Effect) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            EffectBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class MetroBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public MetroBackgroundPainterWrapper() : base(BeepControlStyle.Metro) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MetroBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class OfficeBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public OfficeBackgroundPainterWrapper() : base(BeepControlStyle.Office) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            OfficeBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NeoBrutalistBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NeoBrutalistBackgroundPainterWrapper() : base(BeepControlStyle.NeoBrutalist) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NeoBrutalistBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class HighContrastBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public HighContrastBackgroundPainterWrapper() : base(BeepControlStyle.HighContrast) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            HighContrastBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class GnomeBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public GnomeBackgroundPainterWrapper() : base(BeepControlStyle.Gnome) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GnomeBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class KdeBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public KdeBackgroundPainterWrapper() : base(BeepControlStyle.Kde) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            KdeBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class CinnamonBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public CinnamonBackgroundPainterWrapper() : base(BeepControlStyle.Cinnamon) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            CinnamonBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class ElementaryBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public ElementaryBackgroundPainterWrapper() : base(BeepControlStyle.Elementary) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ElementaryBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class GamingBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public GamingBackgroundPainterWrapper() : base(BeepControlStyle.Gaming) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GamingBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NeonBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NeonBackgroundPainterWrapper() : base(BeepControlStyle.Neon) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NeonBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class RetroBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public RetroBackgroundPainterWrapper() : base(BeepControlStyle.Retro) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            RetroBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class TerminalBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public TerminalBackgroundPainterWrapper() : base(BeepControlStyle.Terminal) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            TerminalBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class ArcLinuxBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public ArcLinuxBackgroundPainterWrapper() : base(BeepControlStyle.ArcLinux) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ArcLinuxBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class BrutalistBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public BrutalistBackgroundPainterWrapper() : base(BeepControlStyle.Brutalist) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            BrutalistBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class CartoonBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public CartoonBackgroundPainterWrapper() : base(BeepControlStyle.Cartoon) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            CartoonBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class ChatBubbleBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public ChatBubbleBackgroundPainterWrapper() : base(BeepControlStyle.ChatBubble) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ChatBubbleBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class CyberpunkBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public CyberpunkBackgroundPainterWrapper() : base(BeepControlStyle.Cyberpunk) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            CyberpunkBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class DraculaBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public DraculaBackgroundPainterWrapper() : base(BeepControlStyle.Dracula) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            DraculaBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class GlassmorphismBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public GlassmorphismBackgroundPainterWrapper() : base(BeepControlStyle.Glassmorphism) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GlassmorphismBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class HolographicBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public HolographicBackgroundPainterWrapper() : base(BeepControlStyle.Holographic) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            HolographicBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class GruvBoxBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public GruvBoxBackgroundPainterWrapper() : base(BeepControlStyle.GruvBox) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GruvBoxBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class Metro2BackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public Metro2BackgroundPainterWrapper() : base(BeepControlStyle.Metro2) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Metro2BackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class ModernBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public ModernBackgroundPainterWrapper() : base(BeepControlStyle.Modern) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ModernBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NordBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NordBackgroundPainterWrapper() : base(BeepControlStyle.Nord) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NordBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NordicBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NordicBackgroundPainterWrapper() : base(BeepControlStyle.Nordic) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NordicBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class OneDarkBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public OneDarkBackgroundPainterWrapper() : base(BeepControlStyle.OneDark) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            OneDarkBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class PaperBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public PaperBackgroundPainterWrapper() : base(BeepControlStyle.Paper) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            PaperBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class SolarizedBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public SolarizedBackgroundPainterWrapper() : base(BeepControlStyle.Solarized) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            SolarizedBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class TokyoBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public TokyoBackgroundPainterWrapper() : base(BeepControlStyle.Tokyo) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            TokyoBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class UbuntuBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public UbuntuBackgroundPainterWrapper() : base(BeepControlStyle.Ubuntu) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            UbuntuBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class ShadcnBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public ShadcnBackgroundPainterWrapper() : base(BeepControlStyle.Shadcn) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ShadcnBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class RadixUIBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public RadixUIBackgroundPainterWrapper() : base(BeepControlStyle.RadixUI) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            RadixUIBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class NextJSBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public NextJSBackgroundPainterWrapper() : base(BeepControlStyle.NextJS) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NextJSBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }

    public class LinearBackgroundPainterWrapper : BackgroundPainterWrapperBase
    {
        public LinearBackgroundPainterWrapper() : base(BeepControlStyle.Linear) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            LinearBackgroundPainter.Paint(g, path, style, theme, useThemeColors, state);
        }
    }
    #endregion
}
