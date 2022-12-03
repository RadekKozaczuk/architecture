using Shared;

namespace Common.Signals
{
    /// <summary>
    /// Universal signal indicating that something has changed in the inventory.
    /// An item was added, removed, moved to a different slot, or its quantity has changed.
    /// </summary>
    public sealed class InventoryChangedSignal : AbstractSignal { }
}