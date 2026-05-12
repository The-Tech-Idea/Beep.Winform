using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public IDisposable DeferVisualUpdate()
        {
            return new VisualUpdateScope(this);
        }

        private sealed class VisualUpdateScope : IDisposable
        {
            private readonly BeepCalendar _owner;
            private bool _disposed;

            public VisualUpdateScope(BeepCalendar owner)
            {
                _owner = owner;
                _owner.BeginVisualUpdate();
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _owner.EndVisualUpdate(flush: true);
            }
        }
    }
}