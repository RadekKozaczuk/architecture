namespace Presentation.Config
{
    static class PresentationConfigContainer
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal static PresentationDebugConfig PresentationDebugConfig;
#endif

        internal static PresentationPlayerConfig PresentationPlayerConfig;
    }
}