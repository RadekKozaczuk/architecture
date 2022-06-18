using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Config;
using Common.Enums;
using UnityEngine;

namespace Common.Systems
{
    public delegate void StateChangeRequestEvent(GameState current, GameState requested, string[] args);

    public delegate bool IsTransitioningEvent();

    public static class GameStateSystem
    {
        public static event StateChangeRequestEvent StateChangeRequestEvent;
        public static event IsTransitioningEvent IsTransitioningEvent;

        public static GameState CurrentState { get; private set; } = GameState.Booting;
        public static GameState PreviousState { get; private set; } = GameState.Booting;

        static GameState? _scheduledState;
        static readonly DebugConfig _config;

        public static void CustomUpdate()
        {
            if (_scheduledState == null)
                return;

            // ReSharper disable once PossibleNullReferenceException
            if (IsTransitioningEvent())
                return;

            RequestStateChange(_scheduledState.Value);
            _scheduledState = null;
        }

        /// <summary>
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public static void RequestStateChange(GameState state, params string[] args)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_config.LogRequestedStateChange)
                Debug.Log($"DEBUG LOG: Requested game state change from {CurrentState} to {state}");

            // assertion
            string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            if (new List<string> {"Boot", "Common", "GameLogic", "UI"}.All(x => x != assemblyName))
                throw new Exception(
                    "Calling assembly violates the chosen architectural pattern. "
                    + "RequestStateChange should be called only from 'Boot', 'Common, 'GameLogic' or 'UI' assemblies. "
                    + $"Was called from {assemblyName}.");
#endif

            // ReSharper disable once PossibleNullReferenceException
            StateChangeRequestEvent.Invoke(CurrentState, state, args);
        }

        public static void ScheduleStateChange(GameState state) => _scheduledState = state;

        /// <summary>
        /// Only allowed to be called from <see cref="Boot" /> assembly.
        /// </summary>
        public static void ChangeState(GameState newCurrent)
        {
            // assertion
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            if (assemblyName != "Boot")
                throw new Exception(
                    "Calling assembly violates the chosen architectural pattern. "
                    + "ChangeState method should be called only from 'Boot' assembly. "
                    + $"Was called from {assemblyName}.");
#endif

            PreviousState = CurrentState;
            CurrentState = newCurrent;
        }
    }
}