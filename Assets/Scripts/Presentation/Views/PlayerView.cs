using Presentation.Config;
using UnityEngine;
using UnityEngine.Assertions;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class PlayerView : MonoBehaviour
    {
        static readonly PlayerConfig _playerConfig;

        internal void Move(Vector2 v)
        {
            Assert.IsTrue(v.sqrMagnitude < Vector3.one.sqrMagnitude, "Vector v must be normalized.");

            Vector3 movement = new Vector3(v.x, 0, v.y) * _playerConfig.PlayerSpeed;
            transform.Translate(movement * Time.deltaTime);
        }
    }
}