namespace UI.Systems
{
    static class InputSystem
    {
        internal static bool IsActive;

        internal static void CustomUpdate()
        {
            if (!IsActive)
                return;

            // TODO: use new InputSystem
            /*if (Input.GetKeyDown(KeyCode.Alpha1))
                PresentationViewModel.FirstWolfGo();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                PresentationViewModel.SecondWolfGo();*/

            /*if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();*/
        }
    }
}