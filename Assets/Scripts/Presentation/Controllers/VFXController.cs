using System.Collections.Generic;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Views;
using Shared.Interfaces;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    class VFXController : ICustomLateUpdate
    {
        abstract class AbstractVfxLocalData
        {
            internal readonly VFXView Vfx;

            /// <summary>
            /// Time until destruction in seconds.
            /// Nulls means the vfx never dies.
            /// </summary>
            internal float? TimeLeft;

            protected AbstractVfxLocalData(VFXView view, float? timeLeft = null)
            {
                Vfx = view;
                TimeLeft = timeLeft;
            }
        }

        /// <summary>
        /// Used when the VFX is associated with a given position in space.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        class VfxLocalData_Position : AbstractVfxLocalData
        {
            internal readonly int Id;

            internal VfxLocalData_Position(int id, VFXView vfx, float? timeLeft = null)
                : base(vfx, timeLeft) =>
                Id = id;
        }

        static readonly VFXConfig _config;

        // key is VFX instance, value is its lifetime
        // TODO: change to dictionary with id
        // ReSharper disable once IdentifierTypo
        readonly List<VfxLocalData_Position> _vfxsAtPositions = new();

        static int _idCounter;

        [Preserve]
        VFXController() { }

        public void CustomLateUpdate()
        {
            _vfxsAtPositions.RemoveAll(HasExpired);

            bool HasExpired(AbstractVfxLocalData data)
            {
                if (!data.TimeLeft.HasValue)
                    return false;

                data.TimeLeft = data.TimeLeft.Value - Time.deltaTime;

                if (data.TimeLeft.Value > 0)
                    return false;

                Object.Destroy(data.Vfx.gameObject);
                return true;
            }
        }
    }
}