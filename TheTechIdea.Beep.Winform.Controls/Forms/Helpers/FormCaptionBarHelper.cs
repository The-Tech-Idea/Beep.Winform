using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Forms.Caption;
using TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    internal sealed class FormCaptionBarHelper
    {
        public delegate void PaddingAdjuster(ref Padding padding); // must match BeepiForm signature
        private readonly IBeepModernFormHost _host;
        private readonly FormOverlayPainterRegistry _overlayRegistry;
        private readonly Action<PaddingAdjuster> _registerPaddingProvider;
        
        // Logo/Icon functionality
        private string _logoImagePath = string.Empty;
        private ImagePainter _logoPainter;
        private bool _showLogo = false;
        private Size _logoSize = new Size(20, 20);
        private Padding _logoMargin = new Padding(8, 8, 8, 8);

        public bool ShowCaptionBar { get; set; } = true;
        public int CaptionHeight { get; set; } = 36;
        public bool ShowSystemButtons { get; set; } = true;
        public bool EnableCaptionGradient { get; set; } = true;
        
        // Separate extra buttons
        public bool ShowThemeButton { get; set; } = false;
        public string ThemeButtonIconPath { get; set; } = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.024-dashboard.svg";
        private Rectangle _themeButtonRect;
        private bool _themeHover;

        public bool ShowStyleButton { get; set; } = false; // switches form style
        public string StyleButtonIconPath { get; set; } = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.024-dashboard.svg";
        private Rectangle _styleButtonRect;
        private bool _styleHover;

        // Caption title overrides
        public Font? TitleFontOverride { get; set; }
        public Color? TitleForeColorOverride { get; set; }
        
        // Renderer strategy - now based on BeepFormStyle
        private ICaptionRenderer _renderer;
        public BeepFormStyle CurrentStyle { get; private set; } = BeepFormStyle.Modern;

        // Legacy compatibility property
        [Obsolete("Use CurrentStyle instead. This property maps to BeepFormStyle for compatibility.")]
        public CaptionRendererKind RendererKind 
        { 
            get => MapFormStyleToCaptionKind(CurrentStyle);
            private set => CurrentStyle = MapCaptionKindToFormStyle(value);
        }

        // Logo/Icon properties
        public string LogoImagePath
        {
            get => _logoImagePath;
            set
            {
                if (_logoImagePath != value)
                {
                    _logoImagePath = value;
                    InitializeLogoPainter();
                    _showLogo = !string.IsNullOrEmpty(value);
                    Form.Invalidate();
                }
            }
        }

        public bool ShowLogo
        {
            get => _showLogo;
            set
            {
                if (_showLogo != value)
                {
                    _showLogo = value;
                    Form.Invalidate();
                }
            }
        }

        public Size LogoSize
        {
            get => _logoSize;
            set
            {
                if (_logoSize != value)
                {
                    _logoSize = value;
                    if (_showLogo) Form.Invalidate();
                }
            }
        }

        public Padding LogoMargin
        {
            get => _logoMargin;
            set
            {
                if (_logoMargin != value)
                {
                    _logoMargin = value;
                    if (_showLogo) Form.Invalidate();
                }
            }
        }

        public Rectangle CloseRect { get; private set; }
        public Rectangle MaxRect { get; private set; }
        public Rectangle MinRect { get; private set; }
        private bool _hoverClose, _hoverMax, _hoverMin;
        private Form Form => _host.AsForm;
        private IBeepTheme Theme => _host.CurrentTheme;
        
        public FormCaptionBarHelper(IBeepModernFormHost host, FormOverlayPainterRegistry overlayRegistry, Action<PaddingAdjuster> registerPaddingProvider)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _overlayRegistry = overlayRegistry ?? throw new ArgumentNullException(nameof(overlayRegistry));
            _registerPaddingProvider = registerPaddingProvider ?? throw new ArgumentNullException(nameof(registerPaddingProvider));
            _overlayRegistry.Add(PaintOverlay);
            _registerPaddingProvider((ref Padding p) =>
            {
                if (ShowCaptionBar)
                {
                    // Ensure exactly CaptionHeight is reserved at top for the client-area caption.
                    // Do not add any extra padding for non-client borders.
                    if (p.Top < CaptionHeight) p.Top = CaptionHeight; else p.Top = CaptionHeight;
                }
            });

            SetStyle(BeepFormStyle.Modern);
        }

        private void ForceCaptionRepaint()
        {
            try
            {
                // Clear any hover state from previous renderer to avoid stale visuals
                _hoverClose = _hoverMax = _hoverMin = false;
                var top = new Rectangle(0, 0, Form.ClientSize.Width, Math.Max(CaptionHeight, 24));
                Form.Invalidate(top, true);
                Form.Refresh();
            }
            catch
            {
                Form.Invalidate();
                Form.Refresh();
            }
        }

        // Main method to set style - replaces SetRenderer
        public void SetStyle(BeepFormStyle style)
        {
            CurrentStyle = style;
            _renderer = CreateRendererForStyle(style);
            _renderer.UpdateHost(Form, () => Theme, () => CaptionHeight);
            _renderer.UpdateTheme(Theme);
            _renderer.SetShowSystemButtons(ShowSystemButtons);

            // Apply renderer-specific visual preferences (border, shadow, glow)
            ApplyRendererVisualPreferences(style);

            // Apply defaults (may change CaptionHeight)
            int oldCaptionHeight = CaptionHeight;
            ApplyStyleVisualDefaults(style);
            TryApplyStylePreset(style);

            // If caption metrics changed, ensure layout updates now
            if (CaptionHeight != oldCaptionHeight && Form.IsHandleCreated)
            {
                try { Form.PerformLayout(); } catch { }
            }

            // Force immediate repaint of the caption (no click needed)
            ForceCaptionRepaint();
        }

        /// <summary>
        /// Applies renderer-specific border, shadow, and glow preferences to the form.
        /// Each renderer has its own optimal visual settings.
        /// </summary>
        private void ApplyRendererVisualPreferences(BeepFormStyle style)
        {
            // Get preferences for this style
            var pref = CaptionRendererPreferences.GetPreference(style);

            // Apply to BeepiForm if possible
            if (Form is BeepiForm beepiForm)
            {
                // Don't override BorderThickness - let user control it manually
                // beepiForm.BorderThickness = pref.BorderThickness;
                beepiForm.BorderRadius = pref.BorderRadius;
                beepiForm.ShadowDepth = pref.ShadowDepth;
                beepiForm.EnableGlow = pref.EnableGlow;
                beepiForm.GlowSpread = pref.GlowSpread;
            }
        }

        // Legacy compatibility method
        [Obsolete("Use SetStyle(BeepFormStyle) instead.")]
        public void SwitchRenderer(CaptionRendererKind kind)
        {
            SetStyle(MapCaptionKindToFormStyle(kind));
        }

        private ICaptionRenderer CreateRendererForStyle(BeepFormStyle style)
        {
            return style switch
            {
                BeepFormStyle.Material => new MacLikeCaptionRenderer(),
                BeepFormStyle.Gnome => new GnomeCaptionRenderer(),
                BeepFormStyle.Kde => new KdeCaptionRenderer(),
                BeepFormStyle.Cinnamon => new CinnamonCaptionRenderer(),
                BeepFormStyle.Elementary => new ElementaryCaptionRenderer(),
                BeepFormStyle.Neon => new NeonCaptionRenderer(),
                BeepFormStyle.Retro => new RetroCaptionRenderer(),
                BeepFormStyle.Gaming => new GamingCaptionRenderer(),
                BeepFormStyle.Corporate => new CorporateCaptionRenderer(),
                BeepFormStyle.Artistic => new ArtisticCaptionRenderer(),
                BeepFormStyle.HighContrast => new HighContrastCaptionRenderer(),
                BeepFormStyle.Soft => new SoftCaptionRenderer(),
                BeepFormStyle.Industrial => new IndustrialCaptionRenderer(),
                BeepFormStyle.Office => new OfficeCaptionRenderer(),
                BeepFormStyle.Metro => new MetroCaptionRenderer(),
                _ => new WindowsCaptionRenderer()
            };
        }

        // Legacy mapping methods for compatibility
        private static CaptionRendererKind MapFormStyleToCaptionKind(BeepFormStyle style)
        {
            return style switch
            {
                BeepFormStyle.Material => CaptionRendererKind.MacLike,
                BeepFormStyle.Gnome => CaptionRendererKind.Gnome,
                BeepFormStyle.Kde => CaptionRendererKind.Kde,
                BeepFormStyle.Cinnamon => CaptionRendererKind.Cinnamon,
                BeepFormStyle.Elementary => CaptionRendererKind.Elementary,
                BeepFormStyle.Neon => CaptionRendererKind.Neon,
                BeepFormStyle.Retro => CaptionRendererKind.Retro,
                BeepFormStyle.Gaming => CaptionRendererKind.Gaming,
                BeepFormStyle.Corporate => CaptionRendererKind.Corporate,
                BeepFormStyle.Artistic => CaptionRendererKind.Artistic,
                BeepFormStyle.HighContrast => CaptionRendererKind.HighContrast,
                BeepFormStyle.Soft => CaptionRendererKind.Soft,
                BeepFormStyle.Industrial => CaptionRendererKind.Industrial,
                _ => CaptionRendererKind.Windows
            };
        }

        private static BeepFormStyle MapCaptionKindToFormStyle(CaptionRendererKind kind)
        {
            return kind switch
            {
                CaptionRendererKind.MacLike => BeepFormStyle.Material,
                CaptionRendererKind.Gnome => BeepFormStyle.Gnome,
                CaptionRendererKind.Kde => BeepFormStyle.Kde,
                CaptionRendererKind.Cinnamon => BeepFormStyle.Cinnamon,
                CaptionRendererKind.Elementary => BeepFormStyle.Elementary,
                CaptionRendererKind.Neon => BeepFormStyle.Neon,
                CaptionRendererKind.Retro => BeepFormStyle.Retro,
                CaptionRendererKind.Gaming => BeepFormStyle.Gaming,
                CaptionRendererKind.Corporate => BeepFormStyle.Corporate,
                CaptionRendererKind.Artistic => BeepFormStyle.Artistic,
                CaptionRendererKind.HighContrast => BeepFormStyle.HighContrast,
                CaptionRendererKind.Soft => BeepFormStyle.Soft,
                CaptionRendererKind.Industrial => BeepFormStyle.Industrial,
                _ => BeepFormStyle.Modern
            };
        }

        private void TryApplyStylePreset(BeepFormStyle style)
        {
            try
            {
                var form = _host.AsForm;
                var formType = form.GetType();

                // Respect host flags if available
                bool autoApply = true;
                bool autoPickDark = true;
                var autoApplyProp = formType.GetProperty("AutoApplyRendererPreset");
                var autoDarkProp = formType.GetProperty("AutoPickDarkPresets");
                if (autoApplyProp != null) autoApply = (bool)(autoApplyProp.GetValue(form) ?? true);
                if (autoDarkProp != null) autoPickDark = (bool)(autoDarkProp.GetValue(form) ?? true);

                if (!autoApply) return;

                // Decide light/dark key suffix
                string suffix = ".light";
                if (autoPickDark)
                {
                    try
                    {
                        string themeName = formType.GetProperty("Theme")?.GetValue(form) as string ?? string.Empty;
                        bool nameImpliesDark = themeName.Contains("dark", StringComparison.OrdinalIgnoreCase);
                        var t = Theme;
                        bool colorImpliesDark = t != null && (t.AppBarBackColor.GetBrightness() < 0.5f || t.BackColor.GetBrightness() < 0.5f);
                        if (nameImpliesDark || colorImpliesDark)
                            suffix = ".dark";
                    }
                    catch { }
                }

                // ApplyPreset based on style
                var applyPreset = formType.GetMethod("ApplyPreset", new[] { typeof(string) });
                if (applyPreset != null)
                {
                    string baseKey = style switch
                    {
                        BeepFormStyle.Material => "macos",
                        BeepFormStyle.Gnome => "gnome.adwaita",
                        BeepFormStyle.Kde => "kde.breeze",
                        BeepFormStyle.Cinnamon => "cinnamon.mint",
                        BeepFormStyle.Elementary => "elementary",
                        // For newer styles, we might not have presets yet, so they'll use the FormStyle defaults
                        BeepFormStyle.Neon => "neon",
                        BeepFormStyle.Gaming => "gaming",
                        BeepFormStyle.Industrial => "industrial",
                        _ => string.Empty
                    };
                    if (!string.IsNullOrEmpty(baseKey))
                    {
                        string key = baseKey + suffix;
                        try 
                        { 
                            applyPreset.Invoke(form, new object[] { key }); 
                            
                            // Force form region and visual update after preset application
                            var updateRegionMethod = formType.GetMethod("UpdateFormRegion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            updateRegionMethod?.Invoke(form, null);
                            
                            // Force a full repaint
                            form.Invalidate();
                            form.Update();
                        } 
                        catch { }
                      }
                }
            }
            catch { }
        }

        private void ApplyStyleVisualDefaults(BeepFormStyle style)
        {
            switch (style)
            {
                case BeepFormStyle.Gnome:
                    EnableCaptionGradient = false; if (CaptionHeight < 34) CaptionHeight = 34; break;
                case BeepFormStyle.Kde:
                    EnableCaptionGradient = true; if (CaptionHeight < 36) CaptionHeight = 36; break;
                case BeepFormStyle.Cinnamon:
                    EnableCaptionGradient = true; if (CaptionHeight < 38) CaptionHeight = 38; break;
                case BeepFormStyle.Elementary:
                    EnableCaptionGradient = false; if (CaptionHeight < 40) CaptionHeight = 40; break;
                case BeepFormStyle.Material:
                    EnableCaptionGradient = false; if (CaptionHeight < 36) CaptionHeight = 36; break;
                case BeepFormStyle.Neon:
                    EnableCaptionGradient = true; if (CaptionHeight < 38) CaptionHeight = 38; break;
                case BeepFormStyle.Retro:
                    EnableCaptionGradient = true; if (CaptionHeight < 42) CaptionHeight = 42; break;
                case BeepFormStyle.Gaming:
                    EnableCaptionGradient = false; if (CaptionHeight < 36) CaptionHeight = 36; break;
                case BeepFormStyle.Corporate:
                    EnableCaptionGradient = false; if (CaptionHeight < 32) CaptionHeight = 32; break;
                case BeepFormStyle.Artistic:
                    EnableCaptionGradient = true; if (CaptionHeight < 44) CaptionHeight = 44; break;
                case BeepFormStyle.HighContrast:
                    EnableCaptionGradient = false; if (CaptionHeight < 36) CaptionHeight = 36; break;
                case BeepFormStyle.Soft:
                    EnableCaptionGradient = true; if (CaptionHeight < 38) CaptionHeight = 38; break;
                case BeepFormStyle.Industrial:
                    EnableCaptionGradient = false; if (CaptionHeight < 34) CaptionHeight = 34; break;
                case BeepFormStyle.Modern:
                case BeepFormStyle.Classic:
                case BeepFormStyle.Metro:
                case BeepFormStyle.Glass:
                case BeepFormStyle.Office:
                default:
                    break;
            }
        }

        private void InitializeLogoPainter()
        {
            _logoPainter?.Dispose();
            _logoPainter = null;

            if (!string.IsNullOrEmpty(_logoImagePath))
            {
                try
                {
                    _logoPainter = new ImagePainter(_logoImagePath, Theme);
                    _logoPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                    _logoPainter.Alignment = ContentAlignment.MiddleCenter;
                    _logoPainter.ApplyThemeOnImage = true;
                    _logoPainter.ImageEmbededin = ImageEmbededin.Form;
                }
                catch (Exception)
                {
                    _logoPainter = null;
                }
            }
        }

        public void UpdateTheme()
        {
            if (_logoPainter != null && Theme != null)
            {
                _logoPainter.CurrentTheme = Theme;
                if (_logoPainter.ApplyThemeOnImage)
                {
                    Form.Invalidate();
                }
            }
            _renderer?.UpdateTheme(Theme);
        }

        public bool IsPointInSystemButtons(Point p)
            => (_renderer?.HitTest(p) == true)
               || (ShowThemeButton && _themeButtonRect.Contains(p))
               || (ShowStyleButton && _styleButtonRect.Contains(p));

        public bool IsCursorOverSystemButton => IsPointInSystemButtons(Form.PointToClient(Cursor.Position));
        
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!ShowCaptionBar) return;
            bool invalidate = false;
            if (ShowSystemButtons && _renderer != null)
            {
                if (_renderer.OnMouseMove(e.Location, out var inv))
                {
                  //  Form.Invalidate(inv);
                    invalidate = true;
                }
            }
            bool prevThemeHover = _themeHover;
            bool prevStyleHover = _styleHover;
            _themeHover = ShowThemeButton && _themeButtonRect.Contains(e.Location);
            _styleHover = ShowStyleButton && _styleButtonRect.Contains(e.Location);
            if (prevThemeHover != _themeHover) invalidate = true;
            if (prevStyleHover != _styleHover) invalidate = true;
            if (invalidate)
            {
                var invRect = Rectangle.Union(_themeButtonRect, _styleButtonRect);
                Form.Invalidate(invRect);
            }
        }
        
        public void OnMouseLeave()
        {
            if (_renderer == null && !ShowThemeButton && !ShowStyleButton) return;
            Rectangle inv = Rectangle.Empty;
            if (_renderer != null)
            {
               // _renderer.OnMouseLeave(out inv);
                if (!inv.IsEmpty) Form.Invalidate(inv);
            }
            if (_themeHover) { _themeHover = false; Form.Invalidate(_themeButtonRect); }
            if (_styleHover) { _styleHover = false; Form.Invalidate(_styleButtonRect); }
        }
        
        public void OnMouseDown(MouseEventArgs e)
        {
            if (!ShowCaptionBar) return;
            if (ShowThemeButton && _themeButtonRect.Contains(e.Location))
            {
                ShowThemeMenu();
                Form.Invalidate(_themeButtonRect);
                return;
            }
            if (ShowStyleButton && _styleButtonRect.Contains(e.Location))
            {
                ShowStyleMenu();
                Form.Invalidate(_styleButtonRect);
                return;
            }
            if (ShowSystemButtons && _renderer != null)
            {
                if (_renderer.OnMouseDown(e.Location, Form, out var inv))
                {
                    //if (!inv.IsEmpty) Form.Invalidate(inv);
                }
            }
        }

        private void ShowThemeMenu()
        {
            var menu = new ContextMenuStrip();
            try
            {
                foreach (string theme in BeepThemesManager.GetThemeNames())
                {
                    var item = new ToolStripMenuItem(theme);
                    item.Click += (s, e) =>
                    {
                        try
                        {
                            // Set global current theme like AppBar does
                            BeepThemesManager.SetCurrentTheme(theme);
                            // Set the host's MenuStyle property, which triggers ApplyTheme()
                            _host.AsForm.GetType().GetProperty("Theme")?.SetValue(_host.AsForm, theme);
                        }
                        catch { }
                    };
                    menu.Items.Add(item);
                }
            }
            catch { }
            var pt = Form.PointToScreen(new Point(_themeButtonRect.Left, _themeButtonRect.Bottom));
            menu.Show(pt);
        }

        private void ShowStyleMenu()
        {
            var menu = new ContextMenuStrip();
            void add(BeepFormStyle style, string text)
            {
                var item = new ToolStripMenuItem(text) { Checked = (CurrentStyle == style) };
                item.Click += (s, e) => { SetStyle(style); ForceCaptionRepaint(); };
                menu.Items.Add(item);
            }
            
            // System-inspired styles
            add(BeepFormStyle.Modern, "Modern");
            add(BeepFormStyle.Material, "macOS-like");
            add(BeepFormStyle.Gnome, "GNOME / Adwaita");
            add(BeepFormStyle.Kde, "KDE / Breeze");
            add(BeepFormStyle.Cinnamon, "Cinnamon");
            add(BeepFormStyle.Elementary, "Elementary");
            
            menu.Items.Add(new ToolStripSeparator());
            
            // Modern visual styles
            add(BeepFormStyle.Neon, "Neon");
            add(BeepFormStyle.Retro, "Retro");
            add(BeepFormStyle.Gaming, "Gaming");
            add(BeepFormStyle.Corporate, "Corporate");
            add(BeepFormStyle.Artistic, "Artistic");
            add(BeepFormStyle.HighContrast, "High Contrast");
            add(BeepFormStyle.Soft, "Soft");
            add(BeepFormStyle.Industrial, "Industrial");
            
            menu.Items.Add(new ToolStripSeparator());
            
            // Classic styles
            add(BeepFormStyle.Classic, "Classic");
            add(BeepFormStyle.Metro, "Metro");
            add(BeepFormStyle.Glass, "Glass");
            add(BeepFormStyle.Office, "Office");
            
            var pt = Form.PointToScreen(new Point(_styleButtonRect.Left, _styleButtonRect.Bottom));
            menu.Show(pt);
        }
        
        /// <summary>
        /// Paint caption bar in non-client area (called from WM_NCPAINT)
        /// Uses window coordinates, not client coordinates
        /// </summary>
        public void PaintNonClientCaption(Graphics g, int borderThickness)
        {
            if (!ShowCaptionBar) return;
            
            // Caption bar in window coordinates (non-client area)
            // Position INSIDE the border, not overlapping it
            // Left edge: borderThickness (inside left border)
            // Top edge: borderThickness (inside top border)
            // Width: Window width minus BOTH left and right borders
            var rect = new Rectangle(
                borderThickness, 
                borderThickness, 
                Form.Width - (borderThickness * 2), 
                CaptionHeight
            );
            
            if (rect.Width <= 0 || rect.Height <= 0) return;
            
            // Draw caption background
            Color start, end; LinearGradientMode dir;
            GetCaptionBackground(Theme, out start, out end, out dir);
            using (var brush = new LinearGradientBrush(rect, start, end, dir))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw logo/icon
            int titleStartX = rect.Left + 10;
            if (_showLogo && _logoPainter != null && _logoPainter.HasImage)
            {
                var logoRect = new Rectangle(
                    rect.Left + _logoMargin.Left,
                    rect.Top + _logoMargin.Top + (CaptionHeight - _logoSize.Height) / 2,
                    _logoSize.Width,
                    _logoSize.Height
                );
                try
                {
                    _logoPainter.DrawImage(g, logoRect);
                    titleStartX = logoRect.Right + _logoMargin.Right;
                }
                catch { }
            }

            // System buttons via renderer
            float scale = Form.DeviceDpi / 96f;
            _renderer?.Paint(g, rect, scale, Theme, Form.WindowState, out var invArea);

            // Calculate positions for extra buttons (theme/style)
            int btnSize = Math.Max(24, (int)(CaptionHeight - 8 * scale));
            var margin = (int)(8 * scale);
            _themeButtonRect = Rectangle.Empty;
            _styleButtonRect = Rectangle.Empty;

            bool leftClusterStyle = CurrentStyle == BeepFormStyle.Material;
            int y = rect.Top + (rect.Height - btnSize) / 2;

            int rendererLeftInset = 0, rendererRightInset = 0;
            _renderer?.GetTitleInsets(rect, scale, out rendererLeftInset, out rendererRightInset);

            if (leftClusterStyle)
            {
                int x = rect.Left + rendererLeftInset;
                if (ShowStyleButton) 
                { 
                    _styleButtonRect = new Rectangle(x, y, btnSize, btnSize); 
                    x += btnSize + margin; 
                }
                if (ShowThemeButton) 
                { 
                    _themeButtonRect = new Rectangle(x, y, btnSize, btnSize); 
                    x += btnSize + margin; 
                }
            }
            else
            {
                int x = rect.Right - rendererRightInset - margin;
                if (ShowThemeButton)
                {
                    x -= btnSize;
                    _themeButtonRect = new Rectangle(x, y, btnSize, btnSize);
                    x -= margin;
                }
                if (ShowStyleButton)
                {
                    x -= btnSize;
                    _styleButtonRect = new Rectangle(x, y, btnSize, btnSize);
                    x -= margin;
                }
            }

            // Paint extra buttons - inline code from PaintOverlay
            Color iconColor = Theme?.AppBarTitleForeColor ?? Color.Empty;
            if (iconColor == Color.Empty)
            {
                var styleColor = Theme?.AppBarTitleStyle?.TextColor;
                iconColor = styleColor.HasValue && styleColor.Value.A != 0 ? styleColor.Value : Form.ForeColor;
            }
            using (var p = new Pen(iconColor, 1.6f))
            {
                if (ShowThemeButton && _themeButtonRect != Rectangle.Empty)
                {
                    if (_themeHover) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.FromArgb(40, Color.Gray)); g.FillRectangle(hb, _themeButtonRect); }
                    int btnInset = (int)(6 * scale);
                    int dy = (int)(4 * scale);
                    for (int i = 0; i < 3; i++)
                    {
                        int yLine = _themeButtonRect.Top + btnInset + i * dy;
                        g.DrawLine(p, _themeButtonRect.Left + btnInset, yLine, _themeButtonRect.Right - btnInset, yLine);
                    }
                }
                if (ShowStyleButton && _styleButtonRect != Rectangle.Empty)
                {
                    if (_styleHover) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.FromArgb(40, Color.Gray)); g.FillRectangle(hb, _styleButtonRect); }
                    int btnInset = (int)(6 * scale);
                    var r = Rectangle.Inflate(_styleButtonRect, -btnInset, -btnInset);
                    int cx = r.Left + r.Width / 2;
                    int cy = r.Top + r.Height / 2;
                    g.DrawRectangle(p, r);
                    g.DrawLine(p, cx, r.Top, cx, r.Bottom);
                    g.DrawLine(p, r.Left, cy, r.Right, cy);
                }
            }

            // Paint title text - inline code from PaintOverlay
            // Note: btnSize, margin, and leftClusterStyle already calculated above
            
            int leftInset = rendererLeftInset;
            int rightInset = rendererRightInset;
            
            int extraButtonSpace = 0;
            if (ShowThemeButton) extraButtonSpace += btnSize + margin;
            if (ShowStyleButton) extraButtonSpace += btnSize + margin;
            
            if (leftClusterStyle)
            {
                leftInset += extraButtonSpace;
            }
            else
            {
                rightInset += extraButtonSpace;
            }

            int logoRight = titleStartX;
            leftInset = Math.Max(leftInset, logoRight);

            using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
            var titleFont = TitleFontOverride ?? BeepThemesManager.ToFont(Theme?.AppBarTitleStyle) ?? Form.Font;
            Color titleColor = TitleForeColorOverride ?? (Theme?.AppBarTitleForeColor ?? Color.Empty);
            if (titleColor == Color.Empty)
            {
                var styleColor = Theme?.AppBarTitleStyle?.TextColor;
                titleColor = styleColor.HasValue && styleColor.Value.A != 0 ? styleColor.Value : Form.ForeColor;
            }
            using var titleBrush = new SolidBrush(titleColor);

            Rectangle titleRect;
            if (CurrentStyle == BeepFormStyle.Gnome || CurrentStyle == BeepFormStyle.Material)
            {
                sf.Alignment = StringAlignment.Center;
                titleRect = new Rectangle(rect.Left + leftInset, rect.Top, rect.Width - leftInset - rightInset, rect.Height);
            }
            else
            {
                sf.Alignment = StringAlignment.Near;
                titleRect = new Rectangle(rect.Left + leftInset, rect.Top, rect.Width - leftInset - rightInset, rect.Height);
            }
            g.DrawString(Form.Text, titleFont, titleBrush, titleRect, sf);
        }
        
        public void PaintOverlay(Graphics g)
        {
            if (!ShowCaptionBar) return;
            
            // Caption bar painted in client area for proper mouse interaction
            // Border is painted in non-client area via WM_NCPAINT
            
            // Caption bar rectangle in client coordinates
            // Client area is already offset by any non-client border via WM_NCCALCSIZE.
            // So we draw from (0,0) with full client width.
            var rect = new Rectangle(0, 0, Form.ClientSize.Width, CaptionHeight);
            if (rect.Width <= 0 || rect.Height <= 0) return;
            
            // Draw caption background (style-aware fallbacks)
            Color start, end; LinearGradientMode dir;
            GetCaptionBackground(Theme, out start, out end, out dir);
            using (var brush = new LinearGradientBrush(rect, start, end, dir))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw logo/icon
            int titleStartX = 10; // default start
            if (_showLogo && _logoPainter != null && _logoPainter.HasImage)
            {
                var logoRect = new Rectangle(
                    _logoMargin.Left,
                    _logoMargin.Top + (CaptionHeight - _logoSize.Height) / 2,
                    _logoSize.Width,
                    _logoSize.Height
                );
                try
                {
                    _logoPainter.DrawImage(g, logoRect);
                    titleStartX = logoRect.Right + _logoMargin.Right;
                }
                catch { }
            }

            // System buttons via renderer - let it paint first
            float scale = Form.DeviceDpi / 96f;
            _renderer?.Paint(g, rect, scale, Theme, Form.WindowState, out var invArea);

            // Now calculate positions for our extra buttons (theme/style)
            // These go to the LEFT of system buttons on the right side
            // or to the RIGHT of system buttons on the left side (Mac)
            int btnSize = Math.Max(24, (int)(CaptionHeight - 8 * scale));
            var margin = (int)(8 * scale);
            _themeButtonRect = Rectangle.Empty;
            _styleButtonRect = Rectangle.Empty;

            bool leftClusterStyle = CurrentStyle == BeepFormStyle.Material; // mac-like has left system buttons
            int y = rect.Top + (rect.Height - btnSize) / 2;

            // Get the space renderer uses for system buttons
            int rendererLeftInset = 0, rendererRightInset = 0;
            _renderer?.GetTitleInsets(rect, scale, out rendererLeftInset, out rendererRightInset);

            if (leftClusterStyle)
            {
                // Mac-like: system buttons on LEFT, our extra buttons go to their RIGHT
                // Start from where renderer's buttons end
                int x = rect.Left + rendererLeftInset;
                
                if (ShowStyleButton) 
                { 
                    _styleButtonRect = new Rectangle(x, y, btnSize, btnSize); 
                    x += btnSize + margin; 
                }
                if (ShowThemeButton) 
                { 
                    _themeButtonRect = new Rectangle(x, y, btnSize, btnSize); 
                    x += btnSize + margin; 
                }
            }
            else
            {
                // Windows-like: system buttons on RIGHT, our extra buttons go to their LEFT
                // Start from where renderer's buttons begin (right side minus renderer's right inset)
                int x = rect.Right - rendererRightInset - margin;
                
                // Place buttons from right to left
                if (ShowThemeButton)
                {
                    x -= btnSize;
                    _themeButtonRect = new Rectangle(x, y, btnSize, btnSize);
                    x -= margin;
                }
                
                if (ShowStyleButton)
                {
                    x -= btnSize;
                    _styleButtonRect = new Rectangle(x, y, btnSize, btnSize);
                    x -= margin;
                }
            }

            // Paint our extra buttons AFTER renderer
            // Use AppBarTitleForeColor for icons to match title text and ensure contrast with caption bar
            Color iconColor = Theme?.AppBarTitleForeColor ?? Color.Empty;
            if (iconColor == Color.Empty)
            {
                var styleColor = Theme?.AppBarTitleStyle?.TextColor;
                iconColor = styleColor.HasValue && styleColor.Value.A != 0 ? styleColor.Value : Form.ForeColor;
            }
            using (var p = new Pen(iconColor, 1.6f))
            {
                if (ShowThemeButton && _themeButtonRect != Rectangle.Empty)
                {
                    if (_themeHover) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.FromArgb(40, Color.Gray)); g.FillRectangle(hb, _themeButtonRect); }
                    int btnInset = (int)(6 * scale);
                    int dy = (int)(4 * scale);
                    for (int i = 0; i < 3; i++)
                    {
                        int yLine = _themeButtonRect.Top + btnInset + i * dy;
                        g.DrawLine(p, _themeButtonRect.Left + btnInset, yLine, _themeButtonRect.Right - btnInset, yLine);
                    }
                }
                if (ShowStyleButton && _styleButtonRect != Rectangle.Empty)
                {
                    if (_styleHover) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.FromArgb(40, Color.Gray)); g.FillRectangle(hb, _styleButtonRect); }
                    int btnInset = (int)(6 * scale);
                    var r = Rectangle.Inflate(_styleButtonRect, -btnInset, -btnInset);
                    int cx = r.Left + r.Width / 2;
                    int cy = r.Top + r.Height / 2;
                    g.DrawRectangle(p, r);
                    g.DrawLine(p, cx, r.Top, cx, r.Bottom);
                    g.DrawLine(p, r.Left, cy, r.Right, cy);
                }
            }

            // Compute title insets - start with renderer's insets, then add extra button space
            int leftInset = rendererLeftInset;
            int rightInset = rendererRightInset;
            
            // Add space for our extra buttons
            int extraButtonSpace = 0;
            if (ShowThemeButton) extraButtonSpace += btnSize + margin;
            if (ShowStyleButton) extraButtonSpace += btnSize + margin;
            
            if (leftClusterStyle)
            {
                // Left side (Mac style): extra buttons are to the right of system buttons
                leftInset += extraButtonSpace;
            }
            else
            {
                // Right side: extra buttons are to the left of system buttons
                rightInset += extraButtonSpace;
            }

            // Respect logo width at left
            int logoRight = titleStartX; // updated from logo draw
            leftInset = Math.Max(leftInset, logoRight);

            // Title rectangle
            using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
            var titleFont = TitleFontOverride ?? BeepThemesManager.ToFont(Theme?.AppBarTitleStyle) ?? Form.Font;
            Color titleColor = TitleForeColorOverride ?? (Theme?.AppBarTitleForeColor ?? Color.Empty);
            if (titleColor == Color.Empty)
            {
                var styleColor = Theme?.AppBarTitleStyle?.TextColor;
                titleColor = styleColor.HasValue && styleColor.Value.A != 0 ? styleColor.Value : Form.ForeColor;
            }
            using var titleBrush = new SolidBrush(titleColor);

            Rectangle titleRect;
            if (CurrentStyle == BeepFormStyle.Gnome || CurrentStyle == BeepFormStyle.Material)
            {
                sf.Alignment = StringAlignment.Center;
                titleRect = new Rectangle(rect.Left + leftInset, rect.Top, rect.Width - leftInset - rightInset, rect.Height);
            }
            else
            {
                sf.Alignment = StringAlignment.Near;
                titleRect = new Rectangle(rect.Left + leftInset, rect.Top, rect.Width - leftInset - rightInset, rect.Height);
            }
            g.DrawString(Form.Text, titleFont, titleBrush, titleRect, sf);
        }

        // Expose caption background so host can paint adjacent NC areas consistently
        public void GetCaptionBackgroundForHost(out Color start, out Color end, out LinearGradientMode dir)
        {
            GetCaptionBackground(Theme, out start, out end, out dir);
        }

        private void GetCaptionBackground(IBeepTheme theme, out Color start, out Color end, out LinearGradientMode dir)
        {
            // If theme provides explicit AppBar gradient tokens, use them
            if (EnableCaptionGradient && theme != null && (theme.AppBarGradiantStartColor != Color.Empty || theme.AppBarGradiantEndColor != Color.Empty))
            {
                start = theme.AppBarGradiantStartColor != Color.Empty ? theme.AppBarGradiantStartColor : (theme.AppBarBackColor != Color.Empty ? theme.AppBarBackColor : SystemColors.ControlDark);
                end   = theme.AppBarGradiantEndColor   != Color.Empty ? theme.AppBarGradiantEndColor   : (theme.AppBarBackColor != Color.Empty ? ControlPaint.Dark(theme.AppBarBackColor, .05f) : SystemColors.ControlDark);
                dir   = theme.AppBarGradiantDirection;
                return;
            }

            // ProgressBarStyle-specific fallbacks to give strong identity even without theme tokens
            dir = LinearGradientMode.Vertical;
            switch (CurrentStyle)
            {
                case BeepFormStyle.Material: // flat light/dark based on theme
                    start = end = theme?.BackColor != Color.Empty ? theme.BackColor : Color.FromArgb(240, 240, 240);
                    break;
                case BeepFormStyle.Gnome:
                    start = end = theme?.AppBarBackColor != Color.Empty ? theme.AppBarBackColor : Color.FromArgb(238, 238, 238);
                    break;
                case BeepFormStyle.Kde:
                    start = Color.FromArgb(246, 248, 251);
                    end   = Color.FromArgb(232, 238, 247);
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Cinnamon:
                    start = Color.FromArgb(243, 246, 250);
                    end   = Color.FromArgb(225, 230, 240);
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Elementary:
                    start = end = Color.FromArgb(245, 245, 245);
                    break;
                case BeepFormStyle.Neon:
                    start = Color.FromArgb(20, 20, 24);
                    end   = Color.FromArgb(10, 10, 12);
                    dir   = LinearGradientMode.Horizontal;
                    break;
                case BeepFormStyle.Retro:
                    start = Color.FromArgb(50, 30, 60);
                    end   = Color.FromArgb(30, 15, 40);
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Gaming:
                    start = Color.FromArgb(18, 20, 26);
                    end   = Color.FromArgb(10, 12, 14);
                    dir   = LinearGradientMode.Horizontal;
                    break;
                case BeepFormStyle.Corporate: // Office-like light
                    start = Color.FromArgb(252, 252, 252);
                    end   = Color.FromArgb(236, 236, 236);
                    break;
                case BeepFormStyle.HighContrast:
                    start = end = Color.Black;
                    break;
                case BeepFormStyle.Soft:
                    start = Color.FromArgb(235, 245, 255);
                    end   = Color.FromArgb(220, 235, 255);
                    break;
                case BeepFormStyle.Industrial:
                    start = Color.FromArgb(68, 70, 80);
                    end   = Color.FromArgb(54, 56, 64);
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Modern: // VS Blue-like
                    start = Color.FromArgb(60, 125, 217);  // #3C7DD9
                    end   = Color.FromArgb(43, 91, 168);   // #2B5BA8
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.ModernDark: // VS Dark-like
                    start = Color.FromArgb(45, 45, 48);    // #2D2D30
                    end   = Color.FromArgb(30, 30, 30);    // #1E1E1E
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Office: // Office 2019 Blue-like
                    start = Color.FromArgb(0, 120, 215);   // #0078D7
                    end   = Color.FromArgb(0, 90, 158);    // #005A9E
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Metro: // flat metro
                    start = end = Color.FromArgb(0, 122, 204);
                    break;
                case BeepFormStyle.Fluent: // very light with subtle gradient
                    start = Color.FromArgb(248, 250, 255);
                    end   = Color.FromArgb(238, 244, 255);
                    dir   = LinearGradientMode.Vertical;
                    break;
                case BeepFormStyle.Glass: // glass-like light gray
                    start = Color.FromArgb(245, 248, 255);
                    end   = Color.FromArgb(230, 236, 250);
                    break;
                case BeepFormStyle.Classic:
                    start = SystemColors.Control;
                    end   = ControlPaint.Dark(SystemColors.Control, .05f);
                    break;
                default:
                    start = theme?.AppBarBackColor != Color.Empty ? theme.AppBarBackColor : SystemColors.ControlDark;
                    end   = ControlPaint.Dark(start, .05f);
                    break;
            }
        }
    }
}
