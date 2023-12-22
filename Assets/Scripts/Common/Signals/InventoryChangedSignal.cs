#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using Shared;

namespace Common.Signals
{
    /// <summary>
    /// Universal signal indicating that something has changed in the inventory.
    /// An item was added, removed, moved to a different slot, or its quantity has changed.
    /// </summary>
    public sealed class InventoryChangedSignal : AbstractSignal { }
}