using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Record Locking System
    /// Provides Oracle Forms LOCK_RECORD equivalent functionality
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        private Dictionary<int, RecordLockInfo> _lockedRecords = new Dictionary<int, RecordLockInfo>();
        private LockMode _lockMode = LockMode.Automatic;
        private bool _lockOnEdit = true;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Lock mode for this block
        /// Oracle Forms equivalent: LOCK_RECORD property
        /// </summary>
        public LockMode LockMode
        {
            get => _lockMode;
            set => _lockMode = value;
        }
        
        /// <summary>
        /// Whether to automatically lock record when user starts editing
        /// </summary>
        public bool LockOnEdit
        {
            get => _lockOnEdit;
            set => _lockOnEdit = value;
        }
        
        /// <summary>
        /// Get count of locked records
        /// </summary>
        public int LockedRecordCount => _lockedRecords.Count;
        
        #endregion
        
        #region Lock Methods
        
        /// <summary>
        /// Lock current record
        /// Oracle Forms equivalent: LOCK_RECORD built-in
        /// </summary>
        public async Task<bool> LockCurrentRecord()
        {
            if (Data == null || Data.Units.Current == null)
                return false;
                
            int recordIndex = Data.Units.CurrentIndex;
            
            // Check if already locked
            if (IsRecordLocked(recordIndex))
                return true;
                
            // Fire PRE-LOCK trigger (using closest available trigger)
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreUpdate,  // Closest Oracle Forms equivalent
                RecordValues = GetCurrentRecordValues(),
                Parameters = new Dictionary<string, object> { ["Operation"] = "Lock" }
            };
            
            if (!await ExecuteTriggers(TriggerType.PreUpdate, context))
            {
                return false;  // Lock cancelled by trigger
            }
            
            try
            {
                // Create lock info
                var lockInfo = new RecordLockInfo
                {
                    RecordIndex = recordIndex,
                    LockTime = DateTime.Now,
                    LockedBy = Environment.UserName,
                    Record = Data.Units.Current
                };
                
                _lockedRecords[recordIndex] = lockInfo;
                
                // Visual feedback
                if (_lockedRecords.Count > 0)
                {
                    ShowInfoMessage($"Record locked (Total locked: {_lockedRecords.Count})");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to lock record: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Unlock current record
        /// </summary>
        public bool UnlockCurrentRecord()
        {
            if (Data == null)
                return false;
                
            int recordIndex = Data.Units.CurrentIndex;
            
            if (_lockedRecords.ContainsKey(recordIndex))
            {
                _lockedRecords.Remove(recordIndex);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Unlock all records
        /// </summary>
        public void UnlockAllRecords()
        {
            _lockedRecords.Clear();
        }
        
        /// <summary>
        /// Check if a record is locked
        /// </summary>
        public bool IsRecordLocked(int recordIndex)
        {
            return _lockedRecords.ContainsKey(recordIndex);
        }
        
        /// <summary>
        /// Check if current record is locked
        /// </summary>
        public bool IsCurrentRecordLocked()
        {
            return Data != null && IsRecordLocked(Data.Units.CurrentIndex);
        }
        
        /// <summary>
        /// Get lock info for a record
        /// </summary>
        public RecordLockInfo GetLockInfo(int recordIndex)
        {
            return _lockedRecords.ContainsKey(recordIndex) ? _lockedRecords[recordIndex] : null;
        }
        
        #endregion
        
        #region Auto-Lock Integration
        
        /// <summary>
        /// Auto-lock record when user starts editing (if LockOnEdit is true)
        /// Call this from component change events
        /// </summary>
        internal async Task<bool> AutoLockIfNeeded()
        {
            if (!_lockOnEdit || _lockMode != LockMode.Automatic)
                return true;
                
            if (IsCurrentRecordLocked())
                return true;  // Already locked
                
            return await LockCurrentRecord();
        }
        
        #endregion
    }
    
    #region Supporting Classes
    
    /// <summary>
    /// Lock mode enum
    /// </summary>
    public enum LockMode
    {
        /// <summary>
        /// No locking
        /// </summary>
        None,
        
        /// <summary>
        /// Lock automatically when user starts editing
        /// </summary>
        Automatic,
        
        /// <summary>
        /// Lock only when explicitly called
        /// </summary>
        Manual,
        
        /// <summary>
        /// Lock immediately when record is retrieved
        /// </summary>
        Immediate
    }
    
    /// <summary>
    /// Information about a locked record
    /// </summary>
    public class RecordLockInfo
    {
        public int RecordIndex { get; set; }
        public DateTime LockTime { get; set; }
        public string LockedBy { get; set; }
        public object Record { get; set; }
        
        public TimeSpan LockDuration => DateTime.Now - LockTime;
    }
    
    #endregion
}
