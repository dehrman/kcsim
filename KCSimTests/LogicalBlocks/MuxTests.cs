using System;
using System.Collections.Generic;
using System.Text;
using KCSim;
using KCSim.LogicalBlocks;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.LogicalBlocks
{
    public class MuxTests
    {
        private readonly TestUtil testUtil;
        private readonly ICouplingService couplingService;
        private readonly ICouplingMonitor couplingMonitor;
        private readonly IGateFactory gateFactory;
        private readonly LogicalBlockFactory logicalBlockFactory;

        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));

        public MuxTests()
        {
            var testUtil = new TestUtil();
            this.couplingService = testUtil.GetSingletonCouplingService();
            this.couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            this.gateFactory = testUtil.GetGateFactory();
            this.logicalBlockFactory = new LogicalBlockFactory(couplingService, gateFactory);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void TestThat_Something(int numInputs)
        {
            var mux = new Mux(couplingService, gateFactory, numInputs);
            couplingService.CreateNewLockedCoupling(motor, mux.Power);
            for 
        }
    }
}
