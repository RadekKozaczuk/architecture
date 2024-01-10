#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Dots.ViewModels
{
    [UsedImplicitly]
    public class DotsViewModel : IInitializable
    {
        [Preserve]
        DotsViewModel() { }

        public void Initialize() { }

        public void MoveRequest(Vector3 movement) => DotsData.MoveRequest = new float3(movement);

        public void SpawnPlayer() => DotsData.SpawnPlayer = true;
    }
}
