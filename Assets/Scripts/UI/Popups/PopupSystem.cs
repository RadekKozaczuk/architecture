#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Common.Enums;
using ControlFlow.SignalProcessing;
using UI.Config;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Popups
{
    /// <summary>
    /// Popup systems automatically calls SetActive(true) on each instantiated popup. It is a good practice to make popup
    /// prefabs inactive so that all the changes done to the prefab during <see cref="AbstractPopup.Initialize" /> call are not visible to the player.
    /// </summary>
    [ReactOnSignals]
    static class PopupSystem
    {
        // ReSharper disable once MemberCanBePrivate.Global
        internal static AbstractPopup? CurrentPopup => Popups.Count > 0 ? Popups[0] : null;

        internal static readonly List<AbstractPopup> Popups = new();

        static Image? _blockingPanel;
        static readonly Queue<(PopupType type, bool blockingPanel, object? parameter)> _scheduledPopups = new();
        static readonly PopupConfig _config;
        static readonly UIConfig _uiConfig;

        public static void ShowPopups(params PopupType[] popupTypes)
        {
            Assert.IsFalse(popupTypes.Length == 0, "You cannot show zero popups.");

            int i = 0;
            if (CurrentPopup == null)
            {
                // instantiate
                ShowPopup(popupTypes[0]);
                i++;
            }
        }

        public static void ShowPopup(PopupType popupType, bool blockingPanel = true)
        {
            if (Popups.Count == 0)
            {
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Disable();
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.PopupActionMap).Enable();
            }

            InstantiatePopup(_config.PopupPrefabs[(int)popupType], blockingPanel);
        }

        public static void CloseCurrentPopup()
        {
            Assert.IsFalse(CurrentPopup == null, "You cannot call CloseCurrentPopup if there is no active popup.");

            CurrentPopup!.Close();
            GameObject popupGo = CurrentPopup.gameObject;
            popupGo.SetActive(false);
            Object.Destroy(popupGo);
            Popups.RemoveAt(0);

            if (_blockingPanel != null)
                if (Popups.Count > 0)
                {
                    _blockingPanel.transform.SetSiblingIndex(Popups.Count - 1);
                }
                else
                {
                    GameObject panelGo = _blockingPanel.gameObject;
                    panelGo.SetActive(false);
                    Object.Destroy(panelGo);
                    _blockingPanel = null;
                }

            ShowNextPopupFromQueueIfAny();
        }

        internal static void SetupPopupElementSize(RectTransform parentRect, RectTransform thisRect)
        {
            float widthToSet = parentRect.rect.width;
            float heightToSet = widthToSet * 0.1f;

            thisRect.sizeDelta = new Vector2(widthToSet, heightToSet);
        }

        static void InstantiatePopup(AbstractPopup prefab, bool blockingPanel)
        {
            if (blockingPanel && _blockingPanel == null)
                _blockingPanel = Object.Instantiate(_config.BlockingPanelPrefab, UISceneReferenceHolder.PopupContainer);

            AbstractPopup popup = Object.Instantiate(prefab, UISceneReferenceHolder.PopupContainer)!;

            popup.Initialize();
            popup.gameObject.SetActive(true);
            Popups.Insert(0, popup);

            // blocking panel should be always second from bottom
            if (_blockingPanel != null)
                _blockingPanel.transform.SetSiblingIndex(Popups.Count - 1);
        }

        static void ShowNextPopupFromQueueIfAny()
        {
            if (_scheduledPopups.Count == 0)
            {
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Enable();
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.PopupActionMap).Disable();
                return;
            }

            (PopupType type, bool blockingPanel, object _) = _scheduledPopups.Dequeue();
            ShowPopup(type, blockingPanel);
        }
    }
}