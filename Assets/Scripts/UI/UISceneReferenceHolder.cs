using UnityEngine;

namespace UI
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER UI scene is fully loaded up.
    /// Use, for example, <see cref="Controllers.UIMainController._uiSceneLoaded" /> to control the execution.
    /// </summary>
    public class UISceneReferenceHolder : MonoBehaviour
    {
        public static Transform PopupContainer;

        [SerializeField]
        Transform _popupContainer;

        void Awake() => PopupContainer = _popupContainer;
    }
}