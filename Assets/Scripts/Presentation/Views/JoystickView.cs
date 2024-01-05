using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Presentation
{
    public class JoystickView : MonoBehaviour
    {
       void Awake()
       {
#if PLATFORM_ANDROID || PLATFORM_IOS
            this.gameObject.SetActive(true);
#else
            this.gameObject.SetActive(false);
#endif
        }
    }
}
