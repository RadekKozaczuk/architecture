using UnityEngine;

namespace UI
{
    public class UISceneReferenceHolder : MonoBehaviour
    {
        public static Transform PopupContainer;

        [SerializeField]
        Transform _popupContainer;

        void Awake()
        {
            PopupContainer = _popupContainer;
        }
    }
}