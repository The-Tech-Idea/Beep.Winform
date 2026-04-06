using System;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Performance Optimizations
    /// When coordinated, all caching is delegated to FormsManager.PerformanceManager.
    /// Local state has been removed to keep BeepDataBlock UI/UX-only.
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Trigger Performance

        /// <summary>
        /// Invalidate trigger cache.
        /// When coordinated, delegates to FormsManager.PerformanceManager.
        /// </summary>
        public void InvalidateTriggerCache()
        {
            if (IsCoordinated)
                _formsManager?.PerformanceManager?.ClearCache();
        }

        #endregion

        #region LOV Performance

        /// <summary>
        /// Clear LOV data cache (call when underlying data changes).
        /// When coordinated, delegates to FormsManager.PerformanceManager.
        /// </summary>
        public void InvalidateLOVCache()
        {
            if (IsCoordinated)
                _formsManager?.PerformanceManager?.ClearCache();
            ClearAllLOVCaches();
        }

        #endregion

        #region Validation Performance

        /// <summary>
        /// Returns true to allow validation.
        /// Debouncing is handled by FormsManager when coordinated.
        /// </summary>
        protected bool ShouldValidateNow(string fieldName) => true;

        /// <summary>
        /// No-op: debounce state is owned by FormsManager when coordinated.
        /// </summary>
        public void ClearValidationDebounce() { }

        #endregion

        #region SystemVariables Performance

        /// <summary>
        /// Trigger an immediate SystemVariables update.
        /// FormsManager owns scheduling; BeepDataBlock just calls UpdateSystemVariables directly.
        /// </summary>
        private void ScheduleSystemVariablesUpdate()
        {
            UpdateSystemVariables();
        }

        #endregion

        #region Cache Management

        /// <summary>
        /// Clear all performance caches (delegates to FormsManager when coordinated).
        /// </summary>
        public void ClearAllCaches()
        {
            InvalidateTriggerCache();
            InvalidateLOVCache();
            ClearValidationDebounce();
        }

        /// <summary>
        /// Get cache statistics from FormsManager when coordinated, otherwise returns empty stats.
        /// </summary>
        public CacheStatistics GetCacheStatistics()
        {
            if (IsCoordinated && _formsManager?.PerformanceManager != null)
            {
                try
                {
                    var stats = _formsManager.PerformanceManager.GetPerformanceStatistics();
                    return new CacheStatistics { TriggerCacheSize = stats?.CacheSize ?? 0 };
                }
                catch { }
            }
            return new CacheStatistics();
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Cache statistics
    /// </summary>
    public class CacheStatistics
    {
        public int TriggerCacheSize { get; set; }
        public int LOVCacheSize { get; set; }
        public int ValidationDebounceSize { get; set; }
        public long TotalCacheMemoryEstimate { get; set; }

        public string TotalCacheMemoryFormatted
        {
            get
            {
                if (TotalCacheMemoryEstimate < 1024)
                    return $"{TotalCacheMemoryEstimate} bytes";
                if (TotalCacheMemoryEstimate < 1024 * 1024)
                    return $"{TotalCacheMemoryEstimate / 1024:F1} KB";
                return $"{TotalCacheMemoryEstimate / (1024 * 1024):F1} MB";
            }
        }
    }
    
    #endregion
}
