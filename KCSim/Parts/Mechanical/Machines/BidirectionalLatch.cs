using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public class BidirectionalLatch
    {
        public enum InitialState
        {
            Negative,
            Positive
        }

        public readonly Axle Input;
        public readonly Axle Power;
        public readonly Axle OutputAxlePositive;
        public readonly Axle OutputAxleNegative;

        private readonly Relay positiveRelay;
        private readonly Relay negativeRelay;

        // Keep a reference to the initial state for debugging purposes.
        private readonly InitialState initialState;

        private readonly string name;

        public BidirectionalLatch(
            ICouplingService couplingService,
            IRelayFactory relayFactory,
            InitialState initialState = InitialState.Negative,
            string name = "")
        {
            Input = new Axle(name + "; input control axle");
            Power = new Axle(name + "; input power axle");
            OutputAxlePositive = new Axle(name + "; output axle for the positive signal");
            OutputAxleNegative = new Axle(name + "; output axle for the negative signal");

            // Create the relays.
            this.initialState = initialState;
            positiveRelay = relayFactory.CreateNew(
                enableDirection: Direction.Positive,
                inputDirection: Direction.Positive,
                initialState: initialState.Equals(InitialState.Positive) ? Relay.InitialState.Enabled : Relay.InitialState.Disabled,
                name: name + "; positive relay");
            negativeRelay = relayFactory.CreateNew(
                enableDirection: Direction.Negative,
                inputDirection: Direction.Negative,
                initialState: initialState.Equals(InitialState.Negative) ? Relay.InitialState.Enabled : Relay.InitialState.Disabled,
                name: name + "; negative relay");

            // Provide the relays with input power.
            MediumGear inputGear = new MediumGear(name + "; relay input power gear");
            MediumGear reversedInputGear = new MediumGear(name + "; relay reversed input power gear");
            couplingService.CreateNewLockedCoupling(
                Power,
                positiveRelay.InputAxle,
                name: "coupling from input power to positive relay");
            couplingService.CreateNewLockedCoupling(
                Power,
                inputGear,
                name: "coupling from input power to input power gear");
            couplingService.CreateNewGearCoupling(
                inputGear, reversedInputGear,
                name: "coupling from input power gear to reversed input power gear");
            couplingService.CreateNewLockedCoupling(
                reversedInputGear,
                negativeRelay.InputAxle,
                name: "coupling from reversed input power gear to negative relay");

            // Create the diodes that connect the control axle to the control axles of the relays.
            Diode positiveRelayControlDiode = new Diode(couplingService, Direction.Positive, name: name + "; positive relay control diode");
            Diode negativeRelayControlDiode = new Diode(couplingService, Direction.Negative, name: name + "; negative relay control diode");

            // Now couple the control axle to the inputs of the diodes
            couplingService.CreateNewLockedCoupling(Input, positiveRelayControlDiode.InputAxle);
            couplingService.CreateNewLockedCoupling(Input, negativeRelayControlDiode.InputAxle);

            // And couple the outputs of the diodes to the control axles of the relays.
            couplingService.CreateNewLockedCoupling(
                positiveRelayControlDiode.OutputAxle,
                positiveRelay.Enable,
                name: "coupling from positive relay control diode's output to positive relay's control input");
            couplingService.CreateNewLockedCoupling(
                negativeRelayControlDiode.OutputAxle,
                negativeRelay.Enable,
                name: "coupling from negative relay control diode's output to negative relay's control input");

            // Although it serves little LOGICAL purpose, to more accurately simulate reality,
            // couple the paddles of the two relays. In reality, these paddles are locked to a shared axle.
            // Although the control force is only ever coming from one of the two paddles at a time
            // (thanks to the aforementioned diodes), physical irregularities (e.g. paddle wheels ending up
            // in unfortunate positions) can result in destructive opposing forces.
            couplingService.CreateNewBiPaddleCoupling(
                positiveRelay.ArmPaddle,
                negativeRelay.ArmPaddle,
                name: "coupling between the paddles of the positive and negative relays");

            // Finally, connect the outputs of the relays to the output axles of the bidirectional logic switch.
            couplingService.CreateNewLockedCoupling(positiveRelay.OutputAxle, OutputAxlePositive);
            couplingService.CreateNewLockedCoupling(negativeRelay.OutputAxle, OutputAxleNegative);
        }
    }
}
