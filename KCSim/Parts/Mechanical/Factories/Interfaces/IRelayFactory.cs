using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public interface IRelayFactory
    {
        Relay CreateNew(Direction enableDirection, Direction inputDirection, string name = "");
    }
}
