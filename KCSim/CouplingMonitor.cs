using System;
using System.Collections.Generic;
using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using static KCSim.Physics.Torqueable;
using static KCSim.ICouplingMonitor;

namespace KCSim
{
    public class CouplingMonitor : ICouplingMonitor
    {
        private readonly IDictionary<Coupling, ISet<OnCouplingRemovedDelegate>> onCouplingRemovedDelegates
            = new Dictionary<Coupling, ISet<OnCouplingRemovedDelegate>>();

        private readonly IDictionary<Torqueable, ISet<OnCoupledToInputDelegate>> onCoupledToInputDelegates
            = new Dictionary<Torqueable, ISet<OnCoupledToInputDelegate>>();

        private readonly IPartsGraph partsGraph;
        private readonly ForceEvaluator forceEvaluator;

        public CouplingMonitor(
            IPartsGraph partsGraph,
            ForceEvaluator forceEvaluator)
        {
            this.partsGraph = partsGraph;
            this.forceEvaluator = forceEvaluator;
        }

        /**
         * Invoke a delegate if the provided Coupling is removed.
         */
        public void OnCouplingRemoved(Coupling coupling, OnCouplingRemovedDelegate onCouplingRemovedDelegate)
        {
            if (!onCouplingRemovedDelegates.ContainsKey(coupling))
            {
                onCouplingRemovedDelegates[coupling] = new HashSet<OnCouplingRemovedDelegate>();
            }
            onCouplingRemovedDelegates[coupling].Add(onCouplingRemovedDelegate);
        }

        /**
         * Invoke a delegate if the provided Torqueable is coupled to an input (i.e. coupled as an output).
         */
        public void OnCoupledToInput(Torqueable torqueable, OnCoupledToInputDelegate onCoupledToInputDelegate)
        {
            if (!onCoupledToInputDelegates.ContainsKey(torqueable))
            {
                onCoupledToInputDelegates[torqueable] = new HashSet<OnCoupledToInputDelegate>();
            }
            onCoupledToInputDelegates[torqueable].Add(onCoupledToInputDelegate);
        }

        public void RegisterCoupling(Coupling coupling)
        {
            partsGraph.AddVerticesAndEdge(coupling);
            forceEvaluator.AddToEvaluationQueue(coupling);
            MonitorForceChanges(coupling.Input);
            MonitorForceChanges(coupling.Output);

            if (!onCoupledToInputDelegates.ContainsKey(coupling.Output))
            {
                return;
            }
            var delegates = onCoupledToInputDelegates[coupling.Output];
            if (delegates == null)
            {
                return;
            }
            foreach (var onCoupledToInputDelegate in delegates)
            {
                onCoupledToInputDelegate?.Invoke(coupling);
            }
        }

        public void RemoveCoupling(Coupling coupling)
        {
            partsGraph.RemoveEdge(coupling);
            forceEvaluator.AddToFrontOfEvaluationQueue(coupling);

            if (!onCouplingRemovedDelegates.ContainsKey(coupling))
            {
                return;
            }
            var delegates = onCouplingRemovedDelegates[coupling];
            if (delegates == null)
            {
                return;
            }
            foreach (var onCouplingRemovedDelegate in delegates)
            {
                onCouplingRemovedDelegate?.Invoke(coupling);
            }
        }

        public bool IsCoupled(Torqueable torqueable)
        {
            var couplings = partsGraph.GetCouplings(torqueable);
            return couplings != null && couplings.Count > 0;
        }

        private void MonitorForceChanges(Torqueable torqueable)
        {
            torqueable.OnForceChange += GetOnForceChangeDelegate(torqueable);
        }

        private OnForceChangeDelegate GetOnForceChangeDelegate(Torqueable torqueable)
        {
            return (oldForce, newForce) => { }; // System.Diagnostics.Debug.WriteLine(torqueable + " changed from " + oldForce + " to " + newForce);
        }
    }
}
