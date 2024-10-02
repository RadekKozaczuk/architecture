#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections;
using Core.Dtos;
using Core.Enums;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using TMPro;
using UI.Popups;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    class CreateServerPopup : AbstractPopup
    {
        [SerializeField]
        TextMeshProUGUI _info;

        [SerializeField]
        Button _create;

        [SerializeField]
        Button _back;

        string _resultInfo;

        CreateServerPopup()
            : base(PopupType.CreateServer) { }

        internal override void Initialize()
        {
            base.Initialize();

            _create.onClick.AddListener(CreateServer);
            _back.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                PopupSystem.CloseCurrentPopup();
            });

            _info.text = string.Empty;
        }

        async void CreateServer()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            _create.interactable = false;
            _back.interactable = false;

            _resultInfo = "creating server";
            StartCoroutine(ServerInformationStateCoroutine());

            AllocationDto? allocationData = await GameLogicViewModel.GetFreeTestAllocationAsync();
            string allocationId = allocationData?.allocationId ?? string.Empty;
            if (!string.IsNullOrEmpty(allocationId))
            {
                await GameLogicViewModel.CreateServerAsync(allocationId);
                await GameLogicViewModel.WaitUntilAllocationAsync(allocationId);
            }

            _create.interactable = true;
            _back.interactable = true;

            PopupSystem.CloseCurrentPopup();
            //TODO: join to the server without showing list
        }

        IEnumerator ServerInformationStateCoroutine()
        {
            int dot = 0;
            int dotCount = 3;
            string infoText = _resultInfo;

            while (!_create.interactable)
            {
                string dotString = string.Empty;
                for (int i = 0; i < dot; i++)
                    dotString += ".";

                _info.text = infoText + dotString;
                dot = dot >= dotCount ? 0 : dot + 1;
                yield return new WaitForSeconds(0.3f);
            }

            _info.text = string.Empty;
        }
    }
}
