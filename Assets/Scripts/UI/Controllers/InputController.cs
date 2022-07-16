using JetBrains.Annotations;
using Shared.Interfaces;
using UnityEngine.Scripting;

namespace UI.Controllers
{
    /// <summary>
    /// InputHandler parses player's input and combines it into one universal set of commands.
    /// </summary>
    [UsedImplicitly]
    class InputController : ICustomUpdate
    {
        [Preserve]
        InputController() { }

        public void CustomUpdate() { }
    }
}