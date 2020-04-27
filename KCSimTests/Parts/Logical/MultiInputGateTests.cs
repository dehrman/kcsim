using System;
using System.Collections.Generic;
using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class MultiInputGateTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly IGateFactory gateFactory;
        private readonly ICouplingService couplingService;
        private readonly ForceEvaluator forceEvaluator;

        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));

        public MultiInputGateTests()
        {
            gateFactory = testUtil.GetGateFactory();
            couplingService = testUtil.GetSingletonCouplingService();
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void TestThat_TruthTableHolds_ForMultiInputAndGate(int numInputs)
        {
            TestTruthTableForGate(
                binaryPredicate: (a, b) => a && b,
                numInputs: numInputs,
                () => gateFactory.CreateNewMultiInputAndGate(numInputs));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void TestThat_TruthTableHolds_ForMultiInputOrGate(int numInputs)
        {
            TestTruthTableForGate(
                binaryPredicate: (a, b) => a || b,
                numInputs: numInputs,
                () => gateFactory.CreateNewMultiInputOrGate(numInputs));
        }

        private void TestTruthTableForGate<T>(
            Func<bool, bool, bool> binaryPredicate,
            int numInputs,
            Func<MultiInputGate<T>> gateProvider)
            where T : BinaryInputGate
        {
            var initialStateCouplings = new Coupling[numInputs];

            IDictionary<bool[], bool> truthTable = GetTruthTable(numInputs, binaryPredicate);
            foreach (var testCase in truthTable)
            {
                var gate = gateProvider.Invoke();
                couplingService.CreateNewLockedCoupling(motor, gate.Power);

                var inputs = testCase.Key;
                var expectedOutput = testCase.Value;

                for (int i = 0; i < inputs.Length; i++)
                {
                    var input = inputs[i] ? new Force(1) : new Force(-1);
                    var coupling = couplingService.CreateNewLockedCoupling(new ExternalSwitch(input), gate.Inputs[i]);
                    initialStateCouplings[i] = coupling;
                }

                forceEvaluator.EvaluateForces();

                TestUtil.AssertDirectionsEqual(new Force(expectedOutput ? 1 : -1), gate.Output.GetNetForce());

                // Now tear down the initial-state couplings.
                Array.ForEach(initialStateCouplings, coupling => couplingService.RemoveCoupling(coupling));
            }
        }

        private static IDictionary<bool[], bool> GetTruthTable(int numInputs, Func<bool, bool, bool> binaryPredicate)
        {
            Func<bool[], bool> compositePredicate = (inputs) =>
            {
                bool result = inputs[0];
                for (int i = 0; i < inputs.Length; i++)
                {
                    result = binaryPredicate.Invoke(result, inputs[i]);
                }
                return result;
            };
            return TestUtil.GetTruthTable(numInputs, compositePredicate);
        }
    }
}
