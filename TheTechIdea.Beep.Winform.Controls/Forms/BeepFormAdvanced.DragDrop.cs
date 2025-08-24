using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms
{
    public partial class BeepFormAdvanced
    {
        #region Drag and Drop Support
        private void SetupDragDropSupport()
        {
            if (_contentHost != null)
            {
                // Enable drag-drop only on content panel
                _contentHost.AllowDrop = true;
                _contentHost.DragEnter += ContentHost_DragEnter;
                _contentHost.DragOver += ContentHost_DragOver;
                _contentHost.DragDrop += ContentHost_DragDrop;
                _contentHost.DragLeave += ContentHost_DragLeave;
            }

            // Disable drag-drop on other areas
            if (_titleBar != null) _titleBar.AllowDrop = false;
            if (_statusBar != null) _statusBar.AllowDrop = false;
            
            // Make form itself handle drag events to redirect to content
            this.AllowDrop = true;
            this.DragEnter += BeepFormAdvanced_DragEnter;
            this.DragOver += BeepFormAdvanced_DragOver;
        }

        private void ContentHost_DragEnter(object sender, DragEventArgs e)
        {
            // Check if data contains controls or valid drop data
            if (e.Data.GetDataPresent(typeof(Control)) || 
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent("WindowsForms10PersistentObject"))
            {
                e.Effect = DragDropEffects.Copy;
                
                // Visual feedback: highlight content area
                if (_contentHost != null)
                {
                    _contentHost.BackColor = Color.FromArgb(240, 248, 255); // Light blue
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ContentHost_DragOver(object sender, DragEventArgs e)
        {
            // Continue allowing drop in content area
            if (e.Data.GetDataPresent(typeof(Control)) || 
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent("WindowsForms10PersistentObject"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ContentHost_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Reset visual feedback
                RestoreContentHostAppearance();

                // Handle different types of dropped data
                if (e.Data.GetDataPresent(typeof(Control)))
                {
                    // Dropping an existing control
                    Control droppedControl = (Control)e.Data.GetData(typeof(Control));
                    if (droppedControl != null && droppedControl.Parent != _contentHost)
                    {
                        // Calculate drop position relative to content host
                        Point dropPoint = _contentHost.PointToClient(new Point(e.X, e.Y));
                        droppedControl.Location = dropPoint;
                        
                        // Add to content host
                        _contentHost.Controls.Add(droppedControl);
                        droppedControl.BringToFront();
                    }
                }
                else if (e.Data.GetDataPresent("WindowsForms10PersistentObject"))
                {
                    // Handle control from toolbox (design-time)
                    // This is typically handled by the designer
                }
                else if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    // Handle text drop - could create a label
                    string text = (string)e.Data.GetData(DataFormats.Text);
                    CreateLabelFromText(text, e.X, e.Y);
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                Debug.WriteLine($"Drag-drop error: {ex.Message}");
            }
            finally
            {
                RestoreContentHostAppearance();
            }
        }

        private void ContentHost_DragLeave(object sender, EventArgs e)
        {
            // Remove visual feedback when drag leaves content area
            RestoreContentHostAppearance();
        }

        private void BeepFormAdvanced_DragEnter(object sender, DragEventArgs e)
        {
            // Only allow drops if cursor is over content host
            Point clientPoint = this.PointToClient(new Point(e.X, e.Y));
            
            if (_contentHost != null && _contentHost.Bounds.Contains(clientPoint))
            {
                // Forward to content host
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Reject drops on title bar, status bar, or form chrome
                e.Effect = DragDropEffects.None;
                ShowDropNotAllowedFeedback();
            }
        }

        private void BeepFormAdvanced_DragOver(object sender, DragEventArgs e)
        {
            // Continuously check if over content area
            Point clientPoint = this.PointToClient(new Point(e.X, e.Y));
            
            if (_contentHost != null && _contentHost.Bounds.Contains(clientPoint))
            {
                e.Effect = DragDropEffects.Copy;
                HideDropNotAllowedFeedback();
            }
            else
            {
                e.Effect = DragDropEffects.None;
                ShowDropNotAllowedFeedback();
            }
        }

        private void CreateLabelFromText(string text, int screenX, int screenY)
        {
            // Create a label from dropped text
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = _contentHost.PointToClient(new Point(screenX, screenY))
            };
            
            _contentHost.Controls.Add(label);
            label.BringToFront();
        }

        private void RestoreContentHostAppearance()
        {
            if (_contentHost != null)
            {
                _contentHost.BackColor = _contentBackColor;
            }
        }

        private void ShowDropNotAllowedFeedback()
        {
            // Visual feedback that drop is not allowed
            if (_titleBar != null)
            {
                _titleBar.BackColor = Color.FromArgb(255, 240, 240); // Light red tint
            }
        }

        private void HideDropNotAllowedFeedback()
        {
            // Remove not-allowed feedback
            if (_titleBar != null)
            {
                _titleBar.BackColor = _titleBarColor;
            }
        }

        // Public method to programmatically add controls to content area
        public void AddControlToContent(Control control, Point? location = null)
        {
            if (control == null || _contentHost == null) return;

            if (location.HasValue)
            {
                control.Location = location.Value;
            }
            else
            {
                // Auto-position if no location specified
                control.Location = new Point(10, 10);
            }

            _contentHost.Controls.Add(control);
            control.BringToFront();
        }

        // Public method to remove control from content area
        public void RemoveControlFromContent(Control control)
        {
            if (control != null && _contentHost != null && _contentHost.Controls.Contains(control))
            {
                _contentHost.Controls.Remove(control);
                control.Dispose();
            }
        }

        // Property to get all controls in content area
        [Browsable(false)]
        public Control.ControlCollection ContentControls => _contentHost?.Controls;
        #endregion
    }
}