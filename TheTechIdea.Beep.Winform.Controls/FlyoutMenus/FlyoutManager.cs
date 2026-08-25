using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.FlyoutMenus
{
    /// <summary>
    /// Component that shows any <see cref="Control"/> as a non-modal flyout anchored to a trigger
    /// control or a screen point. One flyout is open at a time; showing a new one closes the
    /// previous. The popup dismisses itself when the pointer leaves the anchor/popup cascade
    /// (via <see cref="BeepPopupForm"/>). The manager never disposes the hosted content: it
    /// detaches it before disposing the transient host form.
    /// </summary>
    [ToolboxItem(true)]
    public class FlyoutManager : Component, IFlyoutManager
    {
        private FlyoutHostForm? _host;
        private Control? _content;
        private DockStyle _originalDock;

        /// <summary>Raised when the active flyout closes, whether via <see cref="Close"/> or outside-click dismissal.</summary>
        public event EventHandler? Closed;

        /// <inheritdoc/>
        public bool IsOpen => _host is { IsDisposed: false, Visible: true };

        /// <inheritdoc/>
        public void Show(Control content, Control? anchor, BeepPopupFormPosition position = BeepPopupFormPosition.Bottom, Size? size = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            Close();

            _content = content;
            _originalDock = content.Dock;

            var host = new FlyoutHostForm();
            var hostSize = ResolveSize(content, size);
            host.Host(content);

            if (anchor != null && !anchor.IsDisposed)
            {
                host.ShowPopup(anchor, position, hostSize.Width, hostSize.Height);
            }
            else
            {
                host.Size = hostSize;
                host.ShowPopup(Cursor.Position, position);
            }

            _host = host;
            host.FormClosed += HostClosed;
        }

        /// <inheritdoc/>
        public void Close()
        {
            var host = _host;
            if (host == null || host.IsDisposed) return;

            _host = null;
            // FormClosed -> HostClosed detaches the content, disposes the host, and raises Closed.
            host.CloseCascade();
        }

        private void HostClosed(object? sender, FormClosedEventArgs e)
        {
            if (sender is not FlyoutHostForm host) return;

            host.FormClosed -= HostClosed;
            if (ReferenceEquals(_host, host)) _host = null;

            DetachContent(host);
            host.Dispose();

            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void DetachContent(FlyoutHostForm host)
        {
            if (_content == null) return;
            if (_content.Parent == host)
            {
                host.Controls.Remove(_content);
            }
            _content.Dock = _originalDock;
            _content = null;
        }

        private static Size ResolveSize(Control content, Size? size)
        {
            if (size is { } s && s.Width > 0 && s.Height > 0) return s;
            if (content.PreferredSize is { Width: > 0, Height: > 0 }) return content.PreferredSize;
            if (content.Size.Width > 0 && content.Size.Height > 0) return content.Size;
            return new Size(240, 200);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            base.Dispose(disposing);
        }
    }
}
