using JetBrains.Annotations;
using Shared.DependencyInjector.Interfaces;
using Shared.Interfaces;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    class PresentationMainController : IInitializable, ICustomUpdate, ICustomFixedUpdate, ICustomLateUpdate
    {
        static readonly VFXController _vfxController;

        static bool _coreSceneLoaded;

        [Preserve]
        PresentationMainController() { }

        public void Initialize() { }

        public void CustomUpdate()
        {
            if (!_coreSceneLoaded)
                return;

            PresentationSceneReferenceHolder.Wolf1.CustomUpdate();
            PresentationSceneReferenceHolder.Wolf2.CustomUpdate();
        }

        public void CustomFixedUpdate() { }

        public void CustomLateUpdate() => _vfxController.CustomLateUpdate();

        internal static void OnCoreSceneLoaded() => _coreSceneLoaded = true;
    }
}