using KCSim.Parts.Logical;

namespace KCSim
{
    public interface IStateMonitor
    {
        T RegisterGate<T>(T gate) where T : Gate;
    }
}
