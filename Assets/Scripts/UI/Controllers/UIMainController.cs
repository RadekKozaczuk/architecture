using Common.Signals;
using ControlFlow.Interfaces;
using ControlFlow.SignalProcessing;
using JetBrains.Annotations;
using UI.Config;
using UI.Systems;
using UI.Views;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
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
        static readonly UIConfig _uiConfig = null!;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static readonly UIDebugConfig _config;
        static bool _debugConsoleInstantiated;
#endif

        [Preserve]
        UIMainController() { }

        public void CustomUpdate()
        {
            if (!_uiSceneLoaded)
                return;

            InputSystem.CustomUpdate();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (UIData.DebugConsoleView != null)
                UIData.DebugConsoleView.UpdatePlaceholderText();
#endif
        }

        public void CustomFixedUpdate() { }

        public void CustomLateUpdate() { }

        internal static void OnUISceneLoaded()
        {
            _uiSceneLoaded = true;

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && (UNITY_ANDROID || UNITY_IPHONE)
            float height = UISceneReferenceHolder.Canvas.GetComponent<RectTransform>().rect.height;

			GameObject debugMobileButton = new GameObject("DebugConsole");

            debugMobileButton.AddComponent<RectTransform>();
			debugMobileButton.AddComponent<DebugMobileButtonView>();
			debugMobileButton.transform.SetParent(UISceneReferenceHolder.Canvas.transform, false);
            var rectButtonComponent = debugMobileButton.GetComponent<RectTransform>();
            Rect rect = rectButtonComponent.rect;
            rectButtonComponent.transform.SetPositionAndRotation(new Vector3(rect.width / 2, height - rect.height / 2, -1), Quaternion.identity);
#endif
        }

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        internal static void InstantiateDebugConsole()
        {
            if (_debugConsoleInstantiated)
            {
                Object.Destroy(UIData.DebugConsoleView.gameObject);
                _debugConsoleInstantiated = false;

                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Enable();
                return;
            }

            GameObject debugConsole = new GameObject("DebugConsole");
            debugConsole.AddComponent<DebugConsoleView>();
            debugConsole.name = "DebugConsole";
            debugConsole.transform.SetParent(UISceneReferenceHolder.Canvas.transform, false);
            UIData.DebugConsoleView = debugConsole.GetComponent<DebugConsoleView>();
            _debugConsoleInstantiated = true;

            _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Disable();
        }
#endif

		[React]
        [Preserve]
        void OnInventoryChangedSignal(InventoryChangedSignal _) { }
    }
}