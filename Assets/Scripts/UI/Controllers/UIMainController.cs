using Common.Signals;
using ControlFlow.Interfaces;
using ControlFlow.SignalProcessing;
using JetBrains.Annotations;
using UI.Config;
using UI.Systems;
using UI.Views;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace UI.Controllers
{
    /// <summary>
    /// Main controller serves 3 distinct roles:
    /// 1) It allows you to order the signal execution order. So instead of reacting separately in two different controllers you can react in main controller
    /// and call adequate methods.
    /// 2) Serves as a 'default' controller. When you don't know where to put some logic or the logic is too small for its own controller you can put it into
    /// the main controller.
    /// 3) Reduces the size of the view model. We could move all (late/fixed)update calls to view model but as the application grows it would lead to view
    /// model doing to much stuff.
    /// For better code readability all controllers meant to interact with this controller should implement
    /// <see cref="ICustomLateUpdate" /> interface.
    /// </summary>
    [ReactOnSignals]
    [UsedImplicitly]
    class UIMainController : ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        static bool _uiSceneLoaded;
        static readonly UIDebugConfig _config;
        static bool _debugConsoleInstantiated;
        static readonly UIConfig _uiConfig = null!;

        [Preserve]
        UIMainController() { }

        public void CustomUpdate()
        {
            if (!_uiSceneLoaded)
                return;

            InputSystem.CustomUpdate();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UIData.DebugConsoleView?.UpdatePlaceholderText();
#endif
        }

        public void CustomFixedUpdate() { }

        public void CustomLateUpdate() { }

        internal static void OnUISceneLoaded()
        {
            _uiSceneLoaded = true;

            //#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && (UNITY_ANDROID || UNITY_IPHONE)
            float height = UISceneReferenceHolder.Canvas.GetComponent<RectTransform>().rect.height;

            DebugMobileConsoleView debugMobileConsole = Object.Instantiate(_config.MobileConsolePrefab, Vector3.zero, Quaternion.identity, UISceneReferenceHolder.Canvas.transform);
            debugMobileConsole.name = "DebugMobileConsole";

            DebugMobileButtonView debugMobileButton = Object.Instantiate(_config.MobileButtonPrefab, Vector3.zero, Quaternion.identity, UISceneReferenceHolder.Canvas.transform);
            debugMobileButton.name = "DebugMobileButton";
            var rectButtonComponent = debugMobileButton.GetComponent<RectTransform>();
            Rect rect = rectButtonComponent.rect;
            rectButtonComponent.transform.SetPositionAndRotation(new Vector3(rect.width / 2, height - rect.height / 2, -1), Quaternion.identity);
            //#endif
        }

        internal static void InstantiateDebugConsole()
        {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && UNITY_STANDALONE_WIN
            if (_debugConsoleInstantiated)
            {
                Object.Destroy(UIData.DebugConsoleView.gameObject);
                _debugConsoleInstantiated = false;

                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Enable();
                return;
            }

            DebugConsoleView debugConsole = Object.Instantiate(_config.ConsolePrefab, Vector3.zero, Quaternion.identity);
            debugConsole.name = "DebugConsole";
            debugConsole.transform.SetParent(UISceneReferenceHolder.Canvas.transform, false);
            var debugConsoleComponent = debugConsole.GetComponent<RectTransform>();
            debugConsoleComponent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, debugConsoleComponent.rect.height);
            _debugConsoleInstantiated = true;

            _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Disable();
#endif
        }

        [React]
        [Preserve]
        void OnInventoryChangedSignal(InventoryChangedSignal _) { }
    }
}