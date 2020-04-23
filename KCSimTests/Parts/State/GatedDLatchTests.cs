using System;
using System.Collections.Generic;
using System.Text;
using KCSim;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.State
{
    public class GatedDLatchTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingService couplingService;
        private readonly ICouplingMonitor couplingMonitor;

        private ExternalSwitch Power = new ExternalSwitch(new Force(1));
        private ExternalSwitch DataIn = new ExternalSwitch();
        private ExternalSwitch WriteEnable = new ExternalSwitch();

        private readonly GatedDLatch latch;

        public GatedDLatchTests()
        {
            couplingService = testUtil.GetSingletonCouplingService();
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();

            latch = testUtil.GetStateFactory().CreateNewGatedDLatch();

            couplingService.CreateNewLockedCoupling(Power, latch.Power);
            couplingService.CreateNewLockedCoupling(DataIn, latch.DataIn);
            couplingService.CreateNewLockedCoupling(WriteEnable, latch.WriteEnable);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TestThat_DataLatchesWhenWriteEnableIsEnabled(int valueToLatch)
        {
            DataIn.Force = new Force(valueToLatch);
            WriteEnable.Force = new Force(1);

            couplingMonitor.EvaluateForces();

            Assert.Equal(new Force(valueToLatch), latch.DataOut.GetNetForce());
            Assert.Equal(new Force(valueToLatch * -1), latch.DataOutInverse.GetNetForce());
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
            couplingMonitor.EvaluateForces();

            // Now disable the write enable and set the data input to the next value.
            WriteEnable.Force = new Force(-1);
            DataIn.Force = new Force(nextValue);
            couplingMonitor.EvaluateForces();

            // We should expect to still see the initially latched-in value on the output wire.
            Assert.Equal(new Force(initialValue), latch.DataOut.GetNetForce());
            Assert.Equal(new Force(initialValue * -1), latch.DataOutInverse.GetNetForce());
        }
    }
}
