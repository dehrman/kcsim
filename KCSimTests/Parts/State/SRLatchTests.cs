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
    public class SRLatchTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingService couplingService;
        private readonly ICouplingMonitor couplingMonitor;

        private ExternalSwitch Power = new ExternalSwitch(new Force(1));
        private ExternalSwitch SetInverse = new ExternalSwitch();
        private ExternalSwitch ResetInverse = new ExternalSwitch();

        private readonly SRLatch latch;

        public SRLatchTests()
        {
            couplingService = testUtil.GetSingletonCouplingService();
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();

            latch = testUtil.GetStateFactory().CreateNewSRLatch();

            couplingService.CreateNewLockedCoupling(Power, latch.Power);
            couplingService.CreateNewLockedCoupling(SetInverse, latch.SetInverse);
            couplingService.CreateNewLockedCoupling(ResetInverse, latch.ResetInverse);
        }

        [Theory]
        [InlineData(-1, -1, 1)]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, -1)]
        public void TestThat_TruthTableHolds(int setInverse, int resetInverse, int expectedOutput)
        {
            SetInverse.Force = new Force(setInverse);
            ResetInverse.Force = new Force(resetInverse);
            couplingMonitor.EvaluateForces();

            Assert.Equal(new Force(expectedOutput), latch.DataOut.GetNetForce());
            Assert.Equal(new Force(expectedOutput * -1), latch.DataOutInverse.GetNetForce());
        }

        [Fact]
        public void TestThat_LatchHoldsValue_IfSetAndResetAreBothHigh()
        {
            SetInverse.Force = new Force(-1);
            ResetInverse.Force = new Force(1);
            couplingMonitor.EvaluateForces();
            SetInverse.Force = new Force(1);
            couplingMonitor.EvaluateForces();

            Assert.Equal(new Force(1), latch.DataOut.GetNetForce());
            Assert.Equal(new Force(-1), latch.DataOutInverse.GetNetForce());
        }
    }
}
