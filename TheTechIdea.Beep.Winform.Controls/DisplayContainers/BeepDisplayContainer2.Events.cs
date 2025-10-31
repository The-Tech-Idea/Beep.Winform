using System;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
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

        #endregion
    }
}

