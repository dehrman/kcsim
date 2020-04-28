﻿using System;
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
            evaluationQueue.Add(new EvaluationNode(coupling.Input, coupling));
            evaluationQueue.Add(new EvaluationNode(coupling.Output, coupling));
        }

        public void AddToEvaluationQueue(Coupling coupling)
        {
            AddToEvaluationQueue(new EvaluationNode(coupling.Input, coupling));
            AddToEvaluationQueue(new EvaluationNode(coupling.Output, coupling));
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

            // Evaluate and propagate forces via breadth-first search.
            while (evaluationQueue.Count > 0)
            {
                // Get the next coupling to evaluate.
                EvaluationNode currentNode = evaluationQueue[0];
                evaluationQueue.RemoveAt(0);

                // Visit the current node.
                VisitNode(currentNode);
            }

            // Invoke any callbacks that were added to the callback queue 
            while (callbackQueue.Count > 0)
            {
                callbackQueue.Dequeue()?.Invoke();
            }

            // If the callbacks added to the queue, evaluate the forces again.
            // Otherwise, we've reached stability and can exit from this function.
            if (evaluationQueue.Count > 0)
            {
                EvaluateForces();
            }
        }

        public void OnAllForcesEvaluated(Action action)
        {
            callbackQueue.Enqueue(action);
        }

        private void VisitNode(EvaluationNode evaluationNode)
        {
            Coupling coupling = evaluationNode.Coupling;
            Torqueable source = evaluationNode.SourceOfForce;
            Torqueable target = coupling.GetOther(source);

            KeyValuePair<Torqueable, Force> sourceNetForce;
            if (partsGraph.IsEdgeStillPresent(coupling))
            {
                // If the net force is coming from the target itself, no need to back-propagate the force;
                // just break early.
                sourceNetForce = source.GetNetForceAndSource();
                if (sourceNetForce.Key == target)
                {
                    return;
                }
            }
            else
            {
                // This coupling has been removed. Hard code the source net force to zero and let that get
                // propagated to torqueables that were formerly connected to this coupling.
                sourceNetForce = new KeyValuePair<Torqueable, Force>(target, Force.ZeroForce);
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
                    if (typeof(BiPaddleCoupling).Equals(adjacentCoupling.GetType()))
                    {
                        // Because this adjacent coupling represents a physical rod rotating about a fulcrum,
                        // the forces should be propagated to the other end of the rod (the other paddle) first.
                        AddToFrontOfEvaluationQueue(target, adjacentCoupling);
                    }
                    else if (targetForce.Equals(Force.ZeroForce))
                    {
                        // Always propagate the removal of force first.
                        AddToFrontOfEvaluationQueue(target, adjacentCoupling);
                    }
                    else
                    {
                        AddToEvaluationQueue(adjacentCoupling);
                    }
                }
            }
        }

        private void AddToEvaluationQueue(EvaluationNode evaluationNode)
        {
            evaluationQueue.Add(evaluationNode);
        }

        private void AddToFrontOfEvaluationQueue(Torqueable sourceOfChangeInForce, Coupling coupling)
        {
            // Add the source of change in force last since this is a queue (adding last means it will get picked up first).
            Torqueable target = coupling.GetOther(sourceOfChangeInForce);
            evaluationQueue.Add(new EvaluationNode(target, coupling));
            evaluationQueue.Add(new EvaluationNode(sourceOfChangeInForce, coupling));
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
