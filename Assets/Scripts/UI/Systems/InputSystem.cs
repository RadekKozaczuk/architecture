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

            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
    }
}