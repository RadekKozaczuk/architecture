using Common.Enums;
using UnityEngine;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class WaitingForAllPlayersPopup : AbstractPopupView
    {
        WaitingForAllPlayersPopup()
            : base(PopupType.WaitingForAllPlayers) { }

        internal override void Initialize() { }

        internal override void Close() { }

    }
}