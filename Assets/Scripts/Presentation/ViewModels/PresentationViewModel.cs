using Common.Enums;
using JetBrains.Annotations;
using Presentation.Controllers;
using Shared.DependencyInjector;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public partial class PresentationViewModel
    {
        [Inject]
        readonly AudioController _audioController;

        public void PlayMusic(Music music)
        {
            _audioController.Play(music);
        }

        public static void OnCoreSceneLoaded()
        {
            
        }
    }
}