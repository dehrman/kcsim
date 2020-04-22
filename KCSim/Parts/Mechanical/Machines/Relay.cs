using System;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics.Couplings;
using KCSim.Physics;
using static KCSim.Parts.Mechanical.Paddle;

namespace KCSim.Parts.Mechanical.Machines
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
        private readonly Direction inputDirection;
        private readonly Direction paddleWheelEnableDirection;
        private readonly bool isInputPositiveDirection;

        // conditional couplings that are created and destroyed as the relay is enabled and disabled respectively
        private Coupling inputToConnectorCoupling;
        private Coupling connectorToOutputCoupling;

        // fixed couplings
        private readonly SmallGear connector;

        private readonly string name;

        private readonly ICouplingService couplingService;

        public Relay(
            ICouplingService couplingService,
            IPaddleFactory paddleFactory,
            Direction enableDirection,
            Direction inputDirection,
            string name = "")
        {
            this.name = name;
            this.couplingService = couplingService;
            this.inputDirection = inputDirection;

            // Because the control axle has an opposing coupling with the control paddle, we record the sign of the control paddle
            // as the inverse of that of the control axle.
            paddleWheelEnableDirection = enableDirection.Opposite();

            InputAxle = new Axle(name + "; input axle");
            InputGear = new SmallGear(name + "; input gear");
            connector = new SmallGear(name + "; connector gear");
            OutputGear = new SmallGear(name + "; output gear");
            OutputAxle = new Axle(name + "; output axle");
            Enable = new Axle(name + "; control axle");

            ArmPaddle = paddleFactory.CreateNew(name: name + "; paddle");
            ArmPaddle.OnPaddlePositionChangedDelegateSet.Add(OnPaddlePositionChanged);

            InitializeCouplings();
        }

        private void OnPaddlePositionChanged(Paddle paddle, Position position)
        {
            // If the paddle is not in the required position to engage the relay, ensure that it is disengaged.
            if (position == Position.Intermediate)
            {
                DisengageRelay();
                return;
            }
            if (paddleWheelEnableDirection == Direction.Positive && position != Position.Positive)
            {
                DisengageRelay();
                return;
            }
            if (paddleWheelEnableDirection == Direction.Negative && position != Position.Negative)
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
                couplingService.RemoveCoupling(inputToConnectorCoupling);
            }
            if (connectorToOutputCoupling != null)
            {
                couplingService.RemoveCoupling(connectorToOutputCoupling);
            }
        }

        private void EngageRelay()
        {
            inputToConnectorCoupling = couplingService.CreateNewOneWayCoupling(
                input: InputGear,
                output: connector,
                direction: inputDirection,
                name: "input gear to connector gear");
            connectorToOutputCoupling = couplingService.CreateNewOneWayCoupling(
                input: connector,
                output: OutputGear,
                direction: inputDirection.Opposite(),
                name: "connector gear to output gear");
        }

        private void InitializeCouplings()
        {
            // create fixed coupling for input
            couplingService.CreateNewLockedCoupling(
                input: InputAxle,
                output: InputGear,
                name: "input axle to input gear");

            // TODO: right now, the arm is not actually used in any functional way.
            Arm arm = new Arm();
            PaddleWheel controlPaddleWheel = new PaddleWheel(name + "; paddle wheel");

            // create fixed couplings for the control mechanism (paddle wheel, paddle, arm, and connector)
            couplingService.CreateNewLockedCoupling( // the control axle rotates the paddle wheel
                input: Enable,
                output: controlPaddleWheel);
            //couplingService.CreateNewLockedCoupling( // the paddle rotates the fulcrum axle
            //    input: ArmPaddle,
            //    output: arm.FulcrumAxle);
            //couplingService.CreateNewFreeFlowingCoupling( // the connector lowers and rises with the arm's end axle
            //    input: arm.EndAxle,
            //    output: connector);

            // The interaction between the paddle wheel and the paddle is at the core of the control mechanism and
            // requires special handling here, via a separate evaluator class.
            couplingService.CreateNewBidirectionalOpposingCoupling(
                input: controlPaddleWheel,
                output: ArmPaddle);

            // create fixed coupling for output
            couplingService.CreateNewLockedCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "output gear to output axle");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
