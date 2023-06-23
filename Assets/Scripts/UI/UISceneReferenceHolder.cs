using System.Net.Mime;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER UI scene is fully loaded up.
    /// Use, for example, <see cref="Controllers.UIMainController._uiSceneLoaded" /> to control the execution.
    /// </summary>
    class UISceneReferenceHolder : MonoBehaviour
    {
        internal static Transform PopupContainer;
        internal static Canvas Canvas;

        [SerializeField]
        Transform _popupContainer;

        [SerializeField]
        Canvas _canvas;

        public UnityEngine.UI.Text RelayCode;

        public static UnityEngine.UI.Text RelayCodeStatic;

        void Awake()
        {
            PopupContainer = _popupContainer;
            Canvas = _canvas;
            RelayCodeStatic = RelayCode;
        }
    }
}