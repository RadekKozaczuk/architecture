using Common.Enums;
using Common.Systems;
#if UNITY_EDITOR
using Common.Views;
using GameLogic.Views;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Shared.Systems;

namespace Boot.Controllers
{
    /// <summary>
    /// Contains all the high-level logic that cannot be executed from within <see cref="GameLogic" /> namespace.
    /// </summary>
    [DisallowMultipleComponent]
    class MainBootController : MonoBehaviour
    {
        [SerializeField]
        EventSystem _eventSystem;

        [SerializeField]
        SceneContext _sceneContext;

        void Start()
        {
            ConfigInjector.Run(new [] {"Boot", "Common", "GameLogic", "Presentation", "UI"});

            GameStateSystem.RequestStateChange(GameState.MainMenu);

            DontDestroyOnLoad(_eventSystem);
            DontDestroyOnLoad(_sceneContext); // must be preserved for ticks to work
            DontDestroyOnLoad(this);

            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);

#if UNITY_EDITOR
            GameObject debugCommands = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            debugCommands.AddComponent<CommonDebugView>();
            debugCommands.AddComponent<GameLogicDebugView>();
            debugCommands.name = "DebugCommands"; // had to add it because if set in the line above, it was named "DebugCommands(Clone)" for some reason
            DontDestroyOnLoad(debugCommands);
#endif
        }
    }
}