using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public interface ICouplingService
    {
        void RemoveCoupling(Coupling coupling);

        Coupling CreateNewGearCoupling(Gear input, Gear output, string name = "");

        Coupling CreateNewLockedCoupling(Torqueable input, Torqueable output, string name = "");

        Coupling CreateNewOneWayCoupling(Gear input, Gear output, Direction direction, string name = "");

        Coupling CreateNewBidirectionalOpposingCoupling(Torqueable input, Torqueable output, string name = "");

        Coupling CreateNewFreeFlowingCoupling(Torqueable input, Torqueable output, string name = "");

        Coupling CreateNewInitialStateCoupling(Force initialForce, Torqueable output, string name = "");
    }
}
