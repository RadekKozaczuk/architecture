using Shared.AI.Interfaces;
using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// Represents an action that navigates to a position
    /// </summary>
    public interface INavigateAction : IStateMachineAction
    {
        /// <summary>
        /// Current navigation target position
        /// </summary>
        Vector3? TargetPosition { get; }
    }
}