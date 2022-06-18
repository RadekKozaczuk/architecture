using System;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    abstract class AbstractObjectView : MonoBehaviour
    {
        /// <summary>
        /// Global unique object id.
        /// </summary>
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal int Id
        {
            get
            {
                if (_id == int.MinValue)
                    throw new Exception($"Tried to retrieve an unassigned field from {gameObject.name}.");

                return _id;
            }
            set
            {
                if (_id == int.MinValue)
                    _id = value;
                else
                    throw new Exception("Id can only be set once. Consequent assignments are invalid.");
            }
        }

        int _id = int.MinValue;
#else
        internal int Id;
#endif

        /// <summary>
        /// The active state of the View, setting this also enables/disables the GameObject
        /// </summary>
        internal bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                gameObject.SetActive(_isActive);
            }
        }

        bool _isActive;
    }
}