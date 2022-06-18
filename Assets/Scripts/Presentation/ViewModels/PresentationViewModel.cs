using Common.Enums;
using JetBrains.Annotations;
using Presentation.Controllers;
using Zenject;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public class PresentationViewModel
    {
        [Inject]
        readonly AudioController _audioController;

        public void PlayMusic(Music music)
        {
            _audioController.Play(music);
        }
    }
}