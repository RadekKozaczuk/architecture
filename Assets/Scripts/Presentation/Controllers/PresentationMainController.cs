using Common.Interfaces;
using Common.SignalProcessing;
using JetBrains.Annotations;
using Zenject;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    [ReactOnSignals]
    public class PresentationMainController : IInitializable, ICustomUpdate, ICustomLateUpdate
    {
        [Inject]
        readonly VFXController _vfxController;

        public void Initialize()
        {
            SignalProcessor.AddReactiveController(this);
        }

        public void CustomUpdate() { }

        public void CustomLateUpdate()
        {
            _vfxController.CustomLateUpdate();
        }

        public static void OnCoreSceneLoaded() { }
    }
}