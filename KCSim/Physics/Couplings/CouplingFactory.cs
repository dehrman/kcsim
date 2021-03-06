﻿using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.State;

namespace KCSim.Physics.Couplings
{
    public class CouplingFactory : ICouplingFactory
    {
        public BiPaddleCoupling CreateNewBiPaddleCoupling(Torqueable input, Torqueable output, string name = "")
        {
            return new BiPaddleCoupling(input, output, name);
        }

        public Coupling CreateNewBidirectionalOpposingCoupling(Torqueable input, Torqueable output, string name = "")
        {
            return new BidirectionalOpposingCoupling(input, output, 1.0, name);
        }

        public OneWayPaddleCoupling CreateNewOneWayPaddleCoupling(PaddleWheel paddleWheel, Paddle paddle, Direction direction, string name = "")
        {
            return new OneWayPaddleCoupling(paddleWheel, paddle, GetInputToOutputRatio(paddleWheel, paddle), direction, name);
        }

        public Coupling CreateNewFreeFlowingCoupling(Torqueable input, Torqueable output, string name = "")
        {
            return new FreeFlowingCoupling(input, output, name);
        }

        public Coupling CreateNewGearCoupling(Gear input, Gear output, string name = "")
        {
            return new BidirectionalOpposingCoupling(input, output, GetInputToOutputRatio(input, output), name);
        }

        public InitialStateCoupling CreateNewInitialStateCoupling(Parts.State.InitialState initialForce, Torqueable output, string name = "")
        {
            return new InitialStateCoupling(initialForce, output, name);
        }

        public Coupling CreateNewLockedCoupling(Torqueable input, Torqueable output, string name = "")
        {
            return new BidirectionalSymmetricalCoupling(input, output, name);
        }

        public Coupling CreateNewOneWayCoupling(Gear input, Gear output, Direction direction, string name = "")
        {
            return new OneWayCoupling(input, output, GetInputToOutputRatio(input, output), direction, name);
        }

        private double GetInputToOutputRatio(Gear input, Gear output)
        {
            return (double)input.GetNumTeeth() / (double)output.GetNumTeeth();
        }
    }
}
