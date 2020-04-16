namespace KCSim.Parts.Mechanical
{
    class BidirectionalLatch
    {
        public readonly Axle InputControl;
        public readonly Axle InputPower;
        public readonly Axle OutputAxlePositive;
        public readonly Axle OutputAxleNegative;

        private readonly Relay positiveRelay;
        private readonly Relay negativeRelay;

        public BidirectionalLatch(
            IRelayFactory relayFactory)
        {
            // Create the relays.
            positiveRelay = relayFactory.CreateNew(isControlPositiveDirection: true, isInputPositiveDirection: true);
            negativeRelay = relayFactory.CreateNew(isControlPositiveDirection: false, isInputPositiveDirection: false);

            // Provide the relays with input power.
            MediumGear inputGear = new MediumGear();
            MediumGear reversedInputGear = new MediumGear();
            Coupling<Axle, Axle>.NewLockedAxleCoupling(InputPower, positiveRelay.InputAxle);
            Coupling<Axle, Gear>.NewLockedAxleToGearCoupling(InputPower, inputGear);
            Coupling<Gear, Gear>.NewGearCoupling(inputGear, reversedInputGear, Physics.CouplingType.BidirectionalOpposing);
            Coupling<Gear, Axle>.NewLockedGearToAxleCoupling(reversedInputGear, negativeRelay.InputAxle);

            // Create the diodes that connect the control axle to the control axles of the relays.
            Diode positiveRelayControlDiode = new Diode(isPositiveDirection: true);
            Diode negativeRelayControlDiode = new Diode(isPositiveDirection: false);

            // Now couple the control axle to the inputs of the diodes
            Coupling<Axle, Axle>.NewLockedAxleCoupling(InputControl, positiveRelayControlDiode.InputAxle);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(InputControl, negativeRelayControlDiode.InputAxle);

            // And couple the outputs of the diodes to the control axles of the relays.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(positiveRelayControlDiode.OutputAxle, positiveRelay.ControlAxle);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(negativeRelayControlDiode.OutputAxle, negativeRelay.ControlAxle);

            // Although it serves little LOGICAL purpose, to more accurately simulate reality,
            // couple the paddles of the two relays. In reality, these paddles are locked to a shared axle.
            // Although the control force is only ever coming from one of the two paddles at a time
            // (thanks to the aforementioned diodes), physical irregularities (e.g. paddle wheels ending up
            // in unfortunate positions) can result in destructive opposing forces.
            new Coupling<Paddle, Paddle>(positiveRelay.ArmPaddle, negativeRelay.ArmPaddle, 1, Physics.CouplingType.BidirectionalSymmetrical);

            // Finally, connect the outputs of the relays to the output axles of the bidirectional logic switch.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(positiveRelay.OutputAxle, OutputAxlePositive);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(negativeRelay.OutputAxle, OutputAxleNegative);
        }
    }
}
