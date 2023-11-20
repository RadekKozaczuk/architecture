using Unity.Mathematics;
using UnityEngine;

namespace Dots.ViewModels
{
    public static class DotsViewModel
    {
        public static void MoveRequest(Vector3 movement) => DotsData.MoveRequest = new float3(movement);

        public static void SpawnPlayer() => DotsData.SpawnPlayer = true;
    }
}
