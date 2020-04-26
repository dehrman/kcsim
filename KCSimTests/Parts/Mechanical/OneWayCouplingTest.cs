using System;
using System.Collections.Generic;
using System.Text;
using KCSim;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using Xunit;

namespace KCSimTests.Parts.Mechanical
{
    public class OneWayCouplingTest
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch input = new ExternalSwitch("input1");

        private readonly Gear inputGear = new SmallGear();
        private readonly Gear outputGear = new SmallGear();

        private readonly Coupling coupling;

        public OneWayCouplingTest()
        {
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
            couplingService = testUtil.GetSingletonCouplingService();
            couplingService.CreateNewLockedCoupling(input, inputGear);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        public void TestThat_OneWayPositiveCoupling_YieldsExpectedOutputs(int inputValue, int expectedOutput)
        {
            input.Force = new Force(inputValue);
            couplingService.CreateNewOneWayCoupling(inputGear, outputGear, Direction.Positive);
            forceEvaluator.EvaluateForces();
            Assert.Equal(expectedOutput, outputGear.GetNetForce().Velocity);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        public void TestThat_OneWayNegativeCoupling_YieldsExpectedOutputs(int inputValue, int expectedOutput)
        {
            input.Force = new Force(inputValue);
            couplingService.CreateNewOneWayCoupling(inputGear, outputGear, Direction.Negative);
            forceEvaluator.EvaluateForces();
            Assert.Equal(expectedOutput, outputGear.GetNetForce().Velocity);
        }
    }
}
