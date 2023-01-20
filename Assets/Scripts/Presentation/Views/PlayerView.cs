using Presentation.Config;
using Shared;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PlayerView : MonoBehaviour
    {
        [SerializeField]
        CharacterController _characterController;

        static readonly PresentationPlayerConfig _playerConfig;

        internal void Move(Vector2 v)
        {
            Assert.True(v.sqrMagnitude < Vector3.one.sqrMagnitude, "Vector v must be normalized.");

            Vector3 movement = new Vector3(v.x, 0, v.y) * _playerConfig.PlayerSpeed;
            _characterController.SimpleMove(movement * Time.deltaTime);
        }
    }
}