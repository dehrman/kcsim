namespace KCSim.Parts.Mechanical
{
    public class BidirectionalLatch
    {
        public readonly Axle Input;
        public readonly Axle Power;
        public readonly Axle OutputAxlePositive;
        public readonly Axle OutputAxleNegative;

        private readonly Relay positiveRelay;
        private readonly Relay negativeRelay;

        private readonly string name;

        public BidirectionalLatch(
            IRelayFactory relayFactory,
            string name = "")
        {
            Input = new Axle(name + "; input control axle");
            Power = new Axle(name + "; input power axle");
            OutputAxlePositive = new Axle(name + "; output axle for the positive signal");
            OutputAxleNegative = new Axle(name + "; output axle for the negative signal");

        // Create the relays.
        positiveRelay = relayFactory.CreateNew(isControlPositiveDirection: true, isInputPositiveDirection: true, name: name + "; positive relay");
            negativeRelay = relayFactory.CreateNew(isControlPositiveDirection: false, isInputPositiveDirection: false, name: name + "; negative relay");

            // Provide the relays with input power.
            MediumGear inputGear = new MediumGear(name + "; relay input power gear");
            MediumGear reversedInputGear = new MediumGear(name + "; relay reversed input power gear");
            Coupling<Axle, Axle>.NewLockedAxleCoupling(
                Power,
                positiveRelay.InputAxle,
                name: "coupling from input power to positive relay");
            Coupling<Axle, Gear>.NewLockedAxleToGearCoupling(
                Power,
                inputGear,
                name: "coupling from input power to input power gear");
            Coupling<Gear, Gear>.NewGearCoupling(
                inputGear, reversedInputGear,
                Physics.CouplingType.BidirectionalOpposing,
                name: "coupling from input power gear to reversed input power gear");
            Coupling<Gear, Axle>.NewLockedGearToAxleCoupling(
                reversedInputGear,
                negativeRelay.InputAxle,
                name: "coupling from reversed input power gear to negative relay");

            // Create the diodes that connect the control axle to the control axles of the relays.
            Diode positiveRelayControlDiode = new Diode(isPositiveDirection: true, name: name + "; positive relay control diode");
            Diode negativeRelayControlDiode = new Diode(isPositiveDirection: false, name: name + "; negative relay control diode");

            // Now couple the control axle to the inputs of the diodes
            Coupling<Axle, Axle>.NewLockedAxleCoupling(Input, positiveRelayControlDiode.InputAxle);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(Input, negativeRelayControlDiode.InputAxle);

            // And couple the outputs of the diodes to the control axles of the relays.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(
                positiveRelayControlDiode.OutputAxle,
                positiveRelay.Enable,
                name: "coupling from positive relay control diode's output to positive relay's control input");
            Coupling<Axle, Axle>.NewLockedAxleCoupling(
                negativeRelayControlDiode.OutputAxle,
                negativeRelay.Enable,
                name: "coupling from negative relay control diode's output to negative relay's control input");

            // Although it serves little LOGICAL purpose, to more accurately simulate reality,
            // couple the paddles of the two relays. In reality, these paddles are locked to a shared axle.
            // Although the control force is only ever coming from one of the two paddles at a time
            // (thanks to the aforementioned diodes), physical irregularities (e.g. paddle wheels ending up
            // in unfortunate positions) can result in destructive opposing forces.
            new Coupling<Paddle, Paddle>(
                positiveRelay.ArmPaddle,
                negativeRelay.ArmPaddle,
                1,
                Physics.CouplingType.BidirectionalSymmetrical,
                name: "coupling between the paddles of the positive and negative relays");

            // Finally, connect the outputs of the relays to the output axles of the bidirectional logic switch.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(positiveRelay.OutputAxle, OutputAxlePositive);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(negativeRelay.OutputAxle, OutputAxleNegative);
        }
    }
}
