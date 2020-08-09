using KCSim;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Mechanical
{
    public class DiodeTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch input1 = new ExternalSwitch("input1");
        private readonly Diode positiveDiode;
        private readonly Diode negativeDiode;

        public DiodeTests()
        {
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
            couplingService = testUtil.GetSingletonCouplingService();
            positiveDiode = new Diode(couplingService, Direction.Positive);
            negativeDiode = new Diode(couplingService, Direction.Negative);

            couplingService.CreateNewLockedCoupling(input1, positiveDiode.InputAxle);
            couplingService.CreateNewLockedCoupling(input1, negativeDiode.InputAxle);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TestThat_PositiveInput_Yields_EquivalentPositiveOutput(int value)
        {
            input1.Force = new Force(value);
            forceEvaluator.EvaluateForces();
            Assert.Equal(value, positiveDiode.OutputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_MultiplePositiveInputs_Yield_MaximumPositiveOutput()
        {
            input1.Force = new Force(1);

            ExternalSwitch input2 = new ExternalSwitch("input2");
            couplingService.CreateNewLockedCoupling(input2, positiveDiode.InputAxle);
            input2.Force = new Force(3);

            ExternalSwitch input3 = new ExternalSwitch("input3");
            couplingService.CreateNewLockedCoupling(input3, positiveDiode.InputAxle);
            input3.Force = new Force(2);

            forceEvaluator.EvaluateForces();
            Assert.Equal(3, positiveDiode.OutputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_RemovingInputForce_Yields_NextHighestPositiveOutput()
        {
            input1.Force = new Force(1);
            
            ExternalSwitch input2 = new ExternalSwitch();
            couplingService.CreateNewLockedCoupling(input2, positiveDiode.InputAxle);
            input2.Force = new Force(3);

            ExternalSwitch input3 = new ExternalSwitch();
            couplingService.CreateNewLockedCoupling(input3, positiveDiode.InputAxle);
            input3.Force = new Force(2);

            // Do a force evaluation.
            forceEvaluator.EvaluateForces();

            // Now remove the highest force (3).
            input2.Force = Force.ZeroForce;

            // Now evaluate the forces again.
            forceEvaluator.EvaluateForces();

            // The output should be equivalent to the next highest force, that is 2 instead of 3.
            Assert.Equal(2, positiveDiode.OutputAxle.GetNetForce().Velocity);
        }
    }
}