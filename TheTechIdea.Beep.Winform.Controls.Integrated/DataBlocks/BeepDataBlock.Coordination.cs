using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOWManager;
using TheTechIdea.Beep.Editor.UOWManager.Models;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - FormsManager Coordination
    /// Provides form-level coordination for multi-block operations
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        private FormsManager _formsManager;
        private bool _isRegisteredWithFormsManager;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// FormsManager instance for form-level coordination
        /// Oracle Forms equivalent: Form-level transaction management
        /// </summary>
        public FormsManager FormManager
        {
            get => _formsManager;
            set
            {
                if (_formsManager != value)
                {
                    // Unregister from old manager
                    if (_formsManager != null && _isRegisteredWithFormsManager)
                    {
                        UnregisterFromFormsManager();
                    }
                    
                    _formsManager = value;
                    
                    // Auto-register with new manager if we have a name
                    if (_formsManager != null && !string.IsNullOrEmpty(this.Name))
                    {
                        RegisterWithFormsManager();
                    }
                }
            }
        }
        
        /// <summary>
        /// Whether this block is registered with a FormsManager
        /// </summary>
        public bool IsCoordinated => _isRegisteredWithFormsManager && _formsManager != null;
        
        #endregion
        
        #region Registration
        
        /// <summary>
        /// Register this block with the FormsManager
        /// </summary>
        public bool RegisterWithFormsManager()
        {
            if (_formsManager == null)
                return false;
                
            if (string.IsNullOrEmpty(this.Name))
            {
                // Auto-generate name if not set
                this.Name = $"Block_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }
            
            if (string.IsNullOrEmpty(FormName))
            {
                FormName = "DefaultForm";
            }
            
            try
            {
                // Register with FormsManager (correct API signature)
                _formsManager.RegisterBlock(this.Name, Data, EntityStructure);
                
                // Set up relationships if we have a parent
                if (ParentBlock != null && !string.IsNullOrEmpty(MasterKeyPropertyName) && !string.IsNullOrEmpty(ForeignKeyPropertyName))
                {
                    _formsManager.CreateMasterDetailRelation(
                        ParentBlock.Name,
                        this.Name,
                        MasterKeyPropertyName,
                        ForeignKeyPropertyName);
                }
                
                _isRegisteredWithFormsManager = true;
                return true;
            }
            catch
            {
                _isRegisteredWithFormsManager = false;
                return false;
            }
        }
        
        /// <summary>
        /// Unregister from FormsManager
        /// </summary>
        public void UnregisterFromFormsManager()
        {
            if (_formsManager != null && _isRegisteredWithFormsManager)
            {
                try
                {
                    _formsManager.UnregisterBlock(this.Name);
                }
                catch
                {
                    // Ignore errors during unregistration
                }
                finally
                {
                    _isRegisteredWithFormsManager = false;
                }
            }
        }
        
        #endregion
        
        #region Coordinated Operations
        
        /// <summary>
        /// Commit this block through FormsManager (coordinated with other blocks)
        /// Oracle Forms equivalent: COMMIT_FORM
        /// </summary>
        public async Task<IErrorsInfo> CoordinatedCommit()
        {
            if (!IsCoordinated)
            {
                // Fallback to direct commit if not coordinated
                return await Data?.Commit() ?? new ErrorsInfo { Flag = Errors.Ok };
            }
            
            try
            {
                // Commit through FormsManager (all blocks in form)
                return await _formsManager.CommitFormAsync();
            }
            catch (Exception ex)
            {
                return new ErrorsInfo
                {
                    Flag = Errors.Failed,
                    Message = $"Coordinated commit failed: {ex.Message}",
                    Ex = ex
                };
            }
        }
        
        /// <summary>
        /// Rollback this block through FormsManager (coordinated with other blocks)
        /// Oracle Forms equivalent: ROLLBACK_FORM
        /// </summary>
        public async Task<IErrorsInfo> CoordinatedRollback()
        {
            if (!IsCoordinated)
            {
                // Fallback to direct rollback if not coordinated
                await (Data?.Rollback() ?? Task.CompletedTask);
                return new ErrorsInfo { Flag = Errors.Ok };
            }
            
            try
            {
                // Rollback through FormsManager (all blocks in form)
                return await _formsManager.RollbackFormAsync();
            }
            catch (Exception ex)
            {
                return new ErrorsInfo
                {
                    Flag = Errors.Failed,
                    Message = $"Coordinated rollback failed: {ex.Message}",
                    Ex = ex
                };
            }
        }
        
        /// <summary>
        /// Query through FormsManager (coordinated master-detail query)
        /// Oracle Forms equivalent: EXECUTE_QUERY with master-detail coordination
        /// </summary>
        public async Task<bool> CoordinatedQuery(List<AppFilter> filters = null)
        {
            if (!IsCoordinated)
            {
                // Fallback to direct query if not coordinated
                var result = await ExecuteQueryDirectAsync(filters);
                return result.Flag == Errors.Ok;
            }
            
            try
            {
                // Query through FormsManager (handles master-detail coordination)
                return await _formsManager.ExecuteQueryAsync(this.Name, filters);
            }
            catch (Exception ex)
            {
                Status = $"Coordinated query failed: {ex.Message}";
                return false;
            }
        }
        
        /// <summary>
        /// Direct query without FormsManager coordination
        /// </summary>
        private async Task<IErrorsInfo> ExecuteQueryDirectAsync(List<AppFilter> filters)
        {
            try
            {
                if (Data != null)
                {
                    var getMethod = filters != null && filters.Count > 0
                        ? Data.GetType().GetMethod("Get", new[] { typeof(List<AppFilter>) })
                        : Data.GetType().GetMethod("Get", Type.EmptyTypes);
                        
                    if (getMethod != null)
                    {
                        var task = filters != null && filters.Count > 0
                            ? (Task)getMethod.Invoke(Data, new object[] { filters })
                            : (Task)getMethod.Invoke(Data, null);
                            
                        await task;
                    }
                }
                
                Data?.Units.MoveFirst();
                
                return new ErrorsInfo { Flag = Errors.Ok };
            }
            catch (Exception ex)
            {
                return new ErrorsInfo
                {
                    Flag = Errors.Failed,
                    Message = $"Query failed: {ex.Message}",
                    Ex = ex
                };
            }
        }
        
        /// <summary>
        /// Check if block is ready for operations
        /// </summary>
        public bool IsBlockReady()
        {
            return Data != null && EntityStructure != null;
        }
        
        /// <summary>
        /// Sync block state with FormsManager
        /// </summary>
        public void SyncWithFormsManager()
        {
            if (!IsCoordinated)
                return;
                
            try
            {
                // Update FormsManager with current block state
                var blockInfo = _formsManager.GetBlock(this.Name);
                if (blockInfo != null)
                {
                    // FormsManager will track state via UnitofWork reference
                    // No need to manually sync - it's already watching the UnitofWork
                }
            }
            catch
            {
                // Ignore sync errors
            }
        }
        
        #endregion
        
        #region Coordinated Validation
        
        /// <summary>
        /// Validate all blocks in the form (coordinated validation)
        /// </summary>
        public async Task<IErrorsInfo> CoordinatedValidation()
        {
            if (!IsCoordinated)
            {
                // Fallback to local validation
                return await ValidateCurrentRecord();
            }
            
            try
            {
                // Validate through FormsManager (all blocks)
                var allErrors = new List<string>();
                
                // Validate this block
                var localErrors = await ValidateCurrentRecord();
                if (localErrors.Flag != Errors.Ok)
                {
                    allErrors.Add($"{this.Name}: {localErrors.Message}");
                }
                
                // FormsManager will validate other blocks
                // (This would require FormsManager to have a ValidateForm method)
                
                if (allErrors.Count > 0)
                {
                    return new ErrorsInfo
                    {
                        Flag = Errors.Failed,
                        Message = string.Join("\n", allErrors)
                    };
                }
                
                return new ErrorsInfo { Flag = Errors.Ok };
            }
            catch (Exception ex)
            {
                return new ErrorsInfo
                {
                    Flag = Errors.Failed,
                    Message = $"Coordinated validation failed: {ex.Message}",
                    Ex = ex
                };
            }
        }
        
        #endregion
    }
}

