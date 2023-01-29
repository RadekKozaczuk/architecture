#nullable enable
using System.Collections.Generic;
using Common.Enums;
using ControlFlow.SignalProcessing;
using Shared;
using UI.Config;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    /// <summary>
    /// Popup systems automatically calls SetActive(true) on each instantiated popup. It is a good practice to make popup
    /// prefabs inactive so that all the changes done to the prefab during <see cref="AbstractPopupView.Initialize" /> call are not visible to the player.
    /// </summary>
    [ReactOnSignals]
    static class PopupSystem
    {
        internal static AbstractPopupView? CurrentPopup { get; private set; }

        static Image? _blockingPanel;
        static readonly Queue<(PopupType type, bool blockingPanel, object? parameter)> _scheduledPopups = new();
        static readonly PopupConfig _config = null!;
        static readonly UIConfig _uiConfig = null!;

        public static void ShowPopup(params PopupType[] popupTypes)
        {
            Assert.False(popupTypes.Length == 0, "You cannot show zero popups.");

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
            if (CurrentPopup == null)
            {
                AbstractPopupView prefab = _config.PopupPrefabs[(int)popupType];

                Assert.False(prefab == null, "Prefab cannot be null.");

                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Disable();
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.PopupActionMap).Enable();

                AbstractPopupView popup;
                if (blockingPanel)
                {
                    _blockingPanel = Object.Instantiate(_config.BlockingPanelPrefab, UISceneReferenceHolder.PopupContainer);
                    popup = Object.Instantiate(prefab, _blockingPanel.transform)!;
                }
                else
                    popup = Object.Instantiate(prefab, UISceneReferenceHolder.PopupContainer)!;

                popup.Initialize();
                popup.gameObject.SetActive(true);
                CurrentPopup = popup;
            }
            else
                _scheduledPopups.Enqueue((popupType, blockingPanel, null));
        }

        public static void CloseCurrentPopup()
        {
            Assert.False(CurrentPopup == null, "You cannot call CloseCurrentPopup if there is no active popup.");

            CurrentPopup!.Close();
            GameObject popupGo = CurrentPopup.gameObject;
            popupGo.SetActive(false);
            Object.Destroy(popupGo);

            if (_blockingPanel != null)
            {
                GameObject panelGo = _blockingPanel.gameObject;
                panelGo.SetActive(false);
                Object.Destroy(panelGo);
                _blockingPanel = null;
            }

            CurrentPopup = null;
            ShowNextPopupFromQueueIfAny();
        }

        internal static void ClosePopup(AbstractPopupView popup)
        {
            Assert.False(CurrentPopup == null, "You cannot call ClosePopup if there is no active popup.");

            popup.Close();
            GameObject popupGo = popup.gameObject;

            popupGo.SetActive(false);
            Object.Destroy(popupGo);

            if (_blockingPanel != null)
            {
                GameObject go = _blockingPanel.gameObject;
                go.SetActive(false);
                Object.Destroy(go);
            }

            CurrentPopup = null;
            ShowNextPopupFromQueueIfAny();
        }

        internal static PopupType? CurrentPopupType()
        {
            if (CurrentPopup == null)
                return null;

            return CurrentPopup.Type;
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