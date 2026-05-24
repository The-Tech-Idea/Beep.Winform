using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Interop;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Manages content control hosting within MDI child windows.
    /// Handles reparenting, ownership tracking, and lifecycle management.
    /// </summary>
    public class ContentHosting : IDisposable
    {
        private readonly Dictionary<string, HostedContent> _hostedControls = new();
        private bool _disposed = false;

        /// <summary>
        /// Represents a hosted content control within an MDI window.
        /// </summary>
        public class HostedContent
        {
            /// <summary>Panel key identifier.</summary>
            public string PanelKey { get; set; }

            /// <summary>The actual content control hosted.</summary>
            public Control ContentControl { get; set; }

            /// <summary>The MDI child window HWND.</summary>
            public IntPtr ChildWindowHandle { get; set; }

            /// <summary>Original parent before reparenting.</summary>
            public Control OriginalParent { get; set; }

            /// <summary>Timestamp when control was hosted.</summary>
            public DateTime HostedAt { get; set; }

            /// <summary>Whether content is currently visible.</summary>
            public bool IsVisible { get; set; }

            /// <summary>The original window style before hosting.</summary>
            public IntPtr OriginalStyle { get; set; }

            /// <summary>The original extended window style before hosting.</summary>
            public IntPtr OriginalExStyle { get; set; }

            /// <summary>Whether this is the active hosted content.</summary>
            public bool IsActive { get; set; }
        }

        /// <summary>
        /// Hosts a content control within an MDI child window.
        /// Reparents the control and fills the window.
        /// </summary>
        public bool HostContent(
            string panelKey,
            Control contentControl,
            IntPtr childWindowHandle)
        {
            if (string.IsNullOrEmpty(panelKey))
                throw new ArgumentException("Panel key cannot be null or empty", nameof(panelKey));

            if (contentControl == null)
                throw new ArgumentNullException(nameof(contentControl));

            if (childWindowHandle == IntPtr.Zero)
                throw new ArgumentException("Child window handle cannot be zero", nameof(childWindowHandle));

            try
            {
                // If already hosting, unhoist first
                if (_hostedControls.ContainsKey(panelKey))
                {
                    UnhostContent(panelKey);
                }

                // Store original parent
                var originalParent = contentControl.Parent;

                // Get original window styles if it's a Control with HWND
                var originalStyle = IntPtr.Zero;
                var originalExStyle = IntPtr.Zero;

                if (contentControl.Handle != IntPtr.Zero)
                {
                    originalStyle = MdiNativeApi.GetWindowLongPtr(
                        contentControl.Handle,
                        (int)WindowStyleIndex.GWL_STYLE
                    );

                    originalExStyle = MdiNativeApi.GetWindowLongPtr(
                        contentControl.Handle,
                        (int)WindowStyleIndex.GWL_EXSTYLE
                    );
                }

                // Reparent control to child window
                // This transfers ownership from its current parent to the MDI window
                contentControl.Parent = null;  // Detach from current parent
                contentControl.Dock = DockStyle.Fill;
                contentControl.Visible = true;

                // Create a temporary container if contentControl doesn't have its own HWND
                Control hostContainer = contentControl;
                if (contentControl.Handle == IntPtr.Zero)
                {
                    // For windowless controls, we need to create a container
                    var container = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BackColor = System.Drawing.SystemColors.Control
                    };
                    container.Controls.Add(contentControl);
                    hostContainer = container;
                    hostContainer.Dock = DockStyle.Fill;
                }

                // If contentControl has HWND, reparent it directly to the MDI child
                if (contentControl.Handle != IntPtr.Zero)
                {
                    MdiNativeApi.SetParent(contentControl.Handle, childWindowHandle);
                    MdiNativeApi.SetWindowPos(
                        contentControl.Handle,
                        IntPtr.Zero,
                        0, 0,
                        0, 0,
                        0x17  // SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED
                    );
                    MdiNativeApi.UpdateWindow(contentControl.Handle);
                    MdiNativeApi.InvalidateRect(contentControl.Handle, IntPtr.Zero, true);
                }

                // Track the hosted content
                var hostedContent = new HostedContent
                {
                    PanelKey = panelKey,
                    ContentControl = contentControl,
                    ChildWindowHandle = childWindowHandle,
                    OriginalParent = originalParent,
                    HostedAt = DateTime.UtcNow,
                    IsVisible = true,
                    IsActive = false,
                    OriginalStyle = originalStyle,
                    OriginalExStyle = originalExStyle
                };

                _hostedControls[panelKey] = hostedContent;

                Debug.WriteLine($"[ContentHosting] Hosted content '{panelKey}' in window 0x{childWindowHandle:X8}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ContentHosting] Failed to host content '{panelKey}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unhosts a content control and restores it to its original parent.
        /// </summary>
        public bool UnhostContent(string panelKey)
        {
            if (!_hostedControls.TryGetValue(panelKey, out var hosted))
                return false;

            try
            {
                // Restore original parent
                if (hosted.OriginalParent != null)
                {
                    hosted.ContentControl.Parent = hosted.OriginalParent;
                }
                else
                {
                    hosted.ContentControl.Parent = null;
                }

                // Restore original window styles if they were saved
                if (hosted.ContentControl.Handle != IntPtr.Zero)
                {
                    if (hosted.OriginalStyle != IntPtr.Zero)
                    {
                        MdiNativeApi.SetWindowLongPtr(
                            hosted.ContentControl.Handle,
                            (int)WindowStyleIndex.GWL_STYLE,
                            hosted.OriginalStyle
                        );
                    }

                    if (hosted.OriginalExStyle != IntPtr.Zero)
                    {
                        MdiNativeApi.SetWindowLongPtr(
                            hosted.ContentControl.Handle,
                            (int)WindowStyleIndex.GWL_EXSTYLE,
                            hosted.OriginalExStyle
                        );
                    }

                    MdiNativeApi.UpdateWindow(hosted.ContentControl.Handle);
                }

                _hostedControls.Remove(panelKey);

                Debug.WriteLine($"[ContentHosting] Unhosted content '{panelKey}'");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ContentHosting] Failed to unhost content '{panelKey}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the hosted content for a panel.
        /// </summary>
        public HostedContent GetHostedContent(string panelKey)
        {
            _hostedControls.TryGetValue(panelKey, out var hosted);
            return hosted;
        }

        /// <summary>
        /// Gets the content control for a panel, or null if not hosted.
        /// </summary>
        public Control GetContentControl(string panelKey)
        {
            return GetHostedContent(panelKey)?.ContentControl;
        }

        /// <summary>
        /// Sets the active hosted content (for styling/highlighting purposes).
        /// </summary>
        public void SetActiveContent(string panelKey)
        {
            // Deactivate all others
            foreach (var kvp in _hostedControls.Values)
            {
                kvp.IsActive = false;
            }

            // Activate the specified one
            if (_hostedControls.TryGetValue(panelKey, out var hosted))
            {
                hosted.IsActive = true;
                Debug.WriteLine($"[ContentHosting] Set active content to '{panelKey}'");
            }
        }

        /// <summary>
        /// Hides a hosted content control.
        /// </summary>
        public bool HideContent(string panelKey)
        {
            if (!_hostedControls.TryGetValue(panelKey, out var hosted))
                return false;

            hosted.ContentControl.Hide();
            hosted.IsVisible = false;
            Debug.WriteLine($"[ContentHosting] Hidden content '{panelKey}'");
            return true;
        }

        /// <summary>
        /// Shows a previously hidden content control.
        /// </summary>
        public bool ShowContent(string panelKey)
        {
            if (!_hostedControls.TryGetValue(panelKey, out var hosted))
                return false;

            hosted.ContentControl.Show();
            hosted.IsVisible = true;
            Debug.WriteLine($"[ContentHosting] Showed content '{panelKey}'");
            return true;
        }

        /// <summary>
        /// Gets all currently hosted content.
        /// </summary>
        public IReadOnlyDictionary<string, HostedContent> GetAllHostedContent() =>
            _hostedControls;

        /// <summary>
        /// Gets diagnostic information about hosted content.
        /// </summary>
        public ContentHostingDiagnostics GetDiagnostics()
        {
            var descriptors = new List<ContentDescriptor>();
            foreach (var kvp in _hostedControls)
            {
                var hosted = kvp.Value;
                descriptors.Add(new ContentDescriptor
                {
                    PanelKey = hosted.PanelKey,
                    ControlType = hosted.ContentControl?.GetType().Name,
                    ChildWindowHwnd = hosted.ChildWindowHandle,
                    IsVisible = hosted.IsVisible,
                    IsActive = hosted.IsActive,
                    HostedAtUtc = hosted.HostedAt
                });
            }

            return new ContentHostingDiagnostics
            {
                TotalHostedControls = _hostedControls.Count,
                VisibleControls = descriptors.Count(x => x.IsVisible),
                HiddenControls = descriptors.Count(x => !x.IsVisible),
                ActiveContent = descriptors.FirstOrDefault(x => x.IsActive)?.PanelKey,
                HostedDescriptors = descriptors
            };
        }

        /// <summary>
        /// Diagnostics result for hosted content.
        /// </summary>
        public class ContentHostingDiagnostics
        {
            public int TotalHostedControls { get; set; }
            public int VisibleControls { get; set; }
            public int HiddenControls { get; set; }
            public string ActiveContent { get; set; }
            public List<ContentDescriptor> HostedDescriptors { get; set; }
        }

        /// <summary>
        /// Descriptor for a single hosted control.
        /// </summary>
        public class ContentDescriptor
        {
            public string PanelKey { get; set; }
            public string ControlType { get; set; }
            public IntPtr ChildWindowHwnd { get; set; }
            public bool IsVisible { get; set; }
            public bool IsActive { get; set; }
            public DateTime HostedAtUtc { get; set; }
        }

        /// <summary>
        /// Window style index for GetWindowLongPtr/SetWindowLongPtr.
        /// </summary>
        private enum WindowStyleIndex
        {
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // Unhost all content
            var keys = new List<string>(_hostedControls.Keys);
            foreach (var key in keys)
            {
                UnhostContent(key);
            }

            _hostedControls.Clear();
            _disposed = true;
            Debug.WriteLine("[ContentHosting] Disposed");
        }
    }
}
