using Common.Signals;
using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;
using Shared.Interfaces;
using Shared.SignalProcessing;
using UI.Systems;
using UnityEngine.Scripting;

namespace UI.Controllers
{
    [ReactOnSignals]
    [UsedImplicitly]
    class UIMainController : IInitializable, ICustomUpdate
    {
        [Inject]
        readonly InputController _inputController;

        static bool _uiSceneLoaded;

        [Preserve]
        UIMainController() { }

        public void Initialize() => SignalProcessor.AddReactiveController(this);

        public void CustomUpdate()
        {
            if (!_uiSceneLoaded)
                return;

            _inputController.CustomUpdate();

            InputSystem.CustomUpdate();
        }

        internal static void OnUISceneLoaded() => _uiSceneLoaded = true;

        [React]
        void OnSomeSignal(InventoryChangedSignal _) { }
    }
}