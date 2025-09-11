using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    internal class MDIAnimationHelper : IDisposable
    {
        private readonly Action _invalidate;
        public MDIAnimationHelper(Action invalidate) => _invalidate = invalidate;
        public void Start(MDIDocument doc, float target) => doc.TargetAnimationProgress = target;
        public void Tick(List<MDIDocument> docs, bool enabled)
        {
            if (!enabled) return;
            bool any = false;
            foreach (var d in docs)
            {
                float current = d.AnimationProgress;
                float target = d.TargetAnimationProgress;
                if (Math.Abs(current - target) > 0.01f)
                {
                    d.AnimationProgress = current + (target - current) * 0.25f;
                    any = true;
                }
                else
                {
                    d.AnimationProgress = target;
                }
            }
            if (any) _invalidate();
        }
        public void Dispose() { }
    }
}
