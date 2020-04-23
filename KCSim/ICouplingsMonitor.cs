using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public interface ICouplingMonitor
    {
        void RegisterCoupling(Coupling coupling);
        void RemoveCoupling(Coupling coupling);
        bool IsCoupled(Torqueable torqueable);
        void EvaluateForces();
    }
}
