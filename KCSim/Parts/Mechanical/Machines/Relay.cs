using System;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics.Couplings;
using KCSim.Physics;
using static KCSim.Parts.Mechanical.Paddle;

namespace KCSim.Parts.Mechanical.Machines
{
    public class Relay
    {
        public enum InitialState
        {
            Disabled,
            Enabled
        }

        public readonly Axle Enable;
        public readonly Axle InputAxle;
        public readonly SmallGear InputGear;
        public readonly SmallGear OutputGear;
        public readonly Axle OutputAxle;
        public readonly Paddle ArmPaddle;

        // functional properties of the part
        private readonly Direction enableDirection;
        private readonly Direction inputDirection;
        private readonly Relay.InitialState initialState;

        // conditional couplings that are created and destroyed as the relay is enabled and disabled respectively
        private Coupling inputToConnectorCoupling;
        private Coupling connectorToOutputCoupling;
        private Coupling paddleWheelToPaddleCoupling;

        // other parts
        private readonly SmallGear connector;
        private readonly PaddleWheel paddleWheel;

        private readonly ICouplingService couplingService;
        private readonly string name;

        public Relay(
            ICouplingService couplingService,
            IPaddleFactory paddleFactory,
            Direction enableDirection,
            Direction inputDirection,
            InitialState initialState = InitialState.Disabled,
            string name = "")
        {
            this.name = name;
            this.couplingService = couplingService;
            this.enableDirection = enableDirection;
            this.inputDirection = inputDirection;
            this.initialState = initialState;

            InputAxle = new Axle(name: name + "; input axle");
            InputGear = new SmallGear(name + "; input gear");
            connector = new SmallGear(name + "; connector gear");
            OutputGear = new SmallGear(name + "; output gear");
            OutputAxle = new Axle(name + "; output axle");
            Enable = new Axle(name + "; control axle");
            paddleWheel = new PaddleWheel(name + "; paddle wheel");

            ArmPaddle = paddleFactory.CreateNew(
                initialPosition: GetInitialPaddlePosition(),
                name: name + "; paddle");
            ArmPaddle.OnPaddlePositionChangedDelegateSet.Add(OnPaddlePositionChanged);

            InitializeCouplings();

            if (initialState.Equals(Relay.InitialState.Enabled))
            {
                EngageRelay();
            }
        }

        private Position GetInitialPaddlePosition()
        {
            if (initialState.Equals(InitialState.Disabled))
            {
                if (enableDirection.Equals(Direction.Positive))
                {
                    // The relay will be enabled when the paddle is rotated in the negative direction,
                    // since it has an opposing coupling with the paddle wheel.
                    return Position.Positive;
                }

                return Position.Negative;
            }

            if (enableDirection.Equals(Direction.Positive))
            {
                // The relay will be enabled when the paddle is rotated in the negative direction,
                // since it has an opposing coupling with the paddle wheel.
                return Position.Negative;
            }

            return Position.Positive;
        }

        private void OnPaddlePositionChanged(Paddle paddle, Position position)
        {
            // If the paddle is not in the required position to engage the relay, ensure that it is disengaged.
            if (position == Position.Intermediate)
            {
                DisengageRelay();
                return;
            }
            if (enableDirection == Direction.Positive && position == Position.Positive)
            {
                DisengageRelay();
                return;
            }
            if (enableDirection == Direction.Negative && position == Position.Negative)
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
                inputToConnectorCoupling = null;
            }
            if (connectorToOutputCoupling != null)
            {
                couplingService.RemoveCoupling(connectorToOutputCoupling);
                connectorToOutputCoupling = null;
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

            // create fixed couplings for the control mechanism (paddle wheel, paddle, arm, and connector)
            couplingService.CreateNewLockedCoupling( // the control axle rotates the paddle wheel
                input: Enable,
                output: paddleWheel);

            // create fixed coupling for output
            couplingService.CreateNewLockedCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "output gear to output axle");

            // The interaction between the paddle wheel and the paddle is at the core of the control mechanism and
            // requires special handling here, via a separate evaluator class.
            paddleWheelToPaddleCoupling = couplingService.CreateNewOneWayPaddleCoupling(
                paddleWheel,
                ArmPaddle,
                enableDirection);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
