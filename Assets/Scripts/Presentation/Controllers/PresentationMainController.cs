#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using ControlFlow.Interfaces;
using ControlFlow.SignalProcessing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.Controllers
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
    [UsedImplicitly]
    class PresentationMainController : IInitializable, ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        [Inject]
        readonly VFXController _vfxController;

        [Inject]
        readonly AudioController _audioController;

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

        public void CustomLateUpdate()
        {
            _vfxController.CustomLateUpdate();
            _audioController.CustomLateUpdate();
        }

        internal static void OnCoreSceneLoaded() => _coreSceneLoaded = true;

        [React]
        [Preserve]
        void OnInventoryChangedSignal()
        {
            Debug.Log("OnInventoryChangedSignal");
        }
    }
}