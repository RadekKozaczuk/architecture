using Presentation.ViewModels;
using UnityEngine;

namespace UI.Systems
{
    public static class InputSystem
    {
        public static bool IsActive;

        internal static void CustomUpdate()
        {
            if (!IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PresentationViewModel.FirstWolfGo();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PresentationViewModel.SecondWolfGo();
            }
            
            /*if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();*/
        }
    }
}