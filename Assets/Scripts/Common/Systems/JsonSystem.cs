#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using System;
using System.Collections;

namespace Common.Systems
{
    public static class JsonSystem
    {
        public static IEnumerator ValidateProfileAsync(string accessCode, Action<bool> callback)
        {
            yield return null;
        }
    }
}