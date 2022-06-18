using Common.Enums;
using Common.SignalProcessing;
using Common.Systems;
using JetBrains.Annotations;
using UI.Systems;
using Zenject;

namespace UI.Controllers
{
    [UsedImplicitly]
    [ReactOnSignals]
    public class UIMainController : IInitializable, ITickable
    {
        [Inject]
        readonly InputHandler _inputHandler;

        #region Unity life-cycle methods
        public void Initialize()
        {
            SignalProcessor.AddReactiveController(this);
        }

        public void Tick()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            _inputHandler.CustomUpdate();

            InputSystem.CustomUpdate();
        }
        #endregion
    }
}