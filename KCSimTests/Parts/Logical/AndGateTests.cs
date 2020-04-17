using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class AndGateTests
    {
        private AndGate andGate;

        public AndGateTests()
        {
            andGate = new AndGate(TestUtil.GetMockBidirectionalLatchFactory().Object);
            andGate.Power.AddForce(new Force(1));
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1, -1, -1)]
        [InlineData(1, 1, 1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            andGate.InputA.AddForce(new Force(inputA));
            Assert.Equal(Force.ZeroForce, andGate.InputB.GetNetForce());
            andGate.InputB.AddForce(new Force(inputB));
            Assert.Equal(new Force(expectedOutput), andGate.Output.GetNetForce());
        }
    }
}
