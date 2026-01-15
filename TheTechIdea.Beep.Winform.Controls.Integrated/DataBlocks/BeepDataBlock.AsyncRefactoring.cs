using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOWManager.Models;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Async/Await Pattern Refactoring
    /// Replaces .Wait() with proper async/await and adds cancellation support
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Async Master-Detail Filter
        
        /// <summary>
        /// Apply master-detail filter with cancellation support (replaces .Wait() pattern)
        /// </summary>
        public async Task<bool> ApplyMasterDetailFilterAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Data?.IsDirty == true)
                {
                    var args = new BeepDataBlockEventArgs("Data has unsaved changes. Please save or cancel before changing master record.");
                    OnAction?.Invoke(this, args);
                    if (args.Cancel)
                        return false;
                }

                if (MasterRecord == null || Relationships == null || !Relationships.Any())
                {
                    // No master: load all records
                    if (Data != null)
                    {
                        var getMethod = Data.GetType().GetMethod("Get", Type.EmptyTypes);
                        if (getMethod != null)
                        {
                            var task = (Task)getMethod.Invoke(Data, null);
                            await task;
                        }
                    }
                }
                else
                {
                    // Build filters based on master keys
                    var filters = new List<AppFilter>();
                    foreach (var rel in Relationships)
                    {
                        // Get property value from master record
                        var masterValue = GetPropertyValueFromDynamic(MasterRecord, rel.RelatedEntityColumnID)?.ToString();
                        if (!string.IsNullOrEmpty(masterValue))
                        {
                            filters.Add(new AppFilter
                            {
                               FieldName = rel.EntityColumnID,
                                Operator = "=",
                                FilterValue = masterValue
                            });
                        }
                    }
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    // Fetch only related records
                    if (Data != null)
                    {
                        var getWithFilters = Data.GetType().GetMethod("Get", new[] { typeof(List<AppFilter>) });
                        if (getWithFilters != null)
                        {
                            var task = (Task)getWithFilters.Invoke(Data, new object[] { filters });
                            await task;
                        }
                    }
                }
                
                Data?.Units.MoveFirst();
                return true;
            }
            catch (OperationCanceledException)
            {
                ShowInfoMessage("Operation cancelled");
                return false;
            }
            catch (Exception ex)
            {
                HandleError(ex, "ApplyMasterDetailFilterAsync");
                return false;
            }
        }
        
        #endregion
        
        #region Async Record Operations
        
        /// <summary>
        /// Insert record with cancellation support (improved version)
        /// </summary>
        public async Task<bool> InsertRecordAsync(object newRecord, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check for unsaved changes if there are child blocks
                if (ChildBlocks.Count > 0 && !await CheckAndHandleUnsavedChangesRecursiveAsync())
                    return false;
                    
                cancellationToken.ThrowIfCancellationRequested();
                
                if (Data != null && newRecord != null)
                {
                    var args = new UnitofWorkParams();
                    OnPreInsert?.Invoke(this, args);
                    if (args.Cancel)
                    {
                        Status = "Insert cancelled by trigger.";
                        return false;
                    }
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    await Data.InsertAsync((Entity)newRecord);
                    
                    OnPostInsert?.Invoke(this, args);
                    ShowSuccessMessage("Record inserted successfully");
                    Status = "Insert completed.";
                    
                    // After insert, update child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        childBlock.SetMasterRecord(Data?.Units.Current);
                    }
                    
                    return true;
                }
                
                return false;
            }
            catch (OperationCanceledException)
            {
                ShowInfoMessage("Insert cancelled");
                return false;
            }
            catch (Exception ex)
            {
                HandleError(ex, "InsertRecordAsync");
                return false;
            }
        }
        
        /// <summary>
        /// Delete record with cancellation support
        /// </summary>
        public async Task<bool> DeleteCurrentRecordAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Confirm delete
                if (!ShowAlert("Confirm Delete", "Delete current record?", AlertStyle.Question, AlertButtons.YesNo).Equals(DialogResult.Yes))
                {
                    return false;
                }
                
                cancellationToken.ThrowIfCancellationRequested();
                
                if (Data != null && Data.Units.Current != null)
                {
                    await Data.DeleteAsync(Data.Units.Current);
                    ShowSuccessMessage("Record deleted successfully");
                    
                    // After delete, update child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        childBlock.SetMasterRecord(Data?.Units.Current);
                    }
                    
                    return true;
                }
                
                return false;
            }
            catch (OperationCanceledException)
            {
                ShowInfoMessage("Delete cancelled");
                return false;
            }
            catch (Exception ex)
            {
                HandleError(ex, "DeleteCurrentRecordAsync");
                return false;
            }
        }
        
        #endregion
        
        #region Async Commit/Rollback
        
        /// <summary>
        /// Commit with cancellation support (replaces .Wait() at line 441)
        /// </summary>
        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Data?.IsDirty != true)
                    return true;
                    
                cancellationToken.ThrowIfCancellationRequested();
                
                var result = await Data.Commit();
                
                if (result.Flag == Errors.Ok)
                {
                    ShowSuccessMessage("Changes saved successfully");
                    return true;
                }
                else
                {
                    HandleWarning(result.Message, "CommitAsync");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                ShowInfoMessage("Commit cancelled");
                return false;
            }
            catch (Exception ex)
            {
                HandleError(ex, "CommitAsync");
                return false;
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Get property value from dynamic object (replacement for EntityHelper.GetPropertyValue)
        /// </summary>
        private object GetPropertyValueFromDynamic(dynamic obj, string propertyName)
        {
            if (obj == null)
                return null;
                
            try
            {
                var type = obj.GetType();
                var property = type.GetProperty(propertyName);
                return property?.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }
        
        #endregion
    }
}
