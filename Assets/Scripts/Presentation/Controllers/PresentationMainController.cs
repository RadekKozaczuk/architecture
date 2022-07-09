using Common.Interfaces;
using JetBrains.Annotations;
using Shared.DependencyInjector.Interfaces;
using Shared.Interfaces;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    public class PresentationMainController : IInitializable, ICustomUpdate, ICustomFixedUpdate, ICustomLateUpdate
    {
        static readonly VFXController _vfxController;

        [Preserve]
        PresentationMainController()
        {
            
        }
        
        public void Initialize() { }

        public void CustomUpdate()
        {
            PresentationSceneReferenceHolder.Enemy.CustomUpdate();
        }
        
        public void CustomFixedUpdate() { }

        public void CustomLateUpdate()
        {
            _vfxController.CustomLateUpdate();
        }

        public static void OnCoreSceneLoaded() { }
    }
}