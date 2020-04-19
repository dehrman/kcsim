using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public class CouplingMonitor : ICouplingMonitor
    {
        private readonly IPartsGraph partsGraph;

        public CouplingMonitor(IPartsGraph partsGraph)
        {
            this.partsGraph = partsGraph;
        }

        public void RegisterCoupling(Coupling coupling)
        {
            partsGraph.AddVerticesAndEdge(coupling);
        }

        public void RemoveCoupling(Coupling coupling)
        {
            partsGraph.RemoveEdge(coupling);
        }

        public void EvaluateForces(Torqueable root)
        {
            // This method uses a breadth-first search to evaluate and propogate forces throughout
            // the machine.

            // First, determine whether or not the provided node has any couplings.
            ISet<Coupling> rootCouplings = partsGraph.GetCouplings(root);
            if (rootCouplings.Count == 0)
            {
                return;
            }

            // Create a set to keep track of visited couplings.
            ISet<Coupling> visitedCouplings = new HashSet<Coupling>();

            // Initialize the evaluation queue.
            Queue<EvaluationNode> evaluationQueue = new Queue<EvaluationNode>();
            Force rootForce = root.GetNetForce();
            foreach (Coupling coupling in rootCouplings)
            {
                evaluationQueue.Enqueue(new EvaluationNode(root, rootForce, coupling));
            }

            while (evaluationQueue.Count > 0)
            {
                // Get the next coupling to evaluate.
                EvaluationNode evaluationNode = evaluationQueue.Dequeue();
                Coupling currentCoupling = evaluationNode.Coupling;
                
                // Visit the current node. If targetOfForce is non-null, that indicates that the force resulting
                // from the evaluation should be propogated to any further couplings emanating from targetOfForce.
                Torqueable targetOfForce = UpdateForce(evaluationNode);
                
                // Mark the coupling as visited.
                visitedCouplings.Add(currentCoupling);
                
                if (targetOfForce != null)
                {
                    // If we have a non-null targetOfForce, that means that the target has received
                    // an updated force, and so we need to propogate that to further couplings.
                    Force force = targetOfForce.GetNetForce();
                    ISet<Coupling> couplingsToEvaluate = partsGraph.GetCouplings(targetOfForce);
                    foreach (Coupling couplingToEvaluate in couplingsToEvaluate)
                    {
                        if (!visitedCouplings.Contains(couplingToEvaluate))
                        {
                            evaluationQueue.Enqueue(new EvaluationNode(targetOfForce, force, couplingToEvaluate));
                        }
                    }
                }
            }
        }

        private Torqueable UpdateForce(EvaluationNode evaluationNode)
        {
            Coupling coupling = evaluationNode.Coupling;
            Torqueable sourceOfForce = evaluationNode.SourceOfForce;
            Force resultingForceOnTarget = coupling.ReceiveForce(sourceOfForce, evaluationNode.Force);
            Torqueable targetOfForce = coupling.Input == sourceOfForce ? coupling.Output : coupling.Input;
            if (targetOfForce.UpdateForce(coupling, resultingForceOnTarget))
            {
                // The net force on the target changed, so return it.
                return targetOfForce;
            }
            else
            {
                // No change in net force on the target, so this is the end of the line.
                return null;
            }
        }

        private class EvaluationNode
        {
            public Torqueable SourceOfForce;
            public Force Force;
            public Coupling Coupling;

            public EvaluationNode(Torqueable sourceOfForce, Force force, Coupling coupling)
            {
                this.SourceOfForce = sourceOfForce;
                this.Force = force;
                this.Coupling = coupling;
            }
        }
    }
}
