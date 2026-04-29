using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal class ComboBoxPopupHostForm : IComboBoxPopupHost
    {
        private BeepPopupForm _form;
        private IPopupContentPanel _contentPanel;
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.OutlineDefault();
        private bool _closeRaised;
        
        public bool IsVisible => _form != null && _form.Visible;

        public event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;
        public event EventHandler<ComboBoxPopupClosedEventArgs> PopupClosed;
        public event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;
        public event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;

        public void ShowPopup(Control owner, ComboBoxPopupModel model, Rectangle triggerBounds)
        {
            if (_form != null) ClosePopup(false);

            _closeRaised = false;
            _profile = CreateProfile(owner, model) ?? ComboBoxPopupHostProfile.OutlineDefault();
            ComboBoxPopupModel effectiveModel = NormalizeModel(model, _profile);
            ComboBoxThemeTokens themeTokens = ResolveThemeTokens(owner, _profile, effectiveModel);

            _form = new BeepPopupForm { AutoClose = true, ShowCaptionBar = false, FormStyle = _profile.FormStyle };
            _form.FormClosed += OnFormClosed;

            // Each variant creates its own content panel with distinct layout and behavior.
            _contentPanel = CreateContentPanel(_profile, themeTokens);
            if (_contentPanel is ComboBoxListBoxPopupContent listBoxContent && owner is BeepComboBox comboBoxOwner)
            {
                ListBoxType mappedType = ComboBoxListBoxTypeMapper.Map(comboBoxOwner.ComboBoxType);
                listBoxContent.SetListBoxType(mappedType);
            }
            var contentControl = (Control)_contentPanel;
            contentControl.Dock = DockStyle.Fill;
            if (owner is BeepComboBox rtlOwner && rtlOwner.IsRtl)
            {
                contentControl.RightToLeft = RightToLeft.Yes;
                _form.RightToLeft = RightToLeft.Yes;
            }
            else
            {
                contentControl.RightToLeft = RightToLeft.No;
                _form.RightToLeft = RightToLeft.No;
            }

            _contentPanel.RowCommitted += (s, e) => RowCommitted?.Invoke(this, e);
            _contentPanel.SearchTextChanged += (s, e) => SearchTextChanged?.Invoke(this, e);
            _contentPanel.KeyboardFocusChanged += (s, e) => KeyboardFocusChanged?.Invoke(this, e);
            _contentPanel.ApplyClicked += (s, e) => ClosePopup(true);
            _contentPanel.CancelClicked += (s, e) => ClosePopup(false);

            _form.Controls.Add(contentControl);

            int targetHeight = CalculatePopupHeight(effectiveModel, _profile);
            int minWidthOverride = 0;
            bool autoFlip = true;
            if (owner is BeepComboBox comboOwner)
            {
                minWidthOverride = Math.Max(0, comboOwner.MinDropdownWidth);
                autoFlip = comboOwner.AutoFlip;
            }

            int desiredWidth = Math.Max(triggerBounds.Width, Math.Max(_profile.MinWidth, minWidthOverride));
            var placement = ComboBoxPopupPlacementHelper.Calculate(owner, desiredWidth, targetHeight, autoFlip);
            _form.Size = new Size(desiredWidth, placement.Height);
            
            _contentPanel.UpdateModel(effectiveModel);

            if (owner is BeepComboBox beepOwner)
            {
                _form.Theme = beepOwner.Theme;
            }

            ConfigurePopupForm(_form, _profile, themeTokens);
            _form.ShowPopup(owner, BeepPopupFormPosition.Bottom, _form.Width, _form.Height);

            if (_contentPanel != null && (effectiveModel.ShowSearchBox || _profile.ForceSearchVisible))
            {
                contentControl.BeginInvoke(new Action(() => _contentPanel.FocusSearchBox()));
            }
        }

        public void ClosePopup(bool commit)
        {
            if (_form != null)
            {
                _form.FormClosed -= OnFormClosed;
                _form.CloseCascade();
                _form = null;
                if (!_closeRaised)
                {
                    _closeRaised = true;
                    PopupClosed?.Invoke(this, new ComboBoxPopupClosedEventArgs(commit));
                }
            }
        }

        public void UpdateModel(ComboBoxPopupModel model)
        {
            _contentPanel?.UpdateModel(model);
        }

        public void UpdateSearchText(string text)
        {
            _contentPanel?.UpdateSearchText(text);
        }

        public void SetKeyboardFocusIndex(int index)
        {
            _contentPanel?.SetKeyboardFocusIndex(index);
        }

        public void FocusItem(SimpleItem item)
        {
            if (item == null || _contentPanel == null) return;
            _contentPanel.FocusItem(item);
        }

        public void Dispose()
        {
            ClosePopup(false);
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (_closeRaised)
            {
                return;
            }

            _closeRaised = true;
            PopupClosed?.Invoke(this, new ComboBoxPopupClosedEventArgs(false));
        }

        protected virtual ComboBoxPopupHostProfile CreateProfile(Control owner, ComboBoxPopupModel model)
        {
            _ = model;
            if (owner is BaseControl baseControl)
            {
                return baseControl.ControlStyle switch
                {
                    BeepControlStyle.Minimal or
                    BeepControlStyle.NotionMinimal or
                    BeepControlStyle.VercelClean => ComboBoxPopupHostProfile.MinimalBorderless(),
                    BeepControlStyle.GlassAcrylic or
                    BeepControlStyle.Glassmorphism => ComboBoxPopupHostProfile.DenseList(),
                    BeepControlStyle.Fluent2 or
                    BeepControlStyle.Material3 or
                    BeepControlStyle.Modern => ComboBoxPopupHostProfile.OutlineSearchable(),
                    _ => ComboBoxPopupHostProfile.OutlineDefault()
                };
            }

            return ComboBoxPopupHostProfile.OutlineDefault();
        }

        /// <summary>
        /// Creates the content panel for this popup variant.
        /// Override in subclasses to return a different content type
        /// (e.g. <see cref="PillGridPopupContent"/>, <see cref="ChipHeaderPopupContent"/>).
        /// </summary>
        protected virtual IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            var content = new ComboBoxListBoxPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        /// <summary>
        /// Calculates the ideal popup height. Override for variants with
        /// non-standard content (pill grid, chip header, etc.).
        /// </summary>
        protected virtual int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            int count = model.FilteredRows?.Count ?? 1;
            int contentHeight = count * profile.BaseRowHeight;
            if (model.ShowSearchBox || profile.ForceSearchVisible) contentHeight += profile.SearchBoxHeight;
            if (model.ShowFooter || profile.ForceFooterVisible) contentHeight += profile.FooterHeight;
            return Math.Min(contentHeight, profile.MaxHeight);
        }

        /// <summary>
        /// Hook for subclasses to customize the popup form itself
        /// (e.g. remove border for MinimalBorderless).
        /// </summary>
        protected virtual void ConfigurePopupForm(BeepPopupForm form, ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Corner radius
            if (profile.PopupCornerRadius > 0)
            {
                form.CornerRadius = new CornerRadius(profile.PopupCornerRadius);
            }

            // Shadow effect
            if (profile.PopupShadowDepth > 0)
            {
                var shadow = new ShadowEffect();
                switch (profile.PopupShadowDepth)
                {
                    case 1: // light
                        shadow.Color = Color.FromArgb(20, 0, 0, 0);
                        shadow.Blur = 6;
                        shadow.OffsetY = 2;
                        break;
                    case 2: // medium
                        shadow.Color = Color.FromArgb(35, 0, 0, 0);
                        shadow.Blur = 12;
                        shadow.OffsetY = 4;
                        break;
                    case 3: // heavy
                        shadow.Color = Color.FromArgb(50, 0, 0, 0);
                        shadow.Blur = 20;
                        shadow.OffsetY = 6;
                        break;
                }
                form.ShadowEffect = shadow;
            }
        }

        private static ComboBoxPopupModel NormalizeModel(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            if (model == null)
            {
                return new ComboBoxPopupModel
                {
                    FilteredRows = System.Array.Empty<ComboBoxPopupRowModel>(),
                    SearchText = string.Empty
                };
            }

            bool showSearch = model.ShowSearchBox || profile.ForceSearchVisible;
            bool showFooter = model.ShowFooter || profile.ForceFooterVisible;

            if (showSearch == model.ShowSearchBox && showFooter == model.ShowFooter)
            {
                return model;
            }

            return new ComboBoxPopupModel
            {
                AllRows = model.AllRows,
                FilteredRows = model.FilteredRows,
                KeyboardFocusIndex = model.KeyboardFocusIndex,
                ShowSearchBox = showSearch,
                ShowFooter = showFooter,
                ShowApplyCancel = model.ShowApplyCancel,
                ShowSelectAll = model.ShowSelectAll,
                SearchText = model.SearchText,
                IsLoading = model.IsLoading,
                IsMultiSelect = model.IsMultiSelect,
                UsePrimaryActionFooter = model.UsePrimaryActionFooter,
                PrimaryActionText = model.PrimaryActionText,
                HasGroupHeaders = model.HasGroupHeaders
            };
        }

        protected virtual ComboBoxThemeTokens ResolveThemeTokens(Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            if (owner is BeepComboBox combo)
            {
                return ComboBoxStateFactory.Build(combo).ThemeTokens ?? ComboBoxThemeTokens.Fallback();
            }

            return ComboBoxThemeTokens.Fallback();
        }

        protected static Color Blend(Color a, Color b, float t)
        {
            if (t <= 0f) return a;
            if (t >= 1f) return b;

            int r = (int)(a.R + ((b.R - a.R) * t));
            int g = (int)(a.G + ((b.G - a.G) * t));
            int bl = (int)(a.B + ((b.B - a.B) * t));
            int alpha = (int)(a.A + ((b.A - a.A) * t));
            return Color.FromArgb(alpha, r, g, bl);
        }

        protected static Color WithAlpha(Color c, int alpha)
            => Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), c.R, c.G, c.B);

        protected static ComboBoxThemeTokens DeriveTokens(
            ComboBoxThemeTokens source,
            Color? popupBack = null,
            Color? popupBorder = null,
            Color? rowHover = null,
            Color? rowSelected = null,
            Color? rowFocus = null,
            Color? groupBack = null,
            Color? groupFore = null,
            Color? separator = null,
            Color? subText = null,
            Color? hoverBorder = null,
            Color? focusBorder = null,
            Color? openBorder = null,
            Color? selectedFore = null)
        {
            if (source == null)
            {
                source = ComboBoxThemeTokens.Fallback();
            }

            return new ComboBoxThemeTokens
            {
                BackColor = source.BackColor,
                ForeColor = source.ForeColor,
                BorderColor = source.BorderColor,
                HoverBorderColor = hoverBorder ?? source.HoverBorderColor,
                FocusBorderColor = focusBorder ?? source.FocusBorderColor,
                OpenBorderColor = openBorder ?? source.OpenBorderColor,
                DisabledBackColor = source.DisabledBackColor,
                DisabledForeColor = source.DisabledForeColor,
                ErrorBorderColor = source.ErrorBorderColor,
                WarningBorderColor = source.WarningBorderColor,
                SuccessBorderColor = source.SuccessBorderColor,
                ErrorForeColor = source.ErrorForeColor,
                PlaceholderColor = source.PlaceholderColor,
                ChevronColor = source.ChevronColor,
                ClearButtonColor = source.ClearButtonColor,
                SelectionHighlight = source.SelectionHighlight,
                ButtonHoverBackground = source.ButtonHoverBackground,
                SelectedBackColor = source.SelectedBackColor,
                SelectedForeColor = selectedFore ?? source.SelectedForeColor,
                SelectedBorderColor = source.SelectedBorderColor,
                PopupBackColor = popupBack ?? source.PopupBackColor,
                PopupBorderColor = popupBorder ?? source.PopupBorderColor,
                PopupRowHoverColor = rowHover ?? source.PopupRowHoverColor,
                PopupRowSelectedColor = rowSelected ?? source.PopupRowSelectedColor,
                PopupRowFocusColor = rowFocus ?? source.PopupRowFocusColor,
                PopupGroupHeaderBack = groupBack ?? source.PopupGroupHeaderBack,
                PopupGroupHeaderFore = groupFore ?? source.PopupGroupHeaderFore,
                PopupSeparatorColor = separator ?? source.PopupSeparatorColor,
                PopupSubTextColor = subText ?? source.PopupSubTextColor,
                LabelFont = source.LabelFont,
                SubTextFont = source.SubTextFont,
            };
        }
    }
}
