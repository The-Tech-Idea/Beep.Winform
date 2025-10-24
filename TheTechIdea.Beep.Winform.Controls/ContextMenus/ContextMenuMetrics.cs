using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Metrics and visual settings for context menu styles
    /// Provides default values for each FormStyle
    /// </summary>
    public sealed class ContextMenuMetrics
    {
        #region Dimensions
        public int ItemHeight { get; set; } = 32;
        public int IconSize { get; set; } = 20;
        public int SeparatorHeight { get; set; } = 8;
        public int Padding { get; set; } = 4;
        public int ItemPaddingLeft { get; set; } = 12;
        public int ItemPaddingRight { get; set; } = 12;
        public int IconTextSpacing { get; set; } = 12;
        public int MinWidth { get; set; } = 150;
        public int MaxWidth { get; set; } = 400;
        public int SubmenuArrowSize { get; set; } = 16;
        public int CheckboxSize { get; set; } = 16;
        #endregion

        #region Border and Shadow
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 8;
        public int ShadowDepth { get; set; } = 4;
        public int ShadowBlur { get; set; } = 8;
        public int ShadowAlpha { get; set; } = 60;
        #endregion

        #region Background Colors
        public Color BackgroundColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color SeparatorColor { get; set; } = Color.FromArgb(230, 230, 230);
        #endregion

        #region Item Colors
        public Color ItemForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ItemBackColor { get; set; } = Color.Transparent;
        public Color ItemHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color ItemHoverForeColor { get; set; } = Color.FromArgb(0, 0, 0);
        public Color ItemSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ItemSelectedForeColor { get; set; } = Color.White;
        public Color ItemDisabledForeColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color ItemDisabledBackColor { get; set; } = Color.Transparent;
        #endregion

        #region Accent Colors
        public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color AccentHoverColor { get; set; } = Color.FromArgb(0, 150, 255);
        public int AccentBarWidth { get; set; } = 0;
        #endregion

        #region Shortcut Colors
        public Color ShortcutForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ShortcutHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        #endregion

        #region Fonts
        public float TextFontSize { get; set; } = 9f;
        public float ShortcutFontSize { get; set; } = 8f;
        public FontStyle TextFontStyle { get; set; } = FontStyle.Regular;
        #endregion

        #region Theme Integration
        public bool UseThemeColors { get; set; } = false;
        public IBeepTheme BeepTheme { get; set; }
        #endregion

        #region Style-Specific Features
        public bool ShowElevation { get; set; } = true;
        public bool ShowRippleEffect { get; set; } = false;
        public bool UseRoundedItems { get; set; } = true;
        public int ItemBorderRadius { get; set; } = 4;
        #endregion

        /// <summary>
        /// Creates default metrics for a specific FormStyle
        /// </summary>
        public static ContextMenuMetrics DefaultFor(FormStyle style, IBeepTheme theme = null, bool useThemeColors = false)
        {
            var m = new ContextMenuMetrics();
            
            // Apply Style-specific defaults
            switch (style)
            {
                case FormStyle.Modern:
                    ApplyModernStyle(m);
                    break;
                case FormStyle.Minimal:
                    ApplyMinimalStyle(m);
                    break;
                case FormStyle.Material:
                    ApplyMaterialStyle(m);
                    break;
                case FormStyle.Fluent:
                    ApplyFluentStyle(m);
                    break;
                case FormStyle.MacOS:
                    ApplyMacOSStyle(m);
                    break;
                case FormStyle.iOS:
                    ApplyiOSStyle(m);
                    break;
                case FormStyle.Glass:
                    ApplyGlassStyle(m);
                    break;
                case FormStyle.Cartoon:
                    ApplyCartoonStyle(m);
                    break;
                case FormStyle.ChatBubble:
                    ApplyChatBubbleStyle(m);
                    break;
                case FormStyle.Metro:
                    ApplyMetroStyle(m);
                    break;
                case FormStyle.Metro2:
                    ApplyMetro2Style(m);
                    break;
                case FormStyle.GNOME:
                    ApplyGNOMEStyle(m);
                    break;
                case FormStyle.NeoMorphism:
                    ApplyNeoMorphismStyle(m);
                    break;
                case FormStyle.Glassmorphism:
                    ApplyGlassmorphismStyle(m);
                    break;
                case FormStyle.Brutalist:
                    ApplyBrutalistStyle(m);
                    break;
                case FormStyle.Retro:
                    ApplyRetroStyle(m);
                    break;
                case FormStyle.Cyberpunk:
                    ApplyCyberpunkStyle(m);
                    break;
                case FormStyle.Nordic:
                    ApplyNordicStyle(m);
                    break;
                case FormStyle.Ubuntu:
                    ApplyUbuntuStyle(m);
                    break;
                case FormStyle.KDE:
                    ApplyKDEStyle(m);
                    break;
                case FormStyle.ArcLinux:
                    ApplyArcLinuxStyle(m);
                    break;
                case FormStyle.Dracula:
                    ApplyDraculaStyle(m);
                    break;
                case FormStyle.Solarized:
                    ApplySolarizedStyle(m);
                    break;
                case FormStyle.OneDark:
                    ApplyOneDarkStyle(m);
                    break;
                case FormStyle.GruvBox:
                    ApplyGruvBoxStyle(m);
                    break;
                case FormStyle.Nord:
                    ApplyNordStyle(m);
                    break;
                case FormStyle.Tokyo:
                    ApplyTokyoStyle(m);
                    break;
                case FormStyle.Paper:
                    ApplyPaperStyle(m);
                    break;
                case FormStyle.Neon:
                    ApplyNeonStyle(m);
                    break;
                case FormStyle.Holographic:
                    ApplyHolographicStyle(m);
                    break;
                case FormStyle.Custom:
                    // Keep defaults for custom
                    break;
                default:
                    ApplyModernStyle(m);
                    break;
            }

            // Apply theme colors if requested
            if (useThemeColors && theme != null)
            {
                ApplyThemeColors(m, theme);
            }

            return m;
        }

        #region Style Application Methods (Private)
        
        private static void ApplyModernStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 8;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(255, 255, 255);
            m.ItemHoverBackColor = Color.FromArgb(240, 240, 240);
            m.ItemSelectedBackColor = Color.FromArgb(0, 120, 215);
        }

        private static void ApplyMinimalStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 28;
            m.IconSize = 18;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.ShadowDepth = 2;
            m.Padding = 2;
            m.BackgroundColor = Color.FromArgb(252, 252, 252);
            m.ItemHoverBackColor = Color.FromArgb(245, 245, 245);
        }

        private static void ApplyMaterialStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 36;
            m.IconSize = 24;
            m.BorderRadius = 8;
            m.ItemBorderRadius = 6;
            m.ShadowDepth = 6;
            m.ShowRippleEffect = true;
            m.AccentBarWidth = 4;
            m.ItemPaddingLeft = 16;
            m.BackgroundColor = Color.FromArgb(255, 255, 255);
            m.AccentColor = Color.FromArgb(33, 150, 243);
        }

        private static void ApplyFluentStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 34;
            m.IconSize = 20;
            m.BorderRadius = 6;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 3;
            m.ShadowBlur = 12;
            m.BackgroundColor = Color.FromArgb(250, 250, 250);
            m.ItemHoverBackColor = Color.FromArgb(243, 243, 243);
            m.UseRoundedItems = true;
        }

        private static void ApplyMacOSStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 20;
            m.BorderRadius = 10;
            m.ItemBorderRadius = 5;
            m.ShadowDepth = 5;
            m.BackgroundColor = Color.FromArgb(250, 250, 250);
            m.ItemHoverBackColor = Color.FromArgb(30, 0, 122, 255);
            m.ItemSelectedBackColor = Color.FromArgb(0, 122, 255);
            m.BorderColor = Color.FromArgb(200, 200, 205);
        }

        private static void ApplyiOSStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 44;
            m.IconSize = 22;
            m.BorderRadius = 12;
            m.ItemBorderRadius = 8;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(248, 248, 252);
            m.ItemHoverBackColor = Color.FromArgb(240, 240, 245);
            m.ItemSelectedBackColor = Color.FromArgb(0, 122, 255);
            m.AccentColor = Color.FromArgb(0, 122, 255);
        }

        private static void ApplyGlassStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 10;
            m.ItemBorderRadius = 6;
            m.ShadowDepth = 8;
            m.ShadowAlpha = 40;
            m.BackgroundColor = Color.FromArgb(250, 255, 255, 255);
            m.BorderColor = Color.FromArgb(100, 255, 255, 255);
        }

        private static void ApplyCartoonStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 38;
            m.IconSize = 24;
            m.BorderRadius = 16;
            m.ItemBorderRadius = 12;
            m.BorderWidth = 3;
            m.ShadowDepth = 6;
            m.BackgroundColor = Color.FromArgb(255, 240, 255);
            m.ItemHoverBackColor = Color.FromArgb(255, 220, 245);
            m.ItemSelectedBackColor = Color.FromArgb(255, 105, 180);
            m.BorderColor = Color.FromArgb(200, 100, 255);
        }

        private static void ApplyChatBubbleStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 34;
            m.IconSize = 20;
            m.BorderRadius = 16;
            m.ItemBorderRadius = 10;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(230, 250, 255);
            m.ItemHoverBackColor = Color.FromArgb(210, 240, 255);
            m.BorderColor = Color.FromArgb(180, 220, 240);
        }

        private static void ApplyMetroStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 0;
            m.ItemBorderRadius = 0;
            m.ShadowDepth = 0;
            m.UseRoundedItems = false;
            m.BackgroundColor = Color.FromArgb(255, 255, 255);
            m.ItemHoverBackColor = Color.FromArgb(0, 120, 215);
            m.ItemHoverForeColor = Color.White;
            m.BorderColor = Color.FromArgb(180, 180, 180);
        }

        private static void ApplyMetro2Style(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 0;
            m.ItemBorderRadius = 0;
            m.ShadowDepth = 2;
            m.AccentBarWidth = 2;
            m.BackgroundColor = Color.FromArgb(240, 240, 240);
            m.AccentColor = Color.FromArgb(0, 120, 215);
            m.BorderColor = Color.FromArgb(0, 120, 215);
        }

        private static void ApplyGNOMEStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 8;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(245, 245, 245);
            m.ItemHoverBackColor = Color.FromArgb(230, 230, 230);
            m.BorderColor = Color.FromArgb(200, 200, 200);
        }

        private static void ApplyNeoMorphismStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 36;
            m.IconSize = 22;
            m.BorderRadius = 12;
            m.ItemBorderRadius = 10;
            m.BorderWidth = 0;
            m.ShadowDepth = 8;
            m.ShadowBlur = 16;
            m.BackgroundColor = Color.FromArgb(240, 240, 245);
            m.ItemHoverBackColor = Color.FromArgb(235, 235, 242);
        }

        private static void ApplyGlassmorphismStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 34;
            m.IconSize = 20;
            m.BorderRadius = 12;
            m.ItemBorderRadius = 8;
            m.ShadowDepth = 6;
            m.ShadowAlpha = 30;
            m.BackgroundColor = Color.FromArgb(240, 245, 250);
            m.BorderColor = Color.FromArgb(80, 255, 255, 255);
            m.ItemHoverBackColor = Color.FromArgb(230, 240, 248);
        }

        private static void ApplyBrutalistStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 36;
            m.IconSize = 22;
            m.BorderRadius = 0;
            m.ItemBorderRadius = 0;
            m.BorderWidth = 3;
            m.ShadowDepth = 0;
            m.UseRoundedItems = false;
            m.BackgroundColor = Color.FromArgb(255, 255, 255);
            m.BorderColor = Color.Black;
            m.ItemHoverBackColor = Color.Black;
            m.ItemHoverForeColor = Color.White;
        }

        private static void ApplyRetroStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 0;
            m.ItemBorderRadius = 0;
            m.BorderWidth = 2;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(30, 25, 50);
            m.ItemForeColor = Color.FromArgb(0, 255, 255);
            m.ItemHoverBackColor = Color.FromArgb(60, 50, 100);
            m.BorderColor = Color.FromArgb(255, 0, 255);
            m.AccentBarWidth = 2;
        }

        private static void ApplyCyberpunkStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 34;
            m.IconSize = 20;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.BorderWidth = 2;
            m.AccentBarWidth = 3;
            m.ShadowDepth = 8;
            m.BackgroundColor = Color.FromArgb(10, 10, 20);
            m.ItemForeColor = Color.FromArgb(0, 255, 255);
            m.AccentColor = Color.FromArgb(0, 255, 255);
            m.BorderColor = Color.FromArgb(0, 255, 255);
            m.ItemHoverBackColor = Color.FromArgb(20, 40, 60);
        }

        private static void ApplyNordicStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 6;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 3;
            m.BackgroundColor = Color.FromArgb(252, 252, 252);
            m.ItemHoverBackColor = Color.FromArgb(245, 245, 245);
            m.BorderColor = Color.FromArgb(220, 220, 220);
        }

        private static void ApplyUbuntuStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 20;
            m.BorderRadius = 6;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(245, 245, 245);
            m.AccentColor = Color.FromArgb(233, 84, 32);
            m.ItemSelectedBackColor = Color.FromArgb(233, 84, 32);
            m.BorderColor = Color.FromArgb(180, 180, 180);
        }

        private static void ApplyKDEStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 6;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(239, 240, 241);
            m.AccentColor = Color.FromArgb(61, 174, 233);
            m.ItemSelectedBackColor = Color.FromArgb(61, 174, 233);
            m.BorderColor = Color.FromArgb(190, 190, 195);
        }

        private static void ApplyArcLinuxStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 20;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.AccentBarWidth = 2;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(56, 60, 74);
            m.ItemForeColor = Color.FromArgb(211, 218, 227);
            m.AccentColor = Color.FromArgb(82, 148, 226);
            m.BorderColor = Color.FromArgb(64, 69, 82);
        }

        private static void ApplyDraculaStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 32;
            m.IconSize = 20;
            m.BorderRadius = 6;
            m.ItemBorderRadius = 4;
            m.ShadowDepth = 6;
            m.BackgroundColor = Color.FromArgb(40, 42, 54);
            m.ItemForeColor = Color.FromArgb(248, 248, 242);
            m.AccentColor = Color.FromArgb(189, 147, 249);
            m.ItemHoverBackColor = Color.FromArgb(68, 71, 90);
            m.BorderColor = Color.FromArgb(68, 71, 90);
        }

        private static void ApplySolarizedStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 20;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.AccentBarWidth = 2;
            m.ShadowDepth = 3;
            m.BackgroundColor = Color.FromArgb(253, 246, 227);
            m.ItemForeColor = Color.FromArgb(88, 110, 117);
            m.AccentColor = Color.FromArgb(38, 139, 210);
            m.BorderColor = Color.FromArgb(147, 161, 161);
        }

        private static void ApplyOneDarkStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 18;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(40, 44, 52);
            m.ItemForeColor = Color.FromArgb(171, 178, 191);
            m.AccentColor = Color.FromArgb(97, 175, 239);
            m.BorderColor = Color.FromArgb(60, 66, 82);
        }

        private static void ApplyGruvBoxStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 18;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(40, 40, 40);
            m.ItemForeColor = Color.FromArgb(235, 219, 178);
            m.AccentColor = Color.FromArgb(254, 128, 25);
            m.BorderColor = Color.FromArgb(80, 73, 69);
        }

        private static void ApplyNordStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 18;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(46, 52, 64);
            m.ItemForeColor = Color.FromArgb(216, 222, 233);
            m.AccentColor = Color.FromArgb(136, 192, 208);
            m.BorderColor = Color.FromArgb(67, 76, 94);
        }

        private static void ApplyTokyoStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 30;
            m.IconSize = 18;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.ShadowDepth = 4;
            m.BackgroundColor = Color.FromArgb(26, 27, 38);
            m.ItemForeColor = Color.FromArgb(169, 177, 214);
            m.AccentColor = Color.FromArgb(125, 207, 255);
            m.BorderColor = Color.FromArgb(41, 46, 73);
        }

        private static void ApplyPaperStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 36;
            m.IconSize = 22;
            m.BorderRadius = 4;
            m.ItemBorderRadius = 2;
            m.BorderWidth = 0;
            m.ShadowDepth = 2;
            m.BackgroundColor = Color.FromArgb(250, 250, 250);
            m.ItemHoverBackColor = Color.FromArgb(245, 245, 245);
            m.BorderColor = Color.FromArgb(230, 230, 230);
        }

        private static void ApplyNeonStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 34;
            m.IconSize = 20;
            m.BorderRadius = 6;
            m.ItemBorderRadius = 4;
            m.BorderWidth = 2;
            m.AccentBarWidth = 4;
            m.ShadowDepth = 8;
            m.BackgroundColor = Color.FromArgb(15, 15, 25);
            m.ItemForeColor = Color.FromArgb(0, 255, 255);
            m.AccentColor = Color.FromArgb(0, 255, 200);
            m.BorderColor = Color.FromArgb(0, 255, 255);
            m.ItemHoverBackColor = Color.FromArgb(20, 30, 40);
        }

        private static void ApplyHolographicStyle(ContextMenuMetrics m)
        {
            m.ItemHeight = 34;
            m.IconSize = 20;
            m.BorderRadius = 8;
            m.ItemBorderRadius = 6;
            m.BorderWidth = 1;
            m.AccentBarWidth = 3;
            m.ShadowDepth = 6;
            m.BackgroundColor = Color.FromArgb(25, 20, 35);
            m.ItemForeColor = Color.FromArgb(200, 150, 255);
            m.AccentColor = Color.FromArgb(150, 200, 255);
            m.BorderColor = Color.FromArgb(150, 200, 255);
        }

        #endregion

        #region Theme Color Application

        private static void ApplyThemeColors(ContextMenuMetrics m, IBeepTheme theme)
        {
            m.BeepTheme = theme;
            m.UseThemeColors = true;

            // Background colors
            m.BackgroundColor = theme.MenuBackColor;
            m.BorderColor = theme.BorderColor;
            m.SeparatorColor = Color.FromArgb(30, theme.MenuItemForeColor);

            // Item colors
            m.ItemForeColor = theme.MenuItemForeColor;
            m.ItemBackColor = Color.Transparent;
            m.ItemHoverBackColor = theme.MenuItemHoverBackColor;
            m.ItemHoverForeColor = theme.MenuItemHoverForeColor;
            m.ItemSelectedBackColor = theme.MenuItemSelectedBackColor;
            m.ItemSelectedForeColor = theme.MenuItemSelectedForeColor;
            m.ItemDisabledForeColor = theme.DisabledForeColor;

            // Accent colors
            m.AccentColor = theme.AccentColor;
            m.AccentHoverColor = theme.ButtonHoverBackColor;

            // Shortcut colors
            m.ShortcutForeColor = Color.FromArgb(160, theme.MenuItemForeColor.R, theme.MenuItemForeColor.G, theme.MenuItemForeColor.B);
            m.ShortcutHoverForeColor = Color.FromArgb(200, theme.MenuItemHoverForeColor.R, theme.MenuItemHoverForeColor.G, theme.MenuItemHoverForeColor.B);
        }

        #endregion
    }
}
