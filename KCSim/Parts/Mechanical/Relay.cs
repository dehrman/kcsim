using System;
using KCSim.Physics;
using static KCSim.Parts.Mechanical.Paddle;

namespace KCSim.Parts.Mechanical
{
    public class Relay
    {
        public readonly Axle Enable;
        public readonly Axle InputAxle;
        public readonly SmallGear InputGear;
        public readonly SmallGear OutputGear;
        public readonly Axle OutputAxle;
        public readonly Paddle ArmPaddle;

        // functional properties of the part
        private readonly bool isControlPaddlePositiveDirection;
        private readonly bool isInputPositiveDirection;

        // conditional couplings that are created and destroyed as the relay is enabled and disabled respectively
        private Coupling<SmallGear, SmallGear>? inputToConnectorCoupling;
        private Coupling<SmallGear, SmallGear>? connectorToOutputCoupling;

        // fixed couplings
        private readonly SmallGear connector;

        private readonly string name;

        public Relay(
            IPaddleFactory paddleFactory,
            bool isControlPositiveDirection = true,
            bool isInputPositiveDirection = true,
            string name = "")
        {
            this.name = name;

            this.isInputPositiveDirection = isInputPositiveDirection;

            // Because the control axle has an opposing coupling with the control paddle, we record the sign of the control paddle
            // as the inverse of that of the control axle.
            this.isControlPaddlePositiveDirection = !isControlPositiveDirection;

            InputAxle = new Axle(name + "; input axle");
            InputGear = new SmallGear(name + "; input gear");
            connector = new SmallGear(name + "; connector gear");
            OutputGear = new SmallGear(name + "; output gear");
            OutputAxle = new Axle(name + "; output axle");
            Enable = new Axle(name + "; control axle");

            PaddleWheel controlPaddleWheel = new PaddleWheel(name + "; paddle wheel");

            ArmPaddle = paddleFactory.CreateNew(name: name + "; paddle");
            ArmPaddle.OnPaddlePositionChangedDelegateSet.Add(OnPaddlePositionChanged);

            Arm arm = new Arm();

            // create fixed coupling for input
            Coupling<Axle, SmallGear>.NewLockedAxleToGearCoupling(
                input: InputAxle,
                output: InputGear,
                name: "input axle to input gear");

            // create fixed couplings for the control mechanism (paddle wheel, paddle, arm, and connector)
            new Coupling<Axle, PaddleWheel>( // the control axle rotates the paddle wheel
                input: Enable,
                output: controlPaddleWheel,
                inputToOutputRatio: 1,
                couplingType: CouplingType.BidirectionalSymmetrical);
            new Coupling<Paddle, Axle>( // the paddle rotates the fulcrum axle
                input: ArmPaddle,
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
                output: ArmPaddle,
                inputToOutputRatio: 1,
                couplingType: CouplingType.BidirectionalOpposing);

            // create fixed coupling for output
            Coupling<SmallGear, Axle>.NewLockedGearToAxleCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "output gear to output axle");
        }

        private void OnPaddlePositionChanged(Paddle paddle, Position position)
        {
            // If the paddle is not in the required position to engage the relay, ensure that it is disengaged.
            if (position == Position.Intermediate)
            {
                DisengageRelay();
                return;
            }
            if (isControlPaddlePositiveDirection && position != Position.Positive)
            {
                DisengageRelay();
                return;
            }
            if (!isControlPaddlePositiveDirection && position != Position.Negative)
            {
                DisengageRelay();
                return;
            }

            // At this point, we know that the paddle is in the position required to engage the relay, so let's engage it.
            EngageRelay();
        }

        private void DisengageRelay()
        {
            if (inputToConnectorCoupling != null)
            {
                inputToConnectorCoupling.Remove();
            }
            if (connectorToOutputCoupling != null)
            {
                connectorToOutputCoupling.Remove();
            }
        }

        private void EngageRelay()
        {
            inputToConnectorCoupling = Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: InputGear,
                output: connector,
                couplingType: isInputPositiveDirection ? CouplingType.OneWayPositive : CouplingType.OneWayNegative,
                name: "input gear to connector gear");
            connectorToOutputCoupling = Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: connector,
                output: OutputGear,
                couplingType: isInputPositiveDirection ? CouplingType.OneWayNegative : CouplingType.OneWayPositive,
                name: "connector gear to output gear");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
