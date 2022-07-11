using JetBrains.Annotations;
using Shared.Interfaces;
using UnityEngine.Scripting;

namespace UI.Controllers
{
    /// <summary>
    /// InputHandler parses player's input and combines it into one universal set of commands.
    /// </summary>
    [UsedImplicitly]
    class InputHandler : ICustomUpdate
    {
        [Preserve]
        InputHandler() { }

        public void CustomUpdate() { }
    }
}