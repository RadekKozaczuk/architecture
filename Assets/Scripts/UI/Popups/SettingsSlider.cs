#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine;

namespace UI.Popups
{
    [ExecuteInEditMode]
    class SettingsSlider : MonoBehaviour
    {
        [SerializeField]
        RectTransform _handleSlideArea;

        [SerializeField]
        RectTransform _handle;

        void Awake()
        {
            SetupNewSize();
        }

        void Update()
        {
            if (!Application.isPlaying)
                SetupNewSize();
        }

        void SetupNewSize()
        {
            float handleSlideAreaHeight = RectTransformUtility.PixelAdjustRect(_handleSlideArea, UISceneReferenceHolder.Canvas).height;
            _handle.sizeDelta = new Vector2(handleSlideAreaHeight, 0);
        }
    }
}