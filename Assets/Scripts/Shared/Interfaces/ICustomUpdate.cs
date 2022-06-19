namespace Shared.Interfaces
{
    /// <summary>
    /// Indicates that this class is synchronized with Unity life-cycle. The synchronization happens in main controllers.
    /// </summary>
    public interface ICustomUpdate
    {
        void CustomUpdate();
    }
}