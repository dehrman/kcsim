using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.State;

namespace KCSim.Physics.Couplings
{
    public interface ICouplingFactory
    {
        Coupling CreateNewGearCoupling(Gear input, Gear output, string name = "");

        Coupling CreateNewLockedCoupling(Torqueable input, Torqueable output, string name = "");

        BiPaddleCoupling CreateNewBiPaddleCoupling(Torqueable input, Torqueable output, string name = "");

        Coupling CreateNewBidirectionalOpposingCoupling(Torqueable input, Torqueable output, string name = "");

        Coupling CreateNewOneWayCoupling(Gear input, Gear output, Direction direction, string name = "");

        Coupling CreateNewOneWayPaddleCoupling(Gear input, Gear output, Direction direction, string name = "");

        Coupling CreateNewFreeFlowingCoupling(Torqueable input, Torqueable output, string name = "");

        InitialStateCoupling CreateNewInitialStateCoupling(InitialState initialForce, Torqueable output, string name = "");
    }
}
