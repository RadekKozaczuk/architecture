using Sirenix.OdinInspector;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Config
{
    [CreateAssetMenu(fileName = "PopupConfig", menuName = "Config/UI/PopupConfig")]
    class PopupConfig : ScriptableObject
    {
        [InfoBox("Order should match PopupType enum.", InfoMessageType.None)]
        [SerializeField]
        internal AbstractPopupView[] PopupPrefabs;

        [SerializeField]
        internal Image BlockingPanelPrefab;
    }
}