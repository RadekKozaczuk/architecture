using Shared.Systems;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Common
{
    public static class Signals2
    {
        public static void InventoryChanged() => _signals.InventoryChanged();
        public static void HpChanged(int a) => _signals.HpChanged(a);

        static readonly ISignals _signals = Architecture.Interception<ISignals>(new SignalsImplementation());
    }

    class SignalsImplementation : ISignals
    {
        public void InventoryChanged() { }

        public void HpChanged(int a) { }
    }

    public interface ISignals
    {
        void InventoryChanged();
        void HpChanged(int a);
    }
}


