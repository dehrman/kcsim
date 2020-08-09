using System;
using System.Collections.Generic;
using System.Linq;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using static KCSim.ICouplingMonitor;

namespace KCSim
{
    public class ForceEvaluator
    {
        private readonly IPartsGraph partsGraph;
        private readonly List<EvaluationNode> evaluationQueue;
        private readonly Queue<Action> callbackQueue;

        public ForceEvaluator(IPartsGraph partsGraph)
        {
            this.partsGraph = partsGraph;
            evaluationQueue = new List<EvaluationNode>();
            callbackQueue = new Queue<Action>();
        }

        public void AddToFrontOfEvaluationQueue(Coupling coupling)
        {
            AddToFrontOfEvaluationQueue(new EvaluationNode(coupling.Input, coupling));
            AddToFrontOfEvaluationQueue(new EvaluationNode(coupling.Output, coupling));
        }

        public void AddToBackOfEvaluationQueue(Coupling coupling)
        {
            AddToBackOfEvaluationQueue(new EvaluationNode(coupling.Input, coupling));
            AddToBackOfEvaluationQueue(new EvaluationNode(coupling.Output, coupling));
        }

        public void EvaluateForces()
        {
            // Add leaf nodes first.
            foreach (KeyValuePair<Torqueable, Coupling> leaf in partsGraph.GetLeafVertices())
            {
                AddToBackOfEvaluationQueue(new EvaluationNode(leaf.Key, leaf.Value));
            }

            while (evaluationQueue.Count > 0)
            {
                VisitAllNodesInQueue();

                // Invoke any callbacks that were added to the callback queue.
                // Since these callbacks may also add to the evaluation queue, we'll need to go
                // back and check once more that the evaluation queue is empty before returning.
                while (callbackQueue.Count > 0)
                {
                    callbackQueue.Dequeue()?.Invoke();
                }
            }
        }

        public void OnAllForcesEvaluated(Action action)
        {
            callbackQueue.Enqueue(action);
        }

        private void VisitAllNodesInQueue()
        {
            // This method uses a mixed breadth-first and depth-first search to evaluate and
            // propagate forces throughout the machine.
            //
            // In this algorithm, we represent a <vertex, edge> pair as a "node."
            //
            // The primary algorithm is breadth-first; however, some special cases will result
            // in new nodes getting priority, at which point they will get added to the FRONT
            // of the queue rather than the back.

            // Evaluate and propagate forces via breadth-first search.
            while (evaluationQueue.Count > 0)
            {
                // Get the next coupling to evaluate.
                EvaluationNode currentNode = evaluationQueue[0];
                lock (evaluationQueue)
                {
                    evaluationQueue.RemoveAt(0);
                }

                // Visit the current node.
                VisitNode(currentNode);
            }
        }

        private void VisitNode(EvaluationNode evaluationNode)
        {
            Coupling coupling = evaluationNode.Coupling;
            Torqueable source = evaluationNode.SourceOfForce;
            Torqueable target = coupling.GetOther(source);

            if (target.name.StartsWith("SR latch; NAND gate 1; AND gate; inputLatchB; negative relay control diode"))
            {
                System.Diagnostics.Debug.WriteLine("Hey!");
            }

            KeyValuePair <Torqueable, Force> sourceNetForce;
            if (partsGraph.IsEdgeStillPresent(coupling))
            {
                // If the net force is coming from the target itself, no need to back-propagate the force;
                // just break early.
                sourceNetForce = source.GetNetForceAndSource();
            }
            else
            {
                // This coupling has been removed. Hard code the source net force to zero and let that get
                // propagated to torqueables that were formerly connected to this coupling.
                sourceNetForce = new KeyValuePair<Torqueable, Force>(target, Force.ZeroForce);
            }

            // Apply the force to the coupling.
            Force targetForce = coupling.ReceiveForce(source, sourceNetForce.Value);

            // Before proceeding, we need to check to make sure we're not back-propagating a force that
            // originated with the target.
            //
            // The only condition in which we would want to continue propagating this force is if the
            // force no longer equals the net force coming from the target. Otherwise, we should break
            // early. TODO: This doesn't make sense to me (as of 2020-08-01).
            if (sourceNetForce.Key == target
                && targetForce.Equals(target.GetNetForce()))
            {
                return;
            }

            // Apply the force to the target.
            bool didTargetNetForceChange = target.UpdateForce(source, targetForce);

            // If the net force on the target did not change, no need to propagate the force;
            // just break early.
            if (!didTargetNetForceChange)
            {
                return;
            }

            // Get adjacent couplings, and add them to the queue.
            foreach (var adjacentCoupling in partsGraph.GetCouplings(target))
            {
                if (!adjacentCoupling.Equals(coupling))
                {
                    if (RequiresFrontOfQueueProcessing(adjacentCoupling, targetForce))
                    {
                        AddToFrontOfEvaluationQueue(target, adjacentCoupling);
                    }
                    else
                    {
                        AddToBackOfEvaluationQueue(adjacentCoupling);
                    }
                }
            }
        }

        private static bool RequiresFrontOfQueueProcessing(Coupling coupling, Force forceFromSource)
        {
            if (typeof(BiPaddleCoupling).Equals(coupling.GetType()))
            {
                // Because this adjacent coupling represents a physical rod rotating about a fulcrum,
                // the forces should be propagated to the other end of the rod (the other paddle) first.
                return true;
            }
            
            if (forceFromSource.Equals(Force.ZeroForce))
            {
                // Always propagate the removal of force first.
                return true;
            }

            return false;
        }

        private void AddToFrontOfEvaluationQueue(Torqueable sourceOfChangeInForce, Coupling coupling)
        {
            // Add the source of change in force last since this is a queue (adding last means it will get picked up first).
            Torqueable target = coupling.GetOther(sourceOfChangeInForce);
            AddToFrontOfEvaluationQueue(new EvaluationNode(target, coupling));
            AddToFrontOfEvaluationQueue(new EvaluationNode(sourceOfChangeInForce, coupling));
        }

        private void AddToBackOfEvaluationQueue(EvaluationNode evaluationNode)
        {
            if (evaluationQueue.Contains(evaluationNode))
            {
                return;
            }
            lock (evaluationQueue)
            {
                evaluationQueue.Add(evaluationNode);
            }
        }

        private void AddToFrontOfEvaluationQueue(EvaluationNode evaluationNode)
        {
            if (evaluationQueue.Contains(evaluationNode))
            {
                evaluationQueue.Remove(evaluationNode);
            }
            lock (evaluationQueue)
            {
                evaluationQueue.Insert(0, evaluationNode);
            }
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
