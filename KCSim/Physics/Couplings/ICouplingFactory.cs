using KCSim.Parts.Mechanical;
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

        OneWayPaddleCoupling CreateNewOneWayPaddleCoupling(PaddleWheel paddleWheel, Paddle paddle, Direction direction, string name = "");

        Coupling CreateNewFreeFlowingCoupling(Torqueable input, Torqueable output, string name = "");

        InitialStateCoupling CreateNewInitialStateCoupling(Parts.State.InitialState initialForce, Torqueable output, string name = "");
    }
}
