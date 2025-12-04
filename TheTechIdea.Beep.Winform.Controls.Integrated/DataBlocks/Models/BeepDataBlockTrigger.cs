using System;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Represents a trigger/event handler for BeepDataBlock
    /// Similar to Oracle Forms triggers
    /// </summary>
    public class BeepDataBlockTrigger
    {
        #region Properties
        
        /// <summary>
        /// Unique trigger name (optional, for named triggers)
        /// </summary>
        public string TriggerName { get; set; }
        
        /// <summary>
        /// Type of trigger (WHEN-NEW-RECORD-INSTANCE, PRE-INSERT, etc.)
        /// </summary>
        public TriggerType TriggerType { get; set; }
        
        /// <summary>
        /// Trigger timing classification (Before, After, On, When)
        /// </summary>
        public TriggerTiming Timing { get; set; }
        
        /// <summary>
        /// Trigger scope (Form, Block, Record, Item, Navigation, System)
        /// </summary>
        public TriggerScope Scope { get; set; }
        
        /// <summary>
        /// The handler function to execute
        /// Returns true to continue, false to cancel operation
        /// </summary>
        public Func<TriggerContext, Task<bool>> Handler { get; set; }
        
        /// <summary>
        /// Execution order (lower numbers execute first)
        /// Default: 0 (execution order = registration order)
        /// </summary>
        public int ExecutionOrder { get; set; }
        
        /// <summary>
        /// Whether this trigger is enabled
        /// Disabled triggers are skipped during execution
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// Description of what this trigger does (for documentation)
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// When this trigger was registered
        /// </summary>
        public DateTime RegisteredDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Who registered this trigger (for audit)
        /// </summary>
        public string RegisteredBy { get; set; }
        
        #endregion
        
        #region Statistics (for monitoring)
        
        /// <summary>
        /// Number of times this trigger has been executed
        /// </summary>
        public int ExecutionCount { get; set; }
        
        /// <summary>
        /// Last execution time
        /// </summary>
        public DateTime? LastExecutionTime { get; set; }
        
        /// <summary>
        /// Average execution duration (milliseconds)
        /// </summary>
        public double AverageExecutionMs { get; set; }
        
        /// <summary>
        /// Number of times this trigger cancelled an operation
        /// </summary>
        public int CancellationCount { get; set; }
        
        /// <summary>
        /// Number of times this trigger threw an exception
        /// </summary>
        public int ErrorCount { get; set; }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Execute the trigger handler with statistics tracking
        /// </summary>
        public async Task<bool> ExecuteAsync(TriggerContext context)
        {
            if (!IsEnabled)
                return true;
                
            var startTime = DateTime.Now;
            ExecutionCount++;
            
            try
            {
                var result = await Handler(context);
                
                // Update statistics
                LastExecutionTime = DateTime.Now;
                var durationMs = (DateTime.Now - startTime).TotalMilliseconds;
                AverageExecutionMs = ((AverageExecutionMs * (ExecutionCount - 1)) + durationMs) / ExecutionCount;
                
                if (!result || context.Cancel)
                {
                    CancellationCount++;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                ErrorCount++;
                LastExecutionTime = DateTime.Now;
                
                // Re-throw to be handled by trigger execution engine
                throw new TriggerExecutionException(
                    $"Error executing trigger '{TriggerName ?? TriggerType.ToString()}': {ex.Message}",
                    ex,
                    this,
                    context);
            }
        }
        
        /// <summary>
        /// Get timing from trigger type
        /// </summary>
        public static TriggerTiming GetTimingFromType(TriggerType type)
        {
            var typeString = type.ToString();
            
            if (typeString.StartsWith("Pre"))
                return TriggerTiming.Before;
            if (typeString.StartsWith("Post"))
                return TriggerTiming.After;
            if (typeString.StartsWith("When"))
                return TriggerTiming.When;
                
            return TriggerTiming.On;
        }
        
        /// <summary>
        /// Get scope from trigger type
        /// </summary>
        public static TriggerScope GetScopeFromType(TriggerType type)
        {
            int typeValue = (int)type;
            
            if (typeValue < 100)
                return TriggerScope.Form;
            if (typeValue < 200)
                return TriggerScope.Block;
            if (typeValue < 300)
                return TriggerScope.Record;
            if (typeValue < 400)
                return TriggerScope.Item;
            if (typeValue < 500)
                return TriggerScope.Navigation;
                
            return TriggerScope.System;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Exception thrown when trigger execution fails
    /// </summary>
    public class TriggerExecutionException : Exception
    {
        public BeepDataBlockTrigger Trigger { get; }
        public TriggerContext Context { get; }
        
        public TriggerExecutionException(string message, Exception innerException, 
            BeepDataBlockTrigger trigger, TriggerContext context)
            : base(message, innerException)
        {
            Trigger = trigger;
            Context = context;
        }
    }
}

