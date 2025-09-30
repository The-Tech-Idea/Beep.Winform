using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
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

        public bool ShowStyleButton { get; set; } = false; // switches caption renderer (Windows/Mac/GNOME/KDE)
        public string StyleButtonIconPath { get; set; } = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.024-dashboard.svg";
        private Rectangle _styleButtonRect;
        private bool _styleHover;

        // Caption title overrides
        public Font? TitleFontOverride { get; set; }
        public Color? TitleForeColorOverride { get; set; }
        
        // Renderer strategy
        private ICaptionRenderer _renderer;
        public CaptionRendererKind RendererKind { get; private set; } = CaptionRendererKind.Windows;

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
            _registerPaddingProvider((ref Padding p) => { if (ShowCaptionBar) p.Top += CaptionHeight; });

            SetRenderer(CaptionRendererKind.Windows);
        }

        private void SetRenderer(CaptionRendererKind kind)
        {
            RendererKind = kind;
            _renderer = kind switch
            {
                CaptionRendererKind.MacLike => new MacLikeCaptionRenderer(),
                CaptionRendererKind.Gnome => new GnomeCaptionRenderer(),
                CaptionRendererKind.Kde => new KdeCaptionRenderer(),
                CaptionRendererKind.Cinnamon => new CinnamonCaptionRenderer(),
                CaptionRendererKind.Elementary => new ElementaryCaptionRenderer(),
                _ => new WindowsCaptionRenderer()
            };
            _renderer.UpdateHost(Form, () => Theme, () => CaptionHeight);
            _renderer.SetShowSystemButtons(ShowSystemButtons);
            ApplyRendererVisualDefaults(kind);
            TryMapFormStyleAndPreset(kind);
        }

        private void TryMapFormStyleAndPreset(CaptionRendererKind kind)
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

                // Map FormStyle
                var formStyleProp = formType.GetProperty("FormStyle");
                if (formStyleProp != null)
                {
                    var enumType = formStyleProp.PropertyType;
                    string targetStyleName = kind switch
                    {
                        CaptionRendererKind.MacLike => "Material",
                        CaptionRendererKind.Gnome => "Gnome",
                        CaptionRendererKind.Kde => "Kde",
                        CaptionRendererKind.Cinnamon => "Cinnamon",
                        CaptionRendererKind.Elementary => "Elementary",
                        _ => "Modern"
                    };
                    foreach (var v in Enum.GetValues(enumType))
                    {
                        if (string.Equals(v.ToString(), targetStyleName, StringComparison.OrdinalIgnoreCase))
                        {
                            formStyleProp.SetValue(form, v);
                            break;
                        }
                    }
                }

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

                // ApplyPreset based on renderer
                var applyPreset = formType.GetMethod("ApplyPreset", new[] { typeof(string) });
                if (applyPreset != null)
                {
                    string baseKey = kind switch
                    {
                        CaptionRendererKind.MacLike => "macos",
                        CaptionRendererKind.Gnome => "gnome.adwaita",
                        CaptionRendererKind.Kde => "kde.breeze",
                        CaptionRendererKind.Cinnamon => "cinnamon.mint",
                        CaptionRendererKind.Elementary => "elementary",
                        _ => string.Empty
                    };
                    if (!string.IsNullOrEmpty(baseKey))
                    {
                        string key = baseKey + suffix;
                        try { applyPreset.Invoke(form, new object[] { key }); } catch { }
                      }
                }
            }
            catch { }
        }

        private void ApplyRendererVisualDefaults(CaptionRendererKind kind)
        {
            switch (kind)
            {
                case CaptionRendererKind.Gnome:
                    EnableCaptionGradient = false; if (CaptionHeight < 34) CaptionHeight = 34; break;
                case CaptionRendererKind.Kde:
                    EnableCaptionGradient = true; if (CaptionHeight < 36) CaptionHeight = 36; break;
                case CaptionRendererKind.Cinnamon:
                    EnableCaptionGradient = true; if (CaptionHeight < 38) CaptionHeight = 38; break;
                case CaptionRendererKind.Elementary:
                    EnableCaptionGradient = false; if (CaptionHeight < 40) CaptionHeight = 40; break;
                case CaptionRendererKind.MacLike:
                    EnableCaptionGradient = false; if (CaptionHeight < 36) CaptionHeight = 36; break;
                case CaptionRendererKind.Windows:
                default:
                    break;
            }
        }

        public void SwitchRenderer(CaptionRendererKind kind)
        {
            if (RendererKind == kind) return;
            SetRenderer(kind);
            Form.Invalidate();
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
                    Form.Invalidate(inv);
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
                _renderer.OnMouseLeave(out inv);
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
                ShowRendererMenu();
                Form.Invalidate(_styleButtonRect);
                return;
            }
            if (ShowSystemButtons && _renderer != null)
            {
                if (_renderer.OnMouseDown(e.Location, Form, out var inv))
                {
                    if (!inv.IsEmpty) Form.Invalidate(inv);
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
                            // Set the host's Theme property, which triggers ApplyTheme()
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

        private void ShowRendererMenu()
        {
            var menu = new ContextMenuStrip();
            void add(CaptionRendererKind k, string text)
            {
                var item = new ToolStripMenuItem(text) { Checked = (RendererKind == k) };
                item.Click += (s, e) => SwitchRenderer(k);
                menu.Items.Add(item);
            }
            add(CaptionRendererKind.Windows, "Windows");
            add(CaptionRendererKind.MacLike, "macOS-like");
            add(CaptionRendererKind.Gnome,   "GNOME / Adwaita");
            add(CaptionRendererKind.Kde,     "KDE / Breeze");
            add(CaptionRendererKind.Cinnamon,"Cinnamon");
            add(CaptionRendererKind.Elementary, "Elementary");
            var pt = Form.PointToScreen(new Point(_styleButtonRect.Left, _styleButtonRect.Bottom));
            menu.Show(pt);
        }
        
        private void LayoutButtons()
        {
            // no-op here; renderers compute their own layout during Paint
        }
        
        private void PaintOverlay(Graphics g)
        {
            if (!ShowCaptionBar) return;
            var rect = new Rectangle(0, 0, Form.ClientSize.Width, CaptionHeight);
            if (rect.Width <= 0 || rect.Height <= 0) return;
            
            // Draw caption background
            if (EnableCaptionGradient && Theme != null)
            {
                // Prefer theme-provided gradient tokens
                var start = Theme.AppBarGradiantStartColor != Color.Empty ? Theme.AppBarGradiantStartColor : (Theme.AppBarBackColor != Color.Empty ? Theme.AppBarBackColor : SystemColors.ControlDark);
                var end   = Theme.AppBarGradiantEndColor   != Color.Empty ? Theme.AppBarGradiantEndColor   : (Theme.AppBarBackColor != Color.Empty ? ControlPaint.Dark(Theme.AppBarBackColor, .05f) : SystemColors.ControlDark);
                var dir   = Theme.AppBarGradiantDirection;
                using var brush = new LinearGradientBrush(rect, start, end, dir);
                g.FillRectangle(brush, rect);
            }
            else
            {
                using var b = new SolidBrush(Theme?.AppBarBackColor ?? SystemColors.ControlDark);
                g.FillRectangle(b, rect);
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

            // System buttons via renderer
            float scale = Form.DeviceDpi / 96f;
            _renderer?.Paint(g, rect, scale, Theme, Form.WindowState, out var invArea);

            // Layout our extra buttons (to the left of system buttons cluster)
            int btnSize = Math.Max(24, (int)(CaptionHeight - 8 * scale));
            var margin = (int)(8 * scale);
            _themeButtonRect = Rectangle.Empty;
            _styleButtonRect = Rectangle.Empty;

            int sysClusterWidth = ShowSystemButtons ? (btnSize * 3 + margin * 3) : 0;
            int right = rect.Right - margin - sysClusterWidth;
            int y = rect.Top + (rect.Height - btnSize) / 2;

            if (ShowThemeButton)
            {
                _themeButtonRect = new Rectangle(right - btnSize, y, btnSize, btnSize);
                right = _themeButtonRect.Left - margin;
            }
            if (ShowStyleButton)
            {
                _styleButtonRect = new Rectangle(right - btnSize, y, btnSize, btnSize);
                right = _styleButtonRect.Left - margin;
            }

            // Paint theme button glyph
            using (var p = new Pen(Theme?.AppBarButtonForeColor ?? Form.ForeColor, 1.6f))
            {
                if (ShowThemeButton)
                {
                    if (_themeHover) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.FromArgb(40, Color.Gray)); g.FillRectangle(hb, _themeButtonRect); }
                    int inset = (int)(6 * scale);
                    int dy = (int)(4 * scale);
                    for (int i = 0; i < 3; i++)
                    {
                        int yLine = _themeButtonRect.Top + inset + i * dy;
                        g.DrawLine(p, _themeButtonRect.Left + inset, yLine, _themeButtonRect.Right - inset, yLine);
                    }
                }
                if (ShowStyleButton)
                {
                    if (_styleHover) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.FromArgb(40, Color.Gray)); g.FillRectangle(hb, _styleButtonRect); }
                    int inset = (int)(6 * scale);
                    var r = Rectangle.Inflate(_styleButtonRect, -inset, -inset);
                    int cx = r.Left + r.Width / 2;
                    int cy = r.Top + r.Height / 2;
                    g.DrawRectangle(p, r);
                    g.DrawLine(p, cx, r.Top, cx, r.Bottom);
                    g.DrawLine(p, r.Left, cy, r.Right, cy);
                }
            }

            // Compute title insets from renderer to avoid overlap
            int leftInset = 0, rightInset = 0;
            _renderer?.GetTitleInsets(rect, scale, out leftInset, out rightInset);
            // Reserve space for our buttons on the right
            int extraRight = 0;
            if (ShowThemeButton) extraRight += _themeButtonRect.Width + margin;
            if (ShowStyleButton) extraRight += _styleButtonRect.Width + margin;
            rightInset += extraRight;
            titleStartX = Math.Max(titleStartX, rect.Left + leftInset);

            // Draw form title
            using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
            // Title font and color from overrides/theme if available
            var titleFont = TitleFontOverride ?? BeepThemesManager.ToFont(Theme?.AppBarTitleStyle) ?? Form.Font;
            Color titleColor = TitleForeColorOverride ?? (Theme?.AppBarTitleForeColor ?? Color.Empty);
            if (titleColor == Color.Empty)
            {
                var styleColor = Theme?.AppBarTitleStyle?.TextColor;
                if (styleColor.HasValue && styleColor.Value.A != 0)
                    titleColor = styleColor.Value;
                else
                    titleColor = Form.ForeColor;
            }
            using var titleBrush = new SolidBrush(titleColor);

            Rectangle titleRect;
            if (RendererKind == CaptionRendererKind.Gnome || RendererKind == CaptionRendererKind.MacLike)
            {
                // Center title within available caption width (common for GNOME and macOS-like skins)
                sf.Alignment = StringAlignment.Center;
                titleRect = new Rectangle(rect.Left + leftInset, 0, rect.Width - leftInset - rightInset, rect.Height);
            }
            else
            {
                // Left-align title after logo/insets
                sf.Alignment = StringAlignment.Near;
                titleRect = new Rectangle(titleStartX, 0, rect.Width - titleStartX - rightInset, rect.Height);
            }
            g.DrawString(Form.Text, titleFont, titleBrush, titleRect, sf);
        }

        public void Dispose()
        {
            _logoPainter?.Dispose();
        }
    }
}
