using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;
using Shared.Interfaces;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    class PresentationMainController : IInitializable, ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        [Inject]
        readonly VFXController _vfxController;

        static bool _coreSceneLoaded;

        [Preserve]
        PresentationMainController() { }

        public void Initialize() { }

        public void CustomFixedUpdate() { }

        public void CustomUpdate()
        {
            if (!_coreSceneLoaded)
                return;
        }

        public void CustomLateUpdate() => _vfxController.CustomLateUpdate();

        internal static void OnCoreSceneLoaded() => _coreSceneLoaded = true;
    }
}