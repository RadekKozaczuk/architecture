using Common.Interfaces;
using JetBrains.Annotations;
using Zenject;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    public class PresentationMainController : IInitializable, ICustomUpdate, ICustomLateUpdate
    {
        [Inject]
        readonly VFXController _vfxController;

        public void Initialize() { }

        public void CustomUpdate() { }

        public void CustomLateUpdate()
        {
            _vfxController.CustomLateUpdate();
        }

        public static void OnCoreSceneLoaded() { }
    }
}