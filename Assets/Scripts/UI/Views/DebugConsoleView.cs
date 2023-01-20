using TMPro;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugConsoleView : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField _commandInputField;
        [SerializeField]
        GameObject _debugConsole;

        void ToggleConsole(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.action.triggered)
                return;

            if (_debugConsole.activeSelf)
            {
                _debugConsole.SetActive(false);
            }
            else
            {
                _debugConsole.SetActive(true);
                _commandInputField.ActivateInputField();
            }
        }

        void CallCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            Debug.Log(command);
            DebugConsoleViewModel.ManageReceivedCommand(command);

            ClearInputField();
        }

        void ClearInputField()
        {
            _commandInputField.text = string.Empty;
            _commandInputField.ActivateInputField();
        }
    }
}