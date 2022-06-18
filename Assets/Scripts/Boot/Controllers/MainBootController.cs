using Common.Enums;
using Common.Systems;
using Common.Views;
using GameLogic.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Boot.Controllers
{
    /// <summary>
    ///     Contains all the high-level logic that cannot be executed from within <see cref="GameLogic" /> namespace.
    /// </summary>
    [DisallowMultipleComponent]
    public class MainBootController : MonoBehaviour
    {
        [SerializeField]
        EventSystem _eventSystem;

        [SerializeField]
        SceneContext _sceneContext;

        void Start()
        {
            GameStateSystem.RequestStateChange(GameState.MainMenu);

            DontDestroyOnLoad(_eventSystem);
            DontDestroyOnLoad(_sceneContext); // must be preserved for ticks to work

            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);

#if UNITY_EDITOR
            GameObject debugCommands = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            debugCommands.AddComponent<CommonDebugView>();
            debugCommands.AddComponent<GameLogicDebugView>();
            debugCommands.name
                = "DebugCommands"; // had to add it because if set in the line above then it was named "DebugCommands(Clone)" for some reason
            DontDestroyOnLoad(debugCommands);
#endif
        }
    }
}