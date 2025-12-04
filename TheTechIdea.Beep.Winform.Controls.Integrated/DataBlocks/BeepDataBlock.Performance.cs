using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Performance Optimizations
    /// Cache frequently accessed data, debounce expensive operations
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        private Dictionary<TriggerType, List<BeepDataBlockTrigger>> _triggerCache = new Dictionary<TriggerType, List<BeepDataBlockTrigger>>();
        private Dictionary<string, object> _lovDataCache = new Dictionary<string, object>();
        private Dictionary<string, DateTime> _validationDebounceTimers = new Dictionary<string, DateTime>();
        private const int DebounceDelayMs = 300;
        private System.Timers.Timer _systemVariablesUpdateTimer;
        private bool _systemVariablesNeedUpdate = false;
        
        #endregion
        
        #region Trigger Performance
        
        /// <summary>
        /// Get cached triggers for a type (faster than dictionary lookup every time)
        /// </summary>
        private List<BeepDataBlockTrigger> GetCachedTriggers(TriggerType type)
        {
            if (_triggerCache.TryGetValue(type, out var cached))
                return cached;
                
            // Not in cache - get from main dictionary and cache
            if (_triggers.ContainsKey(type))
            {
                var triggers = _triggers[type].Where(t => t.IsEnabled).ToList();
                _triggerCache[type] = triggers;
                return triggers;
            }
            
            return new List<BeepDataBlockTrigger>();
        }
        
        /// <summary>
        /// Invalidate trigger cache (call when triggers are added/removed/modified)
        /// </summary>
        public void InvalidateTriggerCache()
        {
            _triggerCache.Clear();
        }
        
        #endregion
        
        #region LOV Performance
        
        /// <summary>
        /// Clear LOV data cache (call when underlying data changes)
        /// </summary>
        public void InvalidateLOVCache()
        {
            _lovDataCache.Clear();
            ClearAllLOVCaches();
        }
        
        #endregion
        
        #region Validation Performance
        
        /// <summary>
        /// Debounced validation - only validate if enough time has passed
        /// Prevents validation on every keystroke
        /// </summary>
        protected bool ShouldValidateNow(string fieldName)
        {
            if (!_validationDebounceTimers.ContainsKey(fieldName))
            {
                _validationDebounceTimers[fieldName] = DateTime.Now;
                return true;
            }
            
            var lastValidation = _validationDebounceTimers[fieldName];
            var elapsed = (DateTime.Now - lastValidation).TotalMilliseconds;
            
            if (elapsed >= DebounceDelayMs)
            {
                _validationDebounceTimers[fieldName] = DateTime.Now;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Clear validation debounce timers
        /// </summary>
        public void ClearValidationDebounce()
        {
            _validationDebounceTimers.Clear();
        }
        
        #endregion
        
        #region SystemVariables Performance
        
        /// <summary>
        /// Schedule SystemVariables update (batched, not on every change)
        /// </summary>
        private void ScheduleSystemVariablesUpdate()
        {
            _systemVariablesNeedUpdate = true;
            
            if (_systemVariablesUpdateTimer == null)
            {
                _systemVariablesUpdateTimer = new System.Timers.Timer(100);  // 100ms delay
                _systemVariablesUpdateTimer.Elapsed += SystemVariablesUpdateTimer_Elapsed;
                _systemVariablesUpdateTimer.AutoReset = false;
            }
            
            _systemVariablesUpdateTimer.Stop();
            _systemVariablesUpdateTimer.Start();
        }
        
        private void SystemVariablesUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_systemVariablesNeedUpdate)
            {
                UpdateSystemVariables();
                _systemVariablesNeedUpdate = false;
            }
        }
        
        #endregion
        
        #region Cache Management
        
        /// <summary>
        /// Clear all performance caches
        /// </summary>
        public void ClearAllCaches()
        {
            InvalidateTriggerCache();
            InvalidateLOVCache();
            ClearValidationDebounce();
            _systemVariablesNeedUpdate = false;
        }
        
        /// <summary>
        /// Get cache statistics
        /// </summary>
        public CacheStatistics GetCacheStatistics()
        {
            return new CacheStatistics
            {
                TriggerCacheSize = _triggerCache.Count,
                LOVCacheSize = _lovDataCache.Count,
                ValidationDebounceSize = _validationDebounceTimers.Count,
                TotalCacheMemoryEstimate = EstimateCacheMemory()
            };
        }
        
        private long EstimateCacheMemory()
        {
            // Rough estimate in bytes
            long total = 0;
            total += _triggerCache.Count * 1000;  // ~1KB per trigger list
            total += _lovDataCache.Count * 10000;  // ~10KB per LOV cache
            total += _validationDebounceTimers.Count * 100;  // ~100 bytes per debounce entry
            return total;
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
