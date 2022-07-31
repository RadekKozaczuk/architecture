using Common.Signals;
using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using Shared.Interfaces;
using Shared.SignalProcessing;
using UI.Systems;
using UnityEngine.Scripting;

namespace UI.Controllers
{
    [ReactOnSignals]
    [UsedImplicitly]
    class UIMainController : ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate 
    {
        [Inject]
        readonly InputController _inputController;

        static bool _uiSceneLoaded;

        [Preserve]
        UIMainController() { }

        public void CustomFixedUpdate() { }
        
        public void CustomUpdate()
        {
            if (!_uiSceneLoaded)
                return;

            _inputController.CustomUpdate();

            InputSystem.CustomUpdate();
        }

        public void CustomLateUpdate() { }

        internal static void OnUISceneLoaded() => _uiSceneLoaded = true;

        [React]
        void OnSomeSignal(InventoryChangedSignal _) { }
    }
}