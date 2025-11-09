using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

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
            try
            {
                var id = Guid.NewGuid().ToString();
                var tab = new AddinTab
                {
                    Id = id,
                    Title = titleText,
                    Addin = control,
                    CanClose = true,
                    IsVisible = true
                };

                _tabs.Add(tab);
                _addins[id] = control;

                // Add control to Controls collection immediately to preserve it
                var addinControl = control as Control;
                if (addinControl != null && !Controls.Contains(addinControl))
                {
                    Controls.Add(addinControl);
                }

                // Handle activation based on display mode
                if (_displayMode == ContainerDisplayMode.Single)
                {
                    // In Single mode, always activate the newly added control
                    _activeTab = tab;
                    // Hide all other tabs
                    foreach (var existingTab in _tabs.Where(t => t != tab))
                    {
                        var ctrl = existingTab.Addin as Control;
                        if (ctrl != null)
                        {
                            ctrl.Visible = false;
                        }
                        existingTab.IsVisible = false;
                    }
                    // Ensure new tab is visible
                    tab.IsVisible = true;
                }
                else
                {
                    // Tabbed mode: Always activate the newly added tab automatically
                    // This ensures the new tab is shown and navigated to
                    // In Tabbed mode, ensure tab is visible (layout will set proper bounds)
                    tab.IsVisible = true;
                    
                    // Don't set _activeTab here - let ActivateTab handle it
                }

                // Activate the tab to ensure proper positioning and visibility
                // This will set _activeTab and handle all activation logic
                // ActivateTab will call RecalculateLayout internally
                ActivateTab(tab);
                
                OnAddinAdded(new ContainerEvents 
                { 
                    TitleText = titleText, 
                    Control = control, 
                    ContainerType = pcontainerType, 
                    Guidid = control?.GuidID 
                });
                
                // Only invalidate once if we're not in batch mode
                if (!_batchMode && IsHandleCreated)
                {
                    Invalidate();
                }

                return true;
            }
            catch
            {
                return false;
            }
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

