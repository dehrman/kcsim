using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public interface ICouplingService
    {
        void RemoveCoupling(Coupling coupling);

        Coupling CreateNewGearCoupling(Gear input, Gear output, string name = "");

        Coupling CreateNewLockedCoupling(Torqueable input, Torqueable output, string name = "");

        BiPaddleCoupling CreateNewBiPaddleCoupling(Paddle paddle1, Paddle paddle2, string name = "");

        Coupling CreateNewOneWayCoupling(Gear input, Gear output, Direction direction, string name = "");

        Coupling CreateNewOneWayPaddleCoupling(Gear input, Gear output, Direction direction, string name = "");

        Coupling CreateNewBidirectionalOpposingCoupling(Torqueable input, Torqueable output, string name = "");

        Coupling CreateNewFreeFlowingCoupling(Torqueable input, Torqueable output, string name = "");

        InitialStateCoupling CreateNewInitialStateCoupling(InitialState initialForce, Torqueable output, string name = "");
    }
}
