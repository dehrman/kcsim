using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public interface ICouplingMonitor
    {
        delegate void OnCouplingRemovedDelegate(Coupling coupling);
        delegate void OnCoupledToInputDelegate(Coupling coupling);

        void OnCouplingRemoved(Coupling coupling, OnCouplingRemovedDelegate onCouplingRemovedDelegate);
        void OnCoupledToInput(Torqueable torqueable, OnCoupledToInputDelegate onCoupledToInputDelegate);
        void RegisterCoupling(Coupling coupling);
        void RemoveCoupling(Coupling coupling);
        bool IsCoupled(Torqueable torqueable);
    }
}
