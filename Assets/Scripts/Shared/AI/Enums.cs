namespace Shared.AI
{
    /// <summary>
    /// States in which a single action can be at any given time
    /// </summary>
    public enum StateMachineActionState
    {
        /// <summary>
        /// Action is scheduled for execution, not running
        /// </summary>
        Awaiting,

        /// <summary>
        /// Action is initializing
        /// </summary>
        Starting,

        /// <summary>
        /// Action is in progress
        /// </summary>
        InProgress,

        /// <summary>
        /// Action is finishing
        /// </summary>
        Finishing,

        /// <summary>
        /// Action finished successfully
        /// </summary>
        Succeeded,

        /// <summary>
        /// Action finished with failure
        /// </summary>
        Failed
    }
}