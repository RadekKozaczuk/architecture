#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Mathematics;
using UnityEngine;

namespace DataOriented.ViewModels
{
    public static class DotsViewModel
    {
        public static void MoveRequest(Vector3 movement) => DataOrientedData.MoveRequest = new float3(movement);

        public static void SpawnPlayer() => DataOrientedData.SpawnPlayer = true;
    }
}
