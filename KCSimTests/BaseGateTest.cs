using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KCSim;
using KCSim.Parts.Logical;

namespace KCSimTests.Parts.Logical
{
    public abstract class BaseGateTest
    {
        protected readonly TestUtil testUtil = new TestUtil();
        protected readonly ICouplingService couplingService;
        protected readonly IGateFactory gateFactory;
        
        private readonly ForceEvaluator forceEvaluator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGateTest"/> class.
        /// </summary>
        public BaseGateTest()
        {
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
            couplingService = testUtil.GetSingletonCouplingService();
            gateFactory = testUtil.GetGateFactory();
        }

        protected void EvaluateForcesWithDelay()
        {
            EvaluateForcesWithDelay(4, 30);
        }

        protected void EvaluateForcesWithDelay(int numEvaluations, int sleepMsBetweenEvaluations)
        {
            for (int i = 0; i < numEvaluations; i++)
            {
                Thread.Sleep(sleepMsBetweenEvaluations);
                forceEvaluator.EvaluateForces();
            }
        }
    }
}
