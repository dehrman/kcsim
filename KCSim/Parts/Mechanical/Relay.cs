using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public class Relay
    {
        public readonly Axle ControlAxle;
        public readonly Axle InputAxle;
        public readonly SmallGear InputGear;
        public readonly SmallGear OutputGear;
        public readonly Axle OutputAxle;

        // functional properties of the part
        private readonly bool isControlPositiveDirection;
        private readonly bool isInputPositiveDirection;

        // conditional couplings that are created and destroyed as the relay is enabled and disabled respectively
        private Coupling<SmallGear, SmallGear>? inputToConnectorCoupling;
        private Coupling<SmallGear, SmallGear>? connectorToOutputCoupling;

        // fixed couplings
        private readonly Coupling<Axle, Paddle> controlAxleToPaddleCoupling;
        private readonly Coupling<Paddle, Paddle> paddleToPaddleCoupling;
        private readonly Coupling<Paddle, Axle> paddleToArmFulcrumCoupling;
        private readonly Coupling<Axle, Gear> armEndAxleToConnectorGearCoupling;
        private readonly SmallGear connector;
        private readonly Coupling<Axle, SmallGear> inputAxleToInputGearCoupling;
        private readonly Coupling<SmallGear, Axle> outputGearToOutputAxleCoupling;

        public Relay(
            IPaddleFactory paddleFactory,
            bool isControlPositiveDirection = true,
            bool isInputPositiveDirection = true)
        {
            this.isControlPositiveDirection = isControlPositiveDirection;
            this.isInputPositiveDirection = isInputPositiveDirection;

            InputAxle = new Axle("input axle");
            InputGear = new SmallGear("input gear");
            connector = new SmallGear("connector gear");
            OutputGear = new SmallGear("output gear");
            OutputAxle = new Axle("output axle");
            ControlAxle = new Axle("control axle");

            PaddleWheel controlPaddleWheel = new PaddleWheel();
            Paddle armPaddle = paddleFactory.CreateNew();
            Arm arm = new Arm();

            // create fixed coupling for input
            Coupling<Axle, SmallGear>.NewLockedAxleToGearCoupling(
                input: InputAxle,
                output: InputGear,
                name: "input axle to input gear");

            // create fixed couplings for the control mechanism (paddle wheel, paddle, arm, and connector)
            new Coupling<Axle, PaddleWheel>( // the control axle rotates the paddle wheel
                input: ControlAxle,
                output: controlPaddleWheel,
                inputToOutputRatio: 1,
                couplingType: CouplingType.BidirectionalSymmetrical);
            new Coupling<Paddle, Axle>( // the paddle rotates the fulcrum axle
                input: armPaddle,
                output: arm.FulcrumAxle,
                inputToOutputRatio: 1,
                couplingType: CouplingType.BidirectionalSymmetrical);
            new Coupling<Axle, SmallGear>( // the connector lowers and rises with the arm's end axle
                input: arm.EndAxle,
                output: connector,
                inputToOutputRatio: 1,
                couplingType: CouplingType.FreeFlowing);

            // The interaction between the paddle wheel and the paddle is at the core of the control mechanism and
            // requires special handling here, via a separate evaluator class.
            Coupling<PaddleWheel, Paddle> paddleWheelToPaddleCoupling = new Coupling<PaddleWheel, Paddle>(
                input: controlPaddleWheel,
                output: armPaddle,
                inputToOutputRatio: 1,
                couplingType: CouplingType.BidirectionalOpposing);
            // PaddlePositionEvaluator paddlePositionEvaluator = new PaddlePositionEvaluator(paddleWheelToPaddleCoupling);

            // create fixed coupling for output
            Coupling<SmallGear, Axle>.NewLockedGearToAxleCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "output gear to output axle");
        }

        private void doStuff()
        {
            // initialize default couplings such that the relay is disengaged
            inputToConnectorCoupling = Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: InputGear,
                output: connector,
                couplingType: CouplingType.FreeFlowing, // (isInputPositiveDirection ? CouplingType.OneWayPositive : CouplingType.OneWayNegative),
                name: "input gear to connector gear");
            connectorToOutputCoupling = Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: connector,
                output: OutputGear,
                couplingType: CouplingType.FreeFlowing, // (isInputPositiveDirection ? CouplingType.OneWayNegative : CouplingType.OneWayPositive),
                name: "connector gear to output gear");
        }
    }
}
