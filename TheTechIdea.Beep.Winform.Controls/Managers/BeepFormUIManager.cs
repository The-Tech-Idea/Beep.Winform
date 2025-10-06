using System;
using System.Collections.Generic;
using System.ComponentModel;
 
 
using TheTechIdea.Beep.Winform.Controls.AppBars;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    [DesignerCategory("Component")]
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Manager for BeepiForm and related Beep controls (theme, chrome & wiring).")]
    [DisplayName("Beep Form UI Manager")]
    public class BeepFormUIManager : Component, IBeepFormUIManager
    {
        // ---------------------------- Fields
        private Form _form;               // host form (runtime or design root)
        private BeepiForm _beepiForm;     // strongly-typed reference (if host inherits BeepiForm)
        private BeepSideMenu _beepSideMenu;
        private BeepAppBar _beepAppBar;
        private BeepDisplayContainer _displayContainer;
        private BeepMenuBar _beepMenuBar;
        private BeepFunctionsPanel _functionsPanel;

        private readonly HashSet<Control> _wiredContainers = new();
        private bool _themeSubscribed;
        private ISynchronizeInvoke _invoker;

        private readonly BeepImage _logoHelper = new();
        private bool _applyThemeOnImage;

        // ---------------------------- Ctor
        public BeepFormUIManager() { }
        public BeepFormUIManager(IContainer container)
        {
            container?.Add(this);
        }

        // ---------------------------- Design/runtime detection
        private bool IsInDesignMode => (Site?.DesignMode == true) || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        // ---------------------------- Events
        public event Action<string> OnThemeChanged;

        // ---------------------------- Appearance / Options
        private string _title = "Beep Form";
        [Browsable(true), Category("Appearance"), Description("Window title propagated to BeepiForm / AppBar / SideMenu.")]
        public string Title
        {
            get => _title;
            set { _title = value; TryInvoke(() => { if (_beepiForm != null) _beepiForm.Text = value; if (_beepAppBar != null) _beepAppBar.Title = value; if (_beepSideMenu != null) _beepSideMenu.Title = value; }); }
        }

        private string _logoImage = string.Empty;
        [Browsable(true), Category("Appearance"), Description("Logo image path (also sets BeepiForm icon from SVG when possible)."),
         Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LogoImage
        {
            get => _logoImage;
            set
            {
                _logoImage = value ?? string.Empty;
                TryInvoke(() =>
                {
                    _logoHelper.ImagePath = _logoImage; // this populates svgDocument if SVG
                    if (_beepAppBar != null) _beepAppBar.LogoImage = _logoImage;
                    if (_beepSideMenu != null) _beepSideMenu.LogoImage = _logoImage;
                    if (_beepiForm != null && _logoHelper.svgDocument != null)
                        _beepiForm.Icon = ImageConverters.ConvertSvgToIcon(_logoHelper.svgDocument, 64);
                });
            }
        }

        private bool _showBorder = true;
        [Browsable(true), Category("Beep Chrome"), Description("Apply theme-controlled borders to children.")]
        public bool ShowBorder { get => _showBorder; set { _showBorder = value; TryInvoke(ApplyBorder); } }

        private bool _showShadow;
        [Browsable(true), Category("Beep Chrome"), Description("Apply theme-controlled shadows to children.")]
        public bool ShowShadow { get => _showShadow; set { _showShadow = value; TryInvoke(ApplyShadow); } }

        private bool _isRounded;
        [Browsable(true), Category("Beep Chrome"), Description("Apply rounded corners to children.")]
        public bool IsRounded { get => _isRounded; set { _isRounded = value; TryInvoke(ApplyRounded); } }

        [Browsable(true), Category("Appearance"), Description("Tint/adjust images according to theme when supported.")]
        public bool ApplyThemeOnImage { get => _applyThemeOnImage; set { _applyThemeOnImage = value; TryInvoke(ApplyImage); } }

        private string _theme = "DefaultType";
        [Browsable(true), Category("Theme"), Description("Current theme name. Setting updates form and Beep controls.")]
        public string Theme
        {
            get => _theme;
            set
            {
                if (string.Equals(_theme, value, StringComparison.Ordinal)) return;
                _theme = value ?? "DefaultType";
                OnThemeChanged?.Invoke(_theme);
                TryInvoke(() => { ApplyThemeToForm(); ApplyThemeToAll(_form); });
            }
        }

        [Browsable(true), Category("Wiring"), DefaultValue(true), Description("Find and bind Beep components on the host form automatically.")]
        public bool AutoWireComponents { get; set; } = true;

        [Browsable(true), Category("Wiring"), DefaultValue(true), Description("Mirror common caption/backdrop/border options into BeepiForm (if present).")]
        public bool PropagateCaptionSettings { get; set; } = true;

        // ---- BeepiForm-forwarded options (only applied when BeepiForm is set)
        [Browsable(true), Category("BeepiForm")]
        public bool? ShowSystemButtons { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public bool? ShowCaptionBar { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public int? CaptionHeight { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public Padding? CaptionPadding { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public bool? ShowIconInCaption { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public bool? EnableCaptionGradient { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public int? BorderRadius { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public int? BorderThickness { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public bool? UseImmersiveDarkMode { get; set; }
        [Browsable(true), Category("BeepiForm")]
        public BackdropType? Backdrop { get; set; }

        // ---------------------------- Public API
        public void Initialize(Form runtimeForm)
        {
            _form = runtimeForm ?? throw new ArgumentNullException(nameof(runtimeForm));
            _invoker = _form as ISynchronizeInvoke;
            _beepiForm = _form as BeepiForm;

            if (!_themeSubscribed)
            {
                BeepThemesManager.ThemeChanged -= BeepThemesManager_ThemeChanged;
                BeepThemesManager.ThemeChanged += BeepThemesManager_ThemeChanged;
                _themeSubscribed = true;
            }

            if (AutoWireComponents)
                DiscoverOn(_form);

            ApplyThemeToForm();
            ApplyThemeToAll(_form);
            SyncToBeepiForm();

            AttachContainer(_form);
        }

        public void AttachTo(BeepiForm form)
        {
            Initialize(form);
        }

        // ---------------------------- Wiring helpers
        private void DiscoverOn(Control root)
        {
            if (root == null) return;
            foreach (var c in EnumerateAllChildren(root))
            {
                if (_beepSideMenu == null && c is BeepSideMenu sm) _beepSideMenu = sm;
                else if (_beepAppBar == null && c is BeepAppBar ab) _beepAppBar = ab;
                else if (_displayContainer == null && c is BeepDisplayContainer dc) _displayContainer = dc;
                else if (_beepMenuBar == null && c is BeepMenuBar mb) _beepMenuBar = mb;
                else if (_functionsPanel == null && c is BeepFunctionsPanel fp) _functionsPanel = fp;
            }

            // Push title/logo immediately
            if (_beepSideMenu != null) { _beepSideMenu.Title = Title; _beepSideMenu.LogoImage = LogoImage; WireSideMenuCollapse(); }
            if (_beepAppBar != null) { _beepAppBar.Title = Title; _beepAppBar.LogoImage = LogoImage; }
        }

        private static IEnumerable<Control> EnumerateAllChildren(Control root)
        {
            var stack = new Stack<Control>();
            if (root != null) stack.Push(root);
            while (stack.Count > 0)
            {
                var c = stack.Pop();
                foreach (Control child in c.Controls)
                {
                    yield return child;
                    if (child.HasChildren) stack.Push(child);
                }
            }
        }

        private void AttachContainer(Control container)
        {
            if (container == null || _wiredContainers.Contains(container)) return;
            container.ControlAdded += OnControlAdded;
            _wiredContainers.Add(container);
            foreach (Control child in container.Controls)
                if (child is ContainerControl) AttachContainer(child);
        }
        private void DetachContainer(Control container)
        {
            if (container == null || !_wiredContainers.Contains(container)) return;
            container.ControlAdded -= OnControlAdded;
            _wiredContainers.Remove(container);
            foreach (Control child in container.Controls)
                if (child is ContainerControl) DetachContainer(child);
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            ApplyThemeToControl(e.Control);
            if (e.Control is ContainerControl cc) AttachContainer(cc);

            if (_beepSideMenu == null && e.Control is BeepSideMenu sm) { _beepSideMenu = sm; _beepSideMenu.Title = Title; _beepSideMenu.LogoImage = LogoImage; WireSideMenuCollapse(); }
            if (_beepAppBar == null && e.Control is BeepAppBar ab) { _beepAppBar = ab; _beepAppBar.Title = Title; _beepAppBar.LogoImage = LogoImage; }
            if (_displayContainer == null && e.Control is BeepDisplayContainer dc) _displayContainer = dc;
            if (_beepMenuBar == null && e.Control is BeepMenuBar mb) _beepMenuBar = mb;
            if (_functionsPanel == null && e.Control is BeepFunctionsPanel fp) _functionsPanel = fp;
        }

        private void WireSideMenuCollapse()
        {
            if (_beepSideMenu == null) return;
            _beepSideMenu.EndMenuCollapseExpand -= BeepSideMenu_EndMenuCollapseExpand;
            _beepSideMenu.EndMenuCollapseExpand += BeepSideMenu_EndMenuCollapseExpand;
            _beepSideMenu.StartOnMenuCollapseExpand -= BeepSideMenu_StartOnMenuCollapseExpand;
            _beepSideMenu.StartOnMenuCollapseExpand += BeepSideMenu_StartOnMenuCollapseExpand;
        }

        private void BeepSideMenu_StartOnMenuCollapseExpand(bool _)
        {
            _beepAppBar?.SuspendFormLayout();
            _displayContainer?.SuspendFormLayout();
        }
        private void BeepSideMenu_EndMenuCollapseExpand(bool showTitle)
        {
            if (_beepAppBar != null)
            {
                _beepAppBar.ShowTitle = showTitle;
                _beepAppBar.ShowLogo = false;
                _beepAppBar.ResumeFormLayout();
            }
            _displayContainer?.ResumeFormLayout();
        }

        // ---------------------------- Theme application
        private void BeepThemesManager_ThemeChanged(object sender, EventArgs e)
        {
            TryInvoke(() => { Theme = BeepThemesManager.CurrentThemeName; });
        }

        public void ApplyThemeToForm()
        {
            if (_form == null) return;
            Theme = BeepThemesManager.CurrentThemeName; // updates children via setter
            _beepiForm?.ApplyTheme();
        }

        private void ApplyThemeToAll(Control container)
        {
            if (container == null) return;
            var theme = BeepThemesManager.GetTheme(_theme);
            if (container == _form && theme != null && theme.BackgroundColor != Color.Empty && theme.BackgroundColor != Color.Transparent)
                container.BackColor = theme.BackgroundColor;

            ApplyThemeToControl(container);
            foreach (Control child in container.Controls)
                ApplyThemeToAll(child);
        }

        private void ApplyThemeToControl(Control control)
        {
            if (control == null) return;

            // Set Theme property when available
            var prop = TypeDescriptor.GetProperties(control)["Theme"];
            if (prop != null && prop.PropertyType == typeof(string))
                prop.SetValue(control, _theme);

            // Apply chrome flags according to control capabilities
            if (GetBool(control, "IsBorderAffectedByTheme")) ThemeFunctions.ApplyBorderToControl(control, _showBorder);
            if (GetBool(control, "IsShadowAffectedByTheme")) ThemeFunctions.ApplyShadowToControl(control, _showShadow);
            if (GetBool(control, "IsRoundedAffectedByTheme")) ThemeFunctions.ApplyRoundedToControl(control, _isRounded);
            ThemeFunctions.ApplyThemeOnImageControl(control, _applyThemeOnImage);
        }

        private static bool GetBool(Control c, string name)
        {
            var p = TypeDescriptor.GetProperties(c)[name];
            return p != null && p.PropertyType == typeof(bool) && (bool)p.GetValue(c);
        }

        // ---------------------------- Chrome batches
        private void ApplyBorder()
        {
            if (_form == null) return;
            foreach (Control child in _form.Controls)
                ThemeFunctions.ApplyBorderToControl(child, _showBorder);
        }
        private void ApplyShadow()
        {
            if (_form == null) return;
            foreach (Control child in _form.Controls)
                ThemeFunctions.ApplyShadowToControl(child, _showShadow);
        }
        private void ApplyRounded()
        {
            if (_form == null) return;
            foreach (Control child in _form.Controls)
                ThemeFunctions.ApplyRoundedToControl(child, _isRounded);
        }
        private void ApplyImage()
        {
            if (_form == null) return;
            foreach (Control child in _form.Controls)
                ThemeFunctions.ApplyThemeOnImageControl(child, _applyThemeOnImage);
        }

        // ---------------------------- Interface implementation methods
        public void ApplyThemeToControl(Control control, string theme, bool applyToImage)
        {
            if (control == null) return;

            // Set Theme property when available
            var prop = TypeDescriptor.GetProperties(control)["Theme"];
            if (prop != null && prop.PropertyType == typeof(string))
                prop.SetValue(control, theme);

            // Apply chrome flags according to control capabilities
            if (GetBool(control, "IsBorderAffectedByTheme")) ThemeFunctions.ApplyBorderToControl(control, _showBorder);
            if (GetBool(control, "IsShadowAffectedByTheme")) ThemeFunctions.ApplyShadowToControl(control, _showShadow);
            if (GetBool(control, "IsRoundedAffectedByTheme")) ThemeFunctions.ApplyRoundedToControl(control, _isRounded);
            ThemeFunctions.ApplyThemeOnImageControl(control, applyToImage);
        }

        public void FindBeepSideMenu()
        {
            if (_form == null) return;

            foreach (Control control in _form.Controls)
            {
                if (control is BeepSideMenu sideMenu)
                {
                    _beepSideMenu = sideMenu;
                    if (_beepiForm != null)
                        _beepSideMenu.BeepForm = _beepiForm;

                    _beepSideMenu.Title = Title;
                    _beepSideMenu.LogoImage = LogoImage;
                    WireSideMenuCollapse();
                }
                else if (control is BeepFunctionsPanel fnPanel)
                {
                    _functionsPanel = fnPanel;
                }
                else if (control is BeepiForm beepi)
                {
                    BeepiForm = beepi;

                    if (_beepAppBar != null)
                    {
                        _beepAppBar.LogoImage = LogoImage;
                        _beepAppBar.Title = Title;
                    }
                }
                else if (control is BeepAppBar appBar)
                {
                    _beepAppBar = appBar;
                    _beepAppBar.Title = Title;
                }
            }
        }

        public void ShowTitle(bool show)
        {
            if (_beepAppBar != null)
            {
                _beepAppBar.ShowTitle = show;
            }
        }

        // ---------------------------- Sync manager → BeepiForm
        public void SyncToBeepiForm()
        {
            if (!PropagateCaptionSettings || _beepiForm == null) return;

            void Set<T>(Action<T> setter, T? value) where T : struct { if (value.HasValue) setter(value.Value); }

            Set<bool>(v => _beepiForm.ShowSystemButtons = v, ShowSystemButtons);
            Set<bool>(v => _beepiForm.ShowCaptionBar = v, ShowCaptionBar);
            Set<int>(v => _beepiForm.CaptionHeight = v, CaptionHeight);
            if (CaptionPadding.HasValue) _beepiForm.CaptionPadding = CaptionPadding.Value;
            Set<bool>(v => _beepiForm.ShowIconInCaption = v, ShowIconInCaption);
            Set<bool>(v => _beepiForm.EnableCaptionGradient = v, EnableCaptionGradient);
            Set<int>(v => _beepiForm.BorderRadius = v, BorderRadius);
            Set<int>(v => _beepiForm.BorderThickness = v, BorderThickness);
            Set<bool>(v => _beepiForm.UseImmersiveDarkMode = v, UseImmersiveDarkMode);
            if (Backdrop.HasValue) _beepiForm.Backdrop = Backdrop.Value;
        }

        // ---------------------------- Attach / Detach from a BeepiForm
        [Browsable(false)]
        public BeepiForm BeepiForm
        {
            get => _beepiForm;
            set
            {
                _beepiForm = value;
                _form = value ?? _form;
                _invoker = (_form as ISynchronizeInvoke) ?? _invoker;
                if (value != null) { SyncToBeepiForm(); }
            }
        }

        // Public accessors for components (needed by designer and external code)
        [Browsable(true), Category("Components")]
        public BeepAppBar BeepAppBar
        {
            get => _beepAppBar;
            set
            {
                if (value != null)
                {
                    _beepAppBar = value;
                    if (_beepSideMenu != null)
                    {
                        _beepSideMenu.BeepForm = BeepiForm;
                        _beepSideMenu.BeepAppBar = _beepAppBar;
                    }
                    _beepAppBar.Title = Title;
                    _beepAppBar.LogoImage = LogoImage;
                }
            }
        }

        [Browsable(true), Category("Components")]
        public BeepSideMenu BeepSideMenu
        {
            get => _beepSideMenu;
            set
            {
                _beepSideMenu = value;
                if (BeepiForm != null)
                {
                    _beepSideMenu.BeepForm = BeepiForm;
                }
                if (_beepSideMenu != null)
                {
                    _beepSideMenu.Title = Title;
                    _beepSideMenu.LogoImage = LogoImage;
                    WireSideMenuCollapse();
                }
            }
        }

        [Browsable(true), Category("Components")]
        public BeepMenuBar BeepMenuBar
        {
            get => _beepMenuBar;
            set
            {
                _beepMenuBar = value;
            }
        }

        [Browsable(true), Category("Components")]
        public BeepDisplayContainer DisplayContainer
        {
            get => _displayContainer;
            set
            {
                _displayContainer = value;
            }
        }

        [Browsable(true), Category("Components")]
        public BeepFunctionsPanel BeepFunctionsPanel
        {
            get => _functionsPanel;
            set => _functionsPanel = value;
        }

        // ---------------------------- Convenience API
        public void AttachControlAddedHandlers() { if (_form != null) AttachContainer(_form); }
        public void DetachControlAddedHandlers() { if (_form != null) DetachContainer(_form); }

        // ---------------------------- Apply/Remove generated BeepForm style
        private bool _applyBeepFormStyle;
        [Browsable(true), Category("BeepiForm"), Description("Apply generated BeepForm chrome to host form.")]
        public bool ApplyBeepFormStyle
        {
            get => _applyBeepFormStyle;
            set
            {
                _applyBeepFormStyle = value;
                if (_form == null) return;
                if (value) BeepFormGenerator.ApplyBeepForm(_form, BeepThemesManager.GetTheme(_theme));
                else BeepFormGenerator.RemoveBeepForm(_form);
            }
        }

        // ---------------------------- Thread marshalling helper
        private void TryInvoke(Action action)
        {
            if (action == null) return;
            if (_invoker != null && _invoker.InvokeRequired)
            {
                try { _invoker.BeginInvoke(action, Array.Empty<object>()); } catch { /* ignore */ }
            }
            else action();
        }

        // ---------------------------- Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { if (_themeSubscribed) { BeepThemesManager.ThemeChanged -= BeepThemesManager_ThemeChanged; _themeSubscribed = false; } } catch { }
                try { if (_form != null) DetachContainer(_form); } catch { }
            }
            base.Dispose(disposing);
        }
    }
}