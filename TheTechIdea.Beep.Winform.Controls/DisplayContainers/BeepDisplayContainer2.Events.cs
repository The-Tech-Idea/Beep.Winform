using System;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        /// <summary>
        /// Raised when the container absorbed a failure instead of propagating it.
        /// </summary>
        /// <remarks>
        /// Subscribe to see failures that would otherwise be invisible. A container that cannot
        /// paint must not tear down the form's paint cycle, but it must not fail silently either.
        /// </remarks>
        public event EventHandler<ContainerErrorEventArgs> ContainerError;

        /// <summary>Reports an absorbed failure. Never throws from the reporting path itself.</summary>
        protected virtual void OnContainerError(string context, Exception exception)
        {
            if (exception == null) return;
            System.Diagnostics.Trace.WriteLine($"BeepDisplayContainer2 [{context}]: {exception}");
            ContainerError?.Invoke(this, new ContainerErrorEventArgs(context, exception));
        }

        #region Event Handlers

        protected virtual void OnAddinAdded(ContainerEvents e)
        {
            AddinAdded?.Invoke(this, e);
        }

        protected virtual void OnAddinRemoved(ContainerEvents e)
        {
            AddinRemoved?.Invoke(this, e);
        }

        protected virtual void OnAddinMoved(ContainerEvents e)
        {
            AddinMoved?.Invoke(this, e);
        }

        protected virtual void OnAddinChanging(ContainerEvents e)
        {
            AddinChanging?.Invoke(this, e);
        }

        protected virtual void OnAddinChanged(ContainerEvents e)
        {
            AddinChanged?.Invoke(this, e);
        }

        protected virtual void OnNewTabRequested()
        {
            NewTabRequested?.Invoke(this, new ContainerEvents
            {
                ContainerType = _containerType,
                TitleText = "NewTabRequest"
            });
        }

        #endregion
    }
}

