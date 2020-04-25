using System;
using System.Collections.Generic;
using KCSim.Physics;
using KCSim.Physics.Couplings;
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
        private readonly Queue<EvaluationNode> evaluationQueue;

        public CouplingMonitor(IPartsGraph partsGraph)
        {
            this.partsGraph = partsGraph;
            evaluationQueue = new Queue<EvaluationNode>();
        }

        /**
         * Invoke a delegate if the provided Coupling is removed.
         */
        public void OnCouplingRemoved(Coupling coupling, OnCouplingRemovedDelegate onCouplingRemovedDelegate)
        {
            onCouplingRemovedDelegates[coupling].Add(onCouplingRemovedDelegate);
        }

        /**
         * Invoke a delegate if the provided Torqueable is coupled to an input (i.e. coupled as an output).
         */
        public void OnCoupledToInput(Torqueable torqueable, OnCoupledToInputDelegate onCoupledToInputDelegate)
        {
            onCoupledToInputDelegates[torqueable].Add(onCoupledToInputDelegate);
        }

        public void RegisterCoupling(Coupling coupling)
        {
            partsGraph.AddVerticesAndEdge(coupling);
            AddToEvaluationQueue(coupling);

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
            AddToEvaluationQueue(coupling);

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

        public void EvaluateForces()
        {
            // This method uses a breadth-first search to evaluate and propogate forces throughout
            // the machine. In this algorithm, we represent a <vertex, edge> pair as a "node."

            // Add leaf nodes first.
            foreach (KeyValuePair<Torqueable, Coupling> leaf in partsGraph.GetLeafVertices())
            {
                AddToEvaluationQueue(new EvaluationNode(leaf.Key, leaf.Value));
            }

            while (evaluationQueue.Count > 0)
            {
                // Get the next coupling to evaluate.
                EvaluationNode currentNode = evaluationQueue.Dequeue();
                
                // Visit the current node.
                VisitNode(currentNode);
            }
        }

        private void VisitNode(EvaluationNode evaluationNode)
        {
            Coupling coupling = evaluationNode.Coupling;
            Torqueable source = evaluationNode.SourceOfForce;
            Torqueable target = coupling.GetOther(source);

            // If the net force is coming from the target itself, no need to back-propagate the force;
            // just break early.
            KeyValuePair<Torqueable, Force> sourceNetForce = source.GetNetForceAndSource();
            if (sourceNetForce.Key == target)
            {
                return;
            }

            // Apply the force to the coupling.
            Force targetForce = coupling.ReceiveForce(source, sourceNetForce.Value);

            // Apply the force to the target.
            bool didTargetNetForceChange = target.UpdateForce(source, targetForce);

            // If the net force on the target did not change, no need to back-propagate the force;
            // just break early.
            if (!didTargetNetForceChange)
            {
                return;
            }
            
            // Get adjacent couplings, and add them to the queue.
            foreach (var adjacentCoupling in partsGraph.GetCouplings(target))
            {
                if (adjacentCoupling != coupling)
                {
                    AddToEvaluationQueue(adjacentCoupling);
                }
            }
        }

        private void AddToEvaluationQueue(Coupling coupling)
        {
            AddToEvaluationQueue(new EvaluationNode(coupling.Input, coupling));
            AddToEvaluationQueue(new EvaluationNode(coupling.Output, coupling));
        }

        private void AddToEvaluationQueue(EvaluationNode evaluationNode)
        {
            evaluationQueue.Enqueue(evaluationNode);
        }

        private class EvaluationNode
        {
            public Torqueable SourceOfForce;
            public Coupling Coupling;

            public EvaluationNode(Torqueable sourceOfForce, Coupling coupling)
            {
                this.SourceOfForce = sourceOfForce;
                this.Coupling = coupling;
            }

            public override bool Equals(object obj)
            {
                return obj is EvaluationNode node &&
                       EqualityComparer<Torqueable>.Default.Equals(SourceOfForce, node.SourceOfForce) &&
                       EqualityComparer<Coupling>.Default.Equals(Coupling, node.Coupling);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(SourceOfForce, Coupling);
            }

            public override string ToString()
            {
                return "EvaluationNode: " + SourceOfForce + "; " + Coupling;
            }
        }
    }
}
