﻿using System;
using System.Collections.Generic;
using System.Linq;
using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using Moq;
using Xunit;

namespace KCSimTests
{
    public class TestUtil
    {
        private ICouplingService couplingService = null;
        private ICouplingMonitor couplingMonitor = null;
        private IGateMonitor gateMonitor = null;
        private IStateMonitor stateMonitor = null;
        private IPartsGraph partsGraph = null;
        private MotionTimerFactory motionTimerFactory = null;
        private ForceEvaluator forceEvaluator = null;

        public ForceEvaluator GetSingletonForceEvaluator()
        {
            if (forceEvaluator == null)
            {
                forceEvaluator = new ForceEvaluator(GetSingletonPartsGraph());
            }
            return forceEvaluator;
        }

        public MotionTimerFactory GetSingletonMotionTimerFactory()
        {
            if (motionTimerFactory == null)
            {
                motionTimerFactory = new MotionTimerFactory();
            }

            return motionTimerFactory;
        }

        public IPartsGraph GetSingletonPartsGraph()
        {
            if (partsGraph == null)
            {
                partsGraph = new PartsGraph();
            }
            return partsGraph;
        }

        public ICouplingMonitor GetSingletonCouplingMonitor()
        {
            if (couplingMonitor == null)
            {
                couplingMonitor = new CouplingMonitor(GetSingletonPartsGraph(), GetSingletonForceEvaluator());
            }
            return couplingMonitor;
        }
        
        public ICouplingService GetSingletonCouplingService()
        {
            if (couplingService == null)
            {
                CouplingFactory couplingFactory = new CouplingFactory();
                couplingService = new CouplingService(GetSingletonCouplingMonitor(), couplingFactory);
            }
            return couplingService;
        }

        public Mock<IMotionTimer> GetMockMotionTimer()
        {
            Mock<IMotionTimer> mockMotionTimer = new Mock<IMotionTimer>();

            // Set up the motion timer such that the delegate is invoked as soon as the timer is started.
            var forceEvaluator = GetSingletonForceEvaluator();
            mockMotionTimer.Setup(x =>x.Start(
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<IMotionTimer.OnTimerCompletionDelegate>()))
                .Callback<double, double, IMotionTimer.OnTimerCompletionDelegate>(
                    (d, v, onTimerCompletionDelegate) =>
                        forceEvaluator.OnAllForcesEvaluated(() => onTimerCompletionDelegate.Invoke(v)));

            return mockMotionTimer;
        }

        public IPaddleFactory GetPaddleFactory()
        {
            IPaddleFactory paddleFactory = new PaddleFactory(GetSingletonMotionTimerFactory());
            return paddleFactory;
        }

        public IRelayFactory GetRelayFactory()
        {
            return new RelayFactory(GetSingletonCouplingService(), GetPaddleFactory());
        }

        public IBidirectionalLatchFactory GetBidirectionalLatchFactory()
        {
            return new BidirectionalLatchFactory(GetSingletonCouplingService(), GetRelayFactory());
        }

        public IGateMonitor GetSingletonGateMonitor()
        {
            if (gateMonitor == null)
            {
                gateMonitor = new GateMonitor(GetSingletonPartsGraph(), GetSingletonCouplingMonitor());
            }
            return gateMonitor;
        }

        public IStateMonitor GetSingletonStateMonitor()
        {
            if (stateMonitor == null)
            {
                stateMonitor = new StateMonitor(GetSingletonCouplingMonitor());
            }
            return stateMonitor;
        }

        public IGateFactory GetGateFactory()
        {
            return new GateFactory(
                couplingService: GetSingletonCouplingService(),
                bidirectionalLatchFactory: GetBidirectionalLatchFactory(),
                gateMonitor: GetSingletonGateMonitor());
        }

        public IStateFactory GetStateFactory()
        {
            return new StateFactory(
                couplingService: GetSingletonCouplingService(),
                gateFactory: GetGateFactory(),
                stateMonitor: GetSingletonStateMonitor());
        }

        /**
         * Return a new version of this force in the same direction but at the desired level.
         * 
         * Should only be used for testing purposes.
         */
        public static Force ToLevel(Force forceToChange, Force forceWithDesiredLevel)
        {
            if (forceToChange.Equals(Force.ZeroForce))
            {
                return Force.ZeroForce;
            }
            double sign = forceToChange.Velocity / Math.Abs(forceToChange.Velocity);
            return new Force(Math.Abs(forceWithDesiredLevel.Velocity) * sign);
        }

        /**
         * Return a new version of this force in the same direction but at the desired level.
         * 
         * Should only be used for testing purposes.
         */
        public static Force ToLevel(double forceToChange, double desiredLevel)
        {
            return ToLevel(new Force(forceToChange), new Force(desiredLevel));
        }

        public static void AssertDirectionsEqual(Force expected, Force actual)
        {
            // If the force is the special InitialState, in most cases (unless force2 equals initialState),
            // that should not count as being equal.
            if (Math.Abs(new KCSim.Parts.State.InitialState().Velocity).Equals(Math.Abs(expected.Velocity)))
            {
                Assert.Equal(Math.Abs(new KCSim.Parts.State.InitialState().Velocity), Math.Abs(actual.Velocity));
            }

            if (expected.Equals(actual))
            {
                Assert.Equal(expected, actual);
                return;
            }

            if (expected.Equals(Force.ZeroForce) || actual.Equals(Force.ZeroForce))
            {
                Assert.True(false);
                return;
            }

            var releveledForce1 = ToLevel(expected, actual);
            Assert.Equal(releveledForce1, actual);
        }

        public void InitializeState(StatefulGate gate)
        {
            couplingService.CreateNewInitialStateCoupling(new KCSim.Parts.State.InitialState(), gate.Q);
        }

        public static IDictionary<bool[], bool> GetTruthTable(int numInputs, Func<bool[], bool> predicate)
        {
            IDictionary<bool[], bool> truthTable = new Dictionary<bool[], bool>();

            return Enumerable.Range(0, 1 << numInputs)
                .Select(i => BitMath.GetBitVector(numInputs, i))
                .Select(inputs => new KeyValuePair<bool[], bool>(inputs, predicate.Invoke(inputs)))
                .ToDictionary(
                    keySelector: kvp => kvp.Key,
                    elementSelector: kvp => kvp.Value);
        }
    }
}
