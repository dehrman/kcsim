﻿using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.State
{
    public class GatedDLatchTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private ExternalSwitch Power = new ExternalSwitch(new Force(1));
        private ExternalSwitch DataIn = new ExternalSwitch();
        private ExternalSwitch WriteEnable = new ExternalSwitch();

        private readonly GatedDLatch latch;

        public GatedDLatchTests()
        {
            couplingService = testUtil.GetSingletonCouplingService();
            forceEvaluator = testUtil.GetSingletonForceEvaluator();

            latch = testUtil.GetStateFactory().CreateNewGatedDLatch();

            couplingService.CreateNewLockedCoupling(Power, latch.Power);
            couplingService.CreateNewLockedCoupling(DataIn, latch.Data);
            couplingService.CreateNewLockedCoupling(WriteEnable, latch.Enable);

            testUtil.InitializeState(latch);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TestThat_DataLatchesWhenWriteEnableIsEnabled(int valueToLatch)
        {
            DataIn.Force = new Force(valueToLatch);
            WriteEnable.Force = new Force(1);

            forceEvaluator.EvaluateForces();

            var latchedValue = latch.Q.GetNetForce().Velocity;
            Assert.Equal(new Force(valueToLatch), new Force(latchedValue));
            Assert.Equal(valueToLatch, latchedValue);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_WhenWriteEnableIsDisabled_LatchHoldsValue(int initialValue, int nextValue)
        {
            // Latch in the initial value.
            DataIn.Force = new Force(initialValue);
            WriteEnable.Force = new Force(1);
            forceEvaluator.EvaluateForces();

            // Now disable the write enable and set the data input to the next value.
            WriteEnable.Force = new Force(-1);
            DataIn.Force = new Force(nextValue);
            forceEvaluator.EvaluateForces();

            // We should expect to still see the initially latched-in value on the output wire.
            Assert.Equal(new Force(initialValue), latch.Q.GetNetForce());
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        public void TestThat_LatchCanResetAndStoreNewValue(int oldValue, int newValue)
        {
            // Latch in the initial value.
            DataIn.Force = new Force(oldValue);
            WriteEnable.Force = new Force(1);
            forceEvaluator.EvaluateForces();

            // Now disable the write enable and set the data input to the new value.
            WriteEnable.Force = new Force(-1);
            DataIn.Force = new Force(newValue);
            forceEvaluator.EvaluateForces();

            // Now enable the write enable.
            WriteEnable.Force = new Force(1);
            forceEvaluator.EvaluateForces();

            // We should expect to see that the old value was overwritten, and the new value is present.
            TestUtil.AssertDirectionsEqual(new Force(newValue), latch.Q.GetNetForce());
        }
    }
}
