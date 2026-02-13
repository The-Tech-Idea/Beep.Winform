using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using static TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters.ShadowPainterHelpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Shadows
{
    /// <summary>
    /// Factory for creating shadow painter instances based on BeepControlStyle and shadow type
    /// </summary>
    public static class ShadowPainterFactory
    {
        /// <summary>
        /// Creates a shadow painter instance for the specified Style
        /// </summary>
        /// <param name="style">The control Style to create a shadow painter for</param>
        /// <param name="shadowType">Optional specific shadow type override</param>
        /// <returns>An IShadowPainter implementation, or null for None Style</returns>
        public static IShadowPainter CreatePainter(BeepControlStyle style, ShadowType? shadowType = null)
        {
            // If specific shadow type is requested, use that
            if (shadowType.HasValue)
            {
                return CreateByType(shadowType.Value);
            }
            // If style explicitly has no shadows configured, don't create a painter
            if (!StyleShadows.HasShadow(style))
            {
                return null;
            }

            // Otherwise, create based on Style
            return style switch
            {
                BeepControlStyle.None => null,
                BeepControlStyle.Material3 => new MaterialShadowPainterWrapper(MaterialElevation.Level2),
                BeepControlStyle.iOS15 => new CardShadowPainterWrapper(CardShadowStyle.Small),
                BeepControlStyle.AntDesign => new CardShadowPainterWrapper(CardShadowStyle.Medium),
                BeepControlStyle.Fluent2 => new FloatingShadowPainterWrapper(4),
                BeepControlStyle.MaterialYou => new MaterialShadowPainterWrapper(MaterialElevation.Level3),
                BeepControlStyle.Windows11Mica => new AmbientShadowPainterWrapper(6, 0.2f),
                BeepControlStyle.MacOSBigSur => new CardShadowPainterWrapper(CardShadowStyle.Small),
                BeepControlStyle.ChakraUI => new CardShadowPainterWrapper(CardShadowStyle.Medium),
                BeepControlStyle.TailwindCard => new CardShadowPainterWrapper(CardShadowStyle.Large),
                BeepControlStyle.NotionMinimal => new CardShadowPainterWrapper(CardShadowStyle.Small),
                BeepControlStyle.Minimal => new CardShadowPainterWrapper(CardShadowStyle.Small),
                BeepControlStyle.VercelClean => new CardShadowPainterWrapper(CardShadowStyle.Medium),
                BeepControlStyle.StripeDashboard => new CardShadowPainterWrapper(CardShadowStyle.Large),
                BeepControlStyle.DarkGlow => new NeonGlowPainterWrapper(Color.FromArgb(100, 150, 255), 0.8f, 16),
                BeepControlStyle.DiscordStyle => new ColoredShadowPainterWrapper(Color.FromArgb(88, 101, 242)),
                BeepControlStyle.GradientModern => new FloatingShadowPainterWrapper(8),
                BeepControlStyle.GlassAcrylic => new GlassShadowPainterWrapper(Color.FromArgb(150, 200, 255), 0.3f),
                BeepControlStyle.Neumorphism => new NeumorphicShadowPainterWrapper(SystemColors.Control),
                BeepControlStyle.Bootstrap => new CardShadowPainterWrapper(CardShadowStyle.Medium),
                BeepControlStyle.FigmaCard => new CardShadowPainterWrapper(CardShadowStyle.Medium),
                BeepControlStyle.PillRail => new AmbientShadowPainterWrapper(4, 0.25f),
                BeepControlStyle.Apple => new AppleStyleShadowPainterWrapper(),
                BeepControlStyle.Fluent => new FluentStyleShadowPainterWrapper(),
                BeepControlStyle.Material => new MaterialStyleShadowPainterWrapper(),
                BeepControlStyle.WebFramework => new WebFrameworkStyleShadowPainterWrapper(),
                BeepControlStyle.Effect => new EffectStyleShadowPainterWrapper(),
                BeepControlStyle.Metro => null, // Metro: NO shadows (flat design)
                BeepControlStyle.Office => new CardShadowPainterWrapper(CardShadowStyle.Small), // Office: Subtle shadows
                BeepControlStyle.NeoBrutalist => new NeoBrutalistShadowPainterWrapper(), // NeoBrutalist: Hard offset shadow (visible)
                BeepControlStyle.HighContrast => null, // HighContrast: NO shadows (WCAG AAA)
                BeepControlStyle.Gnome => new AmbientShadowPainterWrapper(4, 0.3f), // Gnome: Subtle ambient
                BeepControlStyle.Kde => new NeonGlowPainterWrapper(Color.FromArgb(61, 174, 233), 0.6f, 12), // Kde: Blue glow
                BeepControlStyle.Cinnamon => new CardShadowPainterWrapper(CardShadowStyle.Medium), // Cinnamon: Standard card
                BeepControlStyle.Elementary => new CardShadowPainterWrapper(CardShadowStyle.Small), // Elementary: Subtle
                BeepControlStyle.Gaming => new NeonGlowPainterWrapper(Color.FromArgb(0, 255, 127), 1.0f, 16), // Gaming: Green glow
                BeepControlStyle.Neon => new NeonGlowPainterWrapper(Color.FromArgb(0, 255, 255), 1.0f, 24), // Neon: Cyan glow
                BeepControlStyle.ArcLinux => new ArcLinuxShadowPainterWrapper(),
                BeepControlStyle.Brutalist => new BrutalistShadowPainterWrapper(),
                BeepControlStyle.Cartoon => new CartoonShadowPainterWrapper(),
                BeepControlStyle.ChatBubble => new ChatBubbleShadowPainterWrapper(),
                BeepControlStyle.Cyberpunk => new CyberpunkShadowPainterWrapper(),
                BeepControlStyle.Dracula => new DraculaShadowPainterWrapper(),
                BeepControlStyle.Glassmorphism => new GlassmorphismShadowPainterWrapper(),
                BeepControlStyle.Holographic => new HolographicShadowPainterWrapper(),
                BeepControlStyle.GruvBox => new GruvBoxShadowPainterWrapper(),
                BeepControlStyle.Metro2 => new Metro2ShadowPainterWrapper(),
                BeepControlStyle.Modern => new ModernShadowPainterWrapper(),
                BeepControlStyle.Nord => new NordShadowPainterWrapper(),
                BeepControlStyle.Nordic => new NordicShadowPainterWrapper(),
                BeepControlStyle.OneDark => new OneDarkShadowPainterWrapper(),
                BeepControlStyle.Paper => new PaperShadowPainterWrapper(),
              
                BeepControlStyle.Solarized => new SolarizedShadowPainterWrapper(),
                BeepControlStyle.Terminal => new TerminalShadowPainterWrapper(),
                BeepControlStyle.Tokyo => new TokyoShadowPainterWrapper(),
                BeepControlStyle.Ubuntu => new UbuntuShadowPainterWrapper(),
                BeepControlStyle.Shadcn => new ShadcnShadowPainterWrapper(),
                BeepControlStyle.RadixUI => new RadixUIShadowPainterWrapper(),
                BeepControlStyle.NextJS => new NextJSShadowPainterWrapper(),
                BeepControlStyle.Linear => new LinearShadowPainterWrapper(),
                _ => null
            };
        }

        /// <summary>
        /// Creates a shadow painter by specific type
        /// </summary>
        public static IShadowPainter CreateByType(ShadowType shadowType)
        {
            return shadowType switch
            {
                ShadowType.None => null,
                ShadowType.Soft => new SoftShadowPainterWrapper(),
                ShadowType.Card => new CardShadowPainterWrapper(CardShadowStyle.Medium),
                ShadowType.Material => new MaterialShadowPainterWrapper(MaterialElevation.Level2),
                ShadowType.Floating => new FloatingShadowPainterWrapper(8),
                ShadowType.Drop => new DropShadowPainterWrapper(),
                ShadowType.Inner => new InnerShadowPainterWrapper(),
                ShadowType.Long => new LongShadowPainterWrapper(),
                ShadowType.Colored => new ColoredShadowPainterWrapper(Color.Blue),
                ShadowType.Neon => new NeonGlowPainterWrapper(Color.Cyan, 1f, 12),
                ShadowType.Ambient => new AmbientShadowPainterWrapper(6, 0.3f),
                ShadowType.Perspective => new PerspectiveShadowPainterWrapper(),
                ShadowType.Double => new DoubleShadowPainterWrapper(),
                ShadowType.Neumorphic => new NeumorphicShadowPainterWrapper(SystemColors.Control),
                ShadowType.Glass => new GlassShadowPainterWrapper(Color.White, 0.2f),
                ShadowType.BorderGlow => new BorderGlowPainterWrapper(Color.Blue),
                _ => null
            };
        }

        /// <summary>
        /// Creates a shadow painter with preset configuration
        /// </summary>
        public static IShadowPainter CreateByPreset(ModernShadowPreset preset)
        {
            return new PresetShadowPainterWrapper(preset);
        }

        /// <summary>
        /// Creates an adaptive shadow painter that calculates optimal shadow based on control size
        /// </summary>
        public static IShadowPainter CreateAdaptive(ShadowIntensity intensity = ShadowIntensity.Medium)
        {
            return new AdaptiveShadowPainterWrapper(intensity);
        }
    }

    #region Shadow Painter Interface
    /// <summary>
    /// Interface for shadow painter implementations
    /// </summary>
    public interface IShadowPainter
    {
        /// <summary>
        /// Paints shadow for the specified path
        /// </summary>
        GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null);
    }
    #endregion

    #region Wrapper Base Class
    /// <summary>
    /// Base wrapper class that implements IShadowPainter by delegating to static helper methods
    /// </summary>
    public abstract class ShadowPainterWrapperBase : IShadowPainter
    {
        public abstract GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null);
    }
    #endregion

    #region Concrete Shadow Painter Wrappers

    /// <summary>
    /// Soft shadow painter wrapper
    /// </summary>
    public class SoftShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly int _offsetX;
        private readonly int _offsetY;
        private readonly Color _shadowColor;
        private readonly float _opacity;
        private readonly int _layers;

        public SoftShadowPainterWrapper(int offsetX = 0, int offsetY = 4, Color? shadowColor = null, float opacity = 0.4f, int layers = 6)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _shadowColor = shadowColor ?? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ShadowColor ?? Color.Black;
            _opacity = opacity;
            _layers = layers;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, _offsetX, _offsetY, _shadowColor, _opacity, _layers);
        }
    }

    /// <summary>
    /// Card shadow painter wrapper
    /// </summary>
    public class CardShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly CardShadowStyle _style;

        public CardShadowPainterWrapper(CardShadowStyle style = CardShadowStyle.Medium)
        {
            _style = style;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintCardShadow(g, bounds, radius, _style);
        }
    }

    /// <summary>
    /// Material shadow painter wrapper
    /// </summary>
    public class MaterialShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly MaterialElevation _elevation;

        public MaterialShadowPainterWrapper(MaterialElevation elevation = MaterialElevation.Level2)
        {
            _elevation = elevation;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            // Use PaintDualLayerShadow with proper Material 3 elevation parameters
            Color shadowColor = theme?.ShadowColor ?? Color.FromArgb(30, 30, 30);
            return ShadowPainterHelpers.PaintDualLayerShadow(g, bounds, radius, (int)_elevation, shadowColor);
        }
    }

    /// <summary>
    /// Floating shadow painter wrapper
    /// </summary>
    public class FloatingShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly int _elevation;

        public FloatingShadowPainterWrapper(int elevation = 8)
        {
            _elevation = elevation;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintFloatingShadow(g, bounds, radius, _elevation);
        }
    }

    /// <summary>
    /// Drop shadow painter wrapper
    /// </summary>
    public class DropShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly int _offsetX;
        private readonly int _offsetY;
        private readonly int _blurRadius;
        private readonly Color _shadowColor;
        private readonly float _opacity;

        public DropShadowPainterWrapper(int offsetX = 0, int offsetY = 4, int blurRadius = 8, Color? shadowColor = null, float opacity = 0.5f)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _blurRadius = blurRadius;
            _shadowColor = shadowColor ?? Color.Black;
            _opacity = opacity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintDropShadow(g, bounds, radius, _offsetX, _offsetY, _blurRadius, _shadowColor, _opacity);
        }
    }

    /// <summary>
    /// Inner shadow painter wrapper
    /// </summary>
    public class InnerShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly int _depth;
        private readonly Color? _shadowColor;

        public InnerShadowPainterWrapper(int depth = 4, Color? shadowColor = null)
        {
            _depth = depth;
            _shadowColor = shadowColor;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintInnerShadow(g, bounds, radius, _depth, _shadowColor);
        }
    }

    /// <summary>
    /// Long shadow painter wrapper
    /// </summary>
    public class LongShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly float _angle;
        private readonly int _length;
        private readonly Color? _shadowColor;

        public LongShadowPainterWrapper(float angle = 45f, int length = 20, Color? shadowColor = null)
        {
            _angle = angle;
            _length = length;
            _shadowColor = shadowColor;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintLongShadow(g, bounds, radius, _angle, _length, _shadowColor);
        }
    }

    /// <summary>
    /// Colored shadow painter wrapper
    /// </summary>
    public class ColoredShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly Color _baseColor;
        private readonly int _offsetX;
        private readonly int _offsetY;
        private readonly float _intensity;

        public ColoredShadowPainterWrapper(Color baseColor, int offsetX = 0, int offsetY = 4, float intensity = 0.6f)
        {
            _baseColor = baseColor;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _intensity = intensity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintColoredShadow(g, bounds, radius, _baseColor, _offsetX, _offsetY, _intensity);
        }
    }

    /// <summary>
    /// Neon glow painter wrapper
    /// </summary>
    public class NeonGlowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly Color _glowColor;
        private readonly float _intensity;
        private readonly int _glowRadius;

        public NeonGlowPainterWrapper(Color glowColor, float intensity = 1f, int glowRadius = 12)
        {
            _glowColor = glowColor;
            _intensity = intensity;
            _glowRadius = glowRadius;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintNeonGlow(g, bounds, radius, _glowColor, _intensity, _glowRadius);
        }
    }

    /// <summary>
    /// Ambient shadow painter wrapper
    /// </summary>
    public class AmbientShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly int _spread;
        private readonly float _opacity;

        public AmbientShadowPainterWrapper(int spread = 4, float opacity = 0.4f)
        {
            _spread = spread;
            _opacity = opacity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintAmbientShadow(g, bounds, radius, _spread, _opacity);
        }
    }

    /// <summary>
    /// Perspective shadow painter wrapper
    /// </summary>
    public class PerspectiveShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly PerspectiveDirection _direction;
        private readonly float _intensity;

        public PerspectiveShadowPainterWrapper(PerspectiveDirection direction = PerspectiveDirection.BottomRight, float intensity = 0.5f)
        {
            _direction = direction;
            _intensity = intensity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintPerspectiveShadow(g, bounds, radius, _direction, _intensity);
        }
    }

    /// <summary>
    /// Double shadow painter wrapper
    /// </summary>
    public class DoubleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly Color _color1;
        private readonly Color _color2;
        private readonly int _offset1X, _offset1Y, _offset2X, _offset2Y;

        public DoubleShadowPainterWrapper(Color? color1 = null, Color? color2 = null, 
            int offset1X = 2, int offset1Y = 2, int offset2X = 4, int offset2Y = 4)
        {
            var baseShadow = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ShadowColor ?? Color.Black;
            _color1 = color1 ?? Color.FromArgb(100, baseShadow);
            _color2 = color2 ?? Color.FromArgb(60, baseShadow);
            _offset1X = offset1X;
            _offset1Y = offset1Y;
            _offset2X = offset2X;
            _offset2Y = offset2Y;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintDoubleShadow(g, bounds, radius, _color1, _color2, 
                _offset1X, _offset1Y, _offset2X, _offset2Y);
        }
    }

    /// <summary>
    /// Neumorphic shadow painter wrapper
    /// </summary>
    public class NeumorphicShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly Color _backgroundColor;

        public NeumorphicShadowPainterWrapper(Color backgroundColor)
        {
            _backgroundColor = backgroundColor;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintNeumorphicShadow(g, bounds, radius, _backgroundColor);
        }
    }

    /// <summary>
    /// Glass shadow painter wrapper
    /// </summary>
    public class GlassShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly Color _tintColor;
        private readonly float _opacity;

        public GlassShadowPainterWrapper(Color tintColor, float opacity = 0.3f)
        {
            _tintColor = tintColor;
            _opacity = opacity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintGlassShadow(g, bounds, radius, _tintColor, _opacity);
        }
    }

    /// <summary>
    /// Border glow painter wrapper
    /// </summary>
    public class BorderGlowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly Color _glowColor;
        private readonly int _glowWidth;
        private readonly float _intensity;

        public BorderGlowPainterWrapper(Color glowColor, int glowWidth = 3, float intensity = 0.8f)
        {
            _glowColor = glowColor;
            _glowWidth = glowWidth;
            _intensity = intensity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.PaintBorderGlow(g, bounds, radius, _glowColor, _glowWidth, _intensity);
        }
    }

    /// <summary>
    /// Preset shadow painter wrapper
    /// </summary>
    public class PresetShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly ModernShadowPreset _preset;

        public PresetShadowPainterWrapper(ModernShadowPreset preset)
        {
            _preset = preset;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadowPainterHelpers.ApplyModernShadowPreset(g, bounds, radius, _preset);
        }
    }

    /// <summary>
    /// Adaptive shadow painter wrapper
    /// </summary>
    public class AdaptiveShadowPainterWrapper : ShadowPainterWrapperBase
    {
        private readonly ShadowIntensity _intensity;

        public AdaptiveShadowPainterWrapper(ShadowIntensity intensity = ShadowIntensity.Medium)
        {
            _intensity = intensity;
        }

        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            var (offsetX, offsetY, blur, opacity) = ShadowPainterHelpers.CalculateAdaptiveShadow(bounds, _intensity);
            var baseShadow = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ShadowColor ?? Color.Black;
            return ShadowPainterHelpers.PaintDropShadow(g, bounds, radius, offsetX, offsetY, blur, baseShadow, opacity);
        }
    }

    /// <summary>
    /// Apple Style shadow painter wrapper
    /// </summary>
    public class AppleStyleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return AppleShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Apple, theme, true, ControlState.Normal);
        }
    }

    /// <summary>
    /// Fluent (legacy) Style shadow painter wrapper
    /// </summary>
    public class FluentStyleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return FluentShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Fluent, theme, true, ControlState.Normal);
        }
    }

    /// <summary>
    /// Material (legacy) Style shadow painter wrapper
    /// </summary>
    public class MaterialStyleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return MaterialShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Material, theme, true, MaterialElevation.Level2, ControlState.Normal);
        }
    }

    /// <summary>
    /// WebFramework Style shadow painter wrapper
    /// </summary>
    public class WebFrameworkStyleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return WebFrameworkShadowPainter.Paint(g, bounds, radius, BeepControlStyle.WebFramework, theme, true, ControlState.Normal);
        }
    }

    /// <summary>
    /// Effect Style shadow painter wrapper
    /// </summary>
    public class EffectStyleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return EffectShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Effect, theme, true, ControlState.Normal);
        }
    }

    public class ArcLinuxShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ArcLinuxShadowPainter.Paint(g, bounds, radius, BeepControlStyle.ArcLinux, theme, true, ControlState.Normal);
        }
    }

    public class BrutalistShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return BrutalistShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Brutalist, theme, true, ControlState.Normal);
        }
    }

    public class CartoonShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return CartoonShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Cartoon, theme, true, ControlState.Normal);
        }
    }

    public class ChatBubbleShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ChatBubbleShadowPainter.Paint(g, bounds, radius, BeepControlStyle.ChatBubble, theme, true, ControlState.Normal);
        }
    }

    public class CyberpunkShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return CyberpunkShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Cyberpunk, theme, true, ControlState.Normal);
        }
    }

    public class DraculaShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return DraculaShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Dracula, theme, true, ControlState.Normal);
        }
    }

    public class GlassmorphismShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return GlassmorphismShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Glassmorphism, theme, true, ControlState.Normal);
        }
    }

    public class HolographicShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return HolographicShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Holographic, theme, true, ControlState.Normal);
        }
    }

    public class GruvBoxShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return GruvBoxShadowPainter.Paint(g, bounds, radius, BeepControlStyle.GruvBox, theme, true, ControlState.Normal);
        }
    }

    public class Metro2ShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return Metro2ShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Metro2, theme, true, ControlState.Normal);
        }
    }

    public class NeoBrutalistShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return NeoBrutalistShadowPainter.Paint(g, bounds, radius, BeepControlStyle.NeoBrutalist, theme, true, MaterialElevation.Level0, ControlState.Normal);
        }
    }

    public class ModernShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ModernShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Modern, theme, true, ControlState.Normal);
        }
    }

    public class NordShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return NordShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Nord, theme, true, ControlState.Normal);
        }
    }

    public class NordicShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return NordicShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Nordic, theme, true, ControlState.Normal);
        }
    }

    public class OneDarkShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return OneDarkShadowPainter.Paint(g, bounds, radius, BeepControlStyle.OneDark, theme, true, ControlState.Normal);
        }
    }

    public class PaperShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return PaperShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Paper, theme, true, ControlState.Normal);
        }
    }

    public class RetroShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return RetroShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Retro, theme, true, ControlState.Normal);
        }
    }

    public class SolarizedShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return SolarizedShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Solarized, theme, true, ControlState.Normal);
        }
    }

    public class TerminalShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return TerminalShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Terminal, theme, true, ControlState.Normal);
        }
    }

    public class TokyoShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return TokyoShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Tokyo, theme, true, ControlState.Normal);
        }
    }

    public class UbuntuShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return UbuntuShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Ubuntu, theme, true, ControlState.Normal);
        }
    }

    public class ShadcnShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return ShadcnShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Shadcn, theme, true, ControlState.Normal);
        }
    }

    public class RadixUIShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return RadixUIShadowPainter.Paint(g, bounds, radius, BeepControlStyle.RadixUI, theme, true, ControlState.Normal);
        }
    }

    public class NextJSShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return NextJSShadowPainter.Paint(g, bounds, radius, BeepControlStyle.NextJS, theme, true, ControlState.Normal);
        }
    }

    public class LinearShadowPainterWrapper : ShadowPainterWrapperBase
    {
        public override GraphicsPath Paint(Graphics g, GraphicsPath bounds, int radius, IBeepTheme theme = null)
        {
            return LinearShadowPainter.Paint(g, bounds, radius, BeepControlStyle.Linear, theme, true, ControlState.Normal);
        }
    }

    #endregion

    #region Shadow Type Enumeration
    /// <summary>
    /// Types of shadows available
    /// </summary>
    public enum ShadowType
    {
        None,
        Soft,
        Card,
        Material,
        Floating,
        Drop,
        Inner,
        Long,
        Colored,
        Neon,
        Ambient,
        Perspective,
        Double,
        Neumorphic,
        Glass,
        BorderGlow
    }
    #endregion
}
