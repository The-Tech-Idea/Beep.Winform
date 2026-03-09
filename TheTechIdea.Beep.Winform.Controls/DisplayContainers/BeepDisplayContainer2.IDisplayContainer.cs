using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region IDisplayContainer Implementation
        
        public async Task<bool> ShowPopup(IDM_Addin view)
        {
            try
            {
                if (view is Form form)
                {
                    if(form is IFormStyle)
                    {
                        IFormStyle formStyle = (IFormStyle)form;
                        formStyle.FormStyle = BeepThemesManager.CurrentStyle    ;
                        formStyle.Theme= BeepThemesManager.CurrentThemeName;
                    }
                    if (!form.IsDisposed)
                    {
                        IBeepUIComponent beepUIComponent = (IBeepUIComponent)view;
                        form.Load += (s, e) =>
                        {
                            //MiscFunctions.SetThemePropertyinControlifexist( form, MenuStyle);
                        };
                        // Ensure this runs on the UI thread
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.StartPosition = FormStartPosition.CenterParent;

                                form.Show();
                            }));
                        }
                        else
                        {
                            form.StartPosition = FormStartPosition.CenterParent;
                          
                            form.Show();
                        }
                    }
                    else
                    {
                       return false;
                    }
                }
                else if (view is Control control)
                {
                    var popupForm = new BeepiFormPro
                    {
                        StartPosition = FormStartPosition.CenterParent,
                        AutoSize = true,
                        FormStyle= BeepThemesManager.CurrentStyle,
                        Theme= BeepThemesManager.CurrentThemeName,
                        ShowThemeButton= false,
                        ShowProfileButton=false,
                        ShowStyleButton= false,
                        ShowMinMaxButtons=false,
                        ShowCloseButton=true

                    };

                    popupForm.Controls.Add(control);
                    control.Dock = DockStyle.Fill;
                    popupForm.Text = view.Details.AddinName;
                    popupForm.OnFormClose += (s, e) =>
                    {
                        try
                        {
                            // remove view from  form without dispose
                            popupForm.Controls.Remove(control);

                        }
                        catch (Exception)
                        {
                            // Addin should handle OnError internally
                        }
                    };
                    // Ensure the dialog is shown on the UI thread
                    popupForm.Show(this.ParentForm);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
              return false;
            }
        }

        public bool AddControl(string titleText, IDM_Addin control, ContainerTypeEnum pcontainerType)
        {
            // Guard: control must be a non-disposed Control
            var addinControl = control as Control;
            if (control == null) return false;
            if (addinControl != null && addinControl.IsDisposed) return false;

            var id = Guid.NewGuid().ToString();
            var tab = new AddinTab
            {
                Id = id,
                Title = titleText ?? string.Empty,
                Addin = control,
                CanClose = true,
                IsVisible = true
            };

            // --- Step 1: register in collections ---
            _tabs.Add(tab);
            _addins[id] = control;

            // --- Step 2: add WinForms control to Controls collection ---
            if (addinControl != null && !Controls.Contains(addinControl))
            {
                try
                {
                    // Reparent: if the control already has a parent, remove it first
                    // so Controls.Add doesn't throw an InvalidOperationException.
                    if (addinControl.Parent != null && addinControl.Parent != this)
                    {
                        addinControl.Parent.Controls.Remove(addinControl);
                    }
                    Controls.Add(addinControl);
                }
                catch
                {
                    // Failed to host the control — roll back and report failure
                    _tabs.Remove(tab);
                    _addins.Remove(id);
                    return false;
                }
            }

            // --- Step 3: mode-specific pre-activation bookkeeping ---
            if (_displayMode == ContainerDisplayMode.Single)
            {
                _activeTab = tab;
                foreach (var existingTab in _tabs.Where(t => t != tab))
                {
                    var ctrl = existingTab.Addin as Control;
                    if (ctrl != null) ctrl.Visible = false;
                    existingTab.IsVisible = false;
                }
                tab.IsVisible = true;
            }
            else
            {
                tab.IsVisible = true;
            }

            // --- Step 4: activate (layout + position + visibility) ---
            try
            {
                ActivateTab(tab);
            }
            catch
            {
                // Layout/activation failed — the tab is registered but mark it hidden
                // so the container stays usable with existing tabs.
                tab.IsVisible = false;
                if (addinControl != null) addinControl.Visible = false;
            }

            // --- Step 5: theme propagation ---
            try
            {
                if (_tabs.Count == 1)
                {
                    ApplyTheme();
                    // ApplyTheme/PropagateThemeToAddins may trigger UserControl.PerformAutoScale()
                    // on hosted controls with AutoScaleMode.Font, resetting their Bounds back to
                    // design-time size and covering the tab header.  Re-clamp after theme propagation.
                    PositionActiveAddin();
                }
                else
                {
                    PropagateThemeToAddins();
                    PositionActiveAddin();
                }
            }
            catch { /* non-fatal */ }

            // --- Step 6: notify and repaint ---
            try
            {
                OnAddinAdded(new ContainerEvents
                {
                    TitleText = titleText,
                    Control = control,
                    ContainerType = pcontainerType,
                    Guidid = control?.GuidID
                });
            }
            catch { /* non-fatal */ }

            if (!_batchMode && IsHandleCreated)
            {
                try { Invalidate(); } catch { /* non-fatal */ }
            }

            return true;
        }

        public void Clear()
        {
            foreach (var tab in _tabs.ToList())
            {
                RemoveTab(tab);
            }
        }

        public bool IsControlExit(IDM_Addin control)
        {
            return _addins.ContainsValue(control);
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            // Handle keyboard shortcuts
            KeyPressed?.Invoke(this, keyCombination);
            return new ErrorsInfo { Flag = Errors.Ok };
        }

        public bool RemoveControl(string titleText, IDM_Addin control)
        {
            var tab = _tabs.FirstOrDefault(t => t.Title == titleText && t.Addin == control);
            if (tab != null)
            {
                RemoveTab(tab);
                return true;
            }
            return false;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            var tab = _tabs.FirstOrDefault(t => t.Id == guidid);
            if (tab != null)
            {
                RemoveTab(tab);
                return true;
            }
            return false;
        }

        public bool RemoveControlByName(string name)
        {
            var tab = _tabs.FirstOrDefault(t => t.Title == name);
            if (tab != null)
            {
                RemoveTab(tab);
                return true;
            }
            return false;
        }

        public bool ShowControl(string titleText, IDM_Addin control)
        {
            var tab = _tabs.FirstOrDefault(t => t.Title == titleText && t.Addin == control);
            if (tab != null)
            {
                ActivateTab(tab);
                return true;
            }
            return false;
        }

        #endregion
    }
}

