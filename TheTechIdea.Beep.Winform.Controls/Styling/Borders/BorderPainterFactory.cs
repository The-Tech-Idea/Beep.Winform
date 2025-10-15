using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Factory for creating border painter instances based on BeepControlStyle
    /// </summary>
    public static class BorderPainterFactory
    {
        /// <summary>
        /// Creates a border painter instance for the specified style
        /// </summary>
        /// <param name="style">The control style to create a painter for</param>
        /// <returns>An IBorderPainter implementation, or null for None style</returns>
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
        protected abstract void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state);

        public void Paint(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            PaintStatic(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }
    #endregion

    #region Concrete Wrapper Implementations
    public class Material3BorderPainterWrapper : BorderPainterWrapperBase
    {
        public Material3BorderPainterWrapper() : base(BeepControlStyle.Material3) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Material3BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class iOS15BorderPainterWrapper : BorderPainterWrapperBase
    {
        public iOS15BorderPainterWrapper() : base(BeepControlStyle.iOS15) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            iOS15BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class AntDesignBorderPainterWrapper : BorderPainterWrapperBase
    {
        public AntDesignBorderPainterWrapper() : base(BeepControlStyle.AntDesign) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            AntDesignBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class Fluent2BorderPainterWrapper : BorderPainterWrapperBase
    {
        public Fluent2BorderPainterWrapper() : base(BeepControlStyle.Fluent2) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Fluent2BorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MaterialYouBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MaterialYouBorderPainterWrapper() : base(BeepControlStyle.MaterialYou) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MaterialYouBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class Windows11MicaBorderPainterWrapper : BorderPainterWrapperBase
    {
        public Windows11MicaBorderPainterWrapper() : base(BeepControlStyle.Windows11Mica) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            Windows11MicaBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MacOSBigSurBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MacOSBigSurBorderPainterWrapper() : base(BeepControlStyle.MacOSBigSur) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MacOSBigSurBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class ChakraUIBorderPainterWrapper : BorderPainterWrapperBase
    {
        public ChakraUIBorderPainterWrapper() : base(BeepControlStyle.ChakraUI) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            ChakraUIBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class TailwindCardBorderPainterWrapper : BorderPainterWrapperBase
    {
        public TailwindCardBorderPainterWrapper() : base(BeepControlStyle.TailwindCard) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            TailwindCardBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NotionMinimalBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NotionMinimalBorderPainterWrapper() : base(BeepControlStyle.NotionMinimal) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NotionMinimalBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MinimalBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MinimalBorderPainterWrapper() : base(BeepControlStyle.Minimal) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MinimalBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class VercelCleanBorderPainterWrapper : BorderPainterWrapperBase
    {
        public VercelCleanBorderPainterWrapper() : base(BeepControlStyle.VercelClean) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            VercelCleanBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class StripeDashboardBorderPainterWrapper : BorderPainterWrapperBase
    {
        public StripeDashboardBorderPainterWrapper() : base(BeepControlStyle.StripeDashboard) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            StripeDashboardBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class DarkGlowBorderPainterWrapper : BorderPainterWrapperBase
    {
        public DarkGlowBorderPainterWrapper() : base(BeepControlStyle.DarkGlow) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            DarkGlowBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class DiscordStyleBorderPainterWrapper : BorderPainterWrapperBase
    {
        public DiscordStyleBorderPainterWrapper() : base(BeepControlStyle.DiscordStyle) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            DiscordStyleBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GradientModernBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GradientModernBorderPainterWrapper() : base(BeepControlStyle.GradientModern) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GradientModernBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class GlassAcrylicBorderPainterWrapper : BorderPainterWrapperBase
    {
        public GlassAcrylicBorderPainterWrapper() : base(BeepControlStyle.GlassAcrylic) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            GlassAcrylicBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class NeumorphismBorderPainterWrapper : BorderPainterWrapperBase
    {
        public NeumorphismBorderPainterWrapper() : base(BeepControlStyle.Neumorphism) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            NeumorphismBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class BootstrapBorderPainterWrapper : BorderPainterWrapperBase
    {
        public BootstrapBorderPainterWrapper() : base(BeepControlStyle.Bootstrap) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            BootstrapBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class FigmaCardBorderPainterWrapper : BorderPainterWrapperBase
    {
        public FigmaCardBorderPainterWrapper() : base(BeepControlStyle.FigmaCard) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            FigmaCardBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class PillRailBorderPainterWrapper : BorderPainterWrapperBase
    {
        public PillRailBorderPainterWrapper() : base(BeepControlStyle.PillRail) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            PillRailBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class AppleBorderPainterWrapper : BorderPainterWrapperBase
    {
        public AppleBorderPainterWrapper() : base(BeepControlStyle.Apple) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            AppleBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class FluentBorderPainterWrapper : BorderPainterWrapperBase
    {
        public FluentBorderPainterWrapper() : base(BeepControlStyle.Fluent) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            FluentBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class MaterialBorderPainterWrapper : BorderPainterWrapperBase
    {
        public MaterialBorderPainterWrapper() : base(BeepControlStyle.Material) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            MaterialBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class WebFrameworkBorderPainterWrapper : BorderPainterWrapperBase
    {
        public WebFrameworkBorderPainterWrapper() : base(BeepControlStyle.WebFramework) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            WebFrameworkBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }

    public class EffectBorderPainterWrapper : BorderPainterWrapperBase
    {
        public EffectBorderPainterWrapper() : base(BeepControlStyle.Effect) { }
        protected override void PaintStatic(Graphics g, GraphicsPath path, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state)
        {
            EffectBorderPainter.Paint(g, path, isFocused, style, theme, useThemeColors, state);
        }
    }
    #endregion
}
