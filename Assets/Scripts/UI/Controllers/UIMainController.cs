using Common.Enums;
using Common.Systems;
using JetBrains.Annotations;
using Shared.DependencyInjector;
using Shared.DependencyInjector.Interfaces;
using Shared.Interfaces;
using UI.Systems;
using UnityEngine.Scripting;

namespace UI.Controllers
{
    [UsedImplicitly]
    public class UIMainController : IInitializable, ICustomUpdate
    {
        [Inject]
        readonly InputController _inputController;

        [Preserve]
        UIMainController() { }

        public void Initialize() { }

        public void CustomUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            _inputController.CustomUpdate();

            InputSystem.CustomUpdate();
        }
    }
}