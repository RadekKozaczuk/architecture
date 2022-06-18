using JetBrains.Annotations;

namespace Common.Config
{
    public static class CommonConfigContainer
    {
        [UsedImplicitly]
        public static PlayerConfig PlayerConfig;

        [UsedImplicitly]
        public static DebugConfig DebugConfig;
    }
}