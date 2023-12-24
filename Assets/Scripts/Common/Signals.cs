using Shared.Systems;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Common
{
    public static class Signals2
    {
        public static readonly ISignals Signals = Architecture.Interception<ISignals>(new SignalsImplementation());
    }

    public class SignalsImplementation : ISignals
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


