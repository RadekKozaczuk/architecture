using Common.Enums;
using Common.Systems;
using System.Collections;
using UnityEngine;

namespace UI.Views
{
    class JoystickView : MonoBehaviour
    {
        [SerializeField]
        GameObject _joystick;

        void OnEnable()
        {
#if PLATFORM_ANDROID || PLATFORM_IOS
            StartCoroutine("SpawnJoystick");
#endif
        }

        IEnumerator SpawnJoystick()
        {
            while(GameStateSystem.CurrentState != GameState.Gameplay)
                yield return null;

            Instantiate(_joystick, transform);
        }
    }
}
