using System;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _animationHelper?.Dispose();
                
                // Dispose all addins
                foreach (var addin in _addins.Values)
                {
                    if (addin is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                
                _tabs.Clear();
                _addins.Clear();
            }
            
            base.Dispose(disposing);
        }

        #endregion
    }
}

